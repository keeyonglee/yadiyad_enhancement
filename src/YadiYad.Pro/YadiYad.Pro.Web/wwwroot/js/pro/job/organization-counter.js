var ComponentOrganizationJobCounter = function (containerSelector, opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};

    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var $container = $(containerSelector);
    var $jobProfileTitle = $('.job-profile-title');
    var $jobProfileInfo = $('.job-profile-info');
    var $jobProfileExpiredInfo = $('.section-expired-info');
    var tmplJobEngagementHeader = $.templates('#tmpl-job-engagement-header');
    var tmplJobEngagementTitle = $.templates('#tmpl-job-engagement-title');

    base.setData = function (opts) {
        model = opts.model || {};
        url = opts.url;
        
        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.jobId = parseInt(urlProp[urlProp.length - 2]) || 0;
    };

    base.loadOrganizationJobItemCounter = function (callback) {
        var settings = {
            "background": true,
            "url": url.getOrganizationJobItemCounter.format(model),
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.getJobProfileInfo = function (callback) {
        var settings = {
            "background": true,
            "url": url.getJobProfileInfo.format(model),
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setOrganizationJobItemCounter = function (response) {
        base.handleResponse(response, function (response) {
            $container.bindText(response.data);
        });
    };

    base.setJobProfileInfo = function (response) {
        base.handleResponse(response, function (response) {
            if (response.data.status !== 1) {
                window.location.href = '/pro/jobapplication/hired';
            }

            if (response.data.viewJobCandidateFullProfileSubscriptionEndDate) {
                response.data.viewJobCandidateFullProfileSubscriptionEndDateText
                    = moment(response.data.viewJobCandidateFullProfileSubscriptionEndDate, 'YYYY-MM-DD')
                        .add(timezone, 'hour')
                        .format('DD MMM YYYY');
            }
            var htmlJobEngagementHeader  = tmplJobEngagementHeader.render(response.data);
            $jobProfileInfo.empty();
            $jobProfileInfo.append(htmlJobEngagementHeader);

            var htmlJobEngagementTitle  = tmplJobEngagementTitle.render(response.data);
            $jobProfileTitle.empty();
            $jobProfileTitle.append(htmlJobEngagementTitle);
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

    base.refresh = function () {
        base.loadOrganizationJobItemCounter(base.setOrganizationJobItemCounter);
    };

    var init = function () {
        base.setData(opts);
        base.loadOrganizationJobItemCounter(base.setOrganizationJobItemCounter);
        base.getJobProfileInfo(base.setJobProfileInfo);
    };

    init();
};

var componentOrganizationJobCounter = new ComponentOrganizationJobCounter(
    '.content-organization-job-counter',
    {
        'url': {
            'getOrganizationJobItemCounter': '/api/pro/job/{{jobId}}/org/counter',
            'getJobProfileInfo': '/api/pro/job/{{jobId}}/info'
        }
    }
);