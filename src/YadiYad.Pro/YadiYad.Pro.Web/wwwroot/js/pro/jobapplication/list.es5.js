'use strict';

var PageJobApplications = function PageJobApplications(opts) {
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var base = this;
    var url = opts.url;
    var msg = opts.msg;
    var selector = ".content-job-application";
    var weeklyScheduler = null;

    var $form = $('#form-job-list');
    var $content = $('.content-job-application');
    var dtPayoutRequest = null;

    $.templates({
        tmplContentJobApplication: "#tmpl-content-job-application"
    });

    Object.defineProperty(base, 'model', {
        get: function get() {
            return model;
        }
    });

    var pagination = new SimplePagination();

    pagination.onPageChanged = function (pageIndex) {
        base.loadJobApplicationData(base.setJobApplicationsList, pageIndex);
    };

    var model = opts.model || {
        productTypeId: 5,
        jobApplicationsList: {
            totalCount: 0,
            data: []
        },
        pagination: pagination,
        selectedJobApplication: null,
        setJobApplicationsResult: function setJobApplicationsResult(result) {
            pagination.set(result);
            console.log(result);
            $.each(result.data, function (i, item) {
                item.createdOn = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');

                if (item.jobProfile) {
                    item.jobProfile.startDateText = moment(item.jobProfile.startDate, 'YYYY-MM-DD').add(0, 'hour').format('DD MMM YYYY');
                    if (item.startDate !== null) {
                        item.startDateText = moment(item.startDate, 'YYYY-MM-DD').format('DD MMM YYYY');
                    }
                    if (item.endDate !== null) {
                        item.endDateText = moment(item.endDate, 'YYYY-MM-DD').format('DD MMM YYYY');
                    }

                    if (item.jobType === 3 && item.jobMilestones.length > 0) {
                        item.startMilestoneText = item.jobMilestones[0].description;

                        if (item.endMilestoneId !== null) {
                            var endMilestone = item.jobMilestones.filter(function (e) {
                                return e.id === item.endMilestoneId;
                            });
                            item.endMilestoneText = endMilestone[0].description;
                        }
                    }
                }

                if (item.consultationProfile) {
                    if (item.appointmentStartDate && item.appointmentEndDate) {
                        item.appointmentDateText = moment(item.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('YYYY-MM-DD (ddd)');
                        item.appointmentStartText = moment(item.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('h:mmA');
                        item.appointmentEndText = moment(item.appointmentEndDate, 'YYYY-MM-DDTHH:mm').format('h:mmA');
                    } else {
                        if (item.consultantAvailableTimeSlots) {
                            item.consultantAvailableTimeSlots.forEach(function (timeSlot, i) {
                                timeSlot.startDateText = moment(timeSlot.startDate, 'YYYY-MM-DDTHH:mm').format('dddd');
                                timeSlot.startTimeText = moment(timeSlot.startDate, 'YYYY-MM-DDTHH:mm').format('hh:mmA');
                                timeSlot.endTimeText = moment(timeSlot.endDate, 'YYYY-MM-DDTHH:mm').format('hh:mmA');
                                timeSlot.start = moment(timeSlot.startDate, 'YYYY-MM-DDTHH:mm').toDate();
                                timeSlot.end = moment(timeSlot.endDate, 'YYYY-MM-DDTHH:mm').toDate();
                                timeSlot.isReadOnly = true;
                            });
                        }
                    }
                }
            });

            $.observable(this.jobApplicationsList).setProperty("data", result.data);
            $.observable(this.jobApplicationsList).setProperty("totalCount", result.totalCount);

            if (this.jobApplicationsList.data && this.jobApplicationsList.data.length > 0) {
                this.setSelectedJobApplication(this.jobApplicationsList.data[0]);
            }

            return false;
        },
        setSelectedJobApplication: function setSelectedJobApplication(jobApplication) {
            var selectedJobApplications = $.grep(this.jobApplicationsList.data, function (item) {
                return item.selected === true;
            });

            if (selectedJobApplications.length > 0) {
                $.observable(selectedJobApplications[0]).setProperty('selected', false);
            }
            $.observable(this).setProperty('selectedJobApplication', null);
            $.observable(this).setProperty('selectedJobApplication', jobApplication);
            $.observable(jobApplication).setProperty('selected', true);

            if (jobApplication.jobProfile) {
                base.updateJobSeekerRead(jobApplication, base.updateJobSeekerReadResponse);
                base.getDepositPayoutDetail(base.setDepositPayoutDetail);
                base.loadDataTable();
            }

            if (jobApplication.consultationProfile) {
                base.updateConsultantRead(jobApplication, base.updateConsultantReadResponse);
                base.loadDataTable();
            }
        }
    };

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    //backed process
    base.loadJobApplicationData = function (callback, pageIndex) {
        var filterData = {
            keyword: ""
        };

        var $dateRangeFilter = $content.find('[name="dateRangeFilter"]');
        var dateRangeFilter = $dateRangeFilter.data('daterangepicker');

        if ($dateRangeFilter.length && $dateRangeFilter.val()) {
            filterData.startDate = dateRangeFilter.startDate;
            filterData.endDate = dateRangeFilter.endDate;
        }

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
            "url": url.getJobApplications,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.updateJobSeekerRead = function (jobApplication, callback) {
        if (jobApplication.isRead) {
            return true;
        }
        jobApplication.isRead = true;
        var settings = {
            "url": url.updateJobSeekerApplicationRead.format(jobApplication),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            background: true
        };

        $.ajax(settings).done(callback);
    };

    base.updateConsultantRead = function (jobApplication, callback) {
        if (jobApplication.isRead) {
            return true;
        }
        jobApplication.isRead = true;
        var settings = {
            "url": url.updateConsultantRead.format(jobApplication),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            background: true
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.getDepositPayoutDetail = function (callback) {
        var settings = {
            "url": url.getDepositPayoutDetail.format({
                id: model.selectedJobApplication.id
            }),
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };
        $.ajax(settings).done(callback);
    };

    base.setDepositPayoutDetail = function (response) {
        base.handleResponse(response, function (response) {
            response.data.totalRemaining = response.data.deposit.totalAmount - response.data.payout.totalAmount;
            if (response.data.deposit.nextDueDate !== null) {
                response.data.deposit.formattedNextDueDate = moment.utc(response.data.deposit.nextDueDate).local().format("DD/MM/YYYY");
            }
            $.observable(model.selectedJobApplication).setProperty('depositPayout', response.data);
        });
    };

    base.setJobApplicationsList = function (response) {
        base.handleResponse(response, function (response) {
            model.setJobApplicationsResult(response.data);
        });
    };

    base.updateJobSeekerReadResponse = function (response) {
        base.handleResponse(response, function (response) {
            componentSeekerCounter && componentSeekerCounter.refresh();
        });
    };

    base.updateConsultantReadResponse = function (response) {
        base.handleResponse(response, function (response) {
            componentSeekerCounter && componentSeekerCounter.refresh();
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

    base.loadDataTable = function () {
        if (dtPayoutRequest) {
            dtPayoutRequest.destroy();
        }

        var cols = [{ "data": "createdDateTime" }, { "data": "payoutCycle" }, { "data": "formattedAmount" }, { "data": "statusName" }, { "data": "lastRemark" }, { "data": "action" }];

        dtPayoutRequest = $content.find(".tbl-payout-request").DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": msg.keywordSearch,
                "lengthMenu": "_MENU_",
                "info": msg.info,
                "search": "",
                "emptyTable": "No Payout Request is created"
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": (model.selectedJobApplication.consultationProfile ? url.getConsultationPayouRequests : url.getPayouRequests).format({
                    id: model.selectedJobApplication.id
                }),
                "type": "post",
                "data": function data(_data) {
                    _data.customFilter = {};
                },
                "dataFilter": function dataFilter(data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;

                    var allowedRaisePayout = result.data.additionalData.allowedRaisePayout;

                    if (allowedRaisePayout) {
                        $content.find('.btn-new-payoutrequest').removeClass('hidden');
                    } else {
                        $content.find('.btn-new-payoutrequest').addClass('hidden');
                    }

                    dataList.forEach(function (item) {
                        var urlGetPayouRequest = url.getPayouRequest.replace(/{{id}}/g, item.id);
                        var buttonText = item.status === -2 ? "Edit" : "View";

                        item.action = item.endDate || item.jobMilestonePhase ? '<input class="btn btn-primary w-auto d-inline-block" value="' + buttonText + '" type="button"' + 'data-toggle="modal" data-target="#modal-payout-request"   ' + 'data-keyboard="false" data-backdrop="static"              ' + 'href="' + urlGetPayouRequest + '" />                      ' : "";

                        if (item.status === 2) {
                            var urlGetInvoiceStatement = url.getPayoutInvoiceStatement.replace(/{{id}}/g, item.invoiceId);
                            item.action += item.action ? "<br/>" : "";

                            item.action += '<a class="text-primary" target="_blank"' + 'href="' + urlGetInvoiceStatement + '">[Invoice]</a>                      ';

                            if (item.serviceChargeInvoiceId) {

                                var urlGetServiceChargeInvoice = url.getServiceChargeInvoiceStatement.replace(/{{id}}/g, item.serviceChargeInvoiceId);
                                item.action += '<br/><a class="text-primary" target="_blank"' + 'href="' + urlGetServiceChargeInvoice + '">[Escrow Invoice]</a>';
                            }
                        }

                        item.createdDateTime = moment.utc(item.createdOnUTC).local().format("DD/MM/YYYY");

                        if (item.endDate) {
                            var endDate = moment.utc(item.endDate).local();

                            item.payoutCycle = endDate.format("YYYY MMM") + " #" + (endDate.toDate().getDate() <= 15 ? 1 : 2);
                        } else if (item.jobMilestonePhase) {
                            item.payoutCycle = "Phase " + item.jobMilestonePhase + " - " + item.jobMilestoneName;
                        } else {
                            item.payoutCycle = "";
                        }
                        item.formattedAmount = item.fee.toFixed(2);

                        if (item.remarks && item.remarks.length) {
                            var lastRemark = item.remarks[item.remarks.length - 1];
                            item.lastRemark = lastRemark.actorName + ': ' + lastRemark.remark;
                        }
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]]
        });
    };

    base.reloadSelectedEngagement = function () {
        base.getDepositPayoutDetail(base.setDepositPayoutDetail);

        base.loadDataTable();
    };
    base.setUserEvent = function () {
        $form.on("click", ".submit", function (e) {
            e.preventDefault();
            e.stopPropagation();
            base.loadJobApplicationData(base.setJobApplicationsList);
        });
    };
    //initital
    var init = function init() {
        base.setUserEvent();
        $.templates.tmplContentJobApplication.link(selector, model);
        base.initDisplayComponent();
        base.loadJobApplicationData(base.setJobApplicationsList);
    };

    init();
};

var pageJobApplications = new PageJobApplications({
    'url': {
        'getJobApplications': '/api/pro/jobapplication/applications',
        'updateJobSeekerApplicationRead': '/api/pro/jobapplication/{{id}}/seeker/read',
        'updateConsultantRead': '/api/pro/consultationinvitation/{{id}}/consultant/read',
        'getPayouRequests': '/api/pro/payoutrequest/jobapplication/{{id}}',
        'getConsultationPayouRequests': '/api/pro/payoutrequest/consultationinvitation/{{id}}',
        'getPayouRequest': '/pro/payoutrequest/{{id}}',
        'getDepositPayoutDetail': '/api/pro/jobApplication/{{id}}/depositPayout',
        'getPayoutInvoiceStatement': '/pro/statement/Pdf/invoice/{{id}}',
        'getServiceChargeInvoiceStatement': '/pro/statement/Pdf/escrow/{{id}}'
    },
    'msg': {
        'keywordSearch': "search by keyword",
        'info': 'this is info'
    }
});

