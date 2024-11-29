var PayProjectDepositRequest = function (opts) {
    var base = this;
    var url = opts.url;
    var model = {
        jobEngagement: null
    };

    //DOM
    var $content = $('.content-payprojectdepositrequest');

    //back-end request
    base.updateDepositRequest = function (callback) {
        var data = app.getFormValue($content);
        if (data.bankInSlipDownloadId === "" || data.bankInSlipDownloadId === "0") {
            swal({
                icon: "warning",
                title: "Fail",
                text: "Please upload bank deposit slip to submit."
            }).then(function () {
            });
        }
        else {
            var settings = {
                "url": url.createUpdateDepositRequest.format({
                    id: model.jobEngagement.id
                }),
                "method": 'POST',
                "headers": {
                    "Content-Type": "application/json"
                },
                "data": JSON.stringify(data)
            };
            $.ajax(settings).done(callback);
        }
    };

    //front-end response
    base.updateDepositRequestResponse = function (response) {
        base.handleResponse(response, function (response) {
            var message = "Project Deposit Bank In has submitted for approval.";
            var updatedJobEngagementStatus = 14;

            if (response.data.status === 1) {
                message = "Project Deposit has been processed.";
            }

            swal({
                icon: "success",
                title: message
            }).then(function () {
                if (response.data.status === 1) {
                    window.location.href = "/pro/jobapplication/hired";
                } else {
                    pageJobApplicant.reloadSelectedEngagement(updatedJobEngagementStatus);
                    $content.closest('.modal').modal('hide');
                }
            });

        });
    };

    //general response handler
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

    //front-end
    base.initForm = function () {
        if (typeof (pageJobApplicant) !== "undefined") {
            var modelJobApplicant = pageJobApplicant && pageJobApplicant.model;
            if (modelJobApplicant !== null) {
                model.jobEngagement = modelJobApplicant && modelJobApplicant.selectedJobEngagement;
            }
        }
        var jobEngagement = model.jobEngagement;

        var viewModel = {
            code: jobEngagement.code,
            payAmount: jobEngagement.payAmount
        };
        app.initFormComponent($content);
        $content.find('[name="payAmount"]').val("RM " + viewModel.payAmount.toFixed(2));
        $content.bindText(viewModel);
        $content.find('[name="bankInSlipDownloadId"]').prop('required', true);
        var $modalDialog = $content.closest('.modal-dialog');

        if ($content.is('.mode-offsetted')) {
            $modalDialog.removeClass('modal-lg');
            $modalDialog.addClass('modal-md');
        } else {
            $modalDialog.removeClass('modal-md');
            $modalDialog.addClass('modal-lg');
        }
    }
    
    //set user event
    base.setUserEvent = function () {
        $content.on('click', '[type="submit"]', function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($content.valid()) {
                base.updateDepositRequest(base.updateDepositRequestResponse);
            }
        });
    };

    //inititalize page
    var init = function () {
        base.initForm();
        base.setUserEvent();
    };

    init();
};

//set init param
var pagePayJobEscrow = new PayProjectDepositRequest({
    'url': {
        'createUpdateDepositRequest': '/api/pro/depositrequest/projectbasedjob/{{id}}'
    }
});