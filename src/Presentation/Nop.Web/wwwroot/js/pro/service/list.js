var PageServiceList = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.serviceList = [];

    var pagination = new SimplePagination('.pagination-service-list');

    var tmplCardServiceEmpty = $.templates("#template-card-service-empty");
    var tmplCardServiceList = $.templates("#template-card-service-list");
    var tmplCardServiceListEmpty = $.templates("#template-card-service-list-empty");
    var tmplCardServiceListResponse = $.templates("#template-card-service-list-response");
    var tmplCardServiceListResponseEmpty = $.templates("#template-card-service-list-response-empty");

    var $content = $('.content-service-search');
    var $listServiceList = $content.find(".list-service-list");
    var $detail = $('.content-service-list-response');

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    pagination.onPageChanged = function (pageIndex) {
        console.log(pageIndex);
        base.loadServiceListData(base.setServiceList, pageIndex);
    }

    base.loadServiceListData = function (callback, pageIndex) {
        var filterData = {
            keyword : ""
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
            "url": url.getServiceList,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.submitServiceProfile = function (callback) {
        swal({
            title: "Are You Sure to Delete?",
            icon: "warning",
            buttons: ["No", "Yes"],
            dangerMode: true,
        }).then((result) => {
            if (result) {
                var settings = {
                    "url": url.deleteServiceProfile.format(model.selectedServiceList),
                    "method": 'DELETE',
                    "headers": {
                        "Content-Type": "application/json"
                    }
                };

                $.ajax(settings).done(callback);
            } else {
                swal("Failed to delete service profile!")
            }
        });
    };

    base.onSubmitServiceProfileResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Service Profile Deleted Successfully",
            }).then(function () {
                location.reload();
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

    base.setServiceList = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {

            if (response.data.data.length > 0) {
                $listServiceList.empty();
                pagination.set(response.data);

                $.each(response.data.data, function (i, item) {
                    if (item.tenureStart !== null) {
                        item.tenureStart = moment(item.tenureStart, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }
                    if (item.tenureEnd !== null) {
                        item.tenureEnd = moment(item.tenureEnd, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }
                    item.updatedOn = moment(item.updatedOnUTC, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD/MM/YYYY');
                });

                $content.bindText(response.data);

                var htmlCardServiceList = tmplCardServiceList.render(response.data.data);
                $listServiceList.append(htmlCardServiceList);
                model.serviceList = model.serviceList.concat(response.data.data);

                model.selectedServiceList = response.data.data[0]
                var htmlCardServiceListResponse = tmplCardServiceListResponse.render(model.selectedServiceList);
                $content.find('.content-service-list-response').replaceWith(htmlCardServiceListResponse);

                $content.find('.card-service-list').on('click', '.btn-edit', function () {
                    location.href = url.editServiceProfilePage.format(model.selectedServiceList);
                });

                $content.find('.card-service-list').on('click', '.btn-delete', function () {
                    base.submitServiceProfile(base.onSubmitServiceProfileResponse);
                });

                var $areaExpertise = $('.area-expertise');
                model.selectedServiceList.serviceExpertises.forEach(function (item) {
                    $areaExpertise.append("<span class='badge badge-pill badge-primary'>" + item.name + "</span>&nbsp;");
                });

                var $grpOnsite = $content.find('.form-group-onsite');
                var $grpCharges = $content.find('.form-group-charges');
                var $grpProjectBased = $content.find('.form-group-projectBased');
                var $grpConsultation = $content.find('.form-group-consultation');
                var $grpFreelancing = $content.find('.form-group-freelancing');
                var $grpPartTime = $content.find('.form-group-part-time');
                var serviceTypeId = model.selectedServiceList.serviceTypeId;
                $grpCharges.addClass('hidden');
                $grpProjectBased.addClass('hidden');
                $grpConsultation.addClass('hidden');
                $grpFreelancing.addClass('hidden');
                $grpPartTime.addClass('hidden');

                switch (serviceTypeId) {
                    case 1:
                        $grpFreelancing.removeClass('hidden');
                        break;
                    case 2:
                        $grpPartTime.removeClass('hidden');
                        break;
                    case 3:
                        $grpConsultation.removeClass('hidden');
                        break;
                    case 4:
                        $grpProjectBased.removeClass('hidden');
                        break;
                }

                if (serviceTypeId && serviceTypeId !== 4) {
                    $grpCharges.removeClass('hidden');
                }
            } else {
                $content.replaceWith(tmplCardServiceEmpty);

                $listServiceList.append(tmplCardServiceListEmpty);
                $content.find('.content-service-list-response').replaceWith(tmplCardServiceListResponseEmpty);
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

    base.setServiceListResponse = function (target) {
        var serviceListId = $(target).data('entity-id');
        var serviceListModel = $.grep(model.serviceList, function (item) {
            return item.id === serviceListId
        });

        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        $(target).parent().find('.action-panel').removeClass('selected');
        $(target).find('.action-panel').addClass('selected');

        model.selectedServiceList = serviceListModel[0]
        var htmlCardServiceListResponse = tmplCardServiceListResponse.render(model.selectedServiceList);
        $content.find('.content-service-list-response').replaceWith(htmlCardServiceListResponse);

        var $areaExpertise = $('.area-expertise');
        model.selectedServiceList.serviceExpertises.forEach(function (item) {
            $areaExpertise.append("<span class='badge badge-pill badge-primary'>" + item.name + "</span>&nbsp;");
        });

        var $grpOnsite = $content.find('.form-group-onsite');
        var $grpCharges = $content.find('.form-group-charges');
        var $grpProjectBased = $content.find('.form-group-projectBased');
        var $grpConsultation = $content.find('.form-group-consultation');
        var $grpFreelancing = $content.find('.form-group-freelancing');
        var $grpPartTime = $content.find('.form-group-part-time');
        var serviceTypeId = model.selectedServiceList.serviceTypeId;
        $grpCharges.addClass('hidden');
        $grpProjectBased.addClass('hidden');
        $grpConsultation.addClass('hidden');
        $grpFreelancing.addClass('hidden');
        $grpPartTime.addClass('hidden');

        switch (serviceTypeId) {
            case 1:
                $grpFreelancing.removeClass('hidden');
                break;
            case 2:
                $grpPartTime.removeClass('hidden');
                break;
            case 3:
                $grpConsultation.removeClass('hidden');
                break;
            case 4:
                $grpProjectBased.removeClass('hidden');
                break;
        }

        if (serviceTypeId && serviceTypeId !== 4) {
            $grpCharges.removeClass('hidden');
        }
    };

    base.setUserEvent = function () {
        $listServiceList.on('click', '.card-service-list', function () {
            base.setServiceListResponse(this);

            $content.find('.card-service-list').on('click', '.btn-edit', function () {
                location.href = url.editServiceProfilePage.format(model.selectedServiceList);
            });

            $content.find('.card-service-list').on('click', '.btn-delete', function () {
                base.submitServiceProfile(base.onSubmitServiceProfileResponse);
            });
        });
    };

    var init = function () {
        base.loadServiceListData(base.setServiceList);
        base.setUserEvent();
    };

    init();
};

var pageServiceList = new PageServiceList({
    'url': {
        'getServiceList': '/api/pro/service/user',
        'editServiceProfilePage': '/pro/service/details/{{id}}',
        'deleteServiceProfile': '/api/pro/service/{{id}}'
    }
});