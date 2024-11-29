'use strict';

var PageServiceConfirms = function PageServiceConfirms(opts) {
    var base = this;
    var url = opts.url;
    var msg = opts.msg;
    var model = opts.model || {};
    model.serviceConfirms = [];
    model.productTypeId = 41;
    model.selectedServiceConfirms = null;
    model.depositRequestId = 0;

    Object.defineProperty(base, 'model', {
        get: function get() {
            return model;
        }
    });

    var pagination = new SimplePagination('.pagination-service-confirms-list');

    var tmplCardServiceConfirms = $.templates("#template-card-service-confirms-list");
    var tmplCardServiceConfirmsResponse = $.templates("#template-card-service-confirms-list-response");
    var tmplCardServiceConfirmsResponseEmpty = $.templates("#template-card-service-confirms-list-response-empty");

    var $form = $('#form-service-confirms');
    var $content = $('.content-service-search');
    var $listServiceConfirms = $content.find(".list-service-confirms-list");
    var $detail = $('.content-service-confirms-list-response');
    var dtPayoutRequest = null;
    var dtDepositRequest = null;
    var dtRefundRequest = null;

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    pagination.onPageChanged = function (pageIndex) {
        base.loadServiceConfirmsData(base.setServiceConfirms, pageIndex);
    };

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    base.getDepositPayoutDetail = function (callback) {
        var settings = {
            "url": url.getDepositPayoutDetail.format({
                id: model.selectedServiceConfirms.id
            }),
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };
        $.ajax(settings).done(callback);
    };

    base.setDepositPayoutDetail = function (response) {
        if (response && response.status && response.status.code === 1) {
            response.data.totalRemaining = response.data.deposit.totalAmount - response.data.payout.totalAmount;
            if (response.data.deposit.nextDueDate !== null) {
                response.data.deposit.formattedNextDueDate = moment.utc(response.data.deposit.nextDueDate).local().format("DD/MM/YYYY");
            }
            model.selectedServiceConfirms.depositPayout = response.data;
            var htmlCardServiceConfirmsResponse = tmplCardServiceConfirmsResponse.render(model.selectedServiceConfirms);
            $content.find('.content-service-confirms-list-response').replaceWith(htmlCardServiceConfirmsResponse);

            base.loadPayoutRequestTable();
            base.loadDepositRequestTable();
            base.loadRefundRequestTable();
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.loadServiceConfirmsData = function (callback, pageIndex) {
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

        model.search = requestData;

        var requestData = {
            "filter": filterData.keyword,
            "offset": offset,
            "recordSize": recordSize,
            "sorting": null,
            "advancedFilter": filterData
        };

        model.search = requestData;

        var settings = {
            "url": url.getServiceConfirms,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.setServiceConfirms = function (response) {
        if (response && response.status && response.status.code === 1) {

            if (response.data.data.length > 0) {

                $.each(response.data.data, function (i, item) {
                    if (item.tenureStart !== null) {
                        item.tenureStart = moment(item.tenureStart, 'YYYY-MM-DDTHH:mm').format('DD MMM YYYY');
                    }
                    if (item.tenureEnd !== null) {
                        item.tenureEnd = moment(item.tenureEnd, 'YYYY-MM-DDTHH:mm').format('DD MMM YYYY');
                    }
                    if (item.startDate !== null) {
                        item.startDate = moment(item.startDate, 'YYYY-MM-DD').format('DD MMM YYYY');
                    }
                    if (item.endDate !== null) {
                        item.endDateText = moment(item.endDate, 'YYYY-MM-DD').format('DD MMM YYYY');
                    }

                    item.createdOnUTC = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                });

                $content.bindText(response.data);
                $listServiceConfirms.empty();
                pagination.set(response.data);
                var htmlCardServiceConfirms = tmplCardServiceConfirms.render(response.data.data);
                $listServiceConfirms.append(htmlCardServiceConfirms);
                model.serviceConfirms = model.serviceConfirms.concat(response.data.data);

                model.selectedServiceConfirms = response.data.data[0];
                var htmlCardServiceConfirmsResponse = tmplCardServiceConfirmsResponse.render(model.selectedServiceConfirms);
                $content.find('.content-service-confirms-list-response').replaceWith(htmlCardServiceConfirmsResponse);
                base.getDepositPayoutDetail(base.setDepositPayoutDetail);
                $content.find('.card-list-info').removeClass('hidden');
            } else {
                $content.find('.card-list-info').addClass('hidden');

                $listServiceConfirms.empty();
                pagination.set(response.data);
                $content.find('.content-service-confirms-list-response').replaceWith(tmplCardServiceConfirmsResponseEmpty);
            }
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.setServiceConfirmsResponse = function (target) {
        var serviceConfirmsId = $(target).data('entity-id');
        var serviceConfirmsModel = $.grep(model.serviceConfirms, function (item) {
            return item.id === serviceConfirmsId;
        });

        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        model.selectedServiceConfirms = serviceConfirmsModel[0];
        var htmlCardServiceConfirmsResponse = tmplCardServiceConfirmsResponse.render(model.selectedServiceConfirms);
        $content.find('.content-service-confirms-list-response').replaceWith(htmlCardServiceConfirmsResponse);
        base.loadPayoutRequestTable();
        base.loadDepositRequestTable();
        base.loadRefundRequestTable();
        base.getDepositPayoutDetail(base.setDepositPayoutDetail);
    };

    //back-end interect
    base.serviceApplicationRefund = function (callback) {
        var settings = {
            "url": url.submitRefund.format({
                id: model.selectedServiceConfirms.id
            }),
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.onServiceApplicationRefunded = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Refund Request Created Successfully"
            }).then(function () {
                base.loadServiceConfirmsData(base.setServiceConfirms);
            });
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

    base.setUserEvent = function () {
        $listServiceConfirms.on('click', '.card-service-confirms-list', function () {
            base.setServiceConfirmsResponse(this);
        });
        $form.find(".submit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            base.loadServiceConfirmsData(base.setServiceConfirms);
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
                }
            }).then(function (isConfirm) {
                if (isConfirm) {
                    base.serviceApplicationRefund(base.onServiceApplicationRefunded);
                }
            });
        });
    };

    base.getModel = function () {
        return model;
    };

    base.loadPayoutRequestTable = function () {
        //if (dtPayoutRequest) {
        //    dtPayoutRequest.destroy();
        //}

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
                "url": url.getPayouRequests.format({
                    id: model.selectedServiceConfirms.id
                }),
                "type": "post",
                "data": function data(_data) {
                    _data.customFilter = {};
                },
                "dataFilter": function dataFilter(data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        var urlGetPayouRequest = url.getPayouRequest.replace(/{{id}}/g, item.id);
                        if (item.status == 0) {
                            item.action = '<input class="btn btn-primary w-auto" value="Edit" type="button"' + 'data-toggle="modal" data-target="#modal-payout-request"   ' + 'data-keyboard="false" data-backdrop="static"              ' + 'href="' + urlGetPayouRequest + '" />                      ';
                        } else {
                            item.action = '<input class="btn btn-primary w-auto" value="View" type="button"' + 'data-toggle="modal" data-target="#modal-payout-request"   ' + 'data-keyboard="false" data-backdrop="static"              ' + 'href="' + urlGetPayouRequest + '" />                      ';
                        }

                        if (item.status === 2) {
                            var urlGetInvoiceStatement = url.getInvoiceStatement.replace(/{{id}}/g, item.invoiceId);
                            item.action += '<a class="text-primary" target="_blank"' + 'href="' + urlGetInvoiceStatement + '">[Invoice]</a>                      ';
                        }

                        item.createdDateTime = moment.utc(item.createdOnUTC).local().format("DD/MM/YYYY");
                        var endDate = moment.utc(item.endDate).local();

                        item.payoutCycle = endDate.format("YYYY MMM") + " #" + (endDate.toDate().getDate() <= 15 ? 1 : 2);
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

    base.loadDepositRequestTable = function () {
        //if (dtDepositRequest) {
        //    dtDepositRequest.destroy();
        //}

        var cols = [{ "data": "formattedRequestDate" }, { "data": "formattedDueDate" }, { "data": "formattedAmount" }, { "data": "statusText" }, { "data": "action" }];

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
                    id: model.selectedServiceConfirms.id
                }),
                "type": "post",
                "data": function data(_data2) {
                    _data2.customFilter = {};
                },
                "dataFilter": function dataFilter(data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {

                        if (item.status === 1) {
                            var urlGetDepositRequestStatement = url.getDepositRequestStatement.replace(/{{id}}/g, item.id);
                            item.action = '<a class="text-primary table-action" target="_blank"' + 'href="' + urlGetDepositRequestStatement + '"><i class="fas fa-file-alt"></i> Statement</a>                      ';

                            if (item.serviceChargeInvoiceId) {
                                var urlGetInvoiceStatement = url.getInvoiceStatement.replace(/{{id}}/g, item.serviceChargeInvoiceId);

                                item.action += '<a class="text-primary table-action" target="_blank"' + 'href="' + urlGetInvoiceStatement + '">[Invoice]</a>';
                            }
                        } else {
                            var urlPayDepositRequest = url.payDepositRequest;
                            item.action = '<input class="btn btn-primary w-auto btn-pay-deposit-request" value="Pay" type="button"' + 'data-toggle="modal" data-target="#modal-job-application-pay"   ' + 'data-keyboard="false" data-backdrop="static"              ' + 'data-id="' + item.id + '"        ' + 'href="' + urlPayDepositRequest + '" />';
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
    };

    base.loadRefundRequestTable = function () {
        //if (dtRefundRequest) {
        //    dtRefundRequest.destroy();
        //}

        var cols = [{ "data": "createdDateTime" }, { "data": "refundNumber" }, { "data": "formattedAmount" }, { "data": "formattedServiceCharge" }, { "data": "statusName" }, { "data": "action" }];

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
                "search": "",
                "emptyTable": "No Refund Request is created"
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
                    id: model.selectedServiceConfirms.id
                }),
                "type": "post",
                "data": function data(_data3) {
                    _data3.customFilter = {};
                },
                "dataFilter": function dataFilter(data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        item.action = '';
                        if (item.statusName.toLowerCase() === 1) {
                            var urlGetRefundStatement = url.getRefundStatement.replace(/{{id}}/g, item.id);
                            item.action += '<a class="text-primary" target="_blank"' + 'href="' + urlGetRefundStatement + '">[Refund]</a>                      ';
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
    };

    base.reloadSelectedEngagement = function () {
        base.loadServiceConfirmsData(base.setServiceConfirms);
    };

    base.reloadPayout = function () {
        base.getDepositPayoutDetail(base.setDepositPayoutDetail);

        return true;
    };

    var init = function init() {
        base.initDisplayComponent();
        base.loadServiceConfirmsData(base.setServiceConfirms, 0);
        base.setUserEvent();
    };

    init();
};

var pageServiceConfirms = new PageServiceConfirms({
    'url': {
        'getServiceConfirms': '/api/pro/serviceApplication/confirms',
        'getPayouRequests': '/api/pro/payoutrequest/serviceapplication/{{id}}',
        'getPayouRequest': '/pro/payoutrequest/{{id}}',
        'getDepositRequests': '/api/pro/depositrequest/serviceapplication/{{id}}',
        'payDepositRequest': '/pro/order/PayDepositRequest',
        'getDepositRequestStatement': '/pro/statement/Pdf/Deposit/{{id}}',
        'getInvoiceStatement': '/pro/statement/Pdf/Invoice/{{id}}',
        'getRefundStatement': '/pro/statement/Pdf/Refund/{{id}}',
        'getDepositPayoutDetail': '/api/pro/serviceApplication/{{id}}/depositPayout',
        'getRefundRequests': '/api/pro/refundrequest/serviceapplication/{{id}}',
        'submitRefund': '/api/pro/serviceApplication/{{id}}/refund'
    },
    'msg': {
        'keywordSearch': "search by keyword",
        'info': 'this is info'
    }
});

