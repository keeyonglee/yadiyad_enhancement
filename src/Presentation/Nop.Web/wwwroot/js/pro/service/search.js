var PageServiceSearch = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.search = null;
    model.serviceSearchResultList = [];
    model.selectedCategoryIds = [];
    model.selectedCategoryNames = [];
    model.selectedExpertiseIds = [];
    model.selectedExpertiseNames = [];
    var pagination = new SimplePagination('.pagination-service-search-list');

    var $content = $('.content-service-search');
    var $formSearchFilter = $content.find('.form-search-filter');
    var $categoryNames = $formSearchFilter.find('[name="categoryNames"]');
    var $expertiseNames = $formSearchFilter.find('[name="expertiseNames"]');

    var $cardCategoryJobServices = $('.card-category-job-services');
    var $rdgCategoryJobServices = $content.find('.rdg-category-job-services');
    var $cardExpertise = $('.card-expertise');
    var $rdgExpertise = $content.find('.rdg-expertise');

    var $cardServiceSearchResult = $('.card-service-search-result');
    var $cardServiceSearchResultPagination = $('.pagination-service-search-list');
    var $cardServiceSearchResultEmpty = $('.card-service-search-result-empty');
    var $listServiceSearchResult = $cardServiceSearchResult.find(".list-service-search-result");

    var tmplRdgCategoryJobServices = $.templates('#template-rdg-category-job-services');
    var tmplRdgExpertise = $.templates('#template-rdg-expertise');
    var tmplServiceProfileSimple = $.templates("#template-service-profile-simple");
    var tmplServiceProfileDetail = $.templates("#template-service-profile-detail");

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    pagination.onPageChanged = function (pageIndex) {
        base.submitServiceSearchRequest(base.setServiceSearchResultList, pageIndex);
    }

    base.getCategoryJobServices = function (callback) {
        var settings = {
            "url": url.getCategoryJobServices,
            "method": 'post',
            "headers": {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
            },
            'data': {
                offset: 0,
                recordSize: 9999999
            }
        };

        $.ajax(settings).done(callback);
    };

    base.getExpertise = function (callback) {
        var settings = {
            "url": url.getExpertise,
            "method": 'post',
            "headers": {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
            },
            'data': {
                offset: 0,
                recordSize: 9999999
            }
        };

        $.ajax(settings).done(callback);
    };

    base.setCategoryJobServices = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            for (var i in response.data) {
                var item = response.data[i];
                var htmlRdgCategoryJobServices = tmplRdgCategoryJobServices.render(item);
                $rdgCategoryJobServices.append(htmlRdgCategoryJobServices);
            }
        }
    };

    base.setExpertise = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            for (var i in response.data) {
                var item = response.data[i];
                var htmlRdgExpertise = tmplRdgExpertise.render(item);
                $rdgExpertise.append(htmlRdgExpertise);
            }
        }
    };

    base.submitServiceSearchRequest = function (callback, pageIndex) {
        var filterData = app.getFormValue($formSearchFilter);
        filterData.categoryIds = model.selectedCategoryIds;
        filterData.expertiseIds = model.selectedExpertiseIds;

        var recordSize = 10;
        var offset = pageIndex ? pageIndex * recordSize : 0;

        var requestData = {
            "filter": filterData.keyword,
            "offset": offset,
            "recordSize": recordSize,
            "sorting": null,
            "advancedFilter": filterData,
            "sortBy": $('#select-service-search-sort').val()
        };

        model.search = requestData;

        var settings = {
            "url": url.searchService,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.setServiceSearchResultList = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {
            pagination.set(response.data);

            if (response.data.data.length > 0) {
                $listServiceSearchResult.empty();

                $.each(response.data.data, function (i, item) {
                    if (item.tenureStart !== null) {
                        item.tenureStart = moment(item.tenureStart, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }
                    if (item.tenureEnd !== null) {
                        item.tenureEnd = moment(item.tenureEnd, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }

                    item.createdOnUTC = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');

                    item.index = (i + 1) + (response.data.pageIndex * response.data.pageSize);
                });

                $cardServiceSearchResult.bindText({
                    keyword: model.search.filter,
                    totalCount: response.data.totalCount
                });

                if (model.search.offset === 0) {
                    base.clearServiceSearchResultList();
                }

                var htmlServiceProfileSimples = tmplServiceProfileSimple.render(response.data.data);
                $listServiceSearchResult.append(htmlServiceProfileSimples);
                model.serviceSearchResultList = model.serviceSearchResultList.concat(response.data.data);

                $cardServiceSearchResult.removeClass('hidden');
                $cardServiceSearchResultPagination.removeClass('hidden');
                $cardServiceSearchResultEmpty.addClass('hidden');
                $cardCategoryJobServices.addClass('hidden');
                $cardExpertise.addClass('hidden');
            } else {
                $cardServiceSearchResultEmpty.removeClass('hidden');
                $cardServiceSearchResult.addClass('hidden');
                $cardServiceSearchResultPagination.addClass('hidden');
                $cardCategoryJobServices.addClass('hidden');
                $cardExpertise.addClass('hidden');
            }


        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {
            });
        }
    };

    base.clearServiceSearchResultList = function () {
        $listServiceSearchResult.empty();
        model.serviceSearchResultList = [];
    };

    base.changeServiceProfileModel = function (target, mode) {
        var serviceProfileId = $(target).data('entity-id');
        console.log(model);
        var serviceProfileModels = $.grep(model.serviceSearchResultList, function (item) {
            return item.id === serviceProfileId;
        });
        
        if (mode === 0) {
            var htmlServiceProfileSimples = tmplServiceProfileSimple.render(serviceProfileModels[0]);
            $(target).replaceWith(htmlServiceProfileSimples);
        }
        if (mode === 1) {
            //$.each(serviceProfileModels, function (i, item) {
            //    item.isEntitledApplyService = model.serviceSeeker.isEntitledApplyJob
            //});

            var htmlServiceProfileDetails = tmplServiceProfileDetail.render(serviceProfileModels[0]);
            $(target).replaceWith(htmlServiceProfileDetails);

            model.selectedServiceProfile = serviceProfileModels[0];
        }
    };

    base.setUserEvent = function () {
        $('#select-service-search-sort').on('change', function () {
            if ($('#select-service-search-sort').val() != 0 && $('#select-service-search-sort').val() != 'undefined') {
                base.submitServiceSearchRequest(base.setServiceSearchResultList);

            }
        })

        $formSearchFilter.find("button.btn-search").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();

            if ($formSearchFilter.valid()) {
                base.submitServiceSearchRequest(base.setServiceSearchResultList);
            }
        });

        $formSearchFilter.find("button.btn-reset").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            app.clearForm($formSearchFilter);

            app.clearForm($rdgCategoryJobServices);
            model.selectedCategoryIds = [];
            model.selectedCategoryNames = [];
            $categoryNames.val('');
            $categoryNames.attr('title', '');
            //$categoryNames.addClass('empty');

            app.clearForm($rdgExpertise);
            model.selectedExpertiseIds = [];
            model.selectedExpertiseNames = [];
            $expertiseNames.val('');
            $expertiseNames.attr('title', '');
            //$expertiseNames.addClass('empty');
        });

        $listServiceSearchResult.on('click', '.card-service-profile-simple', function () {
            $listServiceSearchResult.find('.card-service-profile-detail').each(function (i, target) {
                base.changeServiceProfileModel(this, 0);
            });

            base.changeServiceProfileModel(this, 1);
        });

        $listServiceSearchResult.on('click', '.card-service-profile-detail', function (e) {
            if ($(e.target).hasClass('btn-request') === false) {
                base.changeServiceProfileModel(this, 0);
            }
        });

        $listServiceSearchResult.on('click', '.btn-subcribe-apply-service', function () {

        });

        $rdgCategoryJobServices.on('change', '[type="checkbox"]', function () {
            var $this = $(this);
            var selectedValue = $this.val();
            var isSelected = $this.is(':checked');
            var selectedText = $this.closest('.form-check-label').text().trim();

            if (isSelected) {
                model.selectedCategoryIds.push(selectedValue);
                model.selectedCategoryNames.push(selectedText);
            }
            else {
                model.selectedCategoryIds = $.grep(model.selectedCategoryIds, function (value) {
                    return value !== selectedValue;
                });
                model.selectedCategoryNames = $.grep(model.selectedCategoryNames, function (value) {
                    return value !== selectedText;
                });
            }

            $rdgExpertise.find('[type="checkbox"]').filter(function () {
                var jobServiceCategoryId = $(this).data('jobservicecategoryid');
                var hide = !model.selectedCategoryIds.some(function (id) {
                    return jobServiceCategoryId + "" === id + "";
                });

                if (hide) {
                    $(this).prop('checked', false);
                }

                return hide;
            }).closest('.container-expertise').hide();

            var $availbelExpertises = $rdgExpertise.find('[type="checkbox"]').filter(function () {
                var jobServiceCategoryId = $(this).data('jobservicecategoryid');

                return model.selectedCategoryIds.some(function (id) {
                    return jobServiceCategoryId + "" === id + "";
                });
            }).closest('.container-expertise');

            $availbelExpertises.show();

            if ($availbelExpertises.length === 0) {
                $rdgExpertise.find('[type="checkbox"]').closest('.container-expertise').show();
            }

            var selectedCategoryNames = model.selectedCategoryNames.join(', ');
            $categoryNames.val(selectedCategoryNames);
            $categoryNames.attr('title', selectedCategoryNames);
            //if (selectedCategoryNames) {
            //    $categoryNames.removeClass('empty');
            //} else {
            //    $categoryNames.addClass('empty');
            //}

            updateSelectedExpertise();
        });

        $rdgExpertise.on('change', '[type="checkbox"]', function () {
            var $this = $(this);
            var selectedValue = $this.val();
            var isSelected = $this.is(':checked');
            var selectedText = $this.closest('.form-check-label').text().trim();

            if (isSelected) {
                model.selectedExpertiseIds.push(selectedValue);
                model.selectedExpertiseNames.push(selectedText);
            }
            else {
                model.selectedExpertiseIds = $.grep(model.selectedExpertiseIds, function (value) {
                    return value !== selectedValue;
                });
                model.selectedExpertiseNames = $.grep(model.selectedExpertiseNames, function (value) {
                    return value !== selectedText;
                });
            }

            var selectedExpertiseNames = model.selectedExpertiseNames.join(', ');
            $expertiseNames.val(selectedExpertiseNames);
            $expertiseNames.attr('title', selectedExpertiseNames)
            //if (selectedExpertiseNames) {
            //    $expertiseNames.removeClass('empty');
            //} else {
            //    $expertiseNames.addClass('empty');
            //}
        });

        var updateSelectedExpertise = function () {
            model.selectedExpertiseIds = [];
            model.selectedExpertiseNames = [];
            $rdgExpertise.find('[type="checkbox"]:checked').each(function ($elem, i) {
                var $this = $(this);
                var selectedValue = $this.val();
                var selectedText = $this.closest('.form-check-label').text().trim();

                model.selectedExpertiseIds.push(selectedValue);
                model.selectedExpertiseNames.push(selectedText);
            });

            var selectedExpertiseNames = model.selectedExpertiseNames.join(', ');
            $expertiseNames.val(selectedExpertiseNames);
            $expertiseNames.attr('title', selectedExpertiseNames)
            //if (selectedExpertiseNames) {
            //    $expertiseNames.removeClass('empty');
            //} else {
            //    $expertiseNames.addClass('empty');
            //}
        }

        $categoryNames.on('click', function () {
            //if ($rdgCategoryJobServices.is(':visible')) {
            //    $cardServiceSearchResult.removeClass('hidden');
            //    $cardCategoryJobServices.addClass('hidden');
            //    $cardExpertise.addClass('hidden');
            //} else {
            //    $cardCategoryJobServices.removeClass('hidden');
            //    $cardServiceSearchResult.addClass('hidden');
            //    $cardExpertise.addClass('hidden');
            //}
            $cardCategoryJobServices.removeClass('hidden');
            $cardServiceSearchResultEmpty.addClass('hidden');
            $cardServiceSearchResult.addClass('hidden');
            $cardServiceSearchResultPagination.addClass('hidden');
            $cardExpertise.addClass('hidden');
        });

        $expertiseNames.on('click', function () {
            //if ($rdgExpertise.is(':visible')) {
            //    $cardServiceSearchResult.removeClass('hidden');
            //    $cardExpertise.addClass('hidden');
            //    $cardCategoryJobServices.addClass('hidden');
            //} else {
            //    $cardExpertise.removeClass('hidden');
            //    $cardServiceSearchResult.addClass('hidden');
            //    $cardCategoryJobServices.addClass('hidden');
            //}
            $cardExpertise.removeClass('hidden');
            $cardServiceSearchResultEmpty.addClass('hidden');
            $cardServiceSearchResultPagination.addClass('hidden');
            $cardServiceSearchResult.addClass('hidden');
            $cardCategoryJobServices.addClass('hidden');
        });
    };

    base.getModel = function () {
        return model;
    };

    var init = function () {
        base.getCategoryJobServices(base.setCategoryJobServices);
        base.getExpertise(base.setExpertise);
        app.initFormComponent($formSearchFilter);
        base.setUserEvent();
    };

    init();
};

var pageServiceSearch = new PageServiceSearch({
    'url': {
        'getCategoryJobServices': '/api/pro/source/jobservicecategory',
        'getExpertise': '/api/pro/source/expertise',
        'searchService': '/api/pro/service/search'
    }
});