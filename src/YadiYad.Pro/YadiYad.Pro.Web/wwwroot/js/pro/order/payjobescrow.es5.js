"use strict";

var PagePayJobEscrow = function PagePayJobEscrow(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.selectedPaymentOptionId = null;

    //html template
    var tmplPayJobEscrow = $.templates("#template-payjobescrow");
    var tmplPayment = $.templates("#template-payment");
    var tmplPaymentGateway = $.templates("#template-payment-gateway");

    //DOM
    var $content = $('.content-payjobescrow');
    var $contentPaymentGateway = $('.content-payment-gateway');

    base.loadPaymentOptions = function (callback) {
        var settings = {
            "url": url.getPaymentOptions,
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
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

    base.createOrder = function (callback) {
        if (typeof pageJobApplicant !== "undefined") {
            var modelJobApplicant = pageJobApplicant && pageJobApplicant.model;
            if (modelJobApplicant !== null) {
                model.selectedServiceProfile = modelJobApplicant && modelJobApplicant.selectedJobEngagement;
            }
        }
        var data = {
            productTypeId: 5,
            refId: model.selectedServiceProfile.id
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
        var payableAmount = 0;

        var offsetableOrderItems = $.grep(response.data.orderItems, function (item) {
            return item.price < 0;
        });

        $.each(response.data.orderItems, function (i, item) {
            if (item.price > 0) {
                payableAmount += item.price;
            }
        });

        response.data.offsetableOrderItems = offsetableOrderItems;
        response.data.payableAmount = payableAmount;

        var htmlPayJobEscrow = tmplPayJobEscrow.render(response.data);
        $content.append(htmlPayJobEscrow);
        app.initFormComponent($content);
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
                title: "Escrow payment has been paid"
            }). //text:
            then(function () {
                location.reload();
            });
        });
    };

    base.setPaymentOptions = function (response) {
        base.handleResponse(response, function (response) {
            var htmlPayment = tmplPayment.render({
                paymentMethods: response.data
            });
            $content.empty();
            $content.append(htmlPayment);
        });
    };

    base.redirectToPaymentGateway = function (response) {
        base.handleResponse(response, function (response) {
            $contentPaymentGateway.empty();
            window.location.href = response.data.paymentURL;
            //var htmlPaymentGateway = tmplPaymentGateway.render(response.data);
            //$contentPaymentGateway.append(htmlPaymentGateway);
            //$contentPaymentGateway.find('form').submit();
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

    base.setSelectedPaymentOption = function (target) {
        var $selectedPaymentOption = $(target);
        var id = $selectedPaymentOption.data('id');

        model.selectedPaymentOptionId = id;
        $content.find('.btn-payment-option').removeClass('border-primary');
        $content.find('.btn-payment-option').addClass('border-white');
        $selectedPaymentOption.removeClass('border-white');
        $selectedPaymentOption.addClass('border-primary');

        $content.find('.btn-confirm-pay').removeAttr('disabled');
    };

    //set user event
    base.setUserEvent = function () {
        $content.on('click', '.btn-pay', function () {
            base.generatePaymentLink(base.redirectToPaymentGateway);
            //base.paymentService(base.responsePaymentService);
        });

        $content.on('click', '.btn-payment-option', function (e) {
            base.setSelectedPaymentOption(this);
        });

        $content.on('click', '.btn-confirm-pay', function (e) {
            base.generatePaymentLink(base.redirectToPaymentGateway);
        });
    };

    //inititalize page
    var init = function init() {
        base.createOrder(base.createOrderResponse);
        base.setUserEvent();
    };

    init();
};

//set init param
var pagePayJobEscrow = new PagePayJobEscrow({
    'url': {
        'createOrder': '/api/pro/order/0',
        'updateOrder': '/api/pro/order/{{id}}',
        'getPaymentOptions': '/api/pro/payment/option',
        'generatePaymentLink': '/api/pro/payment'
    }
});

