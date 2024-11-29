var PageJobInvites = function (opts) {
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var base = this;
    var url = opts.url;
    var selector = ".content-job-invites";
    var weeklyScheduler = null;

    var $content = $('.content-job-invites');
    var tmplCardJobInvitesEmpty = $.templates("#template-card-job-invites-empty");

    $.templates({
        tmplContentJobInvite: "#tmpl-content-job-invites"
    });


    Object.defineProperty(base, 'model', {
        get() {
            return model;
        }
    });

    var pagination = new SimplePagination();

    pagination.onPageChanged = function (pageIndex) {
        base.loadJobInviteData(base.setJobInvitesList, pageIndex);
    }

    var model = opts.model || {
        jobInvitesList: {
            totalCount: 0,
            data: []
        },
        pagination: pagination,
        selectedJobInvite: [],
        setJobInvitesResult: function (result) {
            pagination.set(result)
            $.each(result.data, function (i, item) {
                item.createdOn =
                    moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm')
                    .add(timezone, 'hour')
                        .format('DD MMM YYYY');

                if (item.jobProfile) {
                    item.jobProfile.startDateText =
                        moment(item.jobProfile.startDate, 'YYYY-MM-DD')
                        .add(0, 'hour')
                        .format('DD MMM YYYY');
                }

                if (item.consultationProfile) {
                    item.ratePerSession = 100;
                    item.consultationProfile.timeSlots.forEach(function (timeSlot, i) {
                        var startDate = moment(timeSlot.startDate, 'YYYY-MM-DDTHH:mm');

                        var endDate = moment(timeSlot.endDate, 'YYYY-MM-DDTHH:mm');

                        timeSlot.startDateText = startDate.format('dddd');
                        timeSlot.startTimeText = startDate.format('hh:mm A');
                        timeSlot.endTimeText = endDate.format('hh:mm A');
                        timeSlot.start = startDate.toDate();
                        timeSlot.end = endDate.toDate();
                        timeSlot.isReadOnly = true;

                        timeSlot.selected = item.consultantAvailableTimeSlots.filter(function (selectedTimeSlot) {
                            return selectedTimeSlot.startDate === timeSlot.startDate
                                && selectedTimeSlot.endDate === timeSlot.endDate;
                        }).length > 0;
                    });

                    item.consultationProfile.timeSlots = item.consultationProfile.timeSlots.sort(function (a, b) {
                        return (a["start"] > b["start"]) ? 1 : ((a["start"] < b["start"]) ? -1 : 0);
                    });
                }
            });

            $.observable(this.jobInvitesList).setProperty("data", result.data);
            $.observable(this.jobInvitesList).setProperty("totalCount", result.totalCount);

            if (this.jobInvitesList.data && this.jobInvitesList.data.length > 0) {
                this.setSelectedJobInvite(this.jobInvitesList.data[0]);
            } else {
                $content.replaceWith(tmplCardJobInvitesEmpty);
            }

            return false;
        },
        setSelectedJobInvite: function (jobInvite) {
            var vm = this;
            console.log(jobInvite);
            var selectedJobInvites = $.grep(this.jobInvitesList.data, function (item) {
                return item.selected === true;
            });

            if (selectedJobInvites.length > 0) {
                $.observable(selectedJobInvites[0]).setProperty('showQuestionnaire', false);
                $.observable(selectedJobInvites[0]).setProperty('selected', false);
            }
            $.observable(this).setProperty('selectedJobInvite', null);
            $.observable(this).setProperty('selectedJobInvite', jobInvite);
            $.observable(jobInvite).setProperty('selected', true);

            if (jobInvite.consultantAvailableTimeSlots && jobInvite.consultantAvailableTimeSlots.length) {
                $.observable(vm.selectedJobInvite).setProperty('selectedTimeSlot', jobInvite.consultantAvailableTimeSlots);
            } else {
                $.observable(vm.selectedJobInvite).setProperty('selectedTimeSlot', []);
            }

            if (jobInvite.isApproved === false) {
                this.replyJobInvite(2);
            }
        },
        replyJobInvite: function (action, jobInvite) {
            if (this.selectedJobInvite.jobProfile) {
                swal({
                    icon: "warning",
                    title: action === 2 ? "Are You Sure to Accept?" : "Are You Sure to Decline?",
                    buttons: {
                        cancel: {
                            text: "Cancel",
                            value: false,
                            visible: true,
                            className: "btn-fail",
                            closeModal: true
                        },
                        confirm: {
                            text: "Confirm",
                            value: true,
                            visible: true,
                            className: "btn-success",
                            closeModal: true
                        }
                    }
                }).then(function (isConfirm) {
                    if (isConfirm) {
                        base.replyJobInvite(action, base.responseJobInviteUpdates);
                    }
                });
            }
            else if (this.selectedJobInvite.consultationProfile
                && this.selectedJobInvite.showQuestionnaire !== true) {
                $.observable(this.selectedJobInvite).setProperty('showQuestionnaire', true);
                //weeklyScheduler = new WeeklyScheduler('#calendar', true, "week");
                //weeklyScheduler.setSchedule(this.selectedJobInvite.consultationProfile.timeSlots, true);
                //weeklyScheduler.onChanged = function (e) {
                //    var noSchedule = Object.keys(weeklyScheduler.Calendar._controller.schedules.items).length;

                //    if ($content.find('[name="schedule"]').length) {
                //        $content.find('[name="schedule"]').val(noSchedule || "");
                //        $content.find('[name="schedule"]').valid();
                //    }
                //};
                //weeklyScheduler.onChanged();
                app.initFormComponent($('.form-job-invitation'));
            }
            else if (this.selectedJobInvite.consultationProfile
                && this.selectedJobInvite.showQuestionnaire === true) {
                if ($('.form-job-invitation').valid()) {
                    this.selectedJobInvite.consultantAvailableTimeSlots = [].concat(this.selectedJobInvite.selectedTimeSlot);

                    base.replyConsultationInvite(this.selectedJobInvite, base.responseJobInviteUpdates);
                }
            }
        },
        selectConsultationTimeSlot: function (timeSlot) {
            //js background update only
            var isSelected = $.grep(this.selectedJobInvite.selectedTimeSlot, function (item, i) {
                return JSON.stringify({
                    start: item.startDate,
                    end: item.endDate
                }).toLowerCase() === JSON.stringify({
                    start: timeSlot.startDate,
                    end: timeSlot.endDate
                }).toLowerCase()
            }).length === 0;

            if (isSelected) {
                this.selectedJobInvite.selectedTimeSlot.push(timeSlot);
            } else {
                this.selectedJobInvite.selectedTimeSlot = $.grep(this.selectedJobInvite.selectedTimeSlot, function (item, i) {
                    return JSON.stringify({
                        start: item.startDate,
                        end: item.endDate
                    }).toLowerCase() !== JSON.stringify({
                        start: timeSlot.startDate,
                        end: timeSlot.endDate
                    }).toLowerCase()
                });
            }
        }
    };

    //backed process
    base.loadJobInviteData = function (callback, pageIndex) {
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
            "url": url.getJobInvites,
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
            jobInvitationStatus: choice
        };
        var settings = {
            "url": url.replyJobInvite.format({ id: jobInviteId}),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.refresh = function () {
        base.loadJobInviteData(base.setJobInvitesList);
    };

    base.replyConsultationInvite = function (selectedJobInvite, callback) {
        var requestData = {
            questionnaireAnswer: selectedJobInvite.consultantReplys,
            consultantAvailableTimeSlots: selectedJobInvite.consultantAvailableTimeSlots,
            ratesPerSession: selectedJobInvite.ratesPerSession,
            consultationApplicationStatus: 2
        };

        //format questionnaire answer
        var questionnaireAnswer = {};
        $.each(selectedJobInvite.consultantReplys, function (key, item) {
            questionnaireAnswer[item.name] = item.answers;
        });
        requestData.questionnaireAnswer = JSON.stringify(questionnaireAnswer);
        var settings = {
            "url": url.replyConsultationInvite.format({ id: selectedJobInvite.id }),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.setJobInvitesList = function (response) {
        base.handleResponse(response, function (response) {
            model.setJobInvitesResult(response.data);
        });
    };

    base.responseJobInviteUpdates = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Pending for Organization's Review",
                buttons: {
                    confirm: 'OK'
                }
            }).then(function () {
                base.loadJobInviteData(base.setJobInvitesList);
                componentSeekerCounter && componentSeekerCounter.refresh();
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

    base.getModel = function () {
        return model;
    };

    //initital
    var init = function () {
        $.templates.tmplContentJobInvite.link(selector, model);
        base.loadJobInviteData(base.setJobInvitesList);
    };

    init();
};

var pageJobInvites = new PageJobInvites({
    'url': {
        'getJobInvites': '/api/pro/jobinvitation/invites',
        'replyJobInvite': '/api/pro/jobinvitation/{{id}}',
        'replyConsultationInvite': '/api/pro/consultationinvitation/{{id}}'
    }
});