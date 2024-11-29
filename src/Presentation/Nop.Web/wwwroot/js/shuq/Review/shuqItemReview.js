var PageShuqBuyerReview = function (opts) {
    var base = this;
    var url = opts.url;
    var $content = $('.content-shuq-buyer-review');
    var $form = $content.find('#form-shuq-buyer-review');

    base.submitShuqBuyerReview = function (callback) {
        var data = app.getFormValue($form);
        data.reviewPictureIds = $('[name="photos"]').data("downloadIdMultiple");
        var settings = {
            "url": url.submitShuqBuyerReview,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };
        $.ajax(settings).done(callback);
    };

    base.onSubmitShuqBuyerReviewResponse = function (response) {
        if (response
            && response.Status
            && response.Status.Code === 1) {
            swal({
                icon: "success",
                title: "Review Successfully Added",
                text: "Saved successfully"
            }).then(function () {
                location.href = '/shuq/orderdetails/' + $('[name="orderId"]').val();
            });
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
                location.href = '/shuq/orderdetails/' + $('[name="orderId"]').val();
            });
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
                base.submitShuqBuyerReview(base.onSubmitShuqBuyerReviewResponse);
            }
        });
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            location.href = '/shuq';
        });
    };

    var init = function () {
        app.initFormComponent($form);
        base.setUserEvent();
    };
    init();
};
var pageShuqBuyerReview = new PageShuqBuyerReview(
    {
        'url': {
            'submitShuqBuyerReview': '/api/products/shuqbuyerreviewadd',
        }
    }
);