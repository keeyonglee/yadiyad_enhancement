'use strict';

$.fn.toJson = function () {
    var unindexed_array = this.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
};

$.fn.bindText = function (data) {
    var reg = /{{([\d|\w]{1,})}}/g;
    this.find("[text]").each(function () {
        var text = $(this).attr('text');
        var result;
        while ((result = reg.exec(text)) !== null) {
            var replacingText = result[0];
            var property = result[1];
            if (property && data && typeof data[property] !== undefined) {
                text = text.replace(replacingText, data[property]);
            } else {
                text = replacingText;
            }
        }

        if (text !== undefined && text !== null) {
            $(this).html(text);
        }
    });
};

//remove popup content on close
$('body').on('hidden.bs.modal', '.modal', function (e) {
    var $modalContent = $('.modal-content', this);
    if ($modalContent.hasClass('content-loaded') && !$(this).hasClass('retain')) {
        $modalContent.removeClass('content-loaded');
        $(this).removeData('bs.modal');
        $modalContent.empty();
    }
});

//resize column on modal shown
$('body').on('shown.bs.modal', '.modal', function (e) {
    if ($('.dataTable[id]', this).length) {
        var $table = $('.dataTable[id]', this).DataTable();
        $table.columns.adjust();
    }
    $('body').addClass('modal-open');
});

$('body').on('init.dt', function (e, ctx) {
    if ($.fn.dataTable) {
        $('.dataTable[id]').each(function () {
            var $table = $(this).DataTable();

            setTimeout(function () {
                $table.columns.adjust();
                $table = undefined;
            }, 200);
        });
    }
});

//destoy all js component on modal close
$('body').on('click', '[data-toggle="modal"]', function () {
    var contentUrl = $(this).attr("href") || null;
    var $modalContent = $($(this).data("target") + ' .modal-content');

    if (contentUrl !== null) {
        $modalContent.addClass('content-loaded');
        $modalContent.load(contentUrl);
    }
});

window.onerror = function (e, obj, lineNo, msg) {
    app.error = {
        'event': e,
        'obj': obj,
        'lineNo': lineNo,
        'msg': msg
    };

    if (obj && obj.jqXHR && obj.jqXHR.status === 401) {
        return true;
    }
    console.error && console.error(app.error);
    loader && loader.forceHide();
    swal && swal({
        title: "Error",
        text: "Unexpected error occur.",
        icon: "error"
    });
};

//global ajax event
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {
    app.error = {
        'event': event,
        'jqxhr': jqxhr,
        'settings': settings,
        'thrownError': thrownError
    };

    if (jqxhr && jqxhr.status === 0) {
        return true;
    }

    if (jqxhr && jqxhr.status === 401 && settings.url.indexOf("token/exp") !== -1) {
        var sessionTimeoutMsg = "You will be logged in {0} seconds.";
        var countdown = 6;

        var countingDown = setInterval(function () {
            countdown -= 1;

            if (countdown > 0) {
                swal && swal({
                    icon: "warning",
                    title: "Session Timeout",
                    text: sessionTimeoutMsg.format(countdown),
                    timer: countdown * 1000
                }).then(function () {
                    window.location.href = '/logout';
                });
            } else {
                clearInterval(countingDown);
            }
        }, 1000);

        return true;
    }

    console.error && console.error(jqxhr);

    swal && swal({
        title: "Error",
        text: "Unexpected error occur.",
        icon: "error"
    });
}).ajaxSend(function (event, jqxhr, settings) {
    if (settings.background !== true) {
        loader.show();
    }
}).ajaxStart(function () {}).ajaxStop(function () {}).ajaxComplete(function (event, jqxhr, settings) {
    if (settings.background !== true) {
        loader.hide();
    }
});

$(document).ready(function () {
    if (location.pathname === '/') {
        $('#btnBack').addClass('hidden');
    } else {
        $('#btnBack').removeClass('hidden');
    }
});

//loading
var loader = new function () {
    var base = this;
    var loadingCounter = 0;
    var $loader = $('.loader');

    base.show = function () {
        loadingCounter += 1;
        $loader.show();
    };

    base.hide = function () {
        loadingCounter -= 1;

        setTimeout(function () {
            if (loadingCounter <= 0) {
                $loader.hide();
                loadingCounter = 0;
            }
        }, 500);
    };

    base.forceHide = function () {
        $loader.hide();
    };

    window.addEventListener('beforeunload', function (event) {
        base.show();

        setTimeout(function () {
            //force hide if exceed 5s
            $loader.hide();
            loadingCounter = 0;
        }, 3000);
    });

    var pushUrl = function pushUrl(href) {
        history.pushState({}, '', href);
        window.dispatchEvent(new Event('popstate'));
    };
}();

Object.defineProperty(app, 'randomId', {
    get: function get() {
        return new Date().getTime();
    }
});

app.initLibrary = function () {
    //set data table error handle
    if ($.fn.dataTable) {
        $.fn.dataTable.ext.errMode = function (settings, tn, msg) {};
    }
};

app.initLibrary();

app.contains = function ($parents, $childs) {
    var hasChild = false;

    $parents.each(function () {
        var elemParent = this;

        $childs.each(function () {
            var elemChild = this;

            if ($.contains(elemParent, elemChild)) {
                hasChild = true;
                return false;
            }
        });

        if (hasChild) {
            return false;
        }
    });

    return hasChild;
};

