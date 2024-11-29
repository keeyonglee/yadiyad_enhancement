'use strict';

var PageServiceProfileDetails = function PageServiceProfileDetails(opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;
    var defaultCountry = null;

    var serviceModelIdVal = 0;
    var $content = $('.content-service-profile');
    var $form = $content.find('#form-service-profile');
    var $grpOnsite = $content.find('.form-group-onsite');
    var $grpOnsiteCharges = $content.find('.form-group-onsite-charges');
    var $grpCharges = $content.find('.form-group-charges');
    var $grpProjectBased = $content.find('.form-group-projectBased');
    var $grpConsultation = $content.find('.form-group-consultation');
    var $grpFreelancing = $content.find('.form-group-freelancing');
    var $grpPartTime = $content.find('.form-group-part-time');
    var $grpEmployment = $content.find('.form-group-employment');

    var $agreement = $content.find('.area-agreement');
    var $grpAgreement = $content.find('.form-group-agreement');
    var originalAgreement = $content.find('.form-group-agreement').html();
    var $cancelBtn = $content.find('.btn-cancel');
    var $partialHeader = $content.find('.partial-header');

    var $titleCreate = $content.find('.title-create');
    var $titleUpdate = $content.find('.title-update');

    var $addLicenseCertBtn = $content.find('.btn-add-licenseCert');
    var $sectionLicenseCert = $content.find('.section-licenseCert');

    var $addEducationBtn = $content.find('.btn-add-education');
    var $sectionEducation = $content.find('.section-education');

    var $addLanguageBtn = $content.find('.btn-add-language');
    var $sectionLanguage = $content.find('.section-language');

    var $templateUserAgreement = '<div class="col-12 form-group-agreement">' + '<div class="form-group-test row">' + '<div class="col-sm-12 text-center">' + '<div class="radio d-inline-block mr-4">' + '<label class="col-form-label form-check-label">' + '<input type="checkbox" name="agreement" required>' + '<span>I acknowledge that I have read and agreed to be bound by the terms and conditions in the <a class="btn-link text-primary text-bold" target="_blank" href="/terms-of-service">"Terms of Service" </a> and <a class="btn-link text-primary text-bold" target="_blank" href="/consultation-guidelines-and-terms">"Consulting Guidelines and Terms" </a>.</span>' + '</label>' + '</div>' + '</div>' + '</div>' + '</div>';
    var $templateremoteServiceModel = '<div class="col-12">' + '<div class="form-group-service-model row">' + '<label for="" class="col-sm-3 col-form-label">Service Site</label>' + '<div class="col-sm-9">' + '<div class="radio d-inline-block mr-4">' + '<label class="col-form-label form-check-label">' + '<input type="radio" name="serviceModelId" value="3" required="" class="form-init" data-val="true">' + '  Remote' + '</label>' + '</div>' + '</div>' + '</div>' + '</div>';

    var $templateLicenseCert = '<div class="section-inner-licenseCert card card-body">' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Name</label>' + '<div class="col-sm-9">' + '<input type="text" class="form-control" name="professionalAssociationName{{id}}" maxlength="200" required />' + '</div>' + '</div>' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Issuing Organization</label>' + '<div class="col-sm-9">' + '<input type="text" class="form-control" name="licenseCertificateName{{id}}" maxlength="200" required />' + '</div>' + '</div>' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Document</label>' + '<div class="col-sm-9">' + '<input name="licenseCertificateDocument{{id}}" class="form-control input-dropzone" type="hidden" data-name-origin="downloadId" data-name-temp="newDownloadId" required />' + '</div>' + '</div>' + '<div class="row">' + '<ul class="col-sm-6 text-muted" style="list-style-type: circle; padding-left:20px;"><i>' + '<li>If both fields are not applicable, click ‘Remove’</li>' + '<li>If one field is not applicable, enter ‘NA’</li> ' + '</i></ul>' + '<div class="col-sm-6 text-right">' + '<button type="button" class="btn btn-default btn-remove-licenseCert" onclick="this.offsetParent.offsetParent.remove();"><i class="fa fa-trash" aria-hidden="true"></i>&nbsp;&nbsp; Remove</button>' + '</div>' + '</div>' + '</div>';

    var $templateEducation = '<div class="section-inner-education card card-body">' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Qualification</label>' + '<div class="col-sm-9">' + '<select class="form-control select2 select" required' + ' name="academicQualificationTypeId{{id}}" textName="academicQualificationTypeName{{id}}"' + ' placeholder="Qualification"' + ' data-sourceurl="/api/pro/source/academicQualificationType"' + ' data-id="value" data-text="text">' + '</select>' + '</div>' + '</div>' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Qualification Name</label>' + '<div class="col-sm-9">' + '<input type="text" class="form-control"' + ' name="academicQualificationName{{id}}" maxlength="200" required />' + '</div>' + '</div>' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Institution (Optional)</label>' + '<div class="col-sm-9">' + '<input type="text" class="form-control" maxlength="200"' + ' name="academicInstitution{{id}}" />' + '</div>' + '</div>' + '<div class="form-group">' + '<div class="float-right">' + '<button type="button" class="btn btn-default btn-remove-education" onclick="this.offsetParent.remove();"><i class="fa fa-trash" aria-hidden="true"></i>&nbsp;&nbsp; Remove</button>' + '</div>' + '</div>' + '</div>';

    var $templateLanguage = '<div class="section-inner-language card card-body">' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Language</label>' + '<div class="col-sm-9">' + '<select class="form-control select2 select" required' + ' name="languageId{{id}}" textName="languageName{{id}}"' + ' uniquegroup="language"' + ' placeholder="Language"' + ' data-sourceurl="/api/pro/source/language"' + ' data-id="id" data-text="name">' + '</select>' + '</div>' + '</div>' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Proficiency (spoken)</label>' + '<div class="col-sm-9">' + '<select class="form-control select2 select" required' + ' name="proficiencyLevel{{id}}" textName="proficiencyLevelName{{id}}"' + ' placeholder="Proficiency"' + ' data-sourceurl="/api/pro/source/LanguageProficiency"' + ' data-id="id" data-text="name">' + '</select>' + '</div>' + '</div>' + '<div class="form-group row">' + '<label for="" class="col-sm-3 col-form-label">Proficiency (written)</label>' + '<div class="col-sm-9">' + '<select class="form-control select2 select" required' + ' name="proficiencyWrittenLevel{{id}}" textName="proficiencyWrittenLevelName{{id}}"' + ' placeholder="Proficiency"' + ' data-sourceurl="/api/pro/source/LanguageProficiency"' + ' data-id="id" data-text="name">' + '</select>' + '</div>' + '</div>' + '<div class="form-group text-right">' + '<ul class="col-sm-6 text-muted text-left" style="list-style-type: circle; padding-left:20px;"><i>' + '<li>If not applicable, click ‘Remove’</li>' + '</i></ul>' + '<div >' + '<button type="button" class="btn btn-default btn-remove-language" onclick="this.offsetParent.remove();"><i class="fa fa-trash" aria-hidden="true"></i>&nbsp;&nbsp; Remove</button>' + '</div>' + '</div>' + '</div>';

    base.setData = function (opts) {
        model = opts.model || {};
        url = opts.url;

        var urlPage = window.location.pathname;
        var urlProp = urlPage.split('/');
        model.id = parseInt(urlProp[urlProp.length - 1]) || 0;
    };

    base.getServiceProfileData = function (callback) {
        var settings = {
            "url": url.getServiceProfile.format(model),
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

    base.getDefaultCountry = function (callback) {
        var settings = {
            "url": url.getDefaultCountry,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setDefaultCountry = function (response) {
        if (response && response.status && response.status.code === 1) {
            defaultCountry = {
                countryId: response.data.id,
                countryName: response.data.name
            };

            app.setFieldValue('countryId', defaultCountry);
        }
    };

    base.setServiceProfileData = function (response) {
        if (response && response.status && response.status.code === 1) {
            if (response.data.tenureEnd === null) {
                response.data.isPresentCompany = true;
            }

            response.data.tenureEnd = moment(response.data.tenureEnd).format('YYYY/MM/DD');
            response.data.tenureStart = moment(response.data.tenureStart).format('YYYY/MM/DD');

            serviceModelIdVal = response.data.serviceModelId;

            app.setFormValue($form, response.data);
            $form.addClass('mode-update');
            $form.find('[name="serviceTypeId"]:checked').trigger('change', true);
            $form.find('[name="serviceModelId"]:checked').trigger('change', true);
            $form.find('[name="employmentStatus"]:checked').trigger('change', true);
            $form.find('[name="isPresentCompany"]:checked').trigger('change', true);
            $form.find('.form-group-agreement').addClass('hidden');

            response.data.serviceLicenseCertificates.forEach(function (data, i) {
                var templateLicenseCert = $templateLicenseCert.replaceAll("{{id}}", i);
                $sectionLicenseCert.last().append(templateLicenseCert);
            });
            response.data.serviceAcademicQualifications.forEach(function (data, i) {
                var templateEducation = $templateEducation.replaceAll("{{id}}", i);
                $sectionEducation.last().append(templateEducation);
            });
            response.data.serviceLanguageProficiencies.forEach(function (data, i) {
                var templateLanguage = $templateLanguage.replaceAll("{{id}}", i);
                $sectionLanguage.last().append(templateLanguage);
            });
            base.initFormSelect();
            $(".section-inner-licenseCert").each(function (i) {
                var professionalAssociationName = response.data.serviceLicenseCertificates[i].professionalAssociationName;
                var licenseCertificateName = response.data.serviceLicenseCertificates[i].licenseCertificateName;
                var downloadName = response.data.serviceLicenseCertificates[i].downloadName;
                var downloadId = response.data.serviceLicenseCertificates[i].downloadId;
                var downloadGuid = response.data.serviceLicenseCertificates[i].downloadGuid;

                $(this).find('[name^="professionalAssociationName"]').val(professionalAssociationName);
                $(this).find('[name^="licenseCertificateName"]').val(licenseCertificateName);

                if (downloadId !== null) {
                    var $dropzone = $(this).find('[name^="licenseCertificateDocument"]').next('.dropzone');
                    var mockFile = {
                        name: downloadName,
                        size: 12345
                    };
                    var dropzone = new Dropzone.forElement($dropzone[0]);
                    var originProperty = $(this).find('[name^="licenseCertificateDocument"]').data("name-origin");

                    if (originProperty) {
                        var originValue = model[originProperty];
                        $(this).find('[name^="licenseCertificateDocument"]').data(originProperty, originValue);

                        downloadName = downloadName || originValue;
                    }

                    dropzone.removeAllFiles(true);
                    $dropzone.find('.dz-preview').remove();
                    $dropzone.find('.dropzone').removeClass('dz-started');
                    dropzone.options.addedfile.call(dropzone, mockFile);
                    //dropzone.options.thumbnail.call(dropzone, mockFile, downloadName);
                    $(this).find('[name^="licenseCertificateDocument"]').val(downloadName);
                    $(this).find('[name^="licenseCertificateDocument"]').data("downloadId", downloadId);
                    $(this).find('.dz-details').click(function (e) {
                        e.preventDefault();
                        window.open("/api/pro/document?downloadGuid=" + downloadGuid, '_blank');
                    });
                    //$(this).find('[name^="licenseCertificateDocumentDownload"]').attr("href", "/api/pro/document?downloadGuid=" + downloadGuid);
                    //$(this).find('[name^="licenseCertificateDocumentDownload"]').html(downloadName);
                }
            });

            $(".section-inner-education").each(function (i) {
                var academicQualificationTypeId = response.data.serviceAcademicQualifications[i].academicQualificationType;
                var academicQualificationTypeName = response.data.serviceAcademicQualifications[i].academicQualificationTypeName;
                var academicQualificationName = response.data.serviceAcademicQualifications[i].academicQualificationName;
                var academicInstitution = response.data.serviceAcademicQualifications[i].academicInstitution;
                var academicQualificationTypeIdOption = new Option(academicQualificationTypeName, academicQualificationTypeId, true, true);

                $(this).find('[name^="academicQualificationTypeId"]').append(academicQualificationTypeIdOption).trigger('change');
                $(this).find('[name^="academicQualificationName"]').val(academicQualificationName);
                $(this).find('[name^="academicInstitution"]').val(academicInstitution);
            });

            $(".section-inner-language").each(function (i) {
                var languageId = response.data.serviceLanguageProficiencies[i].languageId;
                var languageName = response.data.serviceLanguageProficiencies[i].languageName;
                var proficiencyLevel = response.data.serviceLanguageProficiencies[i].proficiencyLevel;
                var proficiencyLevelName = response.data.serviceLanguageProficiencies[i].proficiencyLevelName;
                var proficiencyWrittenLevel = response.data.serviceLanguageProficiencies[i].proficiencyWrittenLevel;
                var proficiencyWrittenLevelName = response.data.serviceLanguageProficiencies[i].proficiencyWrittenLevelName;
                var languageIdOption = new Option(languageName, languageId, true, true);
                var proficiencyLevelOption = new Option(proficiencyLevelName, proficiencyLevel, true, true);
                var proficiencyWrittenLevelOption = new Option(proficiencyWrittenLevelName, proficiencyWrittenLevel, true, true);

                $(this).find('[name^="languageId"]').append(languageIdOption).trigger('change');
                $(this).find('[name^="proficiencyLevel"]').append(proficiencyLevelOption).trigger('change');
                $(this).find('[name^="proficiencyWrittenLevel"]').append(proficiencyWrittenLevelOption).trigger('change');
            });

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

            var stateSet = {
                stateProvinceId: response.data.stateProvinceId,
                stateProvinceName: response.data.stateProvinceName
            };

            var citySet = {
                cityId: response.data.cityId,
                cityName: response.data.cityName
            };

            app.setFieldValue('stateProvinceId', stateSet);
            app.setFieldValue('cityId', citySet);

            //app.setFormValue($form, response.data);
            $('select[name="categoryId"]').prop("disabled", true);
            $('select[name="serviceExpertises"]').prop("disabled", true);
        }
    };

    base.submitServiceProfile = function (callback) {
        var data = app.getFormValue($form);

        //ServiceLicenseCertificates
        data.serviceLicenseCertificates = [];
        $(".section-inner-licenseCert").each(function (i) {
            var temp = {};
            temp.professionalAssociationName = $(this).find('[name^="professionalAssociationName"]').val();
            temp.licenseCertificateName = $(this).find('[name^="licenseCertificateName"]').val();

            var downloadId = $(this).find('[name^="licenseCertificateDocument"]').data("downloadId");
            var newDownloadId = $(this).find('[name^="licenseCertificateDocument"]').data("newDownloadId");
            temp.downloadId = newDownloadId !== null && newDownloadId !== undefined ? newDownloadId : downloadId;

            if (temp.professionalAssociationName !== "" && temp.licenseCertificateName !== "") {
                data.serviceLicenseCertificates.push(temp);
            }
        });

        //ServiceAcademicQualifications
        data.serviceAcademicQualifications = [];
        $(".section-inner-education").each(function (i) {
            var temp = {};
            temp.academicQualificationTypeId = $(this).find('[name^="academicQualificationTypeId"]').val();
            temp.academicQualificationName = $(this).find('[name^="academicQualificationName"]').val();
            temp.academicInstitution = $(this).find('[name^="academicInstitution"]').val();

            if (temp.academicQualificationTypeId !== null && temp.academicQualificationName !== "" && temp.academicInstitution !== "") {
                data.serviceAcademicQualifications.push(temp);
            }
        });

        //ServiceLanguageProficiencies
        data.serviceLanguageProficiencies = [];
        $(".section-inner-language").each(function (i) {
            var temp = {};
            temp.languageId = $(this).find('[name^="languageId"]').val();
            temp.proficiencyLevel = $(this).find('[name^="proficiencyLevel"]').val();
            temp.proficiencyWrittenLevel = $(this).find('[name^="proficiencyWrittenLevel"]').val();

            if (temp.languageId !== null && temp.proficiencyLevel !== null && temp.proficiencyWrittenLevel !== null) {
                data.serviceLanguageProficiencies.push(temp);
            }
        });

        if (data.employmentStatus != 1) {
            data.tenureEnd = null;
            data.tenureStart = null;
        }

        var settings = {
            "url": model.id ? url.updateServiceProfile.format(model) : url.createServiceProfile.format(model),
            "method": 'POST',
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

    base.onSubmitServiceProfileResponse = function (response) {
        if (response && response.status && response.status.code === 1) {
            swal({
                icon: "success",
                title: model.id === 0 ? "Service Profile Created Successfully" : "Service Profile Updated Successfully"
            }).then(function () {
                location.href = '/pro/service/list/';
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
        $form.find(".btn-submit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (base.validationForm()) {
                base.submitServiceProfile(base.onSubmitServiceProfileResponse);
            }
        });
        $form.find(".btn-cancel").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            location.href = url.serviceProfileList.format(model);
        });

        $form.find('[name="serviceTypeId"]').on('change', function (e, isInit) {
            var value = $(this).val();

            $grpProjectBased.addClass('hidden');
            $grpConsultation.addClass('hidden');
            $grpFreelancing.addClass('hidden');
            $grpPartTime.addClass('hidden');
            if (!isInit) {
                app.clearForm($grpProjectBased);
                app.clearForm($grpConsultation);
                app.clearForm($grpFreelancing);
                app.clearForm($grpPartTime);
            }

            switch (value) {
                case "1":
                    $grpFreelancing.removeClass('hidden');
                    $grpAgreement.html(originalAgreement);
                    $grpOnsite.addClass('hidden');
                    $("[name='serviceModelId'][value='1']").parent().show();
                    $("[name='serviceModelId'][value='2']").parent().show();
                    $("[name='serviceModelId'][value='3']").parent().hide();
                    $("[name='serviceModelId']").prop('checked', false);
                    $(".service-model-radio").addClass('mr-4');
                    $(".service-model-radio").addClass('d-inline-block');
                    if (isInit) if (serviceModelIdVal != 0) $content.find('[name="serviceModelId"]').val([serviceModelIdVal]);
                    break;
                case "2":
                    $grpPartTime.removeClass('hidden');
                    break;
                case "3":
                    $grpConsultation.removeClass('hidden');
                    $("[name='serviceModelId'][value='1']").parent().hide();
                    $("[name='serviceModelId'][value='2']").parent().hide();
                    $("[name='serviceModelId'][value='3']").parent().show();
                    $("[name='serviceModelId']").prop('checked', false);
                    $(".service-model-radio").removeClass('mr-4');
                    $(".service-model-radio").removeClass('d-inline-block');
                    $grpAgreement.html($templateUserAgreement);
                    $grpOnsite.addClass('hidden');
                    if (isInit) if (serviceModelIdVal != 0) $content.find('[name="serviceModelId"]').val([serviceModelIdVal]);
                    break;
                case "4":
                    $grpProjectBased.removeClass('hidden');
                    break;
            }

            if (value && value !== "4") {
                $grpCharges.removeClass('hidden');
            }

            var serviceModelId = $('[name="serviceModelId"]:checked').val();
            $grpOnsiteCharges.addClass('hidden');
            if (value !== "4" && (serviceModelId === "1" || serviceModelId === "2")) {
                $grpOnsiteCharges.removeClass('hidden');
            }
        });

        $(document).on('change', '[name="serviceModelId"]', function () {
            var value = $(this).val();

            $grpOnsite.addClass('hidden');

            //if (!isInit) {
            //    app.clearForm($grpOnsite);
            //}

            switch (value) {
                case "1":
                    $grpOnsite.removeClass('hidden');
                    app.setFieldValue('countryId', defaultCountry);
                    break;
                case "2":
                    $grpOnsite.removeClass('hidden');
                    app.setFieldValue('countryId', defaultCountry);
                    break;
            }

            var serviceTypeId = $('[name="serviceTypeId"]:checked').val();
            $grpOnsiteCharges.addClass('hidden');
            if (value !== "3" && serviceTypeId !== "4") {
                $grpOnsiteCharges.removeClass('hidden');
            }
        });

        $form.find('[name="employmentStatus"]').on('change', function (e, isInit) {
            var value = $(this).val();
            $grpEmployment.addClass('hidden');
            if (!isInit) {
                app.clearForm($grpEmployment);
            }

            switch (value) {
                case "1":
                    $grpEmployment.removeClass('hidden');
                    break;
            }
        });

        $form.find('[name="isPresentCompany"]').on('change', function (e, isInit) {
            var value = $(this).is(":checked");
            var employmentStatus = $($form.find('[name="employmentStatus"]:checked')).val();
            var tenureEnd = $form.find('[name="tenureEnd"]');

            if (employmentStatus != 1) {
                value = false;
            }

            switch (value) {
                case false:
                    $('[name="tenureEnd"]').prop('required', true);
                    $('[name="tenureEnd"]').removeAttr('hidden');
                    $form.find('#tenureEnd-error').closest('div').show();
                    tenureEnd.addClass("input-validation-error");

                    break;
                case true:
                    $('[name="tenureEnd"]').val(null);
                    $('[name="tenureEnd"]').removeAttr('required');
                    $('[name="tenureEnd"]').attr('hidden', true);
                    $form.find('#tenureEnd-error').closest('div').hide();
                    tenureEnd.removeClass("input-validation-error");

                    break;
            }
        });

        $addLicenseCertBtn.on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            var id = $(".section-inner-licenseCert").length;
            var templateLicenseCert = $templateLicenseCert.replaceAll("{{id}}", id);
            $sectionLicenseCert.last().append(templateLicenseCert);

            app.initFormComponent($form);
        });

        $addEducationBtn.on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            var id = $(".section-inner-education").length;
            var templateEducation = $templateEducation.replaceAll("{{id}}", id);
            $sectionEducation.last().append(templateEducation);
            base.initFormSelect();
        });

        $addLanguageBtn.on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            var id = $(".section-inner-language").length;
            var templateLanguage = $templateLanguage.replaceAll("{{id}}", id);
            $sectionLanguage.last().append(templateLanguage);
            base.initFormSelect();
        });
    };

    base.initFormSelect = function () {
        app.initFormComponent($form);
    };

    var init = function init() {
        base.getDefaultCountry(base.setDefaultCountry);
        app.initFormComponent($form);

        base.setData(opts);

        if (model.id !== 0) {
            $agreement.addClass('hidden');
            $titleCreate.addClass('hidden');
            $partialHeader.addClass('hidden');
        } else {
            $cancelBtn.addClass('hidden');
            $titleUpdate.addClass('hidden');
        }

        if (model.id) {
            base.getServiceProfileData(base.setServiceProfileData);
        }
        base.setUserEvent();
    };
    init();
};

var pageServiceProfileDetails = new PageServiceProfileDetails({
    'url': {
        'getServiceProfile': '/api/pro/service/{{id}}',
        'updateServiceProfile': '/api/pro/service/{{id}}',
        'createServiceProfile': '/api/pro/service',
        'serviceProfileList': '/pro/service/list',
        'getDefaultCountry': '/api/pro/source/country/default'
    }
});

