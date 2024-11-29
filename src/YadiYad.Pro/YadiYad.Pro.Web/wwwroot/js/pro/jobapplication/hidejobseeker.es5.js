"use strict";

var PageHideJobSeeker = function PageHideJobSeeker(opts) {
    var base = this;
    var url = opts.url;
    var model = {};

    base.getParentPage = function () {
        var page = null;
        if (typeof pageJobApplicant !== "undefined") {
            page = pageJobApplicant;
        }

        return page;
    };

    //html template
    $.templates({
        tmplHideJobSeeker: "#tmpl-hideJobSeeker"
    });

    //DOM
    var $content = $('.content-hideJobSeeker');

    //back-end interect
    base.setJobApplicationStartDate = function (callback) {
        var data = app.getFormValue($content);
        var parentPage = base.getParentPage();

        var settings = {
            "url": url.setJobApplicationStartDate.format({
                id: parentPage.model.selectedJobEngagement.id
            }),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.onJobApplicationStartDateUpdated = function (response) {
        base.handleResponse(response, function (response) {
            var parentPage = base.getParentPage();
            parentPage.onJobApplicationStartDateUpdated();
            $content.closest('.modal').modal('hide');
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
        $content.find('.btn-hire').on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($content.find('form').valid()) {
                base.setJobApplicationStartDate(base.onJobApplicationStartDateUpdated);
            }
        });
        $content.find('.btn-cancel').on('click', function () {
            $content.closest('.modal').modal('hide');
        });
    };

    //inititalize page
    var init = function init() {
        $.templates.tmplHideJobSeeker.link($content, {});
        app.initFormComponent($content);
        base.setUserEvent();
    };

    init();
};

//set init param
var pageHideJobSeeker = new PageHideJobSeeker({
    'url': {
        'setJobApplicationStartDate': '/api/pro/jobapplication/{{id}}'
    }
});

