var PageConsultationInvited = function (opts) {
    var base = this;
    var selector = ".content-consultation-candidates";
    var url = opts.url;
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var pagination = new SimplePagination();
    pagination.onPageChanged = function (pageIndex) {
        base.getConsultationInvitationList(base.setConsultationInvitationList, pageIndex);
    }
    var model = opts.model || {
        consultationProfile: {},
        invitedCandidates: {
            totalCount: 0,
            data: [],
            selected: false
        },
        pagination: pagination,

        setCandidateResult: function (result) {
            pagination.set(result)

            if (result && result.data) {
                $.each(result.data, function (i, item) {
                    item.createdOnText = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                    item.declineReasons = item.declineReasons && item.declineReasons.replace(/\n/g, '<br/>');
                });
            }

            $.observable(this.invitedCandidates).setProperty(result);
            return false;
        }
    };

    //getter
    Object.defineProperty(base, 'model', {
        get() {
            return model;
        }
    });

  

    //jsViews
    $.templates({
        tmplConsultationJobInvited: "#tmpl-consultation-job-invited"
    });

    var setData = function () {
    };

    //backend interaction
    base.getConsultationInvitationList = function (callback, pageIndex) {
        var filterData = {
            keyword: ""
        };

        var recordSize = 10;
        var offset = pageIndex ? pageIndex * recordSize : 0;

        var data = {
            offset: offset,
            recordSize: recordSize,
            filter: filterData.keyword,
            advancedFilter: {
                consultationInvitationStatuses: [1, 3, 8]
            }
        };
        var settings = {
            "url": url.getConsultationInvitationList,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.updateOrganizationConsultationInvitationRead = function (consultationJob, callback) {
        if (consultationJob.isOrganizationRead) {
            return true;
        }
        consultationJob.isOrganizationRead = true;
        var settings = {
            "url": url.updateOrganizationConsultationInvitationRead.format(consultationJob),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            background: true
        };

        $.ajax(settings).done(callback);
    };

    //front-end response/action
    base.setConsultationInvitationList = function (response) {
        base.handleResponse(response, function (response) {
            model.setCandidateResult(response.data);
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

    //set user event
    var setUserEvent = function () {
    };

    //initilize page
    var init = function () {
        setUserEvent();
        setData();
        $.templates.tmplConsultationJobInvited.link(selector, model);
        base.getConsultationInvitationList(base.setConsultationInvitationList);

    };

    init();
};

var pageConsultationInvited = new PageConsultationInvited({
    url: {
        'getConsultationInvitationList': '/api/pro/consultationinvitation/organization/list',
        'inviteConsultationCondidates': '/api/pro/consultation/{{id}}/invite',
        'getConsultantPastJob': '/api/pro/service/{{id}}/consultation/reviewed',
        'updateOrganizationConsultationInvitationRead': '/api/pro/consultationinvitation/{{id}}/org/read'
    }
});