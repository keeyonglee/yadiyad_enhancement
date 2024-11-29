var ComponentServiceSellerCounter = function (containerSelector, opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};

    var $container = $(containerSelector);

    base.loadServiceSellerItemCounter = function (callback) {
        var settings = {
            "url": url.getServiceSellerItemCounter,
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setServiceSellerItemCounter = function (response) {
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
        base.loadServiceSellerItemCounter(base.setServiceSellerItemCounter);
    };

    init();
};

var componentServiceSellerCounter = new ComponentServiceSellerCounter(
    '.content-service-seller-counter',
    {
        'url': {
            'getServiceSellerItemCounter': '/api/pro/serviceapplication/seller/counter'
        }
    }
);