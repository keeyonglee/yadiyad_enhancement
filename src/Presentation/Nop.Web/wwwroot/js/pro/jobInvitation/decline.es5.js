'use strict';

var PageDeclineJob = function PageDeclineJob(selector, pageJobInvites, opts) {
    var base = this;
    var url = opts.url;

    $.templates({
        tmplDeclineJobInvite: "#tmpl-decline-job-invite"
    });

    Object.defineProperty(base, 'model', {
        get: function get() {
            return model;
        }
    });

    var model = opts.model || {
        declineReasons: [],
        declineReasonOptions: ['The terms of the offer are unsatisfactary', 'The work itself is too difficult or too easy', 'You\'re worried you won\'t get along with Organization', 'The corporate culture doesn\'t feel right', 'The commute is too difficult', 'You\'ve accepted another job offer'],
        otherReason: null,
        declineJobInvitation: function declineJobInvitation() {
            var declineReasons = this.declineReasons;
            if (this.otherReason) {
                declineReasons.push(this.otherReason);
            }

            base.declineJob(declineReasons.join('\r\n'), base.responseApplyJob);
        }
    };

    //load data
    base.declineJob = function (declineReasons, callback) {
        var data = {
            id: pageJobInvites.model.selectedJobInvite.id,
            declineReasons: declineReasons,
            consultationApplicationStatus: 3
        };

        var settings = {
            "url": url.replyConsultationInvite.format(data),
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
        if (response && response.status && response.status.code === 1) {
            successHandler(response);
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.responseApplyJob = function (response) {
        base.handleResponse(response, function (response) {
            $(selector).closest('.modal').modal('hide');
            swal({
                icon: "success",
                title: "Consultant Request Declined Succesfully ",
                buttons: {
                    confirm: 'Back to profile'
                }
            }).then(function () {
                pageJobInvites.refresh();
            });
        });
    };

    //inititalize page
    var init = function init() {
        $.templates.tmplDeclineJobInvite.link(selector, model);
    };

    init();
};

var pageDeclineJob = new PageDeclineJob('.content-decline-job-invite', pageJobInvites, {
    'url': {
        'replyConsultationInvite': '/api/pro/consultationinvitation/{{id}}'
    }
});

