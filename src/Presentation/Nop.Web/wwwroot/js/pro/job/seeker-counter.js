var ComponentSeekerCounter = function (containerSelector, opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};

    var $container = $(containerSelector);

    base.loadJobSeekerItemCounter = function (callback) {
        var settings = {
            "background": true,
            "url": url.getJobSeekerItemCounter,
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setJobSeekerItemCounter = function (response) {
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
        base.loadJobSeekerItemCounter(base.setJobSeekerItemCounter);
    };

    var init = function () {
        base.loadJobSeekerItemCounter(base.setJobSeekerItemCounter);
    };

    init();
};

var componentSeekerCounter = new ComponentSeekerCounter(
    '.content-job-seeker-counter',
    {
    'url': {
        'getJobSeekerItemCounter': '/api/pro/job/seeker/counter'
    }
    });