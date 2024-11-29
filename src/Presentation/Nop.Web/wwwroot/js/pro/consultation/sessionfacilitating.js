
var PageConsultationList = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.consultationList = [];
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var pagination = new SimplePagination('.pagination-consultation-facilitating-list');

    var tmplCardConsultationList = $.templates("#template-card-consultation-list");
    var tmplCardConsultationListEmpty = $.templates("#template-card-consultation-list-empty");
    var tmplCardConsultationListResponse = $.templates("#template-card-consultation-detail");
    var tmplCardConsultationListResponseEmpty = $.templates("#template-card-consultation-detail-empty");

    var $content = $('.content-consultation-search');
    var $content2 = $('.consultation-approval');
    var $listConsultationList = $content.find(".list-consultation-list");
    var $form = $content2.find('#form-consultation-profile-complete');
    var $form2 = $content.find('#form-consultation-profile-filter');
    pagination.onPageChanged = function (pageIndex) {
        base.loadConsultationListData(base.setConsultationList, pageIndex);
    }

    //model object
    Object.defineProperty(base, 'model', {
        get() {
            return model;
        }
    });

    //back-end interation
    base.loadConsultationListData = function (callback, pageIndex) {
        var filterData = {
            keyword: ""
        };

        var recordSize = 10;
        var offset = pageIndex ? pageIndex * recordSize : 0;

        var requestData = {
            "filter": filterData.keyword,
            "offset": offset,
            "recordSize": recordSize,
            "sorting": null,
            "advancedFilter": filterData
        };
        filterData.date = $('#inputFaciFilterDate').val();
        filterData.statusId = $('#selectFaciFilterStatus').val();
        filterData.name = $('#inputOrganizationName').val();

        model.search = requestData;
        var settings = {
            "url": url.getConsultationList,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.setConsultationList = function (response) {

            if (response
                && response.status
                && response.status.code === 1) {

                if (response.data.data.length > 0) {
                    $content.bindText(response.data);
                    $('#divFaciBtnShow').show();

                    response.data.data.forEach(function (value) {
                        value.consultantAvailableTimeSlots = !value.consultantAvailableTimeSlots ? [] : value.consultantAvailableTimeSlots.sort(function (a, b) {
                            if (a.startDate < b.startDate) { return -1; }
                            if (a.startDate > b.startDate) { return 1; }
                            return 0;
                        });

                        //value.consultantAvailableTimeSlots.forEach(function (consultantAvailableTimeSlots, i) {
                        //    if (consultantAvailableTimeSlots) {
                        //        consultantAvailableTimeSlots.startDateText = moment(consultantAvailableTimeSlots.startDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                        //        consultantAvailableTimeSlots.startTimeText = moment(consultantAvailableTimeSlots.startDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('h:mma');
                        //        consultantAvailableTimeSlots.endTimeText = moment(consultantAvailableTimeSlots.endDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('h:mma');
                        //    }
                                
                        //})
                        value.appointmentDateText = moment(value.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('DD MMM YYYY');
                        value.appointmentStartTimeText = moment(value.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('h:mma');
                        value.appointmentEndTimeText = moment(value.appointmentEndDate, 'YYYY-MM-DDTHH:mm').format('h:mma');
                    })

                    var htmlCardConsultationList = tmplCardConsultationList.render(response.data.data);
                    $listConsultationList.empty();
                    pagination.set(response.data);

                    $listConsultationList.append(htmlCardConsultationList);
                    model.consultationList = model.consultationList.concat(response.data.data);

                    model.selectedConsultation = response.data.data[0]

                    //model.selectedConsultation.consultantAvailableTimeSlots.startDateText = moment(model.selectedConsultation.consultantAvailableTimeSlots[0].startDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY (ddd)');
                    //model.selectedConsultation.consultantAvailableTimeSlots.startTimeText = moment(model.selectedConsultation.consultantAvailableTimeSlots[0].startDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('h:mma');
                    //model.selectedConsultation.consultantAvailableTimeSlots.endTimeText = moment(model.selectedConsultation.consultantAvailableTimeSlots[0].endDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('h:mma');
                    //let remarks = model.selectedConsultation.statusRemarks;
                    //let statusEnum = model.selectedConsultation.consultationApplicationStatus;
                    //$('#inputFaciRemarks').val(remarks);
                    //$('#selectCompleteEnum').val(statusEnum);
                    base.refreshConsultationDetail(model.selectedConsultation);
                }
                else {
                    $listConsultationList.empty();
                    pagination.set(response.data);

                    $listConsultationList.append(tmplCardConsultationListEmpty);
                    $content.find('.content-consultation-detail').replaceWith(tmplCardConsultationListResponseEmpty);

                    $('#divFaciBtnShow').hide();
                }

            } else {
                swal({
                    icon: "warning",
                    title: "Fail",
                    text: response.status.message
                }).then(function () {
                });
            }
    };

    base.setConsultationListResponse = function (target) {
        var consultationListId = $(target).data('entity-id');
        var consultationListModel = $.grep(model.consultationList, function (item) {
            return item.id === consultationListId
        });
        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        model.selectedConsultation = consultationListModel[0]
        let remarks = model.selectedConsultation.statusRemarks;
        let statusEnum = model.selectedConsultation.consultationApplicationStatus;
        $('#inputFaciRemarks').val(remarks);
        $('#selectCompleteEnum').val(statusEnum);
        base.refreshConsultationDetail(model.selectedConsultation);
    };

    base.refreshConsultationDetail = function (model) {
        model.consultantAvailableTimeSlots = !model.consultantAvailableTimeSlots ? [] : model.consultantAvailableTimeSlots.sort(function (a, b) {
            if (a.startDate < b.startDate) { return -1; }
            if (a.startDate > b.startDate) { return 1; }
            return 0;
        });
        model.consultantAvailableTimeSlots.forEach(function (consultantAvailableTimeSlots, i) {
            consultantAvailableTimeSlots.startDateText = moment(consultantAvailableTimeSlots.startDate, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('ddd');
            consultantAvailableTimeSlots.startTimeText = moment(consultantAvailableTimeSlots.startDate, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('h:mma');
            consultantAvailableTimeSlots.endTimeText = moment(consultantAvailableTimeSlots.endDate, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('h:mma');
        });

        model.appointmentDateText = moment(model.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('DD MMM YYYY');
        model.appointmentStartTimeText = moment(model.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('h:mma');
        model.appointmentEndTimeText = moment(model.appointmentEndDate, 'YYYY-MM-DDTHH:mm').format('h:mma');

        var htmlCardConsultationListResponse = tmplCardConsultationListResponse.render(model);
        //$.observable(model).setProperty('selectedConsultationData2', model);
        $content.find('.content-consultation-detail').replaceWith(htmlCardConsultationListResponse);
    };

    base.onConsultationProfileDeleted = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Selected Consultation Profile Deleted",
                text: "Delete sucessfully"
            }).then(function () {
                location.reload();
            });
        });
    };

    base.handleResponse = function (response, successHandler) {
        if (response
            && response.status
            && response.status.code === 1) {
            successHandler(response);
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
            });
        }
    };

    //user event handle
    base.setUserEvent = function () {
        $listConsultationList.on('click', '.card-consultation-list', function () {
            base.setConsultationListResponse(this);

            $content.find('.content-consultation-detail').on('click', '.btn-edit', function () {
                location.href = url.getConsultationCandidateList.format(model.selectedConsultation);
            });
        });

        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.createConsultationFacilitating(base.onSubmitConsultationFacilitatingResponse);
            }
        });

        $('#btnFaciSearch').on('click', function () {
            base.loadConsultationListData(base.setConsultationList);

        })

        $('dropdown-menu').on('change', function () {
            var result = $('#dropdownMenuButton').val();
            alert(result);

        })
    };

    //submit form
    base.createConsultationFacilitating = function (callback) {
        var data = app.getFormValue($form);
        data.completionStatus = parseInt($('#selectCompleteEnum').val());
        var settings = {
            "url": $form.attr('action').format(model) + "/" + model.selectedConsultation.id,
            "method": $form.attr('method'),
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };
        $.ajax(settings).done(callback);
    };

    base.validationForm = function () {
        var valid = true;

        valid = $form.valid() && valid;

        return valid;
    };

    base.onSubmitConsultationFacilitatingResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Approval Consultation Invitation",
                text: "Update sucessfully"
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
            });
        }
    };

    //page initilize
    var init = function () {
        base.loadConsultationListData(base.setConsultationList);
        base.setUserEvent();
        app.initFormComponent($form2);
    };

    init();
};

var pageConsultationList = new PageConsultationList({
    'url': {
        'getConsultationList': '/api/pro/ConsultationInvitation/facilitating/list',
        'getReturnUrl': '/pro/consultation/facilitating',
    }
});