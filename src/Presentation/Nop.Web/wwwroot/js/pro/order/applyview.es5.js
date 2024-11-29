"use strict";

var PageSubscriptionApplyView = function PageSubscriptionApplyView(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.selectedPaymentOptionId = null;

    //html template
    var tmplSubscriptionApplyView = $.templates("#template-subscription-applyview");
    var tmplSubscriptionPayment = $.templates("#template-subscription-payment");
    var tmplPaymentGateway = $.templates("#template-payment-gateway");

    //DOM
    var $content = $('.content-subscription-applyview');
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
        if (typeof pageJobSummary !== "undefined") {
            var modelJobSummary = pageJobSummary && pageJobSummary.getModel();
            if (modelJobSummary !== null) {
                model.refId = modelJobSummary && modelJobSummary.selectedJobApplicationId;
            }
        }
        if (typeof pageJobCandidate !== "undefined") {
            var modelJobCandidate = pageJobCandidate && pageJobCandidate.getModel();
            if (modelJobCandidate !== null) {
                model.refId = modelJobCandidate && modelJobCandidate.id;
            }
        }
        if (typeof pageJobList !== "undefined") {
            var modelJobList = pageJobList && pageJobList.getModel();
            if (modelJobList !== null) {
                model.refId = modelJobList && modelJobList.selectedJobApplicationId;
            }
        }
        var data = {
            productTypeId: 2,
            refId: model.refId
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
        var htmlSubscriptionApplyView = tmplSubscriptionApplyView.render(response.data);
        $content.append(htmlSubscriptionApplyView);
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
var pageSubscriptionApplyView = new PageSubscriptionApplyView({
    'url': {
        'createOrder': '/api/pro/order/0',
        'updateOrder': '/api/pro/order/{{id}}',
        'getPaymentOptions': '/api/pro/payment/option',
        'generatePaymentLink': '/api/pro/payment'
    }
});

