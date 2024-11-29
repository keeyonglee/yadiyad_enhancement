var PageServiceHires = function (opts) {
    var base = this;
    var url = opts.url;
    var msg = opts.msg;
    var model = opts.model || {};
    model.serviceHires = [];
    model.productTypeId = 41;
    model.selectedPayoutRequest = null;

    Object.defineProperty(base, 'model', {
        get() {
            return model;
        }
    });


    var pagination = new SimplePagination('.pagination-service-hires-list');
    var tmplCardServiceHires = $.templates("#template-card-service-hires-list");
    var tmplCardServiceHiresListEmpty = $.templates("#template-card-service-hires-list-empty");
    var tmplCardServiceHiresResponse = $.templates("#template-card-service-hires-list-response");
    var tmplCardServiceHiresResponseEmpty = $.templates("#template-card-service-hires-list-response-empty");

    var $form = $('#form-service-hires');
    var $content = $('.content-service-search');
    var $dateRangeFilter = $("[name='dateRangeFilter']");
    var $listServiceHires = $content.find(".list-service-hires-list");
    var dtPayoutRequest = null;

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    pagination.onPageChanged = function (pageIndex) {
        base.loadServiceHiresData(base.setServiceHires, pageIndex);
    }

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    base.getDepositPayoutDetail = function (callback) {
        var settings = {
            "url": url.getDepositPayoutDetail.format({
                id: model.selectedServiceHires.id
            }),
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };
        $.ajax(settings).done(callback);
    };

    base.setDepositPayoutDetail = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            response.data.totalRemaining = response.data.deposit.totalAmount - response.data.payout.totalAmount;
            if (response.data.deposit.nextDueDate !== null) {
                response.data.deposit.formattedNextDueDate = moment.utc(response.data.deposit.nextDueDate).local().format("DD/MM/YYYY");
            }
            model.selectedServiceHires.depositPayout = response.data;
            base.showServiceHire();
            base.loadDataTable();
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
            });
        }
    };

    base.updateServiceSellerRead = function (selectedServiceApplication, callback) {
        if (selectedServiceApplication.providerIsRead) {
            return true;
        }
        selectedServiceApplication.providerIsRead = true;
        var settings = {
            "url": url.updateServiceSellerRead.format(selectedServiceApplication),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            background: true
        };

        $.ajax(settings).done(callback);
    };

    base.updateServiceSellerReadResponse = function () {

    }

    base.loadServiceHiresData = function (callback, pageIndex) {
        var filterData = {
            keyword: "",
            startDate: $dateRangeFilter.val() !== "" ? $dateRangeFilter.data('daterangepicker').startDate : null,
            endDate: $dateRangeFilter.val() !== "" ? $dateRangeFilter.data('daterangepicker').endDate : null
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
            "url": url.getServiceHires,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.setServiceHires = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            console.log(response);
            if (response.data.data.length > 0) {
                $listServiceHires.empty();
                pagination.set(response.data);

                //message data for display
                $.each(response.data.data, function (i, item) {
                    if (item.tenureStart !== null) {
                        item.tenureStart = moment(item.tenureStart, 'YYYY-MM-DDTHH:mm').format('DD MMM YYYY');
                    }
                    if (item.tenureEnd !== null) {
                        item.tenureEnd = moment(item.tenureEnd, 'YYYY-MM-DDTHH:mm').format('DD MMM YYYY');
                    }

                    if (item.startDate) {
                        item.startDateText = moment(item.startDate, 'YYYY-MM-DD').format("DD MMM YYYY");
                    }

                    if (item.endDate) {
                        item.endDateText = moment(item.endDate, 'YYYY-MM-DD').format("DD MMM YYYY");
                    }

                    item.createdOnUTC = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');

                });

                $content.bindText(response.data);

                var htmlCardServiceHires = tmplCardServiceHires.render(response.data.data);
                $listServiceHires.append(htmlCardServiceHires);
                model.serviceHires = model.serviceHires.concat(response.data.data);

                model.selectedServiceHires = response.data.data[0];
                base.showServiceHire();

                base.getDepositPayoutDetail(base.setDepositPayoutDetail);
                $content.find('.card-list-info').removeClass('hidden');
            } else {
                $content.find('.card-list-info').addClass('hidden');
                $listServiceHires.empty();
                $content.find('.content-service-hires-list-response').replaceWith(tmplCardServiceHiresResponseEmpty);
            }


        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
            });
        }
    };

    base.showServiceHire = function () {
        if (model.selectedServiceHires) {
            base.updateServiceSellerRead(model.selectedServiceHires, base.updateServiceSellerReadResponse);
            var htmlCardServiceHiresResponse = tmplCardServiceHiresResponse.render(model.selectedServiceHires);
            $content.find('.content-service-hires-list-response').replaceWith(htmlCardServiceHiresResponse);
        }
    };

    base.setServiceHiresResponse = function (target) {
        var serviceHiresId = $(target).data('entity-id');
        var serviceHiresModel = $.grep(model.serviceHires, function (item) {
            return item.id === serviceHiresId
        });

        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        model.selectedServiceHires = serviceHiresModel[0];
        base.showServiceHire();
        base.getDepositPayoutDetail(base.setDepositPayoutDetail);
    };

    base.setUserEvent = function () {
        $listServiceHires.on('click', '.card-service-hires-list:not(.selected)', function () {
            base.setServiceHiresResponse(this);
        });
        $form.find(".submit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            base.loadServiceHiresData(base.setServiceHires);
        });
    };

    base.getModel = function () {
        return model;
    };

    base.loadDataTable = function () {
        if (dtPayoutRequest) {
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
                    id: model.selectedServiceHires.id
                }),
                "type": "post",
                "data": function (data) {
                    data.customFilter = {};
                },
                "dataFilter": function (data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;

                    var allowedRaisePayout = result.data.additionalData.allowedRaisePayout;

                    if (allowedRaisePayout) {
                        $content.find('.btn-new-payoutrequest').removeClass('hidden');
                    }

                    dataList.forEach(function (item) {
                        var urlGetPayouRequest = url.getPayouRequest.replace(/{{id}}/g, item.id);
                        var buttonText = (item.status === -2) ? "Edit" : "View";

                        item.action =
                            '<input class="btn btn-primary w-auto" value="' + buttonText + '" type="button"'
                            + 'data-toggle="modal" data-target="#modal-payout-request"   '
                            + 'data-keyboard="false" data-backdrop="static"              '
                            + 'href="' + urlGetPayouRequest + '" />                      ';


                        if (item.status === 2) {
                            var urlGetInvoiceStatement = url.getInvoiceStatement.replace(/{{id}}/g, item.invoiceId);
                            item.action +=
                                '<br/><a class="text-primary" target="_blank"'
                                + 'href="' + urlGetInvoiceStatement + '">[Invoice]</a>';

                            if (item.serviceChargeInvoiceId) {

                                var urlGetServiceChargeInvoice = url.getServiceChargeInvoiceStatement.replace(/{{id}}/g, item.serviceChargeInvoiceId);
                                item.action +=
                                    '<br/><a class="text-primary" target="_blank"'
                                + 'href="' + urlGetServiceChargeInvoice + '">[Escrow Invoice]</a>';
                            }
                        }

                        item.createdDateTime = moment.utc(item.createdOnUTC).local().format("DD/MM/YYYY");
                        var endDate = moment.utc(item.endDate).local();

                        item.payoutCycle = endDate.format("YYYY MMM") + " #" + (endDate.toDate().getDate() <= 15 ? 1 : 2)
                        item.formattedAmount = item.fee.toFixed(2);
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]]
        });
    }

    base.reloadSelectedEngagement = function () {
        base.getDepositPayoutDetail(base.setDepositPayoutDetail);
    }

    var init = function () {
        base.initDisplayComponent();
        base.loadServiceHiresData(base.setServiceHires);
        base.setUserEvent();
    };

    init();
};

var pageServiceHires = new PageServiceHires({
    'url': {
        'getServiceHires': '/api/pro/serviceApplication/hires',
        'updateServiceSellerRead': '/api/pro/serviceApplication/{{id}}/seller/read',
        'getPayouRequests': '/api/pro/payoutrequest/serviceapplication/{{id}}',
        'getPayouRequest': '/pro/payoutrequest/{{id}}',
        'getInvoiceStatement': '/pro/statement/Pdf/Invoice/{{id}}',
        'getServiceChargeInvoiceStatement': '/pro/statement/Pdf/escrow/{{id}}',
        'getDepositPayoutDetail': '/api/pro/serviceApplication/{{id}}/depositPayout'
    },
    'msg': {
        'keywordSearch': "search by keyword",
        'info': 'this is info'
    }
});