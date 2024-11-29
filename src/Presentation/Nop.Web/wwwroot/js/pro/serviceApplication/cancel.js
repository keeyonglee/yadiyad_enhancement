var PageCancel = function (opts) {
    var base = this;
    var url = opts.url;
    var model = {};
    var engagementParty;
    var engagementNotify;
    var engagementPenalty;
    var returnPage;
    var $content = $('.content-service-cancel');
    var $form = $content.find('#form-service-cancel');

    base.getParentPage = function () {
        var page = null;
        if (typeof (pageServiceHires) !== "undefined") {
            page = pageServiceHires;
            engagementParty = "Seller";
            engagementNotify = "* Organization will be notified";
            engagementPenalty = "* Subject to investigation / block for 30 days";
            returnPage = "/pro/serviceapplication/hires";
        }
        if (typeof (pageServiceConfirms) !== "undefined") {
            page = pageServiceConfirms;
            engagementParty = "Buyer";
            engagementNotify = "* Job Applicant will be notified";
            engagementPenalty = "* Subject to investigation / refund may not be 100%"
            returnPage = "/pro/serviceapplication/confirms"
        }

        return page;
    };
    var parentPage = base.getParentPage();

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    //back-end interect
    base.submitServiceCancel = function (callback) {
        var data = app.getFormValue($form);
        var engagementId = 0;
        if (engagementParty == "Buyer") {
            engagementId = parentPage.model.selectedServiceConfirms.id
        }
        else if (engagementParty == "Seller") {
            engagementId = parentPage.model.selectedServiceHires.id
        }

        var settings = {
            "url": url.setJobApplicationCancel.format({
                id: engagementId
            }),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.handleResponse = function (response, successHandler) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: " Service Request Cancelled Successfully",
            }).then(function () {
                location.href = returnPage;
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
                location.href = returnPage;
            });
        }
    };

    base.validationForm = function () {
        var valid = true;

        valid = $form.valid() && valid;

        return valid;
    };

    //set user event
    base.setUserEvent = function () {
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.submitServiceCancel(base.handleResponse);
            }
        });
    };

    //inititalize page
    var init = function () {
        $form.find('[name="reasonId"]').data('sourceurl', $form.find('[name="reasonId"]').data('sourceurl').format({
            party: engagementParty
        }));
        $form.find('#investigation').html(engagementPenalty);
        $form.find('#notify').html(engagementNotify);

        app.initFormComponent($form);
        base.setUserEvent();
    };

    init();
};

//set init param
var pageCancel = new PageCancel({
    'url': {
        'setJobApplicationCancel': '/api/pro/serviceapplication/{{id}}/cancel'
    }
});