var ComponentOrganizationJobCounter = function (containerSelector, opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};

    var $container = $(containerSelector);

    base.loadOrganizationJobItemCounter = function (callback) {
        var settings = {
            "background": true,
            "url": url.getOrganizationJobItemCounter,
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setOrganizationJobItemCounter = function (response) {
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

    base.refresh = function () {
        base.loadOrganizationJobItemCounter(base.setOrganizationJobItemCounter);
    };

    var init = function () {
        base.loadOrganizationJobItemCounter(base.setOrganizationJobItemCounter);
    };

    init();
};

var componentOrganizationJobCounter = new ComponentOrganizationJobCounter(
    '.content-organization-job-main-counter',
    {
    'url': {
        'getOrganizationJobItemCounter': '/api/pro/job/org/counter'
    }
    });