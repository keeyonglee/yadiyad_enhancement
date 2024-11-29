'use strict';

var PageOrganizationProfileDetails = function PageOrganizationProfileDetails(opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var $content = $('.content-organization-profile');
    var $form = $content.find('#form-organization-profile-edit');
    var $agreement = $content.find('.area-agreement');
    var $cancelBtn = $content.find('.btn-cancel');
    var $titleCreate = $content.find('.title-create');
    var $titleUpdate = $content.find('.title-update');
    var isNew = false;

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    base.getOrganizationProfileData = function (callback) {
        var settings = {
            "url": url.getOrganizationProfile,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };
    base.setOrganizationProfileData = function (response) {
        if (response && response.status && response.status.code === 1) {
            app.setFormValue($form, response.data);

            if (response.data && response.data.id !== 0) {
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

            var segment = $('select[name="segmentId"]').select2('data');
            if (segment.length) {
                segment[0].code = response.data.segmentId;
            }

            var companySize = $('select[name="companySize"]').select2('data');
            if (companySize.length) {
                companySize[0].code = response.data.companySize;
            }

            var stateProvince = $('select[name="stateProvinceId"]').select2('data');
            if (stateProvince.length) {
                stateProvince[0].code = response.data.stateProvinceId;
            }

            var country = $('select[name="countryId"]').select2('data');
            if (country.length) {
                country[0].code = response.data.countryId;
            }

            var contactPersonTitle = $('select[name="contactPersonTitle"]').select2('data');
            if (contactPersonTitle.length) {
                contactPersonTitle[0].code = response.data.contactPersonTitle;
            }
        }
    };

    base.submitOrganizationProfile = function (callback) {
        var data = app.getFormValue($form);

        data.id = data.id || 0;

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
    base.onSubmitOrganizationProfileResponse = function (response) {
        if (response && response.status && response.status.code === 1) {
            swal({
                icon: "success",
                title: isNew ? "Create Organization Profile" : "Update Organization Profile",
                text: "Saved successfully"
            }).then(function () {
                location.href = url.getOrganizationHomePage;
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.setUserEvent = function () {
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.submitOrganizationProfile(base.onSubmitOrganizationProfileResponse);
            }
        });
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            location.href = url.getOrganizationProfilePage;
        });
        //$form.find('select[name="countryId"]').on("change", function (e) {
        //    e.preventDefault();
        //    e.stopPropagation();
        //    if ($('select[name="countryId"] option:selected')[0].text == "Singapore") {
        //        $('select[name="stateProvinceId"]').attr('disabled', true);
        //    }
        //    else {
        //        $('select[name="stateProvinceId"]').attr('disabled', false);
        //    }
        //});
    };

    base.initDisplayComponent();
    base.getOrganizationProfileData(base.setOrganizationProfileData);
    base.setUserEvent();
};

var pageOrganizationProfileDetails = new PageOrganizationProfileDetails({
    'url': {
        'getOrganizationProfile': '/api/pro/organization',
        'getOrganizationProfilePage': '/pro/organization',
        'getOrganizationCreateJobPage': '/pro/job/details',
        'getOrganizationHomePage': '/pro/organization'
    }
});