app.getVarCode = function (data) {
    var varCode = '';
    for (var key in data) {
        var value = data[key];
        varCode += 'var ' + key + ' = ' + JSON.stringify(value) + ';';
    }

    return varCode;
};

app.getDependentExpressions = function (target, strValue) {
    var dependentExpressionList = [];
    if (strValue) {
        var $scopeContainer = $(target).closest('.card');

        if (!$scopeContainer.length) {
            $scopeContainer = $(target).closest('form');
        }

        var dependentExpressions = strValue.match(/[^;]+/g);
        $.each(dependentExpressions, function (i, exp) {
            var properties = exp.match(/[^:]+/g);
            var key = properties[0];
            var $selector = $scopeContainer.find('[name="' + properties[1] + '"]');

            var dependentExpression = {
                key: key,
                $selector: $selector
            };

            dependentExpressionList.push(dependentExpression);
        });
    }

    return dependentExpressionList;
};

app.initFormComponent = function ($form) {
    var classInitialed = "form-init";
    var validationMsgTemplate = $('#field-validation-msg').html();
    var templateDropzone = $('#template-dropzone').html();
    var templateDropzonePreview = $('#template-dropzone-preview').html();

    if (typeof Dropzone !== 'undefined') {
        Dropzone.autoDiscover = false;
    }

    //set validation message
    $form.find('[name]').filter(':not(.' + classInitialed + ')').addClass(classInitialed).each(function () {
        var $this = $(this);
        var type = $this.attr('type');
        var name = $this.attr('name');
        if (type === 'radio') {
            var $radioContainer = $this.closest('.radio');

            $this.attr('data-val', 'true');

            if ($radioContainer.is(':last-child') || $this.closest('.form-group ').find('.radio:last').find('input[type="radio"]').is($this)) {
                $radioContainer.after(validationMsgTemplate.replace('{{name}}', name));
            }
        } else if (type === 'checkbox') {
            var $checkboxContainer = $this.closest('.checkbox');

            $this.attr('data-val', 'true');

            if ($checkboxContainer.is(':last-child') || $this.closest('.form-group ').find('.checkbox:last').find('input[type="checkbox"]').is($this)) {
                $checkboxContainer.after(validationMsgTemplate.replace('{{name}}', name));
            }
        } else {
            $this.attr('data-val', 'true');

            var $textContainer = $this;

            if ($textContainer.closest('.input-group').length > 0) {
                $textContainer = $textContainer.closest('.input-group');
            }

            $textContainer.after(validationMsgTemplate.replace('{{name}}', name));
        }
    });

    $.validator.setDefaults({
        ignore: ':hidden:not([type="hidden"])'
    });

    $($form[0]).filter('[form-validation-alert]').on('invalid-form', function () {
        var formValidationMsg = "Ops, we can't process your input, kindly review your submission.";
        swal && swal({
            icon: "warning",
            title: formValidationMsg
        });
    });

    $.validator.unobtrusive.parse($form[0]);
    var pagingRecordSize = 20;

    $form.find('[type="date"], .form-control-datepicker').attr('autocomplete', 'off')
    //.filter(':not(.' + classInitialed + ')')
    //.addClass(classInitialed)
    .datepicker({
        enableOnReadonly: false,
        format: 'yyyy-mm-dd',
        startDate: '-200y',
        autoclose: true
    });

    $form.find('[type="date"], .form-control-datepicker-start').attr('autocomplete', 'off').datepicker({
        autoclose: true
    }).on('changeDate', function (selected) {
        var minDate = new Date(selected.date.valueOf());
        $form.find('[type="date"], .form-control-datepicker-end').datepicker('setStartDate', minDate);
    });

    $form.find('[type="date"], .form-control-datepicker-end').attr('autocomplete', 'off').datepicker({
        autoclose: true
    }).on('changeDate', function (selected) {
        var minDate = new Date(selected.date.valueOf());
        $form.find('[type="date"], .form-control-datepicker-start').datepicker('setEndDate', minDate);
    });

    $form.find('[type="date"], .form-control-datepicker-tdp').attr('autocomplete', 'off').each(function () {
        if (this.datepicker) {
            return true;
        }
        var datepicker = new TheDatepicker.Datepicker(this);
        datepicker.options.setInputFormat("d M Y");
        datepicker.options.setShowDeselectButton(false);
        datepicker.options.setShowResetButton(false);
        datepicker.options.onBeforeOpenAndClose(function (event, day, previousDay) {
            if ($(event.target).prop('disabled') || $(event.target).prop('readonly')) {
                return false;
            }
        });

        var minDate = $(this).attr("mindate");
        if (minDate) {
            if (minDate.toLowerCase() === "today") {
                datepicker.options.setMinDate(new Date());
            }

            if (!isNaN(parseInt(minDate))) {
                var mDate = parseInt(minDate);
                var setDate = new Date();
                setDate.setDate(setDate.getDate() + mDate);
                datepicker.options.setMinDate(setDate);
            }
        }

        var attrMaxDate = $(this).attr("maxdate");
        if (attrMaxDate) {
            var maxDate = new Date();

            if (attrMaxDate.toLowerCase() === "today") {
                maxDate = new Date();
            } else if (attrMaxDate.split('').reverse().join('').indexOf('y') === 0 && attrMaxDate.length > 1) {
                var strYear = attrMaxDate.replace('y');
                var year = parseInt(strYear);

                maxDate.setFullYear(maxDate.getFullYear() + year);
            } else if (!isNaN(parseInt(attrMaxDate))) {
                var intMaxDate = parseInt(attrMaxDate);
                maxDate.setDate(maxDate.getDate() + intMaxDate);
            }

            datepicker.options.setMaxDate(maxDate);
        }

        var value = $(this).val();
        var name = $(this).attr("name");
        if (value) {
            app.setFieldValue(name, value);
        }

        //datepicker.options.setShowCloseButton(false);
        datepicker.render();
    });

    if ($.fn["daterangepicker"]) {
        //$form.find('.form-control-daterangepicker').prop('readonly', true);
        $form.find('.form-control-daterangepicker').daterangepicker({
            autoUpdateInput: false,
            locale: {
                cancelLabel: 'Clear'
            }
        }).on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('DD MMM YYYY') + ' - ' + picker.endDate.format('DD MMM YYYY'));
        }).on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
        });
    }

    if ($.fn["bootstrapSwitch"]) {
        $form.find('.switch')
        //.filter(':not(.' + classInitialed + ')')
        //.addClass(classInitialed)
        .bootstrapSwitch();
    }

    if ($.fn["intlTelInput"]) {
        $form.find('[type="tel"]').intlTelInput({
            preferredCountries: ["my"],
            initialDialCode: true,
            nationalMode: false,
            americaMode: false,
            onlyCountries: [],
            customContainer: "w-100",
            separateDialCode: true
        });
    }

    $form.find('.tags')
    //.filter(':not(.' + classInitialed + ')')
    //.addClass(classInitialed)
    .each(function () {
        var $this = $(this);
        $this.tagsinput({
            confirmKeys: [13],
            tagClass: "badge badge-primary"
        });

        $this.prev('.bootstrap-tagsinput').addClass($this.attr('class')).find("input").on('keypress', function (e) {
            if (e.keyCode === 13) {
                e.keyCode = 188;
                e.preventDefault();
            };
        });
    });

    $form.find('.select').filter(':not(.select2-hidden-accessible)')
    //.addClass(classInitialed)
    .each(function () {
        var elem = this;
        var $this = $(this);
        var idProp = $this.data('id');
        var textProp = $this.data('text');
        var codeProp = $this.data('code');
        var sourceURL = $this.data('sourceurl');
        var sourceURLTemplate = $this.data('sourceurl-template');
        var method = $this.data('method');
        var searchEnabled = $this.data('search') !== false;
        var placeholder = $this.attr('placeholder');
        var staticOptions = $this.find("option").length > 0;
        var disabledClear = $this.data('disabled-clear') === true;
        var __cache = {};
        var strDependentExpression = $this.data('dependent');
        var uniquegroup = $this.attr('uniquegroup');
        var dependentExpressions = app.getDependentExpressions(this, strDependentExpression);

        var getAdvancedFilter = function getAdvancedFilter() {
            var data = {};

            $.each(dependentExpressions, function (i, item) {
                data[item.key] = item.$selector.val();
            });

            return data;
        };

        var enabledByDependent = function enabledByDependent() {
            if (dependentExpressions.length > 0) {
                var dependentValue = true;
                $.each(dependentExpressions, function (i, item) {
                    dependentValue = dependentValue && item.$selector.val();
                });

                if (dependentValue) {
                    $this.prop('disabled', false);
                } else {
                    $this.prop('disabled', true);
                }
            }
        };

        var disabledSelectedOption = function disabledSelectedOption(options, excludingOptions) {
            for (var key in options) {
                var option = options[key];
                var excluded = excludingOptions.filter(function (value, i) {
                    return option.id + "" === value;
                }).length > 0;

                if (excluded) {
                    option.disabled = true;
                }
            }
        };

        var uniqueGroupFieldValues = function uniqueGroupFieldValues($form, uniqueGroup, target) {
            var $fields = $form.find('[uniquegroup="' + uniqueGroup + '"]').filter(function (i, field) {
                return field !== target;
            });

            var values = [];

            $fields.each(function (i, item) {
                var value = $(item).val();

                if (value) {
                    values.push(value);
                }
            });

            return values;
        };

        $this.select2({
            minimumResultsForSearch: searchEnabled ? 0 : Infinity,
            placeholder: placeholder,
            allowClear: !disabledClear,
            templateSelection: function templateSelection(data, container) {
                elem.remoteData = elem.remoteData || [];
                elem.remoteData.push(data);

                return data.text;
            },
            ajax: staticOptions ? null : {
                url: sourceURL || sourceURLTemplate && function () {
                    var model = this.closest('form')[0].model;

                    return sourceURLTemplate.format(model);
                },
                method: method || 'POST',
                dataType: 'json',
                data: function data(params) {
                    var query = {
                        offset: ((params.page || 1) - 1) * pagingRecordSize,
                        recordSize: pagingRecordSize,
                        filter: params.term,
                        sorting: {
                            column: textProp,
                            order: 1
                        },
                        advancedFilter: getAdvancedFilter()
                    };
                    return query;
                },
                transport: function transport(params, success, failure) {
                    //retrieve the cached key or default to _ALL_
                    var __cachekey = params.url + '|' + JSON.stringify(params.data || "");
                    var cachedData = __cache[__cachekey] || null;

                    if (cachedData) {
                        //display the cached results
                        success(cachedData);
                        return; /* noop */
                    }
                    var $request = $.ajax(params);
                    $request.then(function (data) {
                        //store data in cache
                        __cache[__cachekey] = data;
                        //display the results
                        success(__cache[__cachekey]);
                    });
                    $request.fail(failure);
                    return $request;
                },
                processResults: function processResults(response) {
                    var items = [];
                    if (response.status.code === 1) {
                        var data = response.data;
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            if (item[codeProp] !== null) {
                                items.push({
                                    id: item[idProp],
                                    text: item[textProp],
                                    code: item[codeProp],
                                    data: item
                                });
                            } else {
                                items.push({
                                    id: item[idProp],
                                    text: item[textProp],
                                    data: item
                                });
                            }

                            if (uniquegroup) {
                                var excludingValues = uniqueGroupFieldValues($form, uniquegroup, $this[0]);
                                disabledSelectedOption(items, excludingValues);
                            }
                        }
                    }
                    return {
                        "results": items,
                        "pagination": {
                            "more": items.length >= pagingRecordSize
                        }
                    };
                }
            }
        });

        var value = $(this).attr('value');
        var valueText = $(this).attr("value-text");
        var name = $(this).attr("name");
        var textName = $(this).attr("textName");
        var model = {};
        model[name] = value;
        model[textName] = valueText;

        if (value) {
            app.setFieldValue(name, model);
        }

        //implement dependent trigger
        $.each(dependentExpressions, function (i, item) {
            item.$selector.on('change', function () {
                if (!$this.attr('freezeValue')) {
                    $this.val(null);
                }

                $this.trigger('change');
                enabledByDependent();
            });
        });
        enabledByDependent();
    });

    $(document).on('select2:opening.disabled', ':disabled', function () {
        return false;
    });

    $form.find('[type="hidden"].input-dropzone').filter(':not(.' + classInitialed + '-dropzone)').addClass(classInitialed + "-dropzone").each(function () {
        var $this = $(this);
        $this.after(templateDropzone);
        var fileType = $this.data("url");
        var actionUrl = "";
        if (fileType == "picture") {
            actionUrl = "/api/shuq/picture/asyncUpload";
        } else {
            actionUrl = "/api/pro/document";
        }
        var $dropzone = $this.next('.dropzone');
        var maxFile = $this.data("max-file");

        maxFile = maxFile === undefined ? 1 : maxFile;

        $dropzone.dropzone({
            previewTemplate: templateDropzonePreview,
            //paramName: "files",
            params: function params() {
                var oldUrl = null;
                var $imgInput = $this;
                if ($imgInput.length > 0) {
                    var tempProperty = $imgInput.data("name-temp");
                    if (tempProperty) {
                        oldUrl = $imgInput.data(tempProperty);
                    }
                }
                var param = { 'oldUrl': oldUrl };
                return param;
            },
            hiddenInputContainer: $this[0],
            autoProcessQueue: true,
            uploadMultiple: false,
            parallelUploads: 1,
            maxFiles: maxFile,
            url: actionUrl,
            maxFilesize: 5,
            acceptedFiles: "image/*,application/pdf",
            dictMaxFilesExceeded: "Only Maximum of 5 Attachments is allowed to upload",
            dictFileTooBig: "File too Big",
            accept: function accept(file, done) {
                done();
            },
            addedfile: function addedfile(file) {
                //prevent select duplicate file
                var existsCount = 0;
                var duplicatedFile = false;
                var exceedMaxFile = false;

                if (this.files.length) {
                    var i, len;
                    for (i = 0, len = this.files.length; i < len; i++) {
                        var currentFile = this.files[i];

                        if (currentFile.name.toLowerCase() === file.name.toLowerCase() && currentFile.size === file.size) {
                            existsCount++;
                            if (existsCount > 1) {
                                break;
                            }
                        }
                    }
                }

                duplicatedFile = duplicatedFile || existsCount > 1;

                //response on exceed max file
                exceedMaxFile = this.files.length > this.options.maxFiles;

                if (duplicatedFile || exceedMaxFile) {
                    this.removeFile(file);
                } else {
                    var addedfile = Dropzone.prototype.defaultOptions.addedfile.bind(this);
                    addedfile(file);
                }
                $this.val(this.hiddenFileInput.value);
                $this.trigger('change');
            },
            addRemoveLinks: true,
            removedfile: function removedfile(file) {
                var ref;
                $this.val('');
                $this.trigger('change');

                var tempMultipleProperty = $this.data("name-multiple");
                if (tempMultipleProperty && file.previewElement) {
                    //multiple
                    var current = $this.data(tempMultipleProperty);
                    var removed = $(file.previewElement).find('.dz-details').data(tempMultipleProperty);
                    current = current !== undefined ? current : [];
                    current = current.filter(function (e) {
                        return e !== removed;
                    });
                    $this.data(tempMultipleProperty, current);
                }

                return (ref = file.previewElement) !== null && (ref = file.previewElement) !== undefined ? ref.parentNode.removeChild(file.previewElement) : void 0;
            },
            // Dropzone settings
            init: function init() {
                var myDropzone = this;

                //show added file
                var value = $(this.element).attr("value");
                if (value) {
                    var mockFile = {
                        name: value,
                        size: 0
                    };
                    myDropzone.options.addedfile.call(myDropzone, mockFile);
                    myDropzone.options.thumbnail.call(myDropzone, mockFile, value);
                }

                myDropzone.on("sending", function () {});
                myDropzone.on("success", function (files, response) {
                    if (response.message !== undefined) {
                        myDropzone.removeAllFiles(true);
                        swal && swal({
                            icon: 'error',
                            title: 'Error',
                            text: response.message
                        });
                    } else {
                        var $imgInput = $this;
                        if ($imgInput.length > 0) {
                            var tempProperty = $imgInput.data("name-temp");
                            if (tempProperty) {
                                //single
                                $imgInput.val(response.downloadId);
                                $imgInput.data(tempProperty, response.downloadId);

                                $imgInput.next().find('.dz-details').last().click(function (e) {
                                    e.preventDefault();
                                    window.open(response.downloadUrl, '_blank');
                                });
                            }

                            var tempMultipleProperty = $imgInput.data("name-multiple");
                            if (tempMultipleProperty) {
                                //multiple
                                var current = $imgInput.data(tempMultipleProperty);
                                current = current !== undefined ? current : [];
                                current.push(response.downloadId);
                                $imgInput.data(tempMultipleProperty, current);

                                var $preview = $imgInput.next().find('.dz-details').last();
                                $preview.data(tempMultipleProperty, response.downloadId);
                                $preview.click(function (e) {
                                    e.preventDefault();
                                    window.open(response.downloadUrl, '_blank');
                                });
                            }
                        }
                    }
                });
                myDropzone.on("error", function (file, response) {
                    var statusCode = response.status ? response.status.code : null;
                    var statusMsg = response.status ? response.status.msg : null;
                    var errorOccur = true;
                    var isError = false;

                    if (typeof response === 'string') {
                        statusMsg = response;
                    } else if (statusCode === -500) {
                        statusMsg = 'Unexpected error occurred.';
                        $(file.previewElement).find('.dz-error-message').text(statusMsg);
                        isError = true;
                    } else if (statusCode === 1) {
                        $(file.previewElement).find('.dz-error-message').text(statusMsg);
                        isError = true;
                    } else {
                        statusMsg = $(file.previewElement).find('.dz-error-message').text();
                        if (!statusMsg) {
                            statusMsg = statusMsg.substring(0, 120);
                        }
                        isError = true;
                    }

                    swal && swal({
                        icon: isError ? 'error' : 'warning',
                        title: isError ? 'Error' : 'Warning',
                        text: statusMsg
                    });
                });
                myDropzone.on("complete", function (file) {});
            }
        });
    });

    $form.find('[type="text"][format]')
    //.filter(':not(.' + classInitialed + ')')
    //.addClass(classInitialed)
    .each(function () {
        var $this = $(this);
        var isWithInSubForm = $.contains($form.find('.sub-form'), $this);

        if (isWithInSubForm) {
            return;
        }

        $this.blur(function () {
            var $this = $(this);
            var format = $this.attr('format');
            var value = $this.val();
            var disabled = $this.prop('disabled');

            if (typeof value !== 'undefined' && value !== null && value !== '' && value !== '0') {
                $this.formatNumber({
                    format: format,
                    locale: "us"
                });
            } else if (disabled) {
                $this.val('-');
            }
        });

        $this.focus(function () {
            var $this = $(this);
            var format = $this.attr('format');
            var value = $this.val();

            if (value) {
                $this.parseNumber({
                    format: format,
                    locale: "us"
                });
            }
        });
    });

    $form.find('[type="number"]').off('change.decimal').on('change.decimal', function (e) {
        if (this.value && this.value.indexOf('.') > -1) {
            this.value = parseFloat(this.value).toFixed(2);
        }
    });
};

