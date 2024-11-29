var PageReproposeService = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.serviceApplicationId = null;

    //html template

    //DOM
    var $content = $('.content-repropose-service');
    var $form = $content.find('#form-repropose-service');

    var $grpDuration = $content.find('#form-group-duration');

    //load data
    base.loadModel = function () {
        var modelServiceApplication = pageServiceReceives && pageServiceReceives.getModel();
        model = modelServiceApplication && modelServiceApplication.selectedServiceReceives;

        app.setFormValue($form, model);

        if (!model.id) {
            alert('service application id not found');
        } else {
            if (model.serviceTypeId === 1 || model.serviceTypeId === 2) {
                $grpDuration.removeClass('hidden');
            }
        }
    };

    base.reproposeService = function (callback) {
        var data = app.getFormValue($form);

        var settings = {
            "url": url.reproposeService,
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

    base.responseReproposeService = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Repropose Request is sent to the Buyer",
                //text: 
            }).then(function () {
                location.reload();
            });
        });
    };

    //set user event
    base.setUserEvent = function () {
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }
            base.reproposeService(base.responseReproposeService);
        });
    };

    //inititalize page
    var init = function () {
        app.initFormComponent($form);
        base.loadModel();
        base.setUserEvent();
    };

    init();
};

//set init param
var pageReproposeService = new PageReproposeService({
    'url': {
        'reproposeService': '/api/pro/serviceApplication/repropose'
    }
});