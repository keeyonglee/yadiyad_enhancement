"use strict";

var PageServiceRequests = function PageServiceRequests(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.serviceRequests = [];

    var pagination = new SimplePagination('.pagination-service-requests-list');
    var tmplCardServiceRequestsEmpty = $.templates("#template-card-service-requests-empty");

    var tmplCardServiceRequests = $.templates("#template-card-service-requests-list");
    var tmplCardServiceRequestsResponse = $.templates("#template-card-service-requests-list-response");
    var tmplCardServiceRequestsPay = $.templates("#template-card-service-requests-list-pay");

    var $content = $('.content-service-search');
    var $listServiceRequests = $content.find(".list-service-requests-list");
    var $detail = $('.content-service-requests-list-response');

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    pagination.onPageChanged = function (pageIndex) {
        base.loadServiceRequestsData(base.setServiceRequests, pageIndex);
    };

    base.updateServiceBuyerRead = function (selectedServiceApplication, callback) {
        if (selectedServiceApplication.requesterIsRead) {
            return true;
        }
        selectedServiceApplication.requesterIsRead = true;
        var settings = {
            "url": url.updateServiceBuyerRead.format(selectedServiceApplication),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            background: true
        };

        $.ajax(settings).done(callback);
    };

    base.updateServiceBuyerReadResponse = function () {};

    base.loadServiceRequestsData = function (callback, pageIndex) {
        var filterData = {
            keyword: ""
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
            "url": url.getServiceRequests,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.generatePaymentLink = function (callback) {
        var settings = {
            "url": url.generatePaymentLink,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify({
                id: model.orderId
            })
        };

        $.ajax(settings).done(callback);
    };

    base.redirectToPaymentGateway = function (response) {
        base.handleResponse(response, function (response) {
            window.location.href = response.data.paymentURL;
            //var htmlPaymentGateway = tmplPaymentGateway.render(response.data);
            //$contentPaymentGateway.append(htmlPaymentGateway);
            //$contentPaymentGateway.find('form').submit();
        });
    };

    base.setServiceRequests = function (response) {
        if (response && response.status && response.status.code === 1) {

            if (response.data.data.length > 0) {
                $listServiceRequests.empty();
                pagination.set(response.data);
                $.each(response.data.data, function (i, item) {
                    if (item.tenureStart !== null) {
                        item.tenureStart = moment(item.tenureStart, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }
                    if (item.tenureEnd !== null) {
                        item.tenureEnd = moment(item.tenureEnd, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }
                    if (item.tenureStart !== null) {
                        item.startDate = moment(item.startDate, 'YYYY-MM-DD').add(0, 'hour').format('DD MMM YYYY');
                    }

                    item.createdOnUTC = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                });

                $content.bindText(response.data);

                var htmlCardServiceRequests = tmplCardServiceRequests.render(response.data.data);
                $listServiceRequests.append(htmlCardServiceRequests);
                model.serviceRequests = model.serviceRequests.concat(response.data.data);

                model.selectedServiceRequests = response.data.data[0];
                base.updateServiceBuyerRead(model.selectedServiceRequests, base.updateServiceBuyerReadResponse);
                var htmlCardServiceRequestsResponse = tmplCardServiceRequestsResponse.render(model.selectedServiceRequests);
                $content.find('.content-service-requests-list-response').replaceWith(htmlCardServiceRequestsResponse);

                var $areaExpertise = $('.area-expertise');
                model.selectedServiceRequests.serviceProfile.serviceExpertises.forEach(function (item) {
                    $areaExpertise.append("<span class='badge badge-pill badge-primary'>" + item.name + "</span>&nbsp;");
                });

                $content.find(".btn-decline").on("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    swal({
                        icon: "warning",
                        title: 'Are You Sure to Decline?',
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
                            base.declineService(base.responseDeclineService);
                        }
                    });
                });

                $content.find(".btn-accept").on("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    swal({
                        icon: "warning",
                        title: 'Are You Sure to Accept?',
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
                            base.acceptService(base.responseAcceptService);
                        }
                    });
                });

                $content.find(".btn-pay").on("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    base.createOrder(base.createOrderResponse);
                });
            } else {
                $content.replaceWith(tmplCardServiceRequestsEmpty);
            }
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.setServiceRequestsResponse = function (target) {
        var serviceRequestsId = $(target).data('entity-id');
        var serviceRequestsModel = $.grep(model.serviceRequests, function (item) {
            return item.id === serviceRequestsId;
        });

        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        model.selectedServiceRequests = serviceRequestsModel[0];
        base.updateServiceBuyerRead(model.selectedServiceRequests, base.updateServiceBuyerReadResponse);
        var htmlCardServiceRequestsResponse = tmplCardServiceRequestsResponse.render(model.selectedServiceRequests);

        $content.find('.content-service-requests-list-response').replaceWith(htmlCardServiceRequestsResponse);

        var $areaExpertise = $('.area-expertise');
        model.selectedServiceRequests.serviceProfile.serviceExpertises.forEach(function (item) {
            $areaExpertise.append("<span class='badge badge-pill badge-primary'>" + item.name + "</span>&nbsp;");
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

    base.createOrder = function (callback) {
        var data = {
            productTypeId: 41,
            refId: model.selectedServiceRequests.id
        };
        var settings = {
            "url": url.createOrder,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.createOrderResponse = function (response) {
        model.orderId = response.data.id;

        var data = response.data;
        var renderData = {
            customOrderNumber: null,
            orderTotal: 0,
            serviceFeeAmount: 0,
            escrowAmount: 0,
            serviceFeeRateType: null,
            serviceFeeRateAmount: 0
        };

        renderData.customOrderNumber = data.customOrderNumber;
        renderData.orderTotal = data.orderTotal;

        data.orderItems.forEach(function (item) {
            if (item.productTypeId === 4) {
                renderData.serviceFeeAmount = item.price;
            }
            if (item.productTypeId === 41 && item.price > 0) {
                renderData.escrowAmount = item.price;
            }
            if (item.productTypeId === 41 && item.price < 0) {
                renderData.offsetOrderItem = item;
            } else {
                renderData.offsetAmount = null;
            }
        });

        if (data.moreInfo !== null) {
            var moreInfo = JSON.parse(data.moreInfo);
            renderData.serviceFeeRateType = moreInfo.ServiceFeeType;
            renderData.serviceFeeRateAmount = parseFloat(moreInfo.ServiceFeeAmount) * 100;
        }

        var htmlCardServiceRequestsResponse = tmplCardServiceRequestsPay.render(renderData);
        $content.find('.content-service-requests-list-response').replaceWith(htmlCardServiceRequestsResponse);

        generatePayment($content.find('#form-request-payment'));
        app.initFormComponent($content.find('.content-service-requests-list-response'));
    };

    //pay button onclick event
    function generatePayment(id) {
        id.find(".btn-payment").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            if (base.validationForm(id)) {
                base.generatePaymentLink(base.redirectToPaymentGateway);
            }
            //base.paymentService(base.responsePaymentService);
        });
    }

    base.validationForm = function (target) {
        var valid = true;

        valid = target.valid() && valid;

        return valid;
    };

    base.acceptService = function (callback) {
        var serviceRequestsId = model.selectedServiceRequests.id;
        var data = {
            id: serviceRequestsId
        };
        var settings = {
            "url": url.acceptServiceApplication,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responseAcceptService = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Service Request Accepted Succesfully "
            }). //text:
            then(function () {
                location.reload();
            });
        });
    };

    base.declineService = function (callback) {
        var serviceRequestsId = model.selectedServiceRequests.id;
        var data = {
            id: serviceRequestsId
        };
        var settings = {
            "url": url.declineServiceApplication,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responseDeclineService = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Service Request Declined Successfully"
            }). //text:
            then(function () {
                location.reload();
            });
        });
    };

    base.paymentService = function (callback) {
        var orderId = model.orderId;
        var data = {
            orderStatusId: 30,
            paymentStatusId: 30
        };

        var settings = {
            "url": url.updateOrder.format({ id: orderId }),
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responsePaymentService = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Service Request Paid Succesfully"
            }). //text:
            then(function () {
                location.reload();
            });
        });
    };

    base.setUserEvent = function () {
        $listServiceRequests.on('click', '.card-service-requests-list', function () {
            base.setServiceRequestsResponse(this);

            $content.find(".btn-decline").on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                swal({
                    icon: "warning",
                    title: 'Are You Sure to Decline?',
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
                        base.declineService(base.responseDeclineService);
                    }
                });
            });

            $content.find(".btn-accept").on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                swal({
                    icon: "warning",
                    title: 'Are You Sure to Accept?',
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
                        base.acceptService(base.responseAcceptService);
                    }
                });
            });

            $content.find(".btn-pay").on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                base.createOrder(base.createOrderResponse);
            });
        });
    };

    base.getModel = function () {
        return model;
    };

    var init = function init() {
        base.loadServiceRequestsData(base.setServiceRequests);
        base.setUserEvent();
    };

    init();
};

var pageServiceRequests = new PageServiceRequests({
    'url': {
        'getServiceRequests': '/api/pro/serviceApplication/requests',
        'acceptServiceApplication': '/api/pro/serviceApplication/accept',
        'declineServiceApplication': '/api/pro/serviceApplication/decline',
        'createOrder': '/api/pro/order/0',
        'updateOrder': '/api/pro/order/{{id}}',
        'updateServiceBuyerRead': '/api/pro/serviceApplication/{{id}}/buyer/read',
        'generatePaymentLink': '/api/pro/payment'
    }
});