app.getModelValue = function (model, fieldExpr) {
    var value = model;
    var propreties = ("." + fieldExpr).match(/(\.[\d|\w]+)|(\[[\d]+\])/g);

    for (var key in propreties) {
        var prop = propreties[key];
        if (prop.charAt(0) === '.') {
            prop = prop.replace(/\./, '');
        } else if (prop.charAt(0) === '[') {
            prop = parseInt(prop.replace(/[\[|\]]/g, ''));
        }
        value = value[prop];

        if (!value) {
            break;
        }
    }

    return value;
};

app.setModelValue = function (model, fieldExpr, newValue) {
    var value = model;
    var propreties = ("." + fieldExpr).match(/(\.[\d|\w]+)|(\[[\d]+\])/g);

    for (var key = 0; key < propreties.length; key++) {
        var prop = propreties[key];
        if (prop.charAt(0) === '.') {
            prop = prop.replace(/\./, '');
        } else if (prop.charAt(0) === '[') {
            prop = parseInt(prop.replace(/[\[|\]]/g, ''));
        }

        if (key + 1 === propreties.length) {
            value[prop] = newValue;
        } else {
            value = value[prop];
        }
    }
};

app.$setFieldValue = function (data, isWithInSubForm) {
    var model = jQuery.extend(true, {}, data);
    var $this = $(this);
    var property = $this.attr('name') || '';
    var format = $this.attr('format');
    var isMultiple = $this.is('[multiple]');
    var type = $this.hasClass('input-dropzone') ? "dropzone" : (($this.hasClass('form-control-datepicker') ? 'date' : '') || ($this.hasClass('form-control-datepicker-tdp') ? 'date-tdp' : '') || ($this.hasClass('switch') ? 'switch' : '') || $this.attr('type') || ($this.is('select.select') ? 'select' : '') || ($this.is('textarea') ? 'textarea' : '') || ($this.is('select.tags') ? 'tags' : '') || ($this.is('label') ? 'label' : '') || ($this.is('span') ? 'span' : '') || ($this.is('table') ? 'table' : '')).toLowerCase();

    if (isWithInSubForm) {
        return;
    }

    var value = app.getModelValue(model, property);

    switch (type) {
        case 'text':
        case 'number':
        case 'hidden':
        case 'textarea':
        case 'email':
        case 'password':
            $this.val(value);

            if (format) {
                $this.formatNumber({
                    format: format,
                    locale: "us"
                });
            }
            break;
        case 'tel':
            $this.intlTelInput('setNumber', value || "");
            break;
        case 'switch':
            $this.bootstrapSwitch('state', value);
            break;
        case 'date':
            value = moment(value, 'YYYY-MM-DDTHH:mm:ss').toDate();
            $this.datepicker("update", value || "");
            break;
        case 'date-tdp':
            var datepicker = $this[0].datepicker;
            if (value) {
                value = moment(value, 'YYYY-MM-DDTHH:mm:ss');
                var year = value.year();
                if (year >= 1900) {
                    datepicker.selectDate(value.format('DD MMM YYYY'));
                } else {
                    datepicker.selectDate(null);
                }
            } else {
                datepicker.selectDate(null);
            }
            break;
        case 'select':
            if (isMultiple) {
                if (value) {
                    var idProp = $this.data('id');
                    var textProp = $this.data('text');

                    for (var key in value) {
                        var itemData = value[key];

                        var option = new Option(itemData[textProp], itemData[idProp], true, true);
                        $this.append(option);
                    }
                }
            } else {
                var textProp = $this.attr('textName');
                var textValue = app.getModelValue(model, textProp);

                if (textValue) {
                    var option = new Option(textValue, value, true, true);
                    $this.append(option);
                } else {
                    $this.val(null);
                }
            }
            $this.trigger('change');
            break;
        case 'radio':
            $this.filter('[value="' + value + '"]').prop('checked', true);
            break;
        case 'checkbox':
            if (typeof value === 'boolean') {
                if (value === true) {

                    $this.prop('checked', true);
                }
            } else if (typeof value === 'object') {

                for (var key in value) {
                    var selectedValue = value[key];
                    $this.filter('[value="' + selectedValue + '"]').prop('checked', true);
                }
            }
            break;
        case 'label':
        case 'span':
            $this.text(value);
            break;
        case 'tags':
            for (var key in value) {
                var selectedValue = value[key];
                $this.tagsinput('add', selectedValue);
            }
            break;
        case 'table':
            var $table = $this.DataTable();
            $table.clear();
            $table.rows.add(value).draw();

            app.initFormComponent($this);
            break;
        case 'dropzone':
            var $dropzone = $this.next('.dropzone');

            var dropzone = new Dropzone.forElement($dropzone[0]);
            var originProperty = $this.data("name-origin");
            var originId = $this.data("name-origin-id");
            var originName = $this.data("name");

            var originValue = app.getModelValue(model, originProperty);
            var originIdValue = app.getModelValue(model, originId);
            var originNameValue = app.getModelValue(model, originName);

            var mockFile = {
                name: originNameValue,
                size: 12345
            };

            if (originProperty) {

                $this.data(originProperty, originValue);

                value = value || originValue;
            }
            if (originValue !== null) {
                dropzone.removeAllFiles(true);
                $dropzone.find('.dz-preview').remove();
                $dropzone.find('.dropzone').removeClass('dz-started');
                dropzone.options.addedfile.call(dropzone, mockFile);
                //dropzone.options.thumbnail.call(dropzone, mockFile, value);
                $this.val(value);

                $dropzone.find('.dz-details').click(function (e) {
                    e.preventDefault();
                    window.open("/api/pro/document?downloadGuid=" + originIdValue, '_blank');
                });
            }
            break;

    }
};

