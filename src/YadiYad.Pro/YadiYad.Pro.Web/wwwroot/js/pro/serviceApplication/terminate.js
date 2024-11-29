var PageTerminate = function (opts) {
    var base = this;
    var url = opts.url;
    var model = {};

    base.getParentPage = function () {
        var page = null;
        if (typeof (pageServiceHires) !== "undefined") {
            page = pageServiceHires;
        }
        if (typeof (pageServiceConfirms) !== "undefined") {
            page = pageServiceConfirms;
        }

        return page;
    };

    base.getParentModel = function () {
        var page = null;
        if (typeof (pageServiceHires) !== "undefined") {
            page = pageServiceHires.model.selectedJobEngagement;
        }
        if (typeof (pageServiceConfirms) !== "undefined") {
            page = pageServiceConfirms.model.selectedServiceConfirms;
        }

        return page;
    };

    //html template
    $.templates({
        tmplTerminate: "#tmpl-terminate"
    });

    //DOM
    var $content = $('.content-terminate');

    //back-end interect
    base.setJobApplicationEndDate = function (callback) {
        var data = app.getFormValue($content);
        var serviceEngagement = base.getParentModel();

        var settings = {
            "url": url.setJobApplicationTerminate.format({
                id: serviceEngagement.id
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
    base.onJobApplicationEndDateUpdated = function(response){
        base.handleResponse(response, function (response) {
            var parentPage = base.getParentPage();
            parentPage.reloadSelectedEngagement();
            $content.closest('.modal').modal('hide');
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
        $content.find('.btn-hire').on('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($content.find('form').valid()) {
                base.setJobApplicationEndDate(base.onJobApplicationEndDateUpdated);
            }
        });
        $content.find('.btn-cancel').on('click', function () {
            $content.closest('.modal').modal('hide');
        });
    };

    //inititalize page
    var init = function () {
        $.templates.tmplTerminate.link($content, {});
        app.initFormComponent($content);
        base.setUserEvent();
    };

    init();
};

//set init param
var pageTerminate = new PageTerminate({
    'url': {
        'setJobApplicationTerminate': '/api/pro/serviceapplication/{{id}}/terminate'
    }
});