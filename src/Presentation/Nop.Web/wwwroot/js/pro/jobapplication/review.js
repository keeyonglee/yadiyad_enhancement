var PageReviewJob = function (selector, pageJobApplicant, opts) {
    var base = this;
    var url = opts.url;

    $.templates({
        tmplReviewJob: "#tmpl-review-job"
    });


    Object.defineProperty(base, 'model', {
        get() {
            return model;
        }
    });

    var model = opts.model || {
        reviewText: "",
        knowledgenessRating: 0,
        relevanceRating: 0,
        respondingRating: 0,
        clearnessRating: 0,
        professionalismRating: 0,
        publishReview : function () {
            var data = {
                reviewText: this.reviewText,
                knowledgenessRating: this.knowledgenessRating,
                relevanceRating: this.relevanceRating,
                respondingRating: this.respondingRating,
                clearnessRating: this.clearnessRating,
                professionalismRating: this.professionalismRating,
            };

            base.reviewJobApplicant(
                this.id,
                data,
                base.reviewJobApplicantResponse);

            return false;
        }
    };

    //load data
    base.reviewJobApplicant = function (id, data, callback) {
        var settings = {
            "url": url.reviewApplicant.format({
                id: pageJobApplicant.model.selectedJobEngagement.id
            }),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
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

    base.reviewJobApplicantResponse = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Review Submitted Successfully"
            }).then(function () {
                location.reload();
            });
        });
    };

    //inititalize page
    var init = function () {
        $.templates.tmplReviewJob.link(selector, model);

        $('input.rating[type=number]').each(function () {
            $(this).rating({
                'iconLib': '',
                'inactiveIcon': 'far fa-star',
                'activeIcon': 'fa fa-star',
                'additionalClasses': 'cursor-pointer'
            }).on('change', function () {
                var key = $(this).attr('name');
                var value = $(this).val();
                $.observable(model).setProperty(key, value);
            });
        });
    };

    init();
};

var pageReviewJob = new PageReviewJob(
    '.content-review-job',
    pageJobApplicant,
    {
        'url': {
            'reviewApplicant': '/api/pro/jobApplication/{{id}}/review'
        }
    }
);