app.setFieldValue = function (fieldName, value) {
    var model = {};

    if (value && typeof value === 'object') {
        model = value;
    } else {
        model[fieldName] = value;
    }

    $('[name="' + fieldName + '"]:not(.ignore-model)').each(function () {
        app.$setFieldValue.call(this, model);
    });
};

app.setFormValue = function ($form, data) {
    var model = jQuery.extend(true, {}, data);
    //reset validation
    var validator = $form.data("validator");
    if (validator) {
        validator.resetForm();
    }

    if (!model) {
        app.clearForm($form);
        return;
    }

    $form.find('[name]:not(.ignore-model)').attr('freezeValue', true);

    $form.find('[name]:not(.ignore-model)').each(function () {
        var $this = $(this);
        var isWithInSubForm = app.contains($form.find('.sub-form'), $this);
        app.$setFieldValue.call(this, model, isWithInSubForm);
    });

    var varCode = app.getVarCode(model);

    $form.find('.form-control-note').each(function () {
        var $this = $(this);
        var ifFormula = $this.attr('ng-if');
        var value = eval(varCode + ifFormula);

        if (value) {
            $this.removeClass('hidden');
        } else {
            $this.addClass('hidden');
        }
    });

    var reg = /{{([\d|\w|\s]{1,})}}/g;
    $form.find("[text]").each(function () {
        var text = $(this).attr('text');
        var result;
        while ((result = reg.exec(text)) !== null) {
            var replacingText = result[0];
            var property = result[1].trim();
            text = text.replace(replacingText, model[property] === 0 ? 0 : model[property] || '');
        }

        $(this).text(text);
    });

    $form.find('[name]:not(.ignore-model)').removeAttr('freezeValue');
};

