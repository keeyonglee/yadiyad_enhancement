'use strict';

var PageRequestService = function PageRequestService(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.serviceApplicationId = null;

    //html template

    //DOM
    var $content = $('.content-facilitating-cancel');
    var $form = $content.find('.form-facilitating-cancel');

    base.submitCancellation = function (callback) {
        var data2 = app.getFormValue($form);

        var data = {
            Id: $('#Id').val(),
            CancelledBy: data2.cancelledBy,
            ReasonIdBuyer: data2.reasonIdBuyer,
            ReasonIdSeller: data2.reasonIdSeller,
            ReasonOthersBuyer: data2.reasonOthersBuyer,
            ReasonOthersSeller: data2.reasonOthersSeller,
            Rehire: data2.rehire,
            ModeratorRemarks: data2.moderatorRemarks
        };
        var settings = {
            "url": url.submitCompleteConsultationInvitation,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responseSubmitCancellation = function (response, successHandler) {
        if (response && response.status && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Consultation Cancelled Succesfully"
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

    base.setUserEvent = function () {
        $form.find("button[type=submit]").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($form.valid() === false) {
                return;
            }
            base.submitCancellation(base.responseSubmitCancellation);
        });

        $(':input[name="cancelledBy"]').on("change", function () {
            var result = $(':input[name="cancelledBy"]:checked').val();
            if (result == "Seller") {
                $('#divSellerReasons').show();
                $('#divBuyerReasons').hide();
                var sellerReason = $('select[name="reasonIdSeller"]').select2('data');
                if (sellerReason.length) {
                    if (sellerReason[0].text == "Others") {
                        $('#divReasonOthersSeller').show();
                        $('#divReasonOthersBuyer').hide();
                    } else {
                        $('#divReasonOthersSeller').hide();
                        $('#divReasonOthersBuyer').hide();
                    }
                } else {
                    $('#divReasonOthersSeller').hide();
                    $('#divReasonOthersBuyer').hide();
                }
            } else {
                $('#divSellerReasons').hide();
                $('#divBuyerReasons').show();
                var buyerReason = $('select[name="reasonIdBuyer"]').select2('data');
                if (buyerReason.length) {
                    if (buyerReason[0].text == "Others") {
                        $('#divReasonOthersBuyer').show();
                        $('#divReasonOthersSeller').hide();
                    } else {
                        $('#divReasonOthersSeller').hide();
                        $('#divReasonOthersBuyer').hide();
                    }
                } else {
                    $('#divReasonOthersBuyer').hide();
                    $('#divReasonOthersSeller').hide();
                }
            }
        });

        $('select[name="reasonIdBuyer"]').on("change", function () {
            var buyerChoice = $('select[name="reasonIdBuyer"]').select2('data')[0].text;

            if (buyerChoice == "Others") {
                $('#divReasonOthersBuyer').show();
            } else {
                $('#divReasonOthersBuyer').hide();
            }
        });

        $('select[name="reasonIdSeller"]').on("change", function () {
            var sellerChoice = $('select[name="reasonIdSeller"]').select2('data')[0].text;

            if (sellerChoice == "Others") {
                $('#divReasonOthersSeller').show();
            } else {
                $('#divReasonOthersSeller').hide();
            }
        });
    };

    //inititalize page
    var init = function init() {
        app.initFormComponent($form);
        base.setUserEvent();
    };

    init();
};

//set init param
var pageRequestService = new PageRequestService({
    'url': {
        'submitCompleteConsultationInvitation': '/api/pro/consultationinvitation/cancel/',
        'getReturnUrl': '/pro/consultation/facilitating'
    }
});

