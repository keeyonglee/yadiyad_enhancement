'use strict';

var PageReturnRequest = function PageReturnRequest(opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var $content = $('.content-submit-return');
    var $form = $content.find('#form-return-request');

    base.setReturnRequestData = function (response) {
        console.log(response);
        base.submitReturnRequest = function (callback) {
            var data = app.getFormValue($form);

            data.items = [];
            var reason = $form.find("#reason").find(":selected").val();
            var action = $form.find("#action").find(":selected").val();

            data.ReturnRequestReasonId = reason;
            data.ReturnRequestActionId = action;
            data.ReturnRequestImageId = $('[name="photos"]').data("downloadIdMultiple");

            var test = $form.find('[name="photos"]').data("downloadIdMultiple");
            console.log(test);
            response.data.items.forEach(function (datas, i) {
                var temp = {};
                temp.ProductName = datas.productName;
                temp.ProductId = datas.productId;

                data.items.push(temp);
            });

            $('.itemNo').each(function (i) {
                var qty = $(this).find(":selected").val();

                data.items[i].quantity = qty;
            });

            data.orderId = response.data.orderId;

            var settings = {
                "url": url.submitReturnRequest,
                "method": 'POST',
                "headers": {
                    "Content-Type": "application/json; charset=utf-8"
                },
                'data': JSON.stringify(data)
            };

            $.ajax(settings).done(callback);
        };

        base.setUserEvent();
    };

    base.onSubmitReturnRequestResponse = function (response) {
        if (response && response.status && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Return Request Successfully Created",
                text: "Saved successfully"
            }).then(function () {
                location.href = '/shuq';
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.validationForm = function () {
        var valid = true;
        valid = $form.valid() && valid;
        return valid;
    };

    base.setUserEvent = function () {
        $form.find(".btn-submit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.submitReturnRequest(base.onSubmitReturnRequestResponse);
            }
        });
    };

    base.getReturnRequestData = function (callback) {
        var settings = {
            "url": url.getReturnRequest.format({ id: orderId }),
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    var init = function init() {
        app.initFormComponent($form);
        base.getReturnRequestData(base.setReturnRequestData);
    };
    init();
};
var pageReturnRequest = new PageReturnRequest({
    'url': {
        'submitReturnRequest': '/api/shuq/return/create',
        'getReturnRequest': '/api/shuq/return/{{id}}'
    }
});