app.getFormValue = function ($form, fieldName) {
    var model = {};

    $form.find(fieldName ? '[name="' + fieldName + '"]' : '[name]').each(function () {
        var fieldModel = model;
        var $this = $(this);
        var property = $this.attr('name') || '';
        var isMultiple = $this.is('[multiple]');
        var type = $this.hasClass('input-dropzone') ? "dropzone" : (($this.hasClass('form-control-datepicker') ? 'date' : '') || ($this.hasClass('form-control-datepicker-tdp') ? 'date-tdp' : '') || $this.attr('type') || ($this.is('select.select') ? 'select' : '') || ($this.is('textarea') ? 'textarea' : '') || ($this.is('select.tags') ? 'tags' : '')).toLowerCase();

        var parse = $(this).attr('parse');

        //cater for multiple list
        var propertyProps = property.match(/([\w|\d]*)\[([-|\d]*)\]\.([\w|\d]*)/);
        if (propertyProps) {
            if (!fieldModel[propertyProps[1]]) {
                fieldModel[propertyProps[1]] = {};
            }
            if (!fieldModel[propertyProps[1]][propertyProps[2]]) {
                fieldModel[propertyProps[1]][propertyProps[2]] = {};
            }

            fieldModel = fieldModel[propertyProps[1]][propertyProps[2]];
            property = propertyProps[3];
        }

        switch (type) {
            case 'text':
            case 'number':
            case 'hidden':
            case 'textarea':
            case 'password':
            case 'email':
                fieldModel[property] = $this.val();
                if (parse === 'JSON') {
                    fieldModel[property] = JSON.parse(fieldModel[property]);
                }

                break;
            case 'tel':
                fieldModel[property] = $this.intlTelInput('getNumber');
                break;
            case 'date':
                fieldModel[property] = $this.val();
                break;
            case 'date-tdp':
                var value = $this[0].datepicker.getSelectedDate();
                value = value && moment(value, 'YYYY-MM-DDTHH:mm:ss');
                fieldModel[property] = value && value.format('YYYY-MM-DD');
                break;
            case 'select':
                var selection = $this.select2('data');
                var idProp = $this.data('id');
                var textProp = $this.data('text');
                if (isMultiple) {
                    var selectionData = [];
                    for (var key in selection) {
                        var itemData = {};
                        itemData[textProp] = selection[key].text;
                        itemData[idProp] = selection[key].id;

                        selectionData.push(itemData);
                    }

                    fieldModel[property] = selectionData;
                } else {
                    var singleSelection = selection.length ? selection[0] : {};

                    textProp = $this.attr('textName');

                    fieldModel[textProp] = singleSelection.text;
                    fieldModel[property] = singleSelection.id;
                }
                break;
            case 'radio':
                fieldModel[property] = $form.find('[name="' + property + '"]:checked').val();
                break;
            case 'checkbox':
                fieldModel[property] = [];
                if ($form.find('[name="' + property + '"]').length > 1) {
                    $form.find('[name="' + property + '"]:checked').each(function () {
                        model[property].push($(this).val());
                    });
                } else {
                    fieldModel[property] = $form.find('[name="' + property + '"]').is(":checked");
                }

                break;
            case 'tags':
                fieldModel[property] = $this.tagsinput('items');
                break;
            case 'dropzone':
                var $dropzone = $this.next('.dropzone');
                var dropzone = new Dropzone.forElement($dropzone[0]);
                var originProperty = $this.data("name-origin");
                var tempProperty = $this.data("name-temp");

                if (dropzone.files.length > 0) {
                    if (tempProperty) {
                        fieldModel[property] = $this.data(tempProperty);
                    } else {
                        fieldModel[property] = dropzone.files[0].dataURL;
                    }
                } else {
                    var $img = $dropzone.find(".dz-preview").find("img");

                    fieldModel[property] = $img.length > 0 ? $img[0].src : undefined;
                }

                if (originProperty) {
                    app.setModelValue(model, originProperty, $this.data(tempProperty) || $this.data(originProperty));
                    //fieldModel[originProperty] = $this.data(originProperty);
                }

                break;

        }
    });

    for (var prop in model) {
        if (typeof model[prop] === 'object' && model[prop] !== null && typeof model[prop].length === 'undefined') {
            var arrayValue = [];

            for (var key in model[prop]) {
                arrayValue.push(model[prop][key]);
            }

            model[prop] = arrayValue;
        }
    }

    return fieldName ? model[fieldName] : model;
};

