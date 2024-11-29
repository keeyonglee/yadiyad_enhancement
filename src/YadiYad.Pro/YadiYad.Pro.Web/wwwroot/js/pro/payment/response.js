var PageSubscriptionApplyJob = function (opts) {
    var base = this;
    var url = opts.url;
    var model = {};

    //DOM
    var $content = $('.content');

    base.generatePaymentLink = function (callback) {
        var data = window.location.getUrlQueryParams();
        var orderId = data.orderid;

        var settings = {
            "url": url.generatePaymentLink,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify({
                id: orderId
            })
        };

        $.ajax(settings).done(callback);
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
        base.setUserEvent();
    };

    init();
};

//set init param
var pageSubscriptionApplyJob = new PageSubscriptionApplyJob({
    'url': {
        'generatePaymentLink': '/api/pro/payment'
    }
});