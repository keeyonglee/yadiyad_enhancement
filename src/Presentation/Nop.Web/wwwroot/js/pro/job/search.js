var PageJobSearch = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    var simplePagination = new SimplePagination('.simple-pagination');
    model.search = null;
    model.jobSearchResultList = [];
    model.jobSeeker = {
        isEntitledApplyJob: false
    };

    var $content = $('.content-job-search');

    var $cardJobSearchResult = $('.card-job-search-result');
    var $listJobSearchResult = $cardJobSearchResult.find(".list-job-search-result");

    var tmplJobProfileSimple = $.templates("#template-job-profile-simple");
    var tmplJobProfileDetail = $.templates("#template-job-profile-detail");


    base.setData = function (opts) {
    };

    simplePagination.onPageChanged = function (pageIndex) {
        base.submitJobSearchRequest(base.setJobSearchResultList, pageIndex);
    }

    base.submitJobSearchRequest = function (callback, pageIndex) {
        var filterData = {
        };
        var recordSize = 10;
        var offset = pageIndex ? pageIndex * recordSize : 0;

        var requestData = {
            "filter": null,
            "offset": offset,
            "recordSize": recordSize,
            "sorting": null,
            "advancedFilter": filterData
        };

        model.search = requestData;

        var settings = {
            "url": url.searchJob,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.setJobSearchResultList = function (response) {
        base.handleResponse(response, function (response) {
            simplePagination.set(response.data);

            $cardJobSearchResult.bindText({
                totalCount: response.data.totalCount
            });
            $listJobSearchResult.empty();

            $.each(response.data.data, function (i, item) {
                item.index = (i + 1) + (response.data.pageIndex * response.data.pageSize);
            });

            var htmlJobProfileSimples = tmplJobProfileSimple.render(response.data.data);
            $listJobSearchResult.append(htmlJobProfileSimples);
            model.jobSearchResultList = model.jobSearchResultList.concat(response.data.data);
            model.jobSeeker.isEntitledApplyJob = response.data.additionalData.isEntitledApplyJob;

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

    base.submitJobApplication = function (target) {
        var $jobDetials = $(target).closest(".card-job-profile-detail");
        var jobProfileId = $jobDetials.data('entity-id');
        model.selectedJobProfileId = jobProfileId;
    };

    base.changeJobProfileModel = function (target, mode) {
        var jobProfileId = $(target).data('entity-id');
        var jobProfileModels = $.grep(model.jobSearchResultList, function (item) {
            return item.id === jobProfileId
        });

        if (mode === 0) {
            var htmlJobProfileSimples = tmplJobProfileSimple.render(jobProfileModels);
            $(target).replaceWith(htmlJobProfileSimples);
        }
        if (mode === 1) {
            $.each(jobProfileModels, function (i, item) {
                item.isEntitledApplyJob = model.jobSeeker.isEntitledApplyJob
            });

            var htmlJobProfileDetails = tmplJobProfileDetail.render(jobProfileModels);
            $(target).replaceWith(htmlJobProfileDetails);
        }
    };

    base.getModel = function () {
        return model;
    };

    base.setUserEvent = function () {
        $listJobSearchResult.on('click', '.card-job-profile-simple', function (e) {
            if ($(e.target).hasClass('btn-apply-job') === false
                && $(e.target).hasClass('btn-subcribe-apply-job') === false) {

                $listJobSearchResult.find('.card-job-profile-detail').each(function (i, target) {
                    if ($(i.target).hasClass('btn-apply-job') === false) {
                        base.changeJobProfileModel(this, 0);
                    }
                });

                base.changeJobProfileModel(this, 1);
            }

        });

        $listJobSearchResult.on('click', '.card-job-profile-detail', function (e) {
            if ($(e.target).hasClass('btn-apply-job') === false
                && $(e.target).hasClass('btn-subcribe-apply-job') === false) {
                base.changeJobProfileModel(this, 0);
            }
        });

        $listJobSearchResult.on('click', '.btn-apply-job', function () {
            base.submitJobApplication(this);
        });
    };

    var init = function () {

        base.setData(opts);
        base.setUserEvent();
        //base.getJobProfile(base.setJobProfile);
        base.submitJobSearchRequest(base.setJobSearchResultList);

    };

    init();
};

var pageJobSearch = new PageJobSearch({
    'url': {
        'searchJob': '/api/pro/job/search'
    }
});