app.clearForm = function ($form) {

    var validator = $form.data("validator");
    if (validator) {
        validator.resetForm();
    }

    $form.find('.field-validation-error').empty().addClass('field-validation-valid').removeClass('field-validation-error');

    $form.find('[name]').each(function () {
        var $this = $(this);
        var property = $this.attr('name') || '';
        var type = $this.hasClass('input-dropzone') ? "dropzone" : (($this.hasClass('form-control-datepicker') ? 'date' : '') || $this.attr('type') || ($this.is('select.select') ? 'select' : '') || ($this.is('textarea') ? 'textarea' : '') || ($this.is('select.tags') ? 'tags' : '')).toLowerCase();

        switch (type) {
            case 'text':
            case 'number':
            case 'hidden':
            case 'textarea':
            case 'password':
            case 'email':
                $this.val(null);
                break;
            case 'tel':
                $this.intlTelInput('setNumber', "");
                break;
            case 'date':
                $this.datepicker("update", null);
                break;
            case 'select':
                $this.val(null).trigger('change');
                break;
            case 'radio':
                $this.filter(':checked').prop('checked', false);
                break;
            case 'checkbox':
                $this.prop('checked', false);
                break;
            case 'tags':
                $this.tagsinput('removeAll');
                break;
            case 'dropzone':
                var $dropzone = $this.next('.dropzone');
                var dropzone = new Dropzone.forElement($dropzone[0]);
                dropzone.removeAllFiles(true);
                $dropzone.find('.dz-preview').remove();
                $dropzone.removeClass('dz-started');
                $this.val(null);
                var originProperty = $this.data("name-origin");
                if (originProperty) {
                    $this.data(originProperty, null);
                }
                var contentProperty = $this.data("name-content");
                if (contentProperty) {
                    $this.data(contentProperty, null);
                }
                var tempProperty = $this.data("name-temp");
                if (tempProperty) {
                    $this.data(tempProperty, null);
                }
                break;

        }
    });
};

