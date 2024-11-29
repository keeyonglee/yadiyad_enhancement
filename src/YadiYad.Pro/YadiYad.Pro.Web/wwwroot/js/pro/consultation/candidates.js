var PageConsultationCandidates = function (opts) {
    var base = this;
    var selector = ".content-consultation-candidates";
    var url = opts.url;
    var pagination = new SimplePagination();

    pagination.onPageChanged = function (pageIndex) {
        base.getConsultationProfileCandidates(base.setConsultationProfileCandidates, pageIndex);
    }
    var model = opts.model || {
        consultationProfile: {},
        candidateResult: {
            totalCount: 0,
            data: [],
            selected: false
        },
        pagination: pagination,

        searchCandidates: function () {
            base.getConsultationProfileCandidates(base.setConsultationProfileCandidates);
        },
        setCandidateResult: function (result) {
            pagination.set(result)

            $.observable(this.candidateResult).setProperty(result);
            return false;
        },
        setCandidateSelected: function (candidatesSelected) {
            $.observable(this.candidateResult).setProperty("selected", candidatesSelected);
            return false;
        },
        inviteCandidates: function (selectedCandidate) {
            var selectedCandidateIds = [];
            if (selectedCandidate) {
                selectedCandidateIds.push(selectedCandidate.id);
            } else {
                var selectedCandidates = $.grep(this.candidateResult.data, function (item) {
                    return item.selected;
                });

                selectedCandidateIds = $.map(selectedCandidates, function (n, i) {
                    return n.id;
                });
            }

            base.inviteCandidates(selectedCandidateIds, base.inviteCandidatesResponse);
        },
        setCandidatePastJob: function (candidatesSelected, pastJobs) {
            var selectedCandidates = $.grep(this.candidateResult.data, function (item) {
                return item.id === candidatesSelected.id;
            });

            if (selectedCandidates.length === 1) {
                $.observable(selectedCandidates[0]).setProperty("pastJobs", pastJobs);
            }

            return false;
        },
        showMoreCandidateDetail: function (ev, eventArgs) {
            if ($(ev.target).is('[name="invited"]')) {
                return true;
            }

            $.observable(eventArgs.view.data).setProperty("showMore", eventArgs.view.data.showMore !== true);

            if (!eventArgs.view.data.pastJobs) {
                base.getConsultantPastJob(eventArgs.view.data, 0, base.setConsultantPastJob);
            }
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
        tmplConsultationCandidates: "#tmpl-consultation-candidates"
    });

    var setData = function () {
        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.id = parseInt(urlProp[urlProp.length - 2]) || 0;
    };

    //backend interaction
    base.getConsultationProfileCandidates = function (callback, pageIndex) {
        var queryModel = window.location.getUrlQueryParams();
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
                yearExperience: queryModel.yearexperience,
                categoryIds: [queryModel.category],
                expertiseIds: queryModel.expertises
            }
        };
        var settings = {
            "url": url.searchConsultationCondidates.format(model),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };

        if (model.id !== 0) {
            $.ajax(settings).done(callback);
        } else {
            callback(null);
        }
    };

    base.inviteCandidates = function (selectedCandidates, callback) {
        var settings = {
            "url": url.inviteConsultationCondidates.format(model),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(selectedCandidates)
        };

        $.ajax(settings).done(callback);
    };

    base.getConsultantPastJob = function (candidate, pageIndex, callback) {
        var requestData = {
            "offset": pageIndex,
            "recordSize": 10,
            "sorting": null,
            "advancedFilter": {}
        };

        var settings = {
            "url": url.getConsultantPastJob.format(candidate),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(function (response) {
            callback(response, candidate, pageIndex);
        });
    }

    //front-end response/action
    base.setConsultationProfileCandidates = function (response) {
        base.handleResponse(response, function (response) {
            model.setCandidateResult(response.data);
        });
    };

    base.inviteCandidatesResponse = function (response) {
        base.handleResponse(response, function (response) {

            swal({
                icon: "success",
                title: "Consultation Invitation is sent to Candidate",
                buttons: {
                    confirm: 'Back to search results'
                }
            }).then(function () {
                model.searchCandidates()
            });
        });
    };

    base.setConsultantPastJob = function (response, candidate) {
        base.handleResponse(response, function (response) {
            model.setCandidatePastJob(candidate, response.data);
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
        $.templates.tmplConsultationCandidates.link(selector, model);
        model.searchCandidates();


        $.observable(model).observeAll(function () {
            var candidatesSelected = model.candidateResult.data.filter(function (item) { return item.selected }).length !== 0;
            model.setCandidateSelected(candidatesSelected);
            console.log(candidatesSelected);
        });

    };

    init();
};

var pageConsultationCandidates = new PageConsultationCandidates({
    url: {
        'searchConsultationCondidates': '/api/pro/consultation/{{id}}/candidates',
        'inviteConsultationCondidates': '/api/pro/consultation/{{id}}/invite',
        'getConsultantPastJob': '/api/pro/service/{{id}}/consultation/reviewed'
    }
});