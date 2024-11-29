
var PageConsultationList = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.consultationList = [];
    var pagination = new SimplePagination('.pagination-consultation-list-list');
    var tmplCardConsultationEmpty = $.templates("#template-card-consultation-empty");

    var tmplCardConsultationList = $.templates("#template-card-consultation-list");
    var tmplCardConsultationListResponse = $.templates("#template-card-consultation-detail");
    var $content = $('.content-consultation-search');
    var $listConsultationList = $content.find(".list-consultation-list");
    var $formSearchCandidates = $('#form-search-candidates');

    pagination.onPageChanged = function (pageIndex) {
        base.loadConsultationListData(base.setConsultationList, pageIndex);
    }

    var initViewComponent = function () {
        var queryModel = window.location.getUrlQueryParams();

        if (queryModel && queryModel.created === '1') {
            $('.content-consultation-job-created').removeClass('hidden');
        }
    }

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

    base.deleteConsultationProfile = function (callback) {

        var settings = {
            "url": url.deleteConsultationList.format(model.selectedConsultation),
            "method": 'DELETE',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    }

    //front-end response
    base.setConsultationList = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            if (response.data.data.length > 0) {
                $.each(response.data.data, function (i, item) {
                    item.createdOnUTC = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').format('DD MMM YYYY');
                });

                console.log(response);
                $content.bindText(response.data);
                $listConsultationList.empty();
                pagination.set(response.data);
                var htmlCardConsultationList = tmplCardConsultationList.render(response.data.data);

                $listConsultationList.append(htmlCardConsultationList);

                model.consultationList = model.consultationList.concat(response.data.data);

                model.selectedConsultation = response.data.data[0]
                base.refreshConsultationDetail(model.selectedConsultation);
            } else {
                $content.replaceWith(tmplCardConsultationEmpty);

            }

        } else {
            $listConsultationList.empty();
            pagination.set(response.data);
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
        base.refreshConsultationDetail(model.selectedConsultation);
    };

    base.refreshConsultationDetail = function (model) {
        $content.find('.content-consultation-detail').empty();
        if (model) {
            $formSearchCandidates.removeClass('hidden');

            model.timeSlots.forEach(function (timeSlot, i) {
                timeSlot.startDateText = moment(timeSlot.startDate, 'YYYY-MM-DDTHH:mm').format('dddd');
                timeSlot.startTimeText = moment(timeSlot.startDate, 'YYYY-MM-DDTHH:mm').format('hh:mm A');
                timeSlot.endTimeText = moment(timeSlot.endDate, 'YYYY-MM-DDTHH:mm').format('hh:mm A');
            });

            model.timeSlots = !model.timeSlots ? [] : model.timeSlots.sort(function (a, b) {
                if (a.startDate < b.startDate) { return -1; }
                if (a.startDate > b.startDate) { return 1; }
                return 0;
            });

            var htmlCardConsultationListResponse = tmplCardConsultationListResponse.render(model);
            
            $content.find('.content-consultation-detail').replaceWith(htmlCardConsultationListResponse);

            app.initFormComponent($content.find('#form-search-candidates'));
            refreshCandidateBind($content.find('#form-search-candidates'));
        } else {
            $formSearchCandidates.addClass('hidden');
            $content.find('.content-consultation-detail').empty();
        }
    };

    base.onConsultationProfileDeleted = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Consultation Request Deleted Succesfully ",
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

        $content.on('click', '.btn-delete', function () {
            swal({
                icon: "warning",
                title: 'Are You Sure to Delete?',
                buttons: {
                    cancel: {
                        text: "Cancel",
                        value: false,
                        visible: true,
                        className: "btn-secondary",
                        closeModal: true
                    },
                    confirm: {
                        text: "Confirm",
                        value: true,
                        visible: true,
                        className: "btn-primary",
                        closeModal: true
                    }
                },
            }).then(function (isConfirm) {
                if (isConfirm) {
                    base.deleteConsultationProfile(base.onConsultationProfileDeleted);
                }
            });
        });
    };

    //refetch search candidate form id
    function refreshCandidateBind (id) {
        id.on('click', '.btn-search-candidates', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (id.valid() === false) {
                return false;
            }

            var formData = app.getFormValue(id);
            console.log(model.selectedConsultation);
            var actionUrl = url.searchConsultationCondidates.format(model.selectedConsultation);
            var data = {
                category: model.selectedConsultation.categoryId,
                yearExperience: model.selectedConsultation.yearExperience
                //expertises: []
            };

            //formData.expertises.forEach(function (expertise, i) {
            //    data.expertises.push(expertise.id);
            //});

            var queryString = $.param(data);
            location.href = actionUrl + "?" + queryString;
        });
    }

    //page initilize
    var init = function () {
        initViewComponent();
        base.loadConsultationListData(base.setConsultationList);
        base.setUserEvent();
    };

    init();
};

var pageConsultationList = new PageConsultationList({
    'url': {
        'getConsultationList': '/api/pro/consultation/list',
        'deleteConsultationList': '/api/pro/consultation/{{id}}',
        'searchConsultationCondidates': '/pro/consultation/{{id}}/candidates',
    }
});
