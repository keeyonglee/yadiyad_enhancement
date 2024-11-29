var PageBankAccountDetails = function (opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var $content = $('.content-bank-account');
    var $form = $content.find('#form-bank-account');
    var $cancelBtn = $content.find('.btn-cancel');
    var $editBtn = $content.find('.btn-edit');
    var $titleCreate = $content.find('.title-create');
    var $titleUpdate = $content.find('.title-update');
    var isNew = false;

    var $bankAccountInfo = $content.find('.bank-account-info');
    var $bankAccountEdit = $content.find('.bank-account-edit');


    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    base.setDisplayProperty = function (data) {
        data = data.data;
        model = opts.model || {};
        model.id = data.customerId;
        var reg = /{{([\d|\w]{1,})}}/g;

        data.dateOfBirthText = moment(data.dateOfBirth, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');

        $content.find("[text]")
            .each(function () {
                var text = $(this).attr('text');
                var result;
                while ((result = reg.exec(text)) !== null) {
                    var replacingText = result[0];
                    var property = result[1];
                    text = text.replace(replacingText, data[property] === 0 ? 0 : (data[property] || ''));
                }

                $(this).text(text);
            });
    };

    base.getBankAccountData = function (callback) {
        var settings = {
            "url": url.getBankAccount,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };
    base.setBankAccountData = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            app.setFormValue($form, response.data);

            if (response.data && response.data.id !== 0) {

                var data = response.data;
                var reg = /{{([\d|\w]{1,})}}/g;


                $content.find("[text]")
                    .each(function () {
                        var text = $(this).attr('text');
                        var result;
                        while ((result = reg.exec(text)) !== null) {
                            var replacingText = result[0];
                            var property = result[1];
                            text = text.replace(replacingText, data[property] === 0 ? 0 : (data[property] || ''));
                        }

                        $(this).text(text);
                    });

                $bankAccountInfo.removeClass('hidden');

                $cancelBtn.removeClass('hidden');
                $titleCreate.addClass('hidden');
                $titleUpdate.removeClass('hidden');

                $("[name='bankStatementDownloadId']").val(data.bankStatementDownloadId);

                if (response.data.status === "Approved") {
                    $(".label-status").addClass("badge-success");
                }else if (response.data.status === "Invalid") {
                    $(".label-status").addClass("badge-danger");
                } else if (response.data.status === "Pending") {
                    $(".label-status").addClass("badge-warning");
                }

            } else {
                $bankAccountEdit.removeClass('hidden');

                isNew = true;
                $cancelBtn.addClass('hidden');
                $titleCreate.removeClass('hidden');
                $titleUpdate.addClass('hidden');

            }


            var bank = $('select[name="bankId"]').select2('data');
            if (bank.length) {
                bank[0].name = response.data.bankName;
            }
        }
    };

    base.submitBankAccount = function (callback) {
        var data = app.getFormValue($form);
        data.id = data.id || 0;
        data.bankStatementDownloadId = data.bankStatementDownloadId;
        if (data.bankStatementDownloadId === "" || data.bankStatementDownloadId === "0") {
            swal({
                icon: "warning",
                title: "Fail",
                text: "Please upload bank statement header to submit."
            }).then(function () {
            });
        } else {
            var settings = {
                "url": $form.attr('action'),
                "method": $form.attr('method'),
                "headers": {
                    "Content-Type": "application/json"
                },
                'data': JSON.stringify(data)
            };
            $.ajax(settings).done(callback);
        }

    };

    base.validationForm = function () {
        var valid = true;

        valid = $form.valid() && valid;

        return valid;
    };
    base.onSubmitBankAccountResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: isNew ? "Create Bank Account" : "Update Bank Account",
                text: "Saved successfully"
            }).then(function () {
                location.reload();
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
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.submitBankAccount(base.onSubmitBankAccountResponse);
            }
        });
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            $bankAccountEdit.addClass('hidden');
            $bankAccountInfo.removeClass('hidden');
        });

        $content.find(".btn-edit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            $bankAccountInfo.addClass('hidden');
            $bankAccountEdit.removeClass('hidden');

        });
    };



    base.initDisplayComponent();
    base.getBankAccountData(base.setBankAccountData);
    base.setUserEvent();
};

var pageBankAccountDetails = new PageBankAccountDetails({
    'url': {
        'getBankAccount': '/api/pro/bankaccount',
        'getBankAccountPage': '/pro/account/bankaccount'
    }
});