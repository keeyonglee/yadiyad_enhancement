var PageRequestService = function (opts) {
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var base = this;
    var url = opts.url;
    var model = {
        id: 0,
        engagementNo: null,
        engagementDurationType: 0,
        productTypeId: 0,
        referId: 0,
        engagement: null
    };

    var timeSheetScheduler = null;

    Object.defineProperty(base, 'timeSheetScheduler', {
        get() {
            return timeSheetScheduler;
        }
    });

    //DOM
    var $content = $('.content-payout-request');
    var $form = $content.find('.form-payout-request');
    model.id = $form.data('id');

    //load data
    base.loadModel = function () {
        if (window.pageServiceHires) {
            model.engagement = pageServiceHires.model.selectedServiceHires;
            model.productTypeId = pageServiceHires.model.productTypeId;
            model.engagementNo = model.engagement.code;
            model.refId = model.engagement.id;
            model.engagementDurationType = model.engagement.serviceProfileServiceTypeId;
        }
        else if (window.pageServiceConfirms) {
            model.engagement = pageServiceConfirms.model.selectedServiceConfirms;
            model.productTypeId = pageServiceConfirms.model.productTypeId;
            model.engagementNo = model.engagement.code;
            model.refId = model.engagement.id;
            model.engagementDurationType = model.engagement.serviceProfileServiceTypeId;
        }
        else if (window.pageJobApplications) {
            model.engagement = pageJobApplications.model.selectedJobApplication;
            model.productTypeId = pageJobApplications.model.productTypeId;
            model.engagementNo = model.engagement.code;
            model.refId = model.engagement.id;
            model.engagementDurationType = model.engagement.serviceProfileServiceTypeId;
        }
        else if (window.pageJobApplicant) {
            model.engagement = pageJobApplicant.model.selectedJobEngagement;
            model.productTypeId = pageJobApplicant.model.productTypeId;
            model.engagementNo = model.engagement.code;
            model.refId = model.engagement.id;
            model.engagementDurationType = model.engagement.serviceProfileServiceTypeId;
        }
        base.getPayoutRequest(base.onGetPayoutRequestResponse);
    };

    //load UI component
    base.loadUIComponent = function () {
        app.initFormComponent($form);
    };

    //backend integration
    base.getPayoutRequest = function (callback) {
        var settings = {
            "url": url.getPayoutRequest.format(model),
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.createPayoutRequest = function (callback, status) {
        var data = app.getFormValue($form);
        var timeSheet = base.getScheduleCollection();
        data.TimeSheetJson = JSON.stringify(timeSheet);
        data.refId = model.refId;
        data.productTypeId = model.productTypeId;
        data.id = model.id;

        if (status) {
            data.status = status;
        }

        var settings = {
            "url": url.createPayoutRequest,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    //set UI response
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

    base.onCreatePayoutRequestResponse = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "The new info is processed.",
                //text: 
            }).then(function () {
                var $modal = $content.closest('.modal');

                if ($modal) {
                    $modal.modal('hide');
                    var parentPage = null;

                    if (window.pageServiceHires) {
                        parentPage = pageServiceHires;
                    }
                    else if (window.pageServiceConfirms) {
                        parentPage = pageServiceConfirms;
                    }
                    else if (window.pageJobApplications) {
                        parentPage = pageJobApplications;
                    }
                    else if (window.pageJobApplicant) {
                        parentPage = pageJobApplicant;
                    }

                    (parentPage.reloadPayout
                        && parentPage.reloadPayout())
                        || parentPage.reloadSelectedEngagement();
                } else {
                    location.reload();
                }
            });
        });
    };

    base.onGetPayoutRequestResponse = function (response) {
        base.handleResponse(response, function (response) {
            var data = response.data;
            var timeSheetReadOnly = false;
            var timeSheetViewMode = "weekly";

            if (model.engagementDurationType === 2) {
                timeSheetViewMode = "monthly";
            }

            data.serviceEngagementNumber = model.engagementNo;
            app.setFormValue($form, data);

            if ((window.pageServiceHires || window.pageJobApplications)
                && (data.id === 0 || data.status === -2)) {
                $form.find('.btn-submit').removeClass('hidden');
            }
            else if ((window.pageServiceConfirms || window.pageJobApplicant)
                && (data.id !== 0 && data.status === 0)) {
                $form.find('.secProvider [name]').each(function (i, target) {
                    $(target).attr('readonly', true);
                });
                timeSheetReadOnly = true;
                $form.find('[name="attachmentDownloadId"]').closest('.nopui-file-attachment').addClass('readonly');
                $form.find('[name="attachmentDownloadId"]').closest('.nopui-file-attachment').find(".fa-file-alt").css('display', '');
                $form.find('[name="attachmentDownloadId"]').closest('.nopui-file-attachment').find(".lbl-file-name").css('display', 'none');
                $form.find('.btn-more-info').removeClass('hidden');
                $form.find('.btn-approve').removeClass('hidden');
            }
            else {
                $form.find('[name]').each(function (i, target) {
                    $(target).attr('readonly', true);
                });
                timeSheetReadOnly = true;
                $form.find('[name="attachmentDownloadId"]').closest('.nopui-file-attachment').addClass('readonly');
                $form.find('[name="attachmentDownloadId"]').closest('.nopui-file-attachment').find(".fa-file-alt").css('display', '');
                $form.find('[name="attachmentDownloadId"]').closest('.nopui-file-attachment').find(".lbl-file-name").css('display', 'none');
                $form.find('.btn-cancel').text('Back');
            }

            timeSheetScheduler = new WeeklyScheduler(
                '#timesheet',
                timeSheetReadOnly,
                timeSheetViewMode
            );

            //set timesheet
            if (data.workingTimeSlots) {
                data.workingTimeSlots.forEach(function (timeSlot, i) {
                    timeSlot.start = moment(timeSlot.startDateTime, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').toDate();
                    timeSlot.end = moment(timeSlot.endDateTime, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').toDate();
                    timeSlot.isReadOnly = timeSheetReadOnly;
                });

                timeSheetScheduler.setSchedule(data.workingTimeSlots, true);
            }

            //set enabled data range and navigate to start date
            if (data.startDate && data.endDate) {
                var calendarStartDate = moment(data.startDate, 'YYYY-MM-DDTHH:mm').toDate();
                var calendarEndDate = moment(data.endDate, 'YYYY-MM-DDTHH:mm').toDate();
                timeSheetScheduler.setEnabledDateRange(timeSheetViewMode, calendarStartDate, calendarEndDate);
            }

            //set prorated UI response
            if (!data.isProrated) {
                $form.find('.secProrated').hide();
            }
            else {
                var $proratedWorkDuration = $form.find('.secProrated').find('[name="proratedWorkDuration"]');
                var $proratedPayout = $form.find('.secProrated').find('[name="proratedPayout"]');

                $proratedWorkDuration
                    .on('change', function () {
                        var maxMonthlyWorkDuration = 0.00;
                        var feePerDuration = (model.engagement.serviceProfileServiceFee || model.engagement.payAmount)
                            + (model.engagement.serviceProfileOnsiteFee || 0);
                        if (model.engagement.serviceProfileServiceTypeId === 1
                            || model.engagement.jobType === 1) {
                            maxMonthlyWorkDuration = new Decimal((model.engagement.required || model.engagement.jobRequired)).times(4).dividedBy(2);
                        }
                        else if (model.engagement.serviceProfileServiceTypeId === 2
                            || model.engagement.jobType === 2) {
                            maxMonthlyWorkDuration = new Decimal((model.engagement.required || model.engagement.jobRequired)).dividedBy(2);
                        }

                        var proratedWorkDuration = parseFloat($proratedWorkDuration.val() || 0);
                        var proratedPayout = (new Decimal(feePerDuration).times(maxMonthlyWorkDuration)).times(new Decimal(proratedWorkDuration).dividedBy(maxMonthlyWorkDuration));

                        $proratedPayout.val(proratedPayout.toFixed(2));
                    });

                $proratedWorkDuration.trigger('change');

            }

            if (data.jobMilestoneId) {
                $form.find('.secNonProjectBased').hide();
            } else {
                $form.find('.secProjectBased').hide();
            }

            if (data.remarks && data.remarks.length) {
                var remarksHistory = "";

                $.each(data.remarks, function (i, item) {
                    var remarkDate = moment(item.remarkDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY hh:mm A');

                    remarksHistory += (remarksHistory ? "<br/>" : "") + remarkDate +" - "+ item.actorName + ":" + " " + item.remark
                });
                $form.find('.remarks-history').html(remarksHistory);
                $form.find('.remarks-history').closest('.form-group').removeClass('hidden');
            }

            if (data.id && data.statusName) {
                var classStatus =
                    data.status === -3 ? "badge-gray"
                        : data.status === -2 ? "badge-secondary"
                            : data.status === -1 ? "badge-danger"
                                : data.status === 0 ? "badge-gray"
                                    : data.status === 1 ? "badge-success"
                                        : data.status === 2 ? "badge-primary"
                                : "";
                $form.find('.secStatus .badge-status').addClass(classStatus);
                $form.find('.secStatus .badge-status').text(data.statusName);

                $form.find('.secStatus').removeClass('hidden');
            }

        });
    };

    base.getScheduleCollection = function () {
        var schedules = [];

        $.each(timeSheetScheduler.Calendar._controller.schedules.items, function (i, item) {
            var schedule = {
                startDateTime: item.start.toDate(),
                endDateTime: item.end.toDate()
            };

            schedules.push(schedule);
        });

        return schedules;
    };

    //set user event
    base.setUserEvent = function () {
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }
            base.createPayoutRequest(base.onCreatePayoutRequestResponse, 0);
        });
        $form.find(".btn-more-info").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }
            base.createPayoutRequest(base.onCreatePayoutRequestResponse, -2);
        });
        $form.find(".btn-approve").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }
            base.createPayoutRequest(base.onCreatePayoutRequestResponse, 1);
        });
    };

    //inititalize page
    var init = function () {
        base.loadModel();
        base.loadUIComponent();
        base.setUserEvent();
    };

    init();
};

//set init param
var pageRequestService = new PageRequestService({
    'url': {
        'createPayoutRequest': '/api/pro/payoutrequest',
        'getPayoutRequest': '/api/pro/payoutrequest/{{id}}?refId={{refId}}&productTypeId={{productTypeId}}'
    }
});