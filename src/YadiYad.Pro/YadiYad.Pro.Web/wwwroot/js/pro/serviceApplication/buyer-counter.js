var ComponentServiceBuyerCounter = function (containerSelector, opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};

    var $container = $(containerSelector);

    base.loadServiceBuyerItemCounter = function (callback) {
        var settings = {
            "url": url.getServiceBuyerItemCounter,
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setServiceBuyerItemCounter = function (response) {
        base.handleResponse(response, function (response) {
            $container.bindText(response.data);
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

    var init = function () {
        base.loadServiceBuyerItemCounter(base.setServiceBuyerItemCounter);
    };

    init();
};

var componentServiceBuyerCounter = new ComponentServiceBuyerCounter(
    '.content-service-buyer-counter',
    {
        'url': {
            'getServiceBuyerItemCounter': '/api/pro/serviceapplication/buyer/counter'
        }
    }
);