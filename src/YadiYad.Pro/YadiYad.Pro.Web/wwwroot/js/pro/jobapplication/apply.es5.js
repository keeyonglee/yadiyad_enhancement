'use strict';

var PageApplyJob = function PageApplyJob(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.jobProfileId = null;

    //html template

    //DOM
    var $content = $('.content-applyjob');
    var $form = $content.find('.form-apply-job');

    //load data
    base.loadModel = function () {
        var modelJobSearch = pageJobSearch && pageJobSearch.getModel();
        model.jobProfileId = modelJobSearch && modelJobSearch.selectedJobProfileId;
        //model.serviceProfileId = modelJobSearch && modelJobSearch.selectedServiceProfileId;

        if (!model.jobProfileId) {
            alert('job profile id not found');
        }

        //if (!model.serviceProfileId) {
        //    alert('service profile id not found');
        //}
    };

    base.applyJob = function (callback) {
        var data = app.getFormValue($form);
        data.jobProfileId = model.jobProfileId;
        //data.serviceProfileId = model.serviceProfileId;

        var settings = {
            "url": url.applyJob,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    //set UI response
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

    base.responseApplyJob = function (response) {
        base.handleResponse(response, function (response) {
            window.location.href = url.getJobApplicationList;
        });
    };

    //set user event
    base.setUserEvent = function () {
        $content.on('click', '.btn-apply', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }

            base.applyJob(base.responseApplyJob);
        });
    };

    //inititalize page
    var init = function init() {
        app.initFormComponent($form);
        base.loadModel();
        base.setUserEvent();
    };

    init();
};

//set init param
var pageApplyJob = new PageApplyJob({
    'url': {
        'applyJob': '/api/pro/jobapplication/0',
        'getJobApplicationList': '/pro/jobapplication/list?a=1'
    }
});

