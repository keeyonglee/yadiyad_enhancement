'use strict';

var PageConsultationReschedule = function PageConsultationReschedule(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.serviceApplicationId = null;

    //html template

    //DOM
    var $content = $('.content-facilitating-reschedule');
    var $form = $content.find('.form-facilitating-reschedule');
    var $setAppointment = $content.find('.title-set-date');
    var $rescheduleAppointment = $content.find('.title-reschedule-date');
    var $rescheduleRemarks = $content.find('.reschedule-remarks');

    base.getParentPage = function () {
        var page = null;
        if (typeof pageConsultationList !== "undefined") {
            page = pageConsultationList;
        }

        return page;
    };

    var parentPage = base.getParentPage();
    base.setDisplayProperty = function () {
        if (parentPage.model.selectedConsultation.appointmentStartDate && parentPage.model.selectedConsultation.appointmentEndDate) {
            $rescheduleAppointment.removeClass('hidden');
            $rescheduleRemarks.removeClass('hidden');
        } else {
            $setAppointment.removeClass('hidden');
        }
    };

    base.submitReschedule = function (callback) {
        var data2 = app.getFormValue($form);

        var data = {
            Id: $('#Id').val(),
            RescheduleRemarks: data2.resheduleRemarks,
            Date: data2.date,
            Start: data2.start,
            End: data2.end
        };
        var settings = {
            "url": url.submitAppointment,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responseSubmitReschedule = function (response) {
        if (response && response.status && response.status.code === 1 && response.data === "setAppointment") {
            swal({
                icon: "success",
                title: "Appointment Set Successfully"
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        } else if (response && response.status && response.status.code === 1 && response.data === "reschedule") {
            swal({
                icon: "success",
                title: "Appointment Rescheduled Successfully"
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        }
    };

    //set user event
    base.setUserEvent = function () {
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }
            base.submitReschedule(base.responseSubmitReschedule);
        });
    };

    //inititalize page
    var init = function init() {
        base.setDisplayProperty();
        app.initFormComponent($form);
        base.setUserEvent();
    };

    init();
};

//set init param
var pageConsultationReschedule = new PageConsultationReschedule({
    'url': {
        'getConsultationReschedulePage': '/api/pro/consultationinvitation/reschedule/',
        'submitAppointment': "/api/pro/consultationinvitation/set-appointment/",
        'getReturnUrl': '/pro/consultation/facilitating'
    }
});

