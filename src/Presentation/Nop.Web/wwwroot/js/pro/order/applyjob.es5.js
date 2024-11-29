"use strict";

var PageSubscriptionApplyJob = function PageSubscriptionApplyJob(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.selectedPaymentOptionId = null;

    //html template
    var tmplSubscriptionApplyJob = $.templates("#template-subscription-applyjob");
    var tmplSubscriptionPayment = $.templates("#template-subscription-payment");
    var tmplPaymentGateway = $.templates("#template-payment-gateway");

    //DOM
    var $content = $('.content-subscription-applyjob');
    var $contentPaymentGateway = $('.content-payment-gateway');

    base.createOrder = function (callback) {

        var data = {
            productTypeId: 1
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
        var htmlSubscriptionApplyJob = tmplSubscriptionApplyJob.render(response.data);
        $content.append(htmlSubscriptionApplyJob);
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
                title: "Subscription has been paid"
            }). //text:
            then(function () {
                location.reload();
            });
        });
    };

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

    base.setPaymentOptions = function (response) {
        base.handleResponse(response, function (response) {
            var htmlSubscriptionPayment = tmplSubscriptionPayment.render({
                paymentMethods: response.data
            });
            $content.empty();
            $content.append(htmlSubscriptionPayment);
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
var pageSubscriptionApplyJob = new PageSubscriptionApplyJob({
    'url': {
        'createOrder': '/api/pro/order/0',
        'updateOrder': '/api/pro/order/{{id}}',
        'getPaymentOptions': '/api/pro/payment/option',
        'generatePaymentLink': '/api/pro/payment'
    }
});

