'use strict';

var PageTaxAccountDetails = function PageTaxAccountDetails(opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var $content = $('.content-tax-account');
    var $form = $content.find('#form-tax-account');
    var $cancelBtn = $content.find('.btn-cancel');

    var $taxAccountInfo = $content.find('.tax-account-info');
    var $taxAccountEdit = $content.find('.tax-account-edit');

    base.initDisplayComponent = function () {
        app.initFormComponent($form);
    };

    base.setDisplayProperty = function (data) {
        data = data.data;
        model = opts.model || {};
        model.id = data.customerId;
        var reg = /{{([\d|\w]{1,})}}/g;

        $content.find("[text]").each(function () {
            var text = $(this).attr('text');
            var result;
            while ((result = reg.exec(text)) !== null) {
                var replacingText = result[0];
                var property = result[1];
                text = text.replace(replacingText, data[property] === 0 ? 0 : data[property] || '');
            }

            $(this).text(text);
        });
    };

    base.getTaxAccountData = function (callback) {
        var settings = {
            "url": url.getTaxAccount,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };
    base.setTaxAccountData = function (response) {
        if (response && response.status && response.status.code === 1) {
            app.setFormValue($form, response.data);

            if (response.data && response.data.id !== 0) {

                var data = response.data;
                var reg = /{{([\d|\w]{1,})}}/g;

                if (data.sstRegNo === null || data.sstRegNo === "") {
                    data.sstRegNo = "NOT SET";
                }

                $content.find("[text]").each(function () {
                    var text = $(this).attr('text');
                    var result;
                    while ((result = reg.exec(text)) !== null) {
                        var replacingText = result[0];
                        var property = result[1];
                        text = text.replace(replacingText, data[property] === 0 ? 0 : data[property] || '');
                    }

                    $(this).text(text);
                });

                $taxAccountInfo.removeClass('hidden');

                $cancelBtn.removeClass('hidden');
            }
        }
    };

    base.submitTaxAccount = function (callback) {
        var data = app.getFormValue($form);
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
    base.onSubmitTaxAccountResponse = function (response) {
        if (response && response.status && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Update Tax Account",
                text: "Saved successfully"
            }).then(function () {
                location.href = url.getTaxAccountPage;
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
                base.submitTaxAccount(base.onSubmitTaxAccountResponse);
            }
        });
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            $taxAccountEdit.addClass('hidden');
            $taxAccountInfo.removeClass('hidden');
        });

        $content.find(".btn-edit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            $taxAccountInfo.addClass('hidden');
            $taxAccountEdit.removeClass('hidden');
        });
    };

    base.initDisplayComponent();
    base.getTaxAccountData(base.setTaxAccountData);
    base.setUserEvent();
};

var pageTaxAccountDetails = new PageTaxAccountDetails({
    'url': {
        'getTaxAccount': '/api/pro/individual',
        'getTaxAccountPage': '/pro/account/taxaccount'
    }
});

