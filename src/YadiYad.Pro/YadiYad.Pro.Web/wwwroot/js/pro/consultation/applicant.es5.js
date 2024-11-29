'use strict';

var PageConsultationapplicant = function PageConsultationapplicant(opts) {
    var base = this;
    var selector = ".content-consultation-candidates";
    var url = opts.url;
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var pagination = new SimplePagination();

    var model = opts.model || {
        consultationProfile: {},
        applicants: {
            totalCount: 0,
            data: [],
            selected: false
        },
        pagination: pagination,
        setCandidateResult: function setCandidateResult(result) {
            pagination.set(result);
            pagination.onPageChanged = function (pageIndex) {
                base.getConsultationInvitationList(base.setConsultationInvitationList, pageIndex);
            };
            if (result && result.data) {
                $.each(result.data, function (i, item) {
                    item.createdOnText = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                    item.updatedOnText = moment(item.updatedOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                    console.log(item);
                    $.each(item.consultantAvailableTimeSlots, function (i, item) {
                        item.startDateText = moment(item.startDate, 'YYYY-MM-DDTHH:mm').format('dddd');
                        item.startTimeText = moment(item.startDate, 'YYYY-MM-DDTHH:mm').format('hh:mm A');
                        item.endTimeText = moment(item.endDate, 'YYYY-MM-DDTHH:mm').format('hh:mm A');
                    });
                });
            }

            $.observable(this.applicants).setProperty(result);
            return false;
        },
        showApplicantDetail: function showApplicantDetail(applicant) {
            var activeAplicants = $.grep(this.applicants.data, function (item) {
                return item.showMore === true;
            });

            if (activeAplicants.length > 0) {
                $.observable(activeAplicants[0]).setProperty("showMore", false);
            }

            var selectedApplicants = $.grep(this.applicants.data, function (item) {
                return item.id === applicant.id;
            });

            if (selectedApplicants.length === 1) {
                $.observable(selectedApplicants[0]).setProperty("showMore", true);
            }

            return false;
        },
        selectApplicant: function selectApplicant(applicant) {
            var selectedApplicants = $.grep(this.applicants.data, function (item) {
                return item.id === applicant.id;
            });

            if (selectedApplicants.length === 1) {
                $.observable(selectedApplicants[0]).setProperty("selected", true);
                $.observable(this).setProperty("selectedApplicant", selectedApplicants[0]);
            }
            return false;
        }
    };

    //getter
    Object.defineProperty(base, 'model', {
        get: function get() {
            return model;
        }
    });

    //jsViews
    $.templates({
        tmplConsultationJobApplicant: "#tmpl-consultation-job-applicant"
    });

    var setData = function setData() {};

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
            filter: null,
            advancedFilter: {
                consultationInvitationStatuses: [2]
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

    //front-end response/action
    base.setConsultationInvitationList = function (response) {
        base.handleResponse(response, function (response) {
            model.setCandidateResult(response.data);
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

    //set user event
    var setUserEvent = function setUserEvent() {};

    //initilize page
    var init = function init() {
        setUserEvent();
        setData();
        $.templates.tmplConsultationJobApplicant.link(selector, model);
        base.getConsultationInvitationList(base.setConsultationInvitationList);
    };

    init();
};

var pageConsultationapplicant = new PageConsultationapplicant({
    url: {
        'getConsultationInvitationList': '/api/pro/consultationinvitation/organization/list',
        'inviteConsultationCondidates': '/api/pro/consultation/{{id}}/invite',
        'getConsultantPastJob': '/api/pro/service/{{id}}/consultation/reviewed'
    }
});

