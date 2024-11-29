"use strict";

var PagePayConsultationFee = function PagePayConsultationFee(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.orderId = null;

    //html template
    var tmplOrder = $.templates("#template-order");
    var tmplPaymentGateway = $.templates("#template-payment-gateway");

    //DOM
    var $content = $('.content-order');
    var $contentPaymentGateway = $('.content-payment-gateway');

    //load data
    base.loadOrderDetail = function (callback) {
        var data = {
            productTypeId: 31,
            refId: pageConsultationapplicant && pageConsultationapplicant.model.selectedApplicant.id
        };

        var settings = {
            "url": url.getOrderDetail,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
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
                Id: model.orderId
            })
        };

        $.ajax(settings).done(callback);
    };

    //set UI response
    base.setOrderDetail = function (response) {
        base.handleResponse(response, function (response) {
            model.orderId = response.data.id;
            response.data.moreInfo = JSON.parse(response.data.moreInfo);
            var data = response.data;
            data.offsetableOrderItems = $.grep(data.orderItems, function (item, i) {
                return item.price < 0;
            });

            data.chargeOrderItems = $.grep(data.orderItems, function (item, i) {
                return item.price > 0 && item.productTypeId === 3;
            });

            data.salaryOrderItems = $.grep(data.orderItems, function (item, i) {
                return item.price > 0 && item.productTypeId === 31;
            });

            var htmlSubscriptionApplyJob = tmplOrder.render(response.data);
            $content.append(htmlSubscriptionApplyJob);
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

    //set user event
    base.setUserEvent = function () {
        $content.on('click', '.btn-pay', function () {
            base.generatePaymentLink(base.redirectToPaymentGateway);
        });
    };

    //inititalize page
    var init = function init() {
        base.loadOrderDetail(base.setOrderDetail);
        base.setUserEvent();
    };

    init();
};

//set init param
var pagePayConsultationFee = new PagePayConsultationFee({
    'url': {
        'getOrderDetail': '/api/pro/order/0',
        'generatePaymentLink': '/api/pro/payment'
    }
});

