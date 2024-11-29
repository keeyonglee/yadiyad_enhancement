var PageRequestService = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.serviceApplicationId = null;

    //html template

    //DOM
    var $content = $('.content-request-service');
    var $form = $content.find('.form-request-service');

    var $grpDuration = $content.find('.form-group-duration');
    var $grpRequiredTime = $content.find('.form-group-requiredTime');
    var $grpLocation = $content.find('.form-group-location');
    var $lblRequiredTime = $content.find('.label-requiredTime');



    //load data
    base.loadModel = function () {
        var modelServiceProfile = pageServiceSearch && pageServiceSearch.getModel();
        model = modelServiceProfile && modelServiceProfile.selectedServiceProfile;

        if (!model.id) {
            alert('service profile id not found');
        } else {
            if (model.serviceTypeId === 1 || model.serviceTypeId === 2) {
                $grpDuration.removeClass('hidden');
                $grpRequiredTime.removeClass('hidden');

                if (model.serviceTypeId === 1) {
                    $lblRequiredTime.html("hours / week");
                }

                if (model.serviceTypeId === 2) {
                    $lblRequiredTime.html("days / month");
                }
            }
            if (model.serviceModelId === 1 || model.serviceModelId === 2) {
                $grpLocation.removeClass('hidden');
            }

        }
    };

    base.requestService = function (callback) {
        var data = app.getFormValue($form);
        data.serviceProfileId = model.id;
        var settings = {
            "url": url.requestService,
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

    base.responseRequestService = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Your Service Request is sent to the Seller",
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
            base.requestService(base.responseRequestService);
        });
    };

    //inititalize page
    var init = function () {
        app.initFormComponent($form);
        var selectedServiceProfile = pageServiceSearch.getModel().selectedServiceProfile;

        selectedServiceProfile.countryId = selectedServiceProfile.countryId || 131;
        selectedServiceProfile.countryName = selectedServiceProfile.countryName || "Malaysia";
        app.setFormValue($form, selectedServiceProfile);

        if (selectedServiceProfile.stateProvinceId) {
            $content.find('[name="stateProvinceId"]').prop('disabled', true);
        }

        if (selectedServiceProfile.cityId) {
            $content.find('[name="cityId"]').prop('disabled', true);
        }

        base.loadModel();
        base.setUserEvent();
    };

    init();
};

//set init param
var pageRequestService = new PageRequestService({
    'url': {
        'requestService': '/api/pro/serviceApplication/request'
    }
});