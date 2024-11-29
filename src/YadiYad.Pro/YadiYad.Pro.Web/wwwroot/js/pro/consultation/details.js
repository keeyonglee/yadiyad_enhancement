var PageConsultationProfileDetails = function (opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var $content = $('.content-consultation-profile');
    var $form = $content.find('#form-consultation-profile');
    var $agreement = $content.find('.area-agreement');
    var $cancelBtn = $content.find('.btn-cancel');
    var simpleForm = null;
    var weeklyScheduler = null;

    var startDate = new Date();
    startDate.setYear(1990);
    startDate.setMonth(1 - 1);
    startDate.setDate(1);
    startDate.setHours(0, 0, 0, 0);

    var endDate = new Date();
    endDate.setYear(1990);
    endDate.setMonth(1 - 1);
    endDate.setDate(7);
    endDate.setHours(23, 59, 59, 99);

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
        weeklyScheduler = new WeeklyScheduler('#calendar', null, null, null, {
            hourStart: 7,
            hourEnd: 22
        });
        weeklyScheduler.setEnabledDateRange('week', startDate, endDate);
        weeklyScheduler.onChanged = function (e) {
            var noSchedule = Object.keys(weeklyScheduler.Calendar._controller.schedules.items).length;

            $content.find('[name="schedule"]').val(noSchedule || "");
            $content.find('[name="schedule"]').valid();
        };

        //weeklyScheduler.onChanged();
        simpleForm = new SimpleForm({
            selector: "#content-form-design",
            model: null
        });
        var defaultData = {
            timeZoneId: 1,
            timeZoneName: "Malaysia (GMT + 8)"
        };
        app.setFormValue($form, defaultData);

    };

    Object.defineProperty(base, 'weeklyScheduler', {
        get() {
            return weeklyScheduler;
        }
    });

    Object.defineProperty(base, 'simpleForm', {
        get() {
            return simpleForm;
        }
    });

    //backend interaction
    base.getConsultationProfileData = function (callback) {
        var settings = {
            "url": url.getConsultationProfile.format(model),
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        if (model.id !== 0) {
            $content.find('.lbl-page-title').text("Update Consultation Request");
            $.ajax(settings).done(callback);
        } else {
            $content.find('.lbl-page-title').text("Create Consultation Request");
            callback(null);
        }
    };

    base.createConsultationProfile = function (callback) {
        var data = app.getFormValue($form);
        if (!data.id) {
            data.id = 0;
        }

        data.questions = simpleForm.model.fields;
        data.timeslots = base.getScheduleCollection();
        var settings = {
            "url": $form.attr('action').format(model),
            "method": $form.attr('method'),
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };
        $.ajax(settings).done(callback);
    };

    //front-end response and action
    base.setData = function (opts) {
        model = opts.model || {};
        url = opts.url;

        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.id = parseInt(urlProp[urlProp.length - 1]) || 0;

        if (model.id !== 0) {
            $agreement.addClass('hidden');
        } else {
            $cancelBtn.addClass('hidden');
        }
    };

    base.setConsultationProfileData = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {

            var consultationProfile = response.data;
            app.setFormValue($form, consultationProfile);

            var segment = $('select[name="segmentId"]').select2('data');
            if (segment.length) {
                segment[0].code = consultationProfile.segmentId;
            }

            var timeZone = $('select[name="timeZoneId"]').select2('data');
            if (timeZone.length) {
                timeZone[0].code = consultationProfile.timeZoneId;
            }

            simpleForm.setFields(consultationProfile.questions);

            var scheduleData = [];

            consultationProfile.timeSlots.forEach(function (timeslot, i) {
                var schedule = {
                    start: moment(timeslot.startDate, 'YYYY-MM-DDTHH:mm').toDate(),
                    end: moment(timeslot.endDate, 'YYYY-MM-DDTHH:mm').toDate()
                };

                scheduleData.push(schedule);
            })

            weeklyScheduler.setSchedule(scheduleData);
            $content.find('[name="schedule"]').val(scheduleData.length || "");
        }
    };

    base.getScheduleCollection = function () {
        var schedules = [];

        $.each(weeklyScheduler.Calendar._controller.schedules.items, function (i, item) {
            var schedule = {
                startDate: moment(item.start.toDate()).format('YYYY-MM-DD HH:mm'),
                endDate: moment(item.end.toDate()).format('YYYY-MM-DD HH:mm')
            };

            schedules.push(schedule);
        });

        return schedules;
    };

    base.validationForm = function () {
        var valid = true;

        valid = $form.valid() && valid;

        return valid;
    };

    base.onSubmitConsultationProfileResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: model.id === 0 ? "Consultation Profile Created Successfully" : "Consultation Profile Updated Successfully",
            }).then(function () {
                location.href = url.getConsultationProfileListPage;
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

    //frond-end user even handling
    base.setUserEvent = function () {
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.createConsultationProfile(base.onSubmitConsultationProfileResponse);
            }
        });

        $form.find('[type="number"]').off('change.decimal').on('change.decimal', function (e) {
            if (this.value && this.value.indexOf('.') > -1) {
                this.value = parseFloat(this.value).toFixed(1);
            }
        });
    };

    //initialize page
    var init = function () {
        base.initDisplayComponent();
        base.setData(opts);
        base.getConsultationProfileData(base.setConsultationProfileData);
        base.setUserEvent();
    };

    init();
};

var pageConsultationProfileDetails = new PageConsultationProfileDetails({
    'url': {
        'getConsultationProfile': '/api/pro/consultation/{{id}}',
        'getConsultationProfilePage': '/pro/consultation/{{id}}',
        'getConsultationProfileListPage': '/pro/consultation/list'
    }
});