app.disableForm = function ($form) {
    $form.find('[name]').each(function () {
        var $this = $(this);
        var property = $this.attr('name') || '';
        var type = $this.hasClass('input-dropzone') ? "dropzone" : (($this.hasClass('form-control-datepicker') ? 'date' : '') || $this.attr('type') || ($this.is('select.select') ? 'select' : '') || ($this.is('textarea') ? 'textarea' : '') || ($this.is('select.tags') ? 'tags' : '')).toLowerCase();

        switch (type) {
            case 'text':
            case 'tel':
            case 'number':
            case 'hidden':
            case 'textarea':
            case 'password':
            case 'email':
                $this.prop("readonly", true);
                break;
            case 'date':
                $this.prop("readonly", true);
                break;
            case 'select':
                $this.prop('disabled', true);
                break;
            case 'radio':
                $this.prop("disabled", true);
                break;
            case 'checkbox':
                $this.prop("disabled", true);
                break;
            case 'tags':
                var $control = $this.prev('.bootstrap-tagsinput');

                $control.addClass('disabled');

                break;
            case 'dropzone':
                $this.prop("readonly", true);
                break;

        }
    });
};

var dateComparer = {
    convert: function convert(d) {
        // Converts the date in d to a date-object. The input can be:
        //   a date object: returned without modification
        //  an array      : Interpreted as [year,month,day]. NOTE: month is 0-11.
        //   a number     : Interpreted as number of milliseconds
        //                  since 1 Jan 1970 (a timestamp)
        //   a string     : Any format supported by the javascript engine, like
        //                  "YYYY/MM/DD", "MM/DD/YYYY", "Jan 31 2009" etc.
        //  an object     : Interpreted as an object with year, month and date
        //                  attributes.  **NOTE** month is 0-11.
        return d.constructor === Date ? d : d.constructor === Array ? new Date(d[0], d[1], d[2]) : d.constructor === Number ? new Date(d) : d.constructor === String ? new Date(d) : typeof d === "object" ? new Date(d.year, d.month, d.date) : NaN;
    },
    compare: function compare(a, b) {
        // Compare two dates (could be of any type supported by the convert
        // function above) and returns:
        //  -1 : if a < b
        //   0 : if a = b
        //   1 : if a > b
        // NaN : if a or b is an illegal date
        // NOTE: The code inside isFinite does an assignment (=).
        return isFinite(a = this.convert(a).valueOf()) && isFinite(b = this.convert(b).valueOf()) ? (a > b) - (a < b) : NaN;
    },
    compareToday: function compareToday(a) {
        // Compare date with today (could be of any type supported by the convert
        // function above) and returns:
        //  -1 : if a < today
        //   0 : if a = today
        //   1 : if a > today
        // NaN : if a is an illegal date
        var today = new Date();
        return isFinite(a = this.convert(a).valueOf()) ? (a > today) - (a < today) : NaN;
    },
    inRange: function inRange(d, start, end) {
        // Checks if date in d is between dates in start and end.
        // Returns a boolean or NaN:
        //    true  : if d is between start and end (inclusive)
        //    false : if d is before start or after end
        //    NaN   : if one or more of the dates is illegal.
        // NOTE: The code inside isFinite does an assignment (=).
        return isFinite(d = this.convert(d).valueOf()) && isFinite(start = this.convert(start).valueOf()) && isFinite(end = this.convert(end).valueOf()) ? start <= d && d <= end : NaN;
    },
    getDateDifferenceCompareToday: function getDateDifferenceCompareToday(a) {
        var today = new Date();
        var d1 = new Date(today.getFullYear(), today.getMonth(), today.getDate()).getTime();

        if (isFinite(a = this.convert(a).valueOf()) === false) {
            return NaN;
        }

        a = new Date(a);
        var d2 = new Date(a.getFullYear(), a.getMonth(), a.getDate()).getTime();

        return Math.round((d2 - d1) / 86400000);
    }
};

