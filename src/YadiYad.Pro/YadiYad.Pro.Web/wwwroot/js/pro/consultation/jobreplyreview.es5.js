"use strict";

var PageConsultationList = function PageConsultationList(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.consultationList = [];
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var pagination = new SimplePagination('.pagination-consultation-replyreview-list');

    var tmplCardConsultationList = $.templates("#template-card-consultation-list");
    var tmplCardConsultationListResponse = $.templates("#template-card-consultation-detail");
    var tmplCardConsultationListEmpty = $.templates("#template-card-consultation-list-empty");
    var tmplCardConsultationListResponseEmpty = $.templates("#template-card-consultation-detail-empty");

    var $content = $('.content-consultation-search');
    var $listConsultationList = $content.find(".list-consultation-list");
    var $content2 = $('.consultation-approval');
    var $form = $content2.find('#form-consultation-invitation-approval');
    pagination.onPageChanged = function (pageIndex) {
        base.loadConsultationListData(base.setConsultationList, pageIndex);
    };

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

        model.search = requestData;
        var settings = {
            "url": url.getConsultationList,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.setConsultationList = function (response) {
        if (response && response.status && response.status.code === 1) {

            if (response.data.data.length > 0) {
                $content.bindText(response.data);

                var htmlCardConsultationList = tmplCardConsultationList.render(response.data.data);
                $listConsultationList.empty();
                pagination.set(response.data);

                $listConsultationList.append(htmlCardConsultationList);
                model.consultationList = model.consultationList.concat(response.data.data);

                base.refreshConsultationDetail(response.data.data[0]);
            } else {
                $listConsultationList.append(tmplCardConsultationListEmpty);
                $content.find('.content-consultation-detail').replaceWith(tmplCardConsultationListResponseEmpty);

                $('#divReplyBtnShow').hide();
            }
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.setConsultationListResponse = function (target) {
        var consultationListId = $(target).data('entity-id');
        var consultationListModel = $.grep(model.consultationList, function (item) {
            return item.id === consultationListId;
        });
        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        base.refreshConsultationDetail(consultationListModel[0]);
    };

    base.refreshConsultationDetail = function (selectedModel) {
        if (model.selectedConsultation && model.selectedConsultation.id === selectedModel.id) {
            return false;
        }

        app.clearForm($form);
        model.selectedConsultation = selectedModel;
        selectedModel.consultantAvailableTimeSlots = !selectedModel.consultantAvailableTimeSlots ? [] : selectedModel.consultantAvailableTimeSlots.sort(function (a, b) {
            if (a.startDate < b.startDate) {
                return -1;
            }
            if (a.startDate > b.startDate) {
                return 1;
            }
            return 0;
        });

        selectedModel.consultantAvailableTimeSlots.forEach(function (consultantAvailableTimeSlots, i) {
            consultantAvailableTimeSlots.startDateText = moment(consultantAvailableTimeSlots.startDate, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('(ddd)');
            consultantAvailableTimeSlots.startTimeText = moment(consultantAvailableTimeSlots.startDate, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('h:mma');
            consultantAvailableTimeSlots.endTimeText = moment(consultantAvailableTimeSlots.endDate, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('h:mma');
        });

        var htmlCardConsultationListResponse = tmplCardConsultationListResponse.render(selectedModel);
        $content.find('.content-consultation-detail').replaceWith(htmlCardConsultationListResponse);
    };

    //user event handle
    base.setUserEvent = function () {
        $listConsultationList.on('click', '.card-consultation-list', function () {
            base.setConsultationListResponse(this);

            $content.find('.content-consultation-detail').on('click', '.btn-edit', function () {
                location.href = url.getConsultationCandidateList.format(model.selectedConsultation);
            });
        });

        $('[name="isApproved"]').on('change', function (e, isInit) {
            var value = $(this).val();

            switch (value) {
                case "true":
                    $(".grp-remark").addClass('hidden');
                    break;
                case "false":
                    $(".grp-remark").removeClass('hidden');
                    break;
            }
        });

        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.createConsultationInvitationApproval(base.onSubmitConsultationInvitationApprovalResponse);
            }
        });
    };

    //submit form
    base.createConsultationInvitationApproval = function (callback) {
        var data = app.getFormValue($form);
        data.id = model.selectedConsultation.id;

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

    base.onSubmitConsultationInvitationApprovalResponse = function (response) {
        if (response && response.status && response.status.code === 1 && response.data === true) {
            swal({
                icon: "success",
                title: "Consultation Request is Approved"
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        } else if (response && response.status && response.status.code === 1 && response.data === false) {
            swal({
                icon: "success",
                title: "Consultation Request is Rejected, requires Updates"
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        }
    };

    //page initilize
    var init = function init() {
        app.initFormComponent($form);
        base.loadConsultationListData(base.setConsultationList);
        base.setUserEvent();
    };

    init();
};

var pageConsultationList = new PageConsultationList({
    'url': {
        'getConsultationList': '/api/pro/ConsultationInvitation/checking/list',
        'getReturnUrl': '/pro/consultation/reply/review'

    }
});

