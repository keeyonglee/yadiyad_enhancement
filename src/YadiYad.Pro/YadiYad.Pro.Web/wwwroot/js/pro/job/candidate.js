var PageJobCandidate = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.jobCandidateList = [];
    model.selectedJobCandidate = null;
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var pagination = new SimplePagination('.pagination-job-candidate-list');
    var tmplCardJobCandidateEmpty = $.templates("#template-card-job-candidate-empty");

    var tmplCardJobCandidate = $.templates("#template-card-job-candidate");
    var tmplCardJobCandidateResponseDetail = $.templates("#template-card-job-candidate-response-detail");
    var tmplCardJobCandidateResponseEmpty = $.templates("#template-content-job-candidate-response-empty");
    var tmplCardJobCandidateListEmpty = $.templates("#template-card-job-candidate-empty");


    var $content = $('.content-job-search');
    var $listJobCandidate = $content.find(".list-job-candidate");

    pagination.onPageChanged = function (pageIndex) {
        base.loadJobCandidateData(base.setJobCandidateList, pageIndex);
    }

    base.getModel = function () {
        return model;
    }

    base.setData = function () {
        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.id = parseInt(urlProp[urlProp.length - 2]) || 0;
        model.selectedJobApplicationId = model.id;
    };

    //backed process
    base.loadJobCandidateData = function (callback, pageIndex) {
        var filterData = {
            JobProfileId: model.id
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
            "url": url.searchJobCandidate,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };



    //front-end response
    base.setJobCandidateList = function (response) {
        base.handleResponse(response, function (response) {
            $content.bindText(response.data);

            $.each(response.data.data, function (i, item) {
                if (item.dob !== null) {
                    item.dob = moment(item.dob, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                }
                if (item.tenureStart !== null) {
                    item.tenureStart = moment(item.tenureStart, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                }
                if (item.tenureEnd !== null) {
                    item.tenureEnd = moment(item.tenureEnd, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                }
                if (model.viewJobCandidateFullProfileSubscriptionEndDays <= 0) {
                    item.name = item.code;
                }

                item.showFullProfile = response.data.additionalData.showFullProfile;

                item.jobTypes = [];

                if (item.isFreelanceDaily) {
                    item.jobTypes.push("Freelance (Daily)");
                }

                if (item.isFreelanceHourly) {
                    item.jobTypes.push("Freelance (Hourly)");
                }

                if (item.isProjectBased) {
                    item.jobTypes.push("Project-Based");
                }

                item.jobTypeNames = item.jobTypes.join(', ');

                item.jobModels = [];

                if (item.isOnSite) {
                    item.jobModels.push("Onsite");
                }

                if (item.isPartialOnSite) {
                    item.jobModels.push("Partial Onsite");
                }

                if (item.isRemote) {
                    item.jobModels.push("Remote");
                }

                item.jobModelNames = item.jobModels.join(', ');

                item.index = (i + 1) + (response.data.pageIndex * response.data.pageSize);

            });
            if (response.data.additionalData.showFullProfile !== true) {
                $('.section-subscription-pay').removeClass('hidden');
            }

            if (!model.selectedJobCandidate
                && response.data.data
                && response.data.data.length > 0) {
                model.selectedJobCandidate = response.data.data[0]
                model.selectedJobCandidate.isSelected = true;
                var htmlCardJobCandidateResponseDetai = tmplCardJobCandidateResponseDetail.render(model.selectedJobCandidate);
                $content.find('.content-job-candidate').replaceWith(htmlCardJobCandidateResponseDetai);
                $content.find('.content-job-candidate').on('click', '.btn-invite', function () {
                    base.inviteCandidate(this, base.responseInviteCandidate);
                });

            }

            if (response.data.data.length > 0) {
                var htmlCardJobCandidate = tmplCardJobCandidate.render(response.data.data);
                $listJobCandidate.empty();
                pagination.set(response.data);
                $listJobCandidate.append(htmlCardJobCandidate);
                model.jobCandidateList = model.jobCandidateList.concat(response.data.data);
            } else {
                $content.replaceWith(tmplCardJobCandidateEmpty);

                $listJobCandidate.empty();
                pagination.set(response.data);
                $listJobCandidate.append(tmplCardJobCandidateListEmpty);
                $content.find('.content-job-candidate').replaceWith(tmplCardJobCandidateResponseEmpty);
            }
        });
    };

    base.setJobCandidateResponse = function (target) {
        var jobCandidateId = $(target).data('entity-id');
        var jobCandidateModel = $.grep(model.jobCandidateList, function (item) {
            return item.id === jobCandidateId
        });
        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        model.selectedJobCandidate = jobCandidateModel[0];
        model.selectedJobCandidate.isSelected = true;
        var htmlCardJobCandidateResponseDetai = tmplCardJobCandidateResponseDetail.render(model.selectedJobCandidate);
        $content.find('.content-job-candidate').replaceWith(htmlCardJobCandidateResponseDetai);
        $content.find('.content-job-candidate').on('click', '.btn-invite', function () {
            base.inviteCandidate(this, base.responseInviteCandidate);
        });

    };

    base.inviteCandidate = function (target, callback) {
        var jobSeekerProfileId = $(target).closest('.content-job-candidate').data('entity-id');
        var data = {};
        data.jobSeekerProfileId = jobSeekerProfileId;
        data.jobProfileId = model.id;
        var settings = {
            "url": url.inviteCandidate,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responseInviteCandidate = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Job Invitation sent to Candidate",
                //text: 
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

    //handle user event
    base.setUserEvent = function () {
        $listJobCandidate.on('click', '.card-job-candidate', function () {
            base.setJobCandidateResponse(this);
        });


    };

    //initital
    var init = function () {
        base.setData();
        base.loadJobCandidateData(base.setJobCandidateList);
        base.setUserEvent();
    };

    init();
};

var pageJobCandidate = new PageJobCandidate({
    'url': {
        'searchJobCandidate': '/api/pro/job/candidate',
        'inviteCandidate': '/api/pro/jobInvitation/invite',
        'getJobProfile': '/api/pro/job/{{id}}'
    }
});