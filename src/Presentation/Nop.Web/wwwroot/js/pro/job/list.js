var PageJobList = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.jobList = [];
    var pagination = new SimplePagination('.pagination-job-list-list');
    var tmplCardJobEmpty = $.templates("#template-card-job-empty");

    var tmplCardJobList = $.templates("#template-card-job-list");
    var tmplCardJobListEmpty = $.templates("#template-card-job-list-empty");
    var tmplCardJobListResponse = $.templates("#template-card-job-list-response");
    var tmplCardJobListResponseEmpty = $.templates("#template-card-job-list-response-empty");

    var $content = $('.content-job-search');
    var $listJobList = $content.find(".list-job-list");

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    pagination.onPageChanged = function (pageIndex) {
        base.loadJobListData(base.setJobList, pageIndex);
    }

    base.loadJobListData = function (callback, pageIndex) {
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
            "url": url.getJobList,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.submitJobProfile = function (callback) {
        swal({
            title: "Are You Sure to Delete?",
            icon: "warning",
            buttons: ["No", "Yes"],
            dangerMode: true,
        }).then((result) => {
            if (result) {
                model.selectedJobList.deleted = true;
                var settings = {
                    "url": url.updateJobProfile.format(model.selectedJobList),
                    "method": 'POST',
                    "headers": {
                        "Content-Type": "application/json"
                    },
                    'data': JSON.stringify(model.selectedJobList)
                };

                $.ajax(settings).done(callback);
            } else {
                swal("Failed to delete Job profile!")
            }
        });
    };

    base.onSubmitJobProfileResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Delete Job Profile",
                text: "Delete sucessfully"
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

    base.publishJobProfile = function (callback) {

        swal({
            icon: "warning",
            title: 'Are you sure to publish the job ads?',
            text: "- Important: Once Jobs are published, no changes would be allowed",
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
                model.selectedJobList.status = 1;
                var settings = {
                    "url": url.updateJobProfile.format(model.selectedJobList),
                    "method": 'POST',
                    "headers": {
                        "Content-Type": "application/json"
                    },
                    'data': JSON.stringify(model.selectedJobList)
                };
                $.ajax(settings).done(callback);
            }
        });
    };

    base.onPublishJobProfileResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Job Profile Published Successfully",
            }).then(function () {
                base.loadJobListData(base.setJobList, pagination.model.pageIndex);
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

    base.setJobList = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {

            if (response.data.data.length > 0) {

                response.data.data.forEach(function (item) {
                    if (item.startDate !== null) {
                        item.startDate = moment(item.startDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                    }

                    if (item.viewJobCandidateFullProfileSubscriptionEndDate !== null) {
                        item.expiredAtDate = moment(item.viewJobCandidateFullProfileSubscriptionEndDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                    }

                    if (item.payAmount !== null) {
                        item.payAmount = parseFloat(item.payAmount).toFixed(2);
                    }
                });

                $content.bindText(response.data);
                $listJobList.empty();
                pagination.set(response.data);
                var htmlCardJobList = tmplCardJobList.render(response.data.data);
                $listJobList.append(htmlCardJobList);
                model.jobList = model.jobList.concat(response.data.data);

                model.selectedJobList = response.data.data[0]
                if (!model.selectedJobList.stateProvinceId) {
                    model.selectedJobList.stateProvinceName = "All State";
                }
                if (!model.selectedJobList.cityId) {
                    model.selectedJobList.cityName = "All City";
                }
                base.setJobListResponse($listJobList.find('.card')[0]);

                //var htmlCardJobListResponse = tmplCardJobListResponse.render(model.selectedJobList);
                //$content.find('.content-job-list-response').replaceWith(htmlCardJobListResponse);
                //var $areaExpertise = $('.area-expertise');
                //model.selectedJobList.requiredExpertises.forEach(function (item) {
                //    $areaExpertise.append("<span class='badge badge-pill badge-primary-light'>" + item.name + "</span>&nbsp;");
                //});

                //$content.find('.card-job-list-response').on('click', '.btn-edit', function () {
                //    location.href = url.editJobProfilePage.format(model.selectedJobList);
                //});

                ////$content.find('.card-job-list-response').on('click', '.btn-delete', function () {
                ////    base.submitJobProfile(base.onSubmitJobProfileResponse);
                ////});

                //$content.find('.content-job-list-response').on('click', '.btn-search', function () {
                //    location.href = url.searchCandidatePage.format(model.selectedJobList);
                //});

                //$content.find('.content-job-list-response').on('click', '.btn-publish', function () {
                //    base.publishJobProfile(base.onPublishJobProfileResponse);
                //});
            } else {
                $content.replaceWith(tmplCardJobEmpty);

                $listJobList.empty();
                pagination.set(response.data);
                $listJobList.append(tmplCardJobListEmpty);
                $content.find('.content-job-list-response').replaceWith(tmplCardJobListResponseEmpty);
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

    base.setJobListResponse = function (target) {
        var jobListId = $(target).data('entity-id');
        var jobListModel = $.grep(model.jobList, function (item) {
            return item.id === jobListId
        });

        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        $(target).parent().find('.action-panel').removeClass('selected');
        $(target).find('.action-panel').addClass('selected');

        model.selectedJobList = jobListModel[0]
        if (!model.selectedJobList.stateProvinceId) {
            model.selectedJobList.stateProvinceName = "All State";
        }
        if (!model.selectedJobList.cityId) {
            model.selectedJobList.cityName = "All City";
        }
        model.selectedJobApplicationId = jobListModel[0].id;
        var htmlCardJobListResponse = tmplCardJobListResponse.render(model.selectedJobList);
        $content.find('.content-job-list-response').replaceWith(htmlCardJobListResponse);

        var $areaExpertise = $('.area-expertise');
        model.selectedJobList.requiredExpertises.forEach(function (item) {
            $areaExpertise.append("<span class='badge badge-pill badge-primary'>" + item.name + "</span>&nbsp;");
        });

        $content.find('.content-job-list-response').on('click', '.btn-edit', function () {
            location.href = url.editJobProfilePage.format(model.selectedJobList);
        });

        //$content.find('.content-job-list-response').on('click', '.btn-delete', function () {
        //    location.href = url.editJobProfilePage.format(model.selectedJobList);
        //});

        $content.find('.content-job-list-response').on('click', '.btn-search', function () {
            location.href = url.searchCandidatePage.format(model.selectedJobList);
        });

        $content.find('.content-job-list-response').on('click', '.btn-publish', function () {
            base.publishJobProfile(base.onPublishJobProfileResponse);
        });
    };

    base.setUserEvent = function () {
        $listJobList.on('click', '.card-job-list', function () {
            base.setJobListResponse(this);

            $content.find('.card-job-list').on('click', '.btn-edit', function () {
                location.href = url.editJobProfilePage.format(model.selectedJobList);
            });

            $content.find('.card-job-list').on('click', '.btn-delete', function () {
                base.submitJobProfile(base.onSubmitJobProfileResponse);
            });

            $content.find('.card-job-list').on('click', '.btn-publish', function () {
                base.publishJobProfile(base.onPublishJobProfileResponse);
            });
        });
    };

    base.getModel = function () {
        return model;
    };

    var init = function () {
        base.loadJobListData(base.setJobList);
        base.setUserEvent();
    };

    init();
};

var pageJobList = new PageJobList({
    'url': {
        'getJobList': '/api/pro/job',
        'editJobProfilePage': '/pro/job/details/{{id}}',
        'searchCandidatePage': '/pro/job/{{id}}/candidate',
        'updateJobProfile': '/api/pro/job/profile/{{id}}'


    }
});