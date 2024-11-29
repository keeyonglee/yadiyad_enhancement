var PageAcceptJob = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.selectedJobInvite = null;

    //html template

    //DOM
    var $content = $('.content-acceptjob');
    var $form = $content.find('.form-accept-job');

    //load data
    base.loadModel = function () {
        var modelJobInvites = pageJobInvites && pageJobInvites.getModel();
        model.selectedJobInvite = modelJobInvites && modelJobInvites.selectedJobInvite;

        if (!model.selectedJobInvite) {
            alert('job invitation not found');
        }
    };

    base.acceptJob = function (callback) {
        var data = app.getFormValue($form);
        var jobInviteId = model.selectedJobInvite.id;
        var requestData = {
            JobInvitationStatus: 2,
            isEscrow: data.isEscrow
        };
        var settings = {
            "url": url.acceptJob.format({ id: jobInviteId }),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
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

    base.responseAcceptJob = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Job Invitation Accepted Successfully",
                buttons: {
                    confirm: 'Ok'
                }
            }).then(function () {
                location.href = url.returnAcceptInvite;
            });
        });
    };

    //set user event
    base.setUserEvent = function () {
        $content.on('click', '.btn-accept', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }

            base.acceptJob(base.responseAcceptJob);
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
var pageAcceptJob = new PageAcceptJob({
    'url': {
        'acceptJob': '/api/pro/jobinvitation/{{id}}',
        'returnAcceptInvite': '/pro/jobapplication/list'

    }
});