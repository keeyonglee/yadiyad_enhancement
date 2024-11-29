var PageRequestService = function (opts) {
    var base = this;
    var selector = ".content-consultation-moderator-complete";
    var url = opts.url;
    var dict = {};
    var orgRating = {};
    var model = opts.model || {
        selectedConsultationInvitation: null,
        setCandidateResult: function (result) {
            pagination.set(result)

            if (result && result.data) {
                $.each(result.data, function (i, item) {
                    item.createdOnText = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');

                    $.each(item.consultantAvailableTimeSlots, function (i, item) {
                        item.startDateText = moment(item.startDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD/MM/YYYY');
                        item.startTimeText = moment(item.startDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('hh:mma');
                        item.endTimeText = moment(item.endDate, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('hh:mma');
                    });
                });
            }

            $.observable(this.applicants).setProperty(result);

            if (this.applicants.data && this.applicants.data.length > 0) {
                $.observable(this).setProperty('selectedConsultationInvitation', this.applicants.data[0]);
                $.observable(this.applicants.data[0]).setProperty('selected', true);

                $('input.rating[type=number], div.rating').each(function () {
                    $(this).rating({
                        'iconLib': '',
                        'inactiveIcon': 'far fa-star',
                        'activeIcon': 'fa fa-star'
                    })
                });
            }

            return false;
        },
        showApplicantDetail: function () {
            $.observable(this.selectedConsultationInvitation).setProperty("showMore", true);

            return false;
        },
        showReviewForm: function () {
            var model = this;
            $('input.rating[type=number], div.rating').each(function () {
                $(this).rating({
                    'iconLib': '',
                    'inactiveIcon': 'far fa-star',
                    'activeIcon': 'fa fa-star',
                    'additionalClasses': 'cursor-pointer'
                }).on('change', function () {
                    var key = $(this).attr('name');
                    var value = $(this).val();
                    dict[key] = value;
                });
            });

            $('input.organization-rating[type=number], div.rating').each(function () {
                $(this).rating({
                    'iconLib': '',
                    'inactiveIcon': 'far fa-star',
                    'activeIcon': 'fa fa-star',
                    'additionalClasses': 'cursor-pointer'
                }).on('change', function () {
                    var key = $(this).attr('name');
                    var value = $(this).val();
                    orgRating[key] = value;
                });
            });
            return false;
        },
        publishReview: function () {
            $("#messageModeratorReviewStars").attr("hidden", true);
            $("#messageOrganizationReviewStars").attr("hidden", true);

            let isConsultantRating = $("#showConsultantRating").is(':checked')
            if (isConsultantRating) {
                var numOfStarsOrganization = Object.keys(orgRating).length;
                if (numOfStarsOrganization < 5) {
                    $("#messageOrganizationReviewStars").removeAttr("hidden");
                    return;
                }
            }
            var numOfStars = Object.keys(dict).length;

            if (numOfStars <5) {
                $("#messageModeratorReviewStars").removeAttr("hidden");
                return;
            }
            var data = {
                Dict: dict,
                OrganizationRating: orgRating,
                Remarks: $('#textReview').val(),
                OrganizationRemarks: $('#textOrganizationReview').val(),
                Id: $('#Id').val()
            };

            base.reviewConsultationInvitation(
                data,
                base.reviewConsultationInvitationResponse);

            return false;
        },
    };

    //base.reviewConsultationInvitationResponse = function (response) {
    //    base.handleResponse(response, function (response) {
    //        swal({
    //            icon: "success",
    //            title: "Review Submitted Successfully"
    //        }).then(function () {
    //            window.location.href = "/pro/consultation/facilitating"
    //        });
    //    });
    //};

    base.reviewConsultationInvitation = function (data, callback) {
        var settings = {
            "url": url.submitCompleteConsultationInvitation,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    //backend interaction
    base.getConsultationInvitationList = function (callback) {

        var settings = {
            "url": url.getConsultationInvitationList,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    //front-end response/action
    base.setConsultationInvitationList = function (response) {
        base.handleResponse(response, function (response) {
            model.setCandidateResult(response.data);
        });
    };

    //base.handleResponse = function (response, successHandler) {
    //    if (response
    //        && response.status
    //        && response.status.code === 1) {
    //        successHandler(response);
    //    } else {
    //        swal({
    //            icon: "warning",
    //            title: "Fail",
    //            text: response.status.message
    //        }).then(function () {
    //        });
    //    }
    //};

    base.reviewConsultationInvitationResponse = function (response, successHandler) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: "Consultation Completed Succesfully ",
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        }
        else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
                location.href = url.getReturnUrl;
            });
        }
    };

    //jsViews
    $.templates({
        tmplConsultationModeratorComplete: "#tmpl-consultation-moderator-complete"
    });

    var setUserEvent = function () {
        //$('#showConsultantRating').on("click", function () {
        //    let result = $("#showConsultantRating").is(':checked')
        //    if (result) {
        //        $('#divConsultantRating').attr('hidden', false)
        //    }
        //    else {
        //        $('#divConsultantRating').attr('hidden', true)
        //    }
        //})
    };

    var setData = function () {
    };

    //inititalize page
    var init = function () {
        setData();
        $.templates.tmplConsultationModeratorComplete.link(selector, model);
        //base.getConsultationInvitationList(base.setConsultationInvitationList);
        setUserEvent();

        model.showReviewForm();

    };

    init();
};

//set init param
var pageRequestService = new PageRequestService({
    'url': {
        'submitCompleteConsultationInvitation': '/api/pro/consultationinvitation/complete/',
        'getReturnUrl': '/pro/consultation/facilitating',

    }
});