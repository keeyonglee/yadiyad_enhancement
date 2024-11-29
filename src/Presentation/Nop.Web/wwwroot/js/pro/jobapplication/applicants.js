var PageJobApplicant = function (opts) {
    var base = this;
    var url = opts.url;
    var msg = opts.msg;
    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    //js views template
    $.templates({
        tmplContentJobEngagement: "#tmpl-content-job-engagement"
    });

    //ui object
    var $form = $('#form-job-applications');
    var $content = $('.content-job-engagement');
    var dtPayoutRequest = null;
    var dtDepositRequest = null;
    var dtRefundRequest = null;

    //model object
    Object.defineProperty(base, 'model', {
        get() {
            return model;
        }
    });

    var pagination = new SimplePagination();
    pagination.onPageChanged = function (pageIndex) {
        base.getJobEngagement(base.onGetJobEngagement, pageIndex);
    }
    var model = {
        pagination: pagination,
        depositRequestId: 0,
        productTypeId: 5,
        jobEngagements: {
            totalCount: 0
        },
        selectedJobEngagement: null,
        setJobEngagements(result) {
            pagination.set(result);

            $.each(result.data, function (i, data) {
                if (data.serviceIndividualProfile
                    && data.serviceIndividualProfile.dateOfBirth) {
                    data.serviceIndividualProfile.dateOfBirthText
                        = moment(data.serviceIndividualProfile.dateOfBirth, 'YYYY-MM-DDTHH:mm:ss')
                            .format('DD MMM YYYY')
                }
                data.createdOn = moment(data.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
            });



            $.observable(this.jobEngagements).setProperty("data", result.data);
            $.observable(this.jobEngagements).setProperty("totalCount", result.totalCount);

            if (this.jobEngagements.data && this.jobEngagements.data.length > 0) {
                this.setSelectedJobEngagement(this.jobEngagements.data[0]);
            }
        },
        setSelectedJobEngagementStatus: function (status) {
            var engagement = model.selectedJobEngagement;
            engagement.jobApplicationStatus = status;

            var data = this.jobEngagements.data;
            var selectedJobEngagement = $.grep(data, function (item) {
                return item.id === engagement.id;
            });

            selectedJobEngagement[0].jobApplicationStatus = status;
            var totalCount = this.jobEngagements.totalCount;

            $.observable(this.jobEngagements).setProperty("data", []);
            $.observable(this.jobEngagements).setProperty("totalCount", 0);

            $.observable(this.jobEngagements).setProperty("data", data);
            $.observable(this.jobEngagements).setProperty("totalCount", totalCount);
            this.setSelectedJobEngagement(engagement);
        },
        setSelectedJobEngagement: function (jobEngagement) {
            var selectedJobEngagement = $.grep(this.jobEngagements.data, function (item) {
                return item.selected === true;
            });

            if (selectedJobEngagement.length > 0) {
                $.observable(selectedJobEngagement[0]).setProperty('selected', false);
            }
            $.observable(this).setProperty('selectedJobEngagement', null);
            $.observable(this).setProperty('selectedJobEngagement', jobEngagement);
            $.observable(jobEngagement).setProperty('selected', true);

            base.getDepositPayoutDetail(base.setDepositPayoutDetail);
        },
        updateSelectedJobEngagementStatus: function (statusName, status) {
            if (status !== this.selectedJobEngagement.jobApplicationStatus) {
                var id = this.selectedJobEngagement.id;
                swal({
                    icon: "warning",
                    title: `Are you sure to update the status to ${statusName}?`,
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
                        base.updateJobEngagementStatus(function () {
                            base.onUpdateJobEngagementStatus(status);
                        }, id, status);
                    }
                });
            }
        }
    };

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    base.setData = function () {
        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.jobId = parseInt(urlProp[urlProp.length - 2]) || 0;
    };

    //back-end integration
    base.getJobEngagement = function (callback, pageIndex) {

        var filterData = {
            keyword: "",
            jobProfileId: model.jobId
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
            "url": url.getJobEngagement,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.updateJobEngagementStatus = function (callback, jobEngagementId, status) {
        var data = {};
        data.JobApplicationStatus = status;

        var settings = {
            "url": url.updateJobEngagementStatus.format({ id: jobEngagementId }),
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.jobApplicationRefund = function (callback) {
        var settings = {
            "url": url.submitRefund.format({
                id: model.selectedJobEngagement.id
            }),
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
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
            });
        }
    };

    base.onGetJobEngagement = function (response) {
        base.handleResponse(response, function (response) {
            model.setJobEngagements(response.data);
        });
    };

    base.onUpdateJobEngagementStatus = function (response) {
        
        swal({
            icon: "success",
            title: "Applicant Status Updated Successfully",
        }).then(function () {
            if (response === 6) {
                window.location.href = "/pro/jobapplication/hired";
            }
            else {
                base.reloadSelectedEngagement(response);
            }
        });
  
    };

    base.onJobApplicationRefunded = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Refund Request Created Successfully",
            }).then(function () {
            });
        });
    };

    base.loadPayoutRequestTable = function () {
        //if (dtPayoutRequest) {
        //    dtPayoutRequest.destroy();
        //}

        var cols = [
            { "data": "createdDateTime" },
            { "data": "payoutCycle" },
            { "data": "formattedAmount" },
            { "data": "statusName" },
            { "data": "lastRemark" },
            { "data": "action" }
        ];

        var $table = $content.find(".tbl-payout-request");

        if (!$table || !$table.length) {
            return;
        }

        dtPayoutRequest = $table.DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": msg.keywordSearch,
                "lengthMenu": "_MENU_",
                "info": msg.info,
                "search": ""
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getPayoutRequests.format({
                    id: model.selectedJobEngagement.id
                }),
                "type": "post",
                "data": function (data) {
                    data.customFilter = {};
                },
                "dataFilter": function (data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        var urlgetPayoutRequest = url.getPayoutRequest.replace(/{{id}}/g, item.id);
                        item.action =
                            '<input class="btn btn-primary w-auto" value="View" type="button"'
                            + 'data-toggle="modal" data-target="#modal-payout-request"   '
                            + 'data-keyboard="false" data-backdrop="static"              '
                            + 'href="' + urlgetPayoutRequest + '" />                      ';

                        item.createdDateTime = moment.utc(item.createdOnUTC).local().format("DD/MM/YYYY");
                        if (item.endDate) {
                            var endDate = moment.utc(item.endDate).local();

                            item.payoutCycle = endDate.format("YYYY MMM") + " #" + (endDate.toDate().getDate() <= 15 ? 1 : 2);
                        } else {
                            item.payoutCycle = "Phase " + item.jobMilestonePhase + " - " + item.jobMilestoneName;
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
    }

    base.loadDepositRequestTable = function () {
        //if (dtDepositRequest) {
        //    dtDepositRequest.destroy();
        //}

        var cols = [
            { "data": "formattedRequestDate" },
            { "data": "formattedDueDate" },
            { "data": "formattedAmount" },
            { "data": "statusText" },
            { "data": "action" }
        ];

        var $table = $content.find(".tbl-deposit-request");

        if (!$table || !$table.length) {
            return;
        }

        dtDepositRequest = $table.DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": msg.keywordSearch,
                "lengthMenu": "_MENU_",
                "info": msg.info,
                "search": ""
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getDepositRequests.format({
                    id: model.selectedJobEngagement.id
                }),
                "type": "post",
                "data": function (data) {
                    data.customFilter = {};
                },
                "dataFilter": function (data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {

                        if (item.status === 1) {
                            var urlGetDepositRequestStatement = url.getDepositRequestStatement.replace(/{{id}}/g, item.id);
                            item.action =
                                '<a class="text-primary table-action" target="_blank"'
                                + 'href="' + urlGetDepositRequestStatement + '"><i class="fas fa-file-alt"></i></a>                      ';

                            if (item.serviceChargeInvoiceId) {
                                var urlGetInvoiceStatement = url.getInvoiceStatement.replace(/{{id}}/g, item.serviceChargeInvoiceId);

                                item.action +=
                                    '<a class="text-primary table-action" target="_blank"'
                                    + 'href="' + urlGetInvoiceStatement + '">[Invoice]</a>';
                            }
                        } else {
                            var urlPayDepositRequest = url.payDepositRequest;
                            item.action =
                                '<input class="btn btn-primary w-auto btn-pay-deposit-request" value="Pay" type="button"'
                                + 'data-toggle="modal" data-target="#modal-job-application-pay"   '
                                + 'data-keyboard="false" data-backdrop="static"              '
                                + 'data-id="' + item.id + '"        '
                                + 'href="' + urlPayDepositRequest + '" />                      ';
                        }

                        item.formattedRequestDate = moment.utc(item.requestDate).local().format("DD/MM/YYYY");
                        item.formattedDueDate = moment.utc(item.dueDate).local().format("DD/MM/YYYY");
                        item.formattedAmount = item.amount.toFixed(2);
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]]
        });

        $table.on('click', '.btn-pay-deposit-request', function () {
            var $this = $(this);
            var id = $this.data('id');
            model.depositRequestId = id;
        });
    }

    base.loadRefundRequestTable = function () {
        //if (dtRefundRequest) {
        //    dtRefundRequest.destroy();
        //}

        var cols = [
            { "data": "createdDateTime" },
            { "data": "refundNumber" },
            { "data": "formattedAmount" },
            { "data": "formattedServiceCharge" },
            { "data": "statusName" },
            { "data": "action" },
        ];

        var $table = $content.find(".tbl-refund-request");

        if (!$table || !$table.length) {
            return;
        }

        dtRefundRequest = $table.DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": msg.keywordSearch,
                "lengthMenu": "_MENU_",
                "info": msg.info,
                "search": ""
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getRefundRequests.format({
                    id: model.selectedJobEngagement.id
                }),
                "type": "post",
                "data": function (data) {
                    data.customFilter = {};
                },
                "dataFilter": function (data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        item.action = '';
                        if (item.statusName.toLowerCase() === 1) {
                            var urlGetRefundStatement = url.getRefundStatement.replace(/{{id}}/g, item.id);
                            item.action +=
                                '<a class="text-primary" target="_blank"'
                                + 'href="' + urlGetRefundStatement + '">[Refund]</a>                      ';
                        }

                        item.createdDateTime = moment.utc(item.createdOnUTC).local().format("DD/MM/YYYY");
                        item.formattedAmount = 'RM ' + item.amount.toFixed(2);
                        item.formattedServiceCharge = 'RM ' + item.serviceCharge.toFixed(2);
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]]
        });
    }


    base.getDepositPayoutDetail = function (callback) {
        var settings = {
            "url": url.getDepositPayoutDetail.format({
                id: model.selectedJobEngagement.id
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

            $.observable(model.selectedJobEngagement).setProperty('depositPayout', response.data);
            base.loadPayoutRequestTable();
            base.loadDepositRequestTable();
            base.loadRefundRequestTable();
        });
    };

    base.reloadSelectedEngagement = function (statusUpdate) {
        if (statusUpdate) {
            model.setSelectedJobEngagementStatus(statusUpdate);
        }

        base.getDepositPayoutDetail(base.setDepositPayoutDetail);
    }

    base.onJobApplicationStartDateUpdated = function () {
        if (model.selectedJobEngagement.isEscrow) {
            //project based job
            if (model.selectedJobEngagement.jobType === 3) {
                $('#modal-project-deposit-request .modal-content').load(url.payProjectDepositRequest.format({
                    id: model.selectedJobEngagement.id
                }), function () {
                    $(this).addClass('content-loaded');
                    $(this).closest('.modal').modal({ show: true });
                });
            }
            else {
                $('#modal-job-application-pay .modal-content').load(url.payJobEscrow, function () {
                    $(this).addClass('content-loaded');
                    $(this).closest('.modal').modal({ show: true });
                });
            }
        } else {
            model.updateSelectedJobEngagementStatus('Hire', 6);
        }
    };

    base.getModel = function () {
        return model;
    };

    base.setUserEvent = function () {
        $form.on("click", ".submit", function (e) {
            e.preventDefault();
            e.stopPropagation();
            base.getJobEngagement(base.onGetJobEngagement);
        });

        $form.on("click", ".submit-refund", function (e) {
            e.preventDefault();
            e.stopPropagation();

            swal({
                icon: "warning",
                title: 'Are You Sure to Create Refund Request?',
                buttons: {
                    cancel: {
                        text: "Cancel",
                        value: false,
                        visible: true,
                        className: "btn-secondary",
                        closeModal: true
                    },
                    confirm: {
                        text: "Confirm",
                        value: true,
                        visible: true,
                        className: "btn-primary",
                        closeModal: true
                    }
                },
            }).then(function (isConfirm) {
                if (isConfirm) {
                    base.jobApplicationRefund(base.onJobApplicationRefunded);
                }
            });
        });
    };
    var init = function () {
        base.setData();
        $.templates.tmplContentJobEngagement.link('.content-job-engagement', model);
        base.initDisplayComponent();
        base.setUserEvent();
        base.getJobEngagement(base.onGetJobEngagement);
    };

    init();
};

var pageJobApplicant = new PageJobApplicant({
    'url': {
        'getJobEngagement': '/api/pro/jobApplication/applicants',
        'updateJobEngagementStatus': '/api/pro/jobApplication/{{id}}',
        'getPayoutRequests': '/api/pro/payoutrequest/jobapplication/{{id}}',
        'getDepositRequests': '/api/pro/depositrequest/jobapplication/{{id}}',
        'getPayoutRequest': '/pro/payoutrequest/{{id}}',
        'getDepositRequestStatement': '/pro/statement/Pdf/deposit/{{id}}',
        'payJobEscrow': '/pro/order/payjobescrow',
        'payProjectDepositRequest': '/pro/order/payProjectDepositRequest/{{id}}',
        'payDepositRequest': '/pro/order/PayDepositRequest',
        'getDepositPayoutDetail': '/api/pro/jobApplication/{{id}}/depositPayout',
        'getRefundStatement': '/pro/statement/Pdf/Refund/{{id}}',
        'getRefundRequests': '/api/pro/refundrequest/jobapplication/{{id}}',
        'submitRefund': '/api/pro/jobApplication/{{id}}/refund'
    },
    'msg': {
        'keywordSearch': "search by keyword",
        'info': 'this is info'
    }
});