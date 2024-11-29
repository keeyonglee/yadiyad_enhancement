var PageTerminate = function (opts) {
    var base = this;
    var url = opts.url;
    var model = {};

    base.getParentPage = function () {
        var page = null;
        if (typeof (pageJobApplicant) !== "undefined") {
            page = pageJobApplicant;
        }

        return page;
    };

    //html template
    $.templates({
        tmplTerminate: "#tmpl-terminate"
    });

    //DOM
    var $content = $('.content-terminate');
    var $form = $content.find('#form-job-terminate');

    //load data
    base.loadModel = function () {
        var modelJobApplicant = pageJobApplicant && pageJobApplicant.getModel();
        model.jobApplicationId = modelJobApplicant && modelJobApplicant.selectedJobEngagement.id;
        model.jobProfileId = modelJobApplicant && modelJobApplicant.selectedJobEngagement.jobProfileId;
        model.jobType = modelJobApplicant && modelJobApplicant.selectedJobEngagement.jobType;
    };

    //back-end interect
    base.setJobApplicationEndDate = function (callback) {
        var data = app.getFormValue($content);
        var parentPage = base.getParentPage();

        var settings = {
            "url": url.setJobApplicationTerminate.format({
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
        base.loadModel();
        $.templates.tmplTerminate.link($content, {});
        if (model.jobType === 3) {
            $('[name="endMilestoneId"]').data('sourceurl', $('[name="endMilestoneId"]').data('sourceurl').replace("jobApplicationId", model.jobApplicationId));
            $(".section-end-milestone").removeClass("hidden");
            $(".section-end-date").addClass("hidden");
        }
        app.initFormComponent($content);
        base.setUserEvent();
    };

    init();
};

//set init param
var pageTerminate = new PageTerminate({
    'url': {
        'setJobApplicationTerminate': '/api/pro/jobapplication/{{id}}/terminate'
    }
});