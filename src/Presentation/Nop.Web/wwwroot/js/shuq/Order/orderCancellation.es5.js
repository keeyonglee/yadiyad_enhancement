'use strict';

var PageOrderCancellation = function PageOrderCancellation(opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var $content = $('.content-order-cancellation');
    var $form = $content.find('#form-order-cancel');

    base.submitOrderCancel = function (callback) {
        var data = app.getFormValue($form);

        data.cancellationReasonId = $('input[name="radio-group"]:checked').val();
        data.OrderId = orderId;
        console.log(data);

        var settings = {
            "url": url.submitOrderCancellation,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json; charset=utf-8"
            },
            'data': JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.onSubmitOrderCancelResponse = function (response) {
        if (response && response.status && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Your order is cancelled successfully",
                text: "Saved successfully"
            }).then(function () {
                location.href = '/shuq/order/history';
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
                base.submitOrderCancel(base.onSubmitOrderCancelResponse);
            }
        });
    };

    var init = function init() {
        app.initFormComponent($form);
        base.setUserEvent();
    };
    init();
};
var pageOrderCancellation = new PageOrderCancellation({
    'url': {
        'submitOrderCancellation': '/api/shuq/orderAPI/orderCancellation'
    }
});

