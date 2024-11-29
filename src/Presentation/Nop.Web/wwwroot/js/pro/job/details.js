var PageJobProfileDetails = function (opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var pageInit = true;

    var $content = $('.content-job-profile');
    var $form = $content.find('#form-job-profile');
    var $agreement = $content.find('.area-agreement');
    var $cancelBtn = $content.find('.btn-cancel');
    var $partialHeader = $content.find('.partial-header');
    var $titleCreate = $content.find('.title-create');
    var $titleUpdate = $content.find('.title-update');

    var $grpFreelancing = $content.find('.form-group-freelancing');
    var $grpPartTime = $content.find('.form-group-parttime');
    var $grpProjectBased = $content.find('.form-group-projectbased');
    var $grpTimeFrame = $content.find('.form-group-timeframe');
    var $grpLocation = $content.find('.form-group-location');
    var $grpPaymentPhase = $content.find('.form-group-paymentphase');
    var $dailyJobSchedule = $content.find('.form-group-schedule');

    var $grpMilestone = $content.find('.form-group-milestone');
    var $addMilestoneBtn = $content.find('.btn-add-milestone');
    var $sectionMilestone = $content.find('.section-milestone');

    var $jobRequiredTime = $content.find('[name="jobRequired"]');

    var $templateMilestone =
        '<div class="section-inner-milestone card card-body">' +
        '<div class="form-group row">' +
        '<label for="" class="col-sm-3 col-form-label">Description</label>' +
        '<div class="col-sm-9">' +
        '<input type="text" class="form-control"' +
        ' name="description{{id}}" required maxlength="200"/>' +
        '</div>' +
        '</div>' +
        '<div class="form-group row">' +
        '<label for="" class="col-sm-3 col-form-label">Amount</label>' +
        '<div class="col-sm-9 ">' + 
        '<div class="input-group">' +
        '<div class="input-group-prepend">' +
        '<span class="input-group-text">RM</span>' +
        '</div>' +
        '<input type="number" class="form-control text-right milestoneAmount" required ' +
        'name="amount{{id}}" placeholder="0.00" min="1" max="1000000">' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="form-group">' +
        '<div class="float-right">' +
        '<button type="button" class="btn btn-default btn-remove-milestone" onclick="this.offsetParent.remove();">Remove</button>' +
        '</div>' +
        '</div>' +
        '</div>';


    base.initDisplayComponent = function () {
        app.initFormComponent($form);
        var defaultData = {
            countryId: 131,
            countryName: "Malaysia"
        };
        app.setFormValue($form, defaultData);

    };

    base.setData = function (opts) {
        model = opts.model || {};
        url = opts.url;

        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.id = parseInt(urlProp[urlProp.length - 1]) || 0;

        if (model.id !== 0) {
            $agreement.addClass('hidden');
            $titleCreate.addClass('hidden');
            $partialHeader.addClass('hidden');

        } else {
            $cancelBtn.addClass('hidden');
            $titleUpdate.addClass('hidden');
        }
    };

    base.getJobProfileData = function (callback) {
        var settings = {
            "url": url.getJobProfile.format(model),
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        if (model.id !== 0) {
            $.ajax(settings).done(callback);
        } else {
            callback(null);
        }
    };
    base.setJobProfileData = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            app.setFormValue($form, response.data);

            var category = $('select[name="categoryId"]').select2('data');
            if (category.length) {
                category[0].code = response.data.categoryId;
            }

            var city = $('select[name="cityId"]').select2('data');
            if (city.length) {
                city[0].code = response.data.cityId;
            }

            var stateProvince = $('select[name="stateProvinceId"]').select2('data');
            if (stateProvince.length) {
                stateProvince[0].code = response.data.stateProvinceId;
            }

            var country = $('select[name="countryId"]').select2('data');
            if (country.length) {
                country[0].code = response.data.countryId;
            }

            response.data.jobMilestones.forEach(function (data, i) {
                var templateMilestone = $templateMilestone.replaceAll("{{id}}", i);
                $sectionMilestone.last().append(templateMilestone);
            });

            $(".section-inner-milestone").each(function (i) {
                var description = response.data.jobMilestones[i].description;
                var amount = response.data.jobMilestones[i].amount;

                $(this).find('[name^="description"]').val(description);
                $(this).find('[name^="amount"]').val(amount);
            });

            $(".milestoneAmount").change(function () {
                base.recalculateMilestoneTotalAmount();
            });
            $(".btn-remove-milestone").click(function () {
                base.recalculateMilestoneTotalAmount();

                $(".section-inner-milestone").each(function (i) {
                    if (i < 1) {
                        $(".btn-remove-milestone").addClass("hidden");
                    } else {
                        $(".btn-remove-milestone").removeClass("hidden");
                    }
                });
            });

            $form.find('[name="jobType"]:checked').trigger('change', true);
            $form.find('[name="isImmediate"]:checked').trigger('change', true);
            $form.find('[name="isOnsite"]:checked').trigger('change', true);
            $form.find('[name="isPartialOnsite"]:checked').trigger('change', true);

            $grpTimeFrame.removeClass('hidden');
            $grpPaymentPhase.removeClass('hidden');

            pageInit = false;
        }
    };

    base.submitJobProfile = function (callback) {

        var isOnsite = $('[name="isOnsite"]').is(":checked");
        var isPartialOnsite = $('[name="isPartialOnsite"]').is(":checked");
        var isRemote = $('[name="isRemote"]').is(":checked");

        if (!isOnsite && !isPartialOnsite && !isRemote) {
            swal({
                icon: "warning",
                title: "Fail",
                text: "Please select job model"
            }).then(function () {
            });
        } else{
            var data = app.getFormValue($form);
            if (!data.id) {
                data.id = 0;
            }

            data.jobMilestones = [];

            if (data.jobType === '3') {
                $(".section-inner-milestone").each(function (i) {
                    var temp = {};
                    temp.sequence = i;
                    temp.description = $(this).find('[name^="description"]').val();
                    temp.amount = $(this).find('[name^="amount"]').val();

                    if (temp.description !== null && temp.amount !== null) {
                        data.jobMilestones.push(temp);
                    }
                });
                
            };

            if (data.jobType === '4' && data.jobMilestones.length === 0) {
                swal({
                    icon: "warning",
                    title: "Fail",
                    text: "Milestone is required for project-based service profile."
                }).then(function () {
                });
            } else {
                var settings = {
                    "url": $form.attr('action').format(model),
                    "method": $form.attr('method'),
                    "headers": {
                        "Content-Type": "application/json"
                    },
                    'data': JSON.stringify(data)
                };
                $.ajax(settings).done(callback);
            }
        }

    };
    base.validationForm = function () {
        var valid = true;

        valid = $form.valid() && valid;

        return valid;
    };
    base.onSubmitJobProfileResponse = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            swal({
                icon: "success",
                title: model.id === 0 ? "Job Profile Created Successfully" : "Job Profile Updated Successfully",
            }).then(function () {
                location.href = url.getJobProfileListPage;
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
        $form.find(".btn-submit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                var isPublish = $(this).is('.btn-publish');
                if (isPublish) {
                    $('[name="status"]').val(1);
                    swal({
                        icon: "warning",
                        title: 'Are you sure to publish the job ad? ',
                        text: "- Important: Once Jobs are published, no changes would be allowed",
                        buttons: {
                            cancel: {
                                text: "Cancel",
                                value: false,
                                visible: true,
                                className: "btn-secondary",
                                closeModal: true
                            },
                            confirm: {
                                text: "Confirm",
                                value: true,
                                visible: true,
                                className: "btn-primary",
                                closeModal: true
                            }
                        },
                    }).then(function (isConfirm) {
                        if (isConfirm) {
                            base.submitJobProfile(base.onSubmitJobProfileResponse);
                        }
                    });
                }
                else {
                    $('[name="status"]').val(0);
                    base.submitJobProfile(base.onSubmitJobProfileResponse);
                }

            }
        });
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            location.href = url.getJobProfileListPage;
        });

        $form.find('[name="jobType"]').on('change', function (e, isInit) {
            var id = $(".section-inner-milestone").length;
            var value = $(this).val();
            $grpPaymentPhase.addClass('hidden');
            $grpProjectBased.addClass('hidden');
            $grpFreelancing.addClass('hidden');
            $grpPartTime.addClass('hidden');
            $grpMilestone.addClass('hidden');
            $('[name="payAmount"]').attr('disabled', false);
            if (!pageInit) {
                $('[name="payAmount"]').val(0);
            }
            switch (value) {
                case "1":
                    $grpPaymentPhase.removeClass('hidden');
                    $grpFreelancing.removeClass('hidden');
                    $grpTimeFrame.removeClass('hidden');
                    $dailyJobSchedule.removeClass('hidden');

                    $jobRequiredTime.attr('max', 40);
                    app.clearForm($sectionMilestone);
                    break;
                case "2":
                    $grpPaymentPhase.removeClass('hidden');
                    $grpPartTime.removeClass('hidden');
                    $grpTimeFrame.removeClass('hidden');
                    $dailyJobSchedule.removeClass('hidden');

                    $jobRequiredTime.attr('max', 31);

                    break;
                case "3":
                    $grpPaymentPhase.removeClass('hidden');
                    $grpProjectBased.removeClass('hidden');
                    $grpTimeFrame.removeClass('hidden');
                    $grpMilestone.removeClass('hidden');
                    $dailyJobSchedule.addClass('hidden');

                    $('[name="payAmount"]').attr('disabled', true);
                    base.recalculateMilestoneTotalAmount();

                    $jobRequiredTime.attr('max', 120);
                    if (id < 1) {
                        base.showMilestone();
                    }
                    app.clearForm($dailyJobSchedule);
                    break;
            }
        });

        $form.find('[name="isImmediate"]').on('change', function (e, isInit) {
            var value = $(this).is(":checked");

            switch (value) {
                case false:
                    $('[name="startDate"]').prop('required', true);
                    $('[name="startDate"]').prop('disabled', false);
                    break;
                case true:
                    $('[name="startDate"]').val(null);
                    $('[name="startDate"]').removeAttr('required');
                    $('[name="startDate"]').prop('disabled', true);
                    break;
            }
        });

        $form.find('[name="isOnsite"],[name="isPartialOnsite"],[name="isRemote"]').on('change', function (e, isInit) {
            var isOnsite = $('[name="isOnsite"]').is(":checked");
            var isPartialOnsite = $('[name="isPartialOnsite"]').is(":checked");
            var isRemote = $('[name="isRemote"]').is(":checked");

            if (isOnsite || isPartialOnsite) {
                $grpLocation.removeClass('hidden');
            }
            else {
                $grpLocation.addClass('hidden');
                //app.clearForm($grpLocation);
            }
        });

        $addMilestoneBtn.on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            base.showMilestone();
        });

        $form.find('[name="payAmount"]').on('change', function (e, isInit) {
            let value = $(this).val();
            if (value !== null && value !== "") {
                $(this).val(parseFloat(this.value).toFixed(2));
            }

        })
    };

    base.showMilestone = function () {
        var id = $(".section-inner-milestone").length;
        var templateMilestone = $templateMilestone.replaceAll("{{id}}", id);
        $sectionMilestone.last().append(templateMilestone);

        if (id < 1) {
            $(".btn-remove-milestone").addClass("hidden");
        } else {
            $(".btn-remove-milestone").removeClass("hidden");
        }

        $(".milestoneAmount").change(function () {
            base.recalculateMilestoneTotalAmount();
        });
        $(".btn-remove-milestone").click(function () {
            base.recalculateMilestoneTotalAmount();

            $(".section-inner-milestone").each(function (i) {
                if (i < 1) {
                    $(".btn-remove-milestone").addClass("hidden");
                } else {
                    $(".btn-remove-milestone").removeClass("hidden");
                }
            });
        });
        app.initFormComponent($form);
    }

    base.recalculateMilestoneTotalAmount = function () {
        var totalAmount = 0;
        $(".section-inner-milestone").each(function (i) {
            var tempAmount = $(this).find('[name^="amount"]').val();
            if (tempAmount !== null) {
                totalAmount += parseFloat(tempAmount);
            }
        });
        $("[name='payAmount']").val(totalAmount);
    };

    base.initDisplayComponent();
    base.setData(opts);
    base.setUserEvent();
    base.getJobProfileData(base.setJobProfileData);

};

var pageJobProfileDetails = new PageJobProfileDetails({
    'url': {
        'getJobProfile': '/api/pro/job/{{id}}',
        'getJobProfilePage': '/pro/job/{{id}}',
        'getJobProfileListPage': '/pro/job/list'
    }
});