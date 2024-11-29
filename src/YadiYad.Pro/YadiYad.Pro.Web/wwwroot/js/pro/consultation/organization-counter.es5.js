"use strict";

var ComponentConsultationJobOrgCounter = function ComponentConsultationJobOrgCounter(containerSelector, opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};

    var $container = $(containerSelector);

    base.loadConsultationJobOrgCounter = function (callback) {
        var settings = {
            "background": true,
            "url": url.getConsultationJobOrgCounter,
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setConsultationJobOrgCounter = function (response) {
        base.handleResponse(response, function (response) {
            $container.bindText(response.data);
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

    base.refresh = function () {
        base.loadConsultationJobOrgCounter(base.setConsultationJobOrgCounter);
    };

    var init = function init() {
        base.loadConsultationJobOrgCounter(base.setConsultationJobOrgCounter);
    };

    init();
};

var componentConsultationJobOrgCounter = new ComponentConsultationJobOrgCounter('.content-consultation-job-org-counter', {
    'url': {
        'getConsultationJobOrgCounter': '/api/pro/consultationinvitation/org/counter'
    }
});

