var PageJobSeekerProfileDetails = function (selector, opts) {
    var base = this;
    var model = {
        data: {
            categories: [{
                uid: app.randomId
            }],
            licenseCertificates: [],
            academicQualifications: [{
                uid: app.randomId
            }],
            languageProficiencies: [{
                uid: app.randomId
            }],
            preferredLocations: [{
            }],
            jobModels: "",
            jobTypes: "",
            tenureStartDate: "",
            tenureEndDate: "",
            cv: {}
        },
        updateJobModels: function () {
            var jobModels = [];

            if (this.data.isOnSite) {
                jobModels.push('Onsite');
            }

            if (this.data.isPartialOnSite) {
                jobModels.push('Partial Onsite');
            }

            if (this.data.isRemote) {
                jobModels.push('Remote');
            }

            var strJobModels = jobModels.join(', ');
            $.observable(this.data).setProperty('jobModels', strJobModels);
        },
        updateJobTypes: function () {
            var jobTypes = [];

            if (this.data.isFreelanceHourly) {
                jobTypes.push('Freelance (Hourly)');
            }

            if (this.data.isFreelanceDaily) {
                jobTypes.push('Freelance (Daily)');
            }

            if (this.data.isProjectBased) {
                jobTypes.push('Project Based');
            }

            var strJobTypes = jobTypes.join(', ');
            $.observable(this.data).setProperty('jobTypes', strJobTypes);
        },
        updateTenureDate: function () {
            var tenureStartDate = "";
            var tenureEndDate = "";
            if (this.data.tenureStart !== null) {
                tenureStartDate = moment(this.data.tenureStart, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
            }
            $.observable(this.data).setProperty('tenureStartDate', tenureStartDate);
            if (this.data.tenureEnd !== null) {
                tenureEndDate = moment(this.data.tenureEnd, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
            }
            $.observable(this.data).setProperty('tenureEndDate', tenureEndDate);
        },
        setData: function (data) {
            if (!data) {
                return false;
            }
            data.categories
                = data.categories || [{
                    uid: app.randomId
                }];
            data.academicQualifications
                = data.academicQualifications || [{
                    uid: app.randomId
                }];
            data.licenseCertificates
                = data.licenseCertificates || [];
            data.languageProficiencies
                = data.languageProficiencies || [{
                    uid: app.randomId
                }];
            data.cv
                = data.cv || {};
            for (var key in data) {
                $.observable(this.data).setProperty(key, data[key]);
            }
            this.updateJobModels();
            this.updateJobTypes();
            this.updateTenureDate();
            app.initFormComponent($form);
            app.setFormValue($form, data);
        }
    };

    Object.defineProperty(base, 'model', {
        get() {
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

    //front-end interaction
    base.initDisplayComponent = function () {
        $.templates.tmplJobSeekerProfile.link(selector, model);
    };


    base.setJobSeekerProfileData = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            model.setData(response.data);
        }
    };


    base.setUserEvent = function () {
    };

    var init = function () {
        base.initDisplayComponent();
        base.getJobSeekerProfileData(base.setJobSeekerProfileData);
        base.setUserEvent();
    };

    init();
};

var pageJobSeekerProfileDetails = new PageJobSeekerProfileDetails(
    '#form-jobseeker-profile',
    {
        'url': {
            'getJobSeekerProfile': '/api/pro/jobseeker'
        }
    }
);