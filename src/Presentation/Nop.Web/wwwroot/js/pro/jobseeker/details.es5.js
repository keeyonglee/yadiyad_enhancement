'use strict';

var PageJobSeekerProfileDetails = function PageJobSeekerProfileDetails(selector, opts) {
    var base = this;
    var model = {
        data: {
            categories: [],
            licenseCertificates: [],
            academicQualifications: [],
            languageProficiencies: [],
            preferredLocations: [],
            cv: null
        },
        addCategory: function addCategory() {
            var newModel = $.extend(true, {}, {});
            newModel.uid = app.randomId;
            $.observable(this.data.categories).insert(newModel);
            app.initFormComponent($form);
        },
        removeCategory: function removeCategory(model, index) {
            $.observable(this.data.categories).remove(index);
        },
        addLicenseCertificates: function addLicenseCertificates() {
            var newModel = $.extend(true, {}, {});
            newModel.uid = app.randomId;
            $.observable(this.data.licenseCertificates).insert(newModel);
            app.initFormComponent($form);
        },
        removeLicenseCertificates: function removeLicenseCertificates(model, index) {
            $.observable(this.data.licenseCertificates).remove(index);
        },
        addAcademicQualifications: function addAcademicQualifications() {
            var newModel = $.extend(true, {}, {});
            newModel.uid = app.randomId;
            $.observable(this.data.academicQualifications).insert(newModel);
            app.initFormComponent($form);
        },
        removeAcademicQualifications: function removeAcademicQualifications(model, index) {
            $.observable(this.data.academicQualifications).remove(index);
        },
        addLanguageProficiencies: function addLanguageProficiencies() {
            var newModel = $.extend(true, {}, {});
            newModel.uid = app.randomId;
            $.observable(this.data.languageProficiencies).insert(newModel);
            app.initFormComponent($form);
        },
        removeLanguageProficiencies: function removeLanguageProficiencies(model, index) {
            $.observable(this.data.languageProficiencies).remove(index);
        },
        onIsPresentCompanyChanged: function onIsPresentCompanyChanged() {
            var isPresentCompany = !this.data.isPresentCompany;
            var tenureEnd = $form.find('[name="tenureEnd"]');
            if (isPresentCompany) {
                app.setFieldValue('tenureEnd', null);
                $form.find('#tenureEnd-error').closest('div').hide();
                tenureEnd.removeClass("input-validation-error");
            } else {
                $form.find('#tenureEnd-error').closest('div').show();
                tenureEnd.addClass("input-validation-error");
            }
        },
        onIsFreelanceHourlyChanged: function onIsFreelanceHourlyChanged() {
            var isFreelanceHourly = !this.data.isFreelanceHourly;

            if (!isFreelanceHourly) {
                $.observable(this.data).setProperty('hourlyPayAmount', null);
            }
        },
        onIsFreelanceDailyChanged: function onIsFreelanceDailyChanged() {
            var isFreelanceDaily = !this.data.isFreelanceDaily;

            if (!isFreelanceDaily) {
                $.observable(this.data).setProperty('dailyPayAmount', null);
            }
        },
        onJobModelChanged: function onJobModelChanged() {
            var isOnSite = this.data.isPartialOnSite || this.data.isOnSite;

            if (isOnSite) {
                if (this.data.preferredLocations.length === 0) {
                    var newModel = $.extend(true, {}, this.defaultCountry);
                    newModel.uid = app.randomId;
                    $.observable(this.data.preferredLocations).insert(newModel);
                    app.setFieldValue("preferredLocations[0].countryId", this.data);
                    app.initFormComponent($form);
                }
            } else {
                for (var i = this.data.preferredLocations.length - 1; i >= 0; i--) {
                    $.observable(this.data.preferredLocations).remove(i);
                }
            }
        },
        onEmploymentStatusChanged: function onEmploymentStatusChanged(employmentStatus) {
            app.setFieldValue('tenureStart', null);
            app.setFieldValue('tenureEnd', null);
            $.observable(this.data).setProperty('company', null);
            $.observable(this.data).setProperty('position', null);
            $.observable(this.data).setProperty('isPresentCompany', null);
            $.observable(this.data).setProperty('achievementAward', null);
        },
        getData: function getData() {
            var data = this.data;
            data.tenureStart = app.getFormValue($form, 'tenureStart');
            data.tenureEnd = app.getFormValue($form, 'tenureEnd');
            data.licenseCertificates = app.getFormValue($form).licenseCertificates;
            // data.cv.downloadId = app.getFormValue($form).cV ? app.getFormValue($form).cV : 0;
            data.cv = app.getFormValue($form, "cv.downloadId") ? { downloadId: app.getFormValue($form, "cv.downloadId") } : { downloadId: 0 };
            return data;
        },
        setData: function setData(data) {
            data = data || {
                id: 0
            };

            if (!data.id) {
                data.categories = data.categories && data.categories.length > 0 ? data.categories : [{
                    uid: app.randomId
                }];
                data.academicQualifications = data.academicQualifications && data.academicQualifications.length > 0 ? data.academicQualifications : [{
                    uid: app.randomId
                }];
                data.licenseCertificates = data.licenseCertificates && data.licenseCertificates.length > 0 ? data.licenseCertificates : [];

                data.languageProficiencies = data.languageProficiencies || [{
                    uid: app.randomId
                }];
            }
            for (var key in data) {
                $.observable(this.data).setProperty(key, data[key]);
            }
            app.initFormComponent($form);
            console.log(data);
            app.setFormValue($form, data);
        }
    };

    Object.defineProperty(base, 'model', {
        get: function get() {
            return model;
        }
    });

    var url = opts.url;

    $.templates({
        tmplJobSeekerProfile: "#tmpl-job-seeker-profile"
    });

    var $content = $('.content-jobseeker-profile');
    var $form = $content.find('#form-jobseeker-profile');

    //back-end interaction
    base.getJobSeekerProfileData = function (callback) {
        var settings = {
            "url": url.getJobSeekerProfile,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.submitJobSeekerProfile = function (callback) {
        var isOnsite = $('[name="isOnSite"]').is(":checked");
        var isPartialOnsite = $('[name="isPartialOnSite"]').is(":checked");
        var isRemote = $('[name="isRemote"]').is(":checked");

        var isFreelanceHourly = $('[name="isFreelanceHourly"]').is(":checked");
        var isFreelanceDaily = $('[name="isFreelanceDaily"]').is(":checked");
        var isProjectBased = $('[name="isProjectBased"]').is(":checked");

        if (!isOnsite && !isPartialOnsite && !isRemote || !isFreelanceHourly && !isFreelanceDaily && !isProjectBased) {
            swal({
                icon: "warning",
                title: "Fail",
                text: "Please select job seeking availability"
            }).then(function () {});
        } else {
            var data = model.getData();
            var settings = {
                "url": url.submitJobSeekerProfile,
                "method": 'POST',
                "headers": {
                    "Content-Type": "application/json"
                },
                'data': JSON.stringify(data)
            };

            $.ajax(settings).done(callback);
        }
    };

    base.getDefaultCountry = function (callback) {
        var settings = {
            "url": url.getDefaultCountry,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    //front-end interaction
    base.initDisplayComponent = function () {
        $.templates.tmplJobSeekerProfile.link(selector, model);
    };

    base.setDefaultCountry = function (response) {
        if (response && response.status && response.status.code === 1) {
            var defaultCountry = {
                countryId: response.data.id,
                countryName: response.data.name
            };

            $.observable(model).setProperty('defaultCountry', defaultCountry);
        }
    };

    base.validationForm = function () {
        var valid = true;

        valid = $form.valid() && valid;

        return valid;
    };

    base.setJobSeekerProfileData = function (response) {
        if (response && response.status && response.status.code === 1) {
            model.setData(response.data);
        }
    };

    base.onSubmitJobSeekerProfileResponse = function (response) {
        if (response && response.status && response.status.code === 1) {
            swal({
                icon: "success",
                title: model.id === 0 ? "Job CV Created Successfully" : "Job CV Updated Successfully"
            }).then(function () {
                location.href = '/pro/jobseeker/';
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
        $form.find(".btn-submit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            app.initFormComponent($form);
            if (base.validationForm()) {
                base.submitJobSeekerProfile(base.onSubmitJobSeekerProfileResponse);
            }
        });
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            location.href = '/pro/jobseeker/';
        });
    };

    var init = function init() {
        base.getDefaultCountry(base.setDefaultCountry);
        base.initDisplayComponent();
        base.getJobSeekerProfileData(base.setJobSeekerProfileData);
        base.setUserEvent();
    };

    init();
};

var pageJobSeekerProfileDetails = new PageJobSeekerProfileDetails('#form-jobseeker-profile', {
    'url': {
        'getJobSeekerProfile': '/api/pro/jobseeker',
        'submitJobSeekerProfile': '/api/pro/jobseeker',
        'getDefaultCountry': '/api/pro/source/country/default'
    }
});

