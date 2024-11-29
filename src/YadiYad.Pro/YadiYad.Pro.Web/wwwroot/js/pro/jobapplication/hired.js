var PageJobApplicant = function (opts) {
    var base = this;
    var url = opts.url;
    var msg = opts.msg;

    //js views template
    $.templates({
        tmplContentJobEngagement: "#tmpl-content-job-engagement"
    });

    //ui object
    var $form = $('#form-job-hired');
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

            //message data for display
            $.each(result.data, function (i, item) {
                if (item.startDate) {
                    item.startDateText = moment(item.startDate, 'YYYY-MM-DD').format("DD MMM YYYY");
                }
                if (item.endDate) {
                    item.endDateText = moment(item.endDate, 'YYYY-MM-DD').format("DD MMM YYYY");
                }
                
                if (item.jobType === 3 && item.jobMilestones.length > 0) {
                    item.startMilestoneText = item.jobMilestones[0].description;

                    if (item.endMilestoneId !== null) {
                        var endMilestone = item.jobMilestones.filter(function (e) { return e.id === item.endMilestoneId });
                        item.endMilestoneText = endMilestone[0].description;
                    }

                }

            });

            $.observable(this.jobEngagements).setProperty("data", result.data);
            $.observable(this.jobEngagements).setProperty("totalCount", result.totalCount);
            if (this.jobEngagements.data && this.jobEngagements.data.length > 0) {
                this.setSelectedJobEngagement(this.jobEngagements.data[0]);
            }
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

            $('input.rating[type=number], div.rating').each(function () {
                $(this).rating({
                    'iconLib': '',
                    'inactiveIcon': 'far fa-star',
                    'activeIcon': 'fa fa-star'
                })
            });

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
                        base.updateJobEngagementStatus(base.onUpdateJobEngagementStatus, id, status);
                    }
                });
            }
        }
    };

    base.initDisplayComponent = function () {
        app.initFormComponent($form);

    };

    //back-end integration
    base.getJobEngagement = function (callback, pageIndex) {

        var filterData = {
            keyword: "",
            jobApplicationStatus: [6,7,12,13,16,17,18,19]
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

    base.updateJobEngagementRead = function () {
        if (model.selectedJobEngagement.isOrganizationRead === true) {
            return;
        }

        var settings = {
            "url": url.updateJobEngagementRead.format({
                id: model.selectedJobEngagement.id
            }),
            "background": true,
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings);
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
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Applicant Status Updated Successfully",
            }).then(function () {
                base.reloadSelectedEngagement();
            });
        });
    };

    base.onJobApplicationRefunded = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Refund Request Created Successfully",
            }).then(function () {
                base.reloadSelectedEngagement();
            });
        });
    };

    base.loadPayoutRequestTable = function () {
        if (dtPayoutRequest && dtPayoutRequest.context[0].nTable.parentNode) {
            dtPayoutRequest.destroy();
        }

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
                "url": url.getPayoutRequests.format({
                    id: model.selectedJobEngagement.id
                }),
                "type": "post",
                "background": true,
                "data": function (data) {
                    data.customFilter = {};
                },
                "dataFilter": function (data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        var urlgetPayoutRequest = url.getPayoutRequest.replace(/{{id}}/g, item.id);
                        item.action =
                            '<input class="btn btn-primary w-auto" value="Edit" type="button"'
                            + 'data-toggle="modal" data-target="#modal-payout-request"   '
                            + 'data-keyboard="false" data-backdrop="static"              '
                            + 'href="' + urlgetPayoutRequest + '" />                      ';

                        if (item.status === 2) {
                            var urlGetPayoutInvoiceStatement = url.getPayoutInvoiceStatement.replace(/{{id}}/g, item.invoiceId);
                            item.action +=
                                '<a class="text-primary table-action d-inline-block" target="_blank"'
                                + 'href="' + urlGetPayoutInvoiceStatement + '">[Invoice]</a>                      ';
                        }

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
        if (dtDepositRequest && dtDepositRequest.context[0].nTable.parentNode) {
            dtDepositRequest.destroy();
        }

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
                "search": "",
                "emptyTable": "No Deposit Request is created"

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
                "background" :true,
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
                                + 'href="' + urlGetDepositRequestStatement + '"><i class="fas fa-file-alt"></i> Statement</a>                      ';

                            if (item.serviceChargeInvoiceId) {
                                var urlGetInvoiceStatement = url.getInvoiceStatement.replace(/{{id}}/g, item.serviceChargeInvoiceId);

                                item.action +=
                                    '<a class="text-primary table-action" target="_blank"'
                                    + 'href="' + urlGetInvoiceStatement + '"><i class="fas fa-file-alt"></i>Invoice</a>';
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
        if (dtRefundRequest && dtRefundRequest.context[0].nTable.parentNode) {
            dtRefundRequest.destroy();
        }

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
                "background": true,
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
            "background": true,
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
            base.updateJobEngagementRead();
            base.loadPayoutRequestTable();
            base.loadDepositRequestTable();
            base.loadRefundRequestTable();
        });
    };

    base.reloadSelectedEngagement = function () {
        base.getJobEngagement(base.onGetJobEngagement);
    }

    base.reloadPayout = function () {
        base.getDepositPayoutDetail(base.setDepositPayoutDetail);

        return true;
    };

    base.onJobApplicationStartDateUpdated = function () {
        if (model.selectedJobEngagement.isEscrow) {
            if (model.selectedJobEngagement === 3) {
                $('#modal-project-deposit-request .modal-content').load(url.payProjectDepositRequest, function () {
                    $('#modal-project-deposit-request').modal({ show: true });
                });

            } else {
                $('#modal-job-application-pay .modal-content').load(url.payJobEscrow, function () {
                    $('#modal-job-application-pay').modal({ show: true });
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
        'payProjectDepositRequest': '/pro/order/payProjectDepositRequest',
        'payDepositRequest': '/pro/order/PayDepositRequest',
        'getDepositPayoutDetail': '/api/pro/jobApplication/{{id}}/depositPayout',
        'updateJobEngagementRead': '/api/pro/jobApplication/{{id}}/org/read',
        'getRefundStatement': '/pro/statement/Pdf/Refund/{{id}}',
        'getRefundRequests': '/api/pro/refundrequest/jobapplication/{{id}}',
        'submitRefund': '/api/pro/jobApplication/{{id}}/refund',
        'getPayoutInvoiceStatement': '/pro/statement/Pdf/invoice/{{id}}'
    },
    'msg': {
        'keywordSearch': "search by keyword",
        'info': 'this is info'
    }
});