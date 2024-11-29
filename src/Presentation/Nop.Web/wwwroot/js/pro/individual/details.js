var PageIndividualProfileDetails = function (opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var $content = $('.content-individual-profile');
    var $form = $content.find('#form-individual-profile');
    var $agreement = $content.find('.area-agreement');
    var $cancelBtn = $content.find('.btn-cancel');
    var $titleCreate = $content.find('.title-create');
    var $titleUpdate = $content.find('.title-update');
    var isNew = false;

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    base.getIndividualProfileData = function (callback) {
        var settings = {
            "url": url.getIndividualProfile,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };
    base.setIndividualProfileData = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {

            if (response.data.billingAddress === null) {
                response.data.billingAddress = {
                    countryId: 131,
                    countryName: "Malaysia"
                };
            };
            app.setFormValue($form, response.data);
            if (response.data.id !== 0) {
                $agreement.addClass('hidden');
                $cancelBtn.removeClass('hidden');
                $titleCreate.addClass('hidden');
                $titleUpdate.removeClass('hidden');

            } else {
                isNew = true;
                $agreement.removeClass('hidden');
                $cancelBtn.addClass('hidden');
                $titleCreate.removeClass('hidden');
                $titleUpdate.addClass('hidden');

            }

            var addressMatch = true;
            //match residence address and place of residence
            if (response.data.billingAddress.address1 != response.data.address1)
                addressMatch = false;
            else if (response.data.billingAddress.address2 != response.data.address2)
                addressMatch = false;
            else if (response.data.billingAddress.cityId != response.data.cityId)
                addressMatch = false;
            else if (response.data.billingAddress.countryId != response.data.countryId)
                addressMatch = false;
            else if (response.data.billingAddress.stateProvinceId != response.data.stateProvinceId)
                addressMatch = false;
            else if (response.data.billingAddress.zipPostalCode != response.data.zipPostalCode)
                addressMatch = false;

            if (addressMatch == true) {
                $("#sameAddress").prop('checked', true)
                $('.billing-section').hide();
            } else {
                $("#sameAddress").prop('checked', false)
                $('.billing-section').show();
            }

            var title = $('select[name="title"]').select2('data');
            if (title.length) {
                title[0].code = response.data.title;
            }

            var nationality = $('select[name="nationalityId"]').select2('data');
            if (nationality.length) {
                nationality[0].code = response.data.nationalityId;
            }

            var stateProvince = $('select[name="stateProvinceId"]').select2('data');
            if (stateProvince.length) {
                stateProvince[0].code = response.data.stateProvinceId;
            }

            var country = $('select[name="countryId"]').select2('data');
            if (country.length) {
                country[0].code = response.data.countryId;
            }

            if (response.data.billingAddress !== null) {
                var billingCity = $('select[name="billingAddress.cityId"]').select2('data');
                if (billingCity.length) {
                    billingCity[0].code = response.data.billingAddress.cityId;
                }

                var billingStateProvince = $('select[name="billingAddress.stateProvinceId"]').select2('data');
                if (billingStateProvince.length) {
                    billingStateProvince[0].code = response.data.billingAddress.stateProvinceId;
                }

                var billingCountry = $('select[name="billingAddress.countryId"]').select2('data');
                if (billingCountry.length) {
                    billingCountry[0].code = response.data.billingAddress.countryId;
                }
            }

        }
    };

    base.submitIndividualProfile = function (callback) {
        var data = app.getFormValue($form);
        data.address = [data.address1, data.address2, data.zipPostalCode].filter(Boolean).join(", ");

        if ($('#sameAddress').prop("checked") == true) {
            data.billingAddress = {
                address1: data.address1,
                address2: data.address2,
                cityId: data.cityId,
                stateProvinceId: data.stateProvinceId,
                countryId: data.countryId,
                zipPostalCode: data.zipPostalCode
            }
        } else {
            data.billingAddress = {
                address1: data["billingAddress.address1"],
                address2: data["billingAddress.address2"],
                cityId: data["billingAddress.cityId"],
                stateProvinceId: data["billingAddress.stateProvinceId"],
                countryId: data["billingAddress.countryId"],
                zipPostalCode: data["billingAddress.zipPostalCode"]
            };
        }

        var settings = {
            "url": $form.attr('action'),
            "method": $form.attr('method'),
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };
        $.ajax(settings).done(callback);
    };

    base.validationForm = function () {
        var valid = true;

        valid = $form.valid() && valid;

        return valid;
    };
    base.onSubmitIndividualProfileResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: isNew ? "Create Individual Profile" : "Update Individual Profile",
                text: "Saved successfully"
            }).then(function () {
                location.href = url.getIndividualProfilePage;
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
            });
        }
    };

    base.setUserEvent = function () {
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            location.href = url.getIndividualProfilePage;
        });
        $form.on('submit', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.submitIndividualProfile(base.onSubmitIndividualProfileResponse);
            }
        });

        $('#sameAddress').click(function () {
            if ($(this).prop("checked") == true) {
                $('.billing-section').hide();
            } else {
                $('.billing-section').show();
            }
        });
    };



    base.initDisplayComponent();
    base.getIndividualProfileData(base.setIndividualProfileData);
    base.setUserEvent();
};

var pageIndividualProfileDetails = new PageIndividualProfileDetails({
    'url': {
        'getIndividualProfile': '/api/pro/individual',
        'getIndividualProfilePage': '/pro/individual',
        'getIndividualCreateServicePage': '/pro/service/details'
    }
});