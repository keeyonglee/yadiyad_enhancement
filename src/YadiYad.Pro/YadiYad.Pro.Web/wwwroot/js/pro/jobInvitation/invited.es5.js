"use strict";

var PageJobInvited = function PageJobInvited(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.jobInvitedList = [];
    model.selectedJobInvite = null;
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var pagination = new SimplePagination('.pagination-jobinvitation-invited-list');
    var tmplCardJobInviteEmpty = $.templates("#template-card-job-invite-empty");

    var tmplCardJobInvite = $.templates("#template-card-job-invite");
    var tmplCardJobInviteResponseDetail = $.templates("#template-card-job-invite-response-detail");
    var tmplCardJobInviteResponseEmpty = $.templates("#template-content-job-invite-response-empty");
    var tmplCardJobInviteListEmpty = $.templates("#template-card-job-invite-empty");

    var $content = $('.content-job-search');
    var $listJobInvited = $content.find(".list-job-invited");

    pagination.onPageChanged = function (pageIndex) {
        base.loadJobInviteData(base.setJobInvitedList, pageIndex);
    };

    base.setData = function () {
        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.jobId = parseInt(urlProp[urlProp.length - 2]) || 0;
    };

    base.updateOrganizationJobInvitationRead = function (jobApplication, callback) {
        if (jobApplication.isRead) {
            return true;
        }
        jobApplication.isRead = true;
        var settings = {
            "url": url.updateOrganizationJobInvitationRead.format(jobApplication),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            background: true
        };

        $.ajax(settings).done(callback);
    };

    base.updateOrganizationJobInvitationReadResponse = function () {};
    //backed process
    base.loadJobInviteData = function (callback, pageIndex) {
        var filterData = {
            keyword: "",
            jobProfileId: model.jobId
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
            "url": url.getJobInvited,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.replyJobInvite = function (choice, callback) {
        var jobInviteId = model.selectedJobInvite.id;
        var requestData = {
            JobInvitationStatus: choice
        };
        var settings = {
            "url": url.replyJobInvite.format({ id: jobInviteId }),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.setJobInvitedList = function (response) {
        base.handleResponse(response, function (response) {
            $content.bindText(response.data);

            $.each(response.data.data, function (i, item) {
                if (item.serviceIndividualProfile && item.serviceIndividualProfile.dateOfBirth !== null) {
                    item.serviceIndividualProfile.dateOfBirth = moment(item.serviceIndividualProfile.dateOfBirth, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                }
                if (item.jobSeekerProfile.tenureStart !== null) {
                    item.jobSeekerProfile.tenureStart = moment(item.jobSeekerProfile.tenureStart, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                }
                if (item.jobSeekerProfile.tenureEnd !== null) {
                    item.jobSeekerProfile.tenureEnd = moment(item.jobSeekerProfile.tenureEnd, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                }
                item.createdOn = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                item.startDate = moment(item.jobProfile.startDate, 'YYYY-MM-DD').add(0, 'hour').format('DD MMM YYYY');

                item.jobSeekerProfile.jobTypes = [];

                if (item.jobSeekerProfile.isFreelanceDaily) {
                    item.jobSeekerProfile.jobTypes.push("Freelance (Daily)");
                }

                if (item.jobSeekerProfile.isFreelanceHourly) {
                    item.jobSeekerProfile.jobTypes.push("Freelance (Hourly)");
                }

                if (item.jobSeekerProfile.isProjectBased) {
                    item.jobSeekerProfile.jobTypes.push("Project-Based");
                }

                item.jobSeekerProfile.jobTypeNames = item.jobSeekerProfile.jobTypes.join(', ');

                item.jobSeekerProfile.jobModels = [];

                if (item.jobSeekerProfile.isOnSite) {
                    item.jobSeekerProfile.jobModels.push("Onsite");
                }

                if (item.jobSeekerProfile.isPartialOnSite) {
                    item.jobSeekerProfile.jobModels.push("Partial Onsite");
                }

                if (item.jobSeekerProfile.isRemote) {
                    item.jobSeekerProfile.jobModels.push("Remote");
                }

                item.jobSeekerProfile.jobModelNames = item.jobSeekerProfile.jobModels.join(', ');
            });

            if (!model.selectedJobInvite && response.data.data && response.data.data.length > 0) {
                model.selectedJobInvite = response.data.data[0];
                base.updateOrganizationJobInvitationRead(model.selectedJobInvite, base.updateOrganizationJobInvitationReadResponse);
                model.selectedJobInvite.isSelected = true;
                var htmlCardJobInviteResponseDetai = tmplCardJobInviteResponseDetail.render(model.selectedJobInvite);
                $content.find('.content-job-invited').replaceWith(htmlCardJobInviteResponseDetai);
            }

            if (response.data.data.length > 0) {
                var htmlCardJobInvite = tmplCardJobInvite.render(response.data.data);
                $listJobInvited.empty();
                pagination.set(response.data);
                $listJobInvited.append(htmlCardJobInvite);
                model.jobInvitedList = model.jobInvitedList.concat(response.data.data);
            } else {
                $content.replaceWith(tmplCardJobInviteEmpty);

                $listJobInvited.empty();
                pagination.set(response.data);
                $listJobInvited.append(tmplCardJobInviteListEmpty);
                $content.find('.content-job-invited').replaceWith(tmplCardJobInviteResponseEmpty);
            }
        });
    };

    base.setJobInviteResponse = function (target) {
        var jobInviteId = $(target).data('entity-id');
        var jobInviteModel = $.grep(model.jobInvitedList, function (item) {
            return item.id === jobInviteId;
        });
        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        model.selectedJobInvite = jobInviteModel[0];
        base.updateOrganizationJobInvitationRead(model.selectedJobInvite, base.updateOrganizationJobInvitationReadResponse);
        model.selectedJobInvite.isSelected = true;
        var htmlCardJobInviteResponseDetai = tmplCardJobInviteResponseDetail.render(model.selectedJobInvite);
        $content.find('.content-job-invited').replaceWith(htmlCardJobInviteResponseDetai);
    };

    base.responseJobInviteUpdates = function (response) {
        base.handleResponse(response, function (response) {
            $listJobInvited.empty();
            model.selectedJobInvite = null;
            base.loadJobInviteData(base.setJobInvitedList);
        });
    };

    base.handleResponse = function (response, successHandler) {
        if (response && response.status && response.status.code === 1) {
            successHandler(response);
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    //handle user event
    base.setUserEvent = function () {
        $listJobInvited.on('click', '.card-job-invite', function () {
            base.setJobInviteResponse(this);
        });
    };

    //initital
    var init = function init() {
        base.setData();
        base.loadJobInviteData(base.setJobInvitedList);
        base.setUserEvent();
    };

    init();
};

var pageJobInvited = new PageJobInvited({
    'url': {
        'getJobInvited': '/api/pro/jobinvitation/inviteds',
        'replyJobInvite': '/api/pro/jobinvitation/{{id}}',
        'updateOrganizationJobInvitationRead': '/api/pro/jobinvitation/{{id}}/org/read'
    }
});

