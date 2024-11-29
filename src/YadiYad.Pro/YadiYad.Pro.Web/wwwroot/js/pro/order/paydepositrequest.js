var PagePayDepositRequest = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.selectedPaymentOptionId = null;

    //html template
    var tmplPayJobEscrow = $.templates("#tmpl-paydepositrequest");

    //DOM
    var $content = $('.content-payjobescrow');

    //front-end action
    base.getParentPage = function () {
        var model = null;
        if (typeof (pageJobApplicant) !== "undefined") {
            model = pageJobApplicant;
        }
        else if (typeof (pageServiceConfirms) !== "undefined") {
            model = pageServiceConfirms;
        }

        return model;
    }

    //backend interact
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
        var parentPage = base.getParentPage();

        var data = {
            depositRequestId : parentPage.model.depositRequestId
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

    //front-end response
    base.createOrderResponse = function (response) {
        model.orderId = response.data.id;
        var htmlPayJobEscrow = tmplPayJobEscrow.render(response.data);
        $content.append(htmlPayJobEscrow);
        app.initFormComponent($content);
    };

    base.redirectToPaymentGateway = function (response) {
        base.handleResponse(response, function (response) {
            window.location.href = response.data.paymentURL;
        });
    };

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
            }).then(function () {
            });
        }
    };

    //set user event
    base.setUserEvent = function () {
        $content.on('click', '.btn-pay', function () {
            base.generatePaymentLink(base.redirectToPaymentGateway);

        });
    };

    //inititalize page
    var init = function () {
        base.createOrder(base.createOrderResponse);
        base.setUserEvent();
    };

    init();
};

//set init param
var pagePayDepositRequest = new PagePayDepositRequest({
    'url': {
        'createOrder': '/api/pro/order/0',
        'generatePaymentLink': '/api/pro/payment'
    }
});