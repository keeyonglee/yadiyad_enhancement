'use strict';

var PageJobSummary = function PageJobSummary(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.search = null;
    model.jobSummaryResultList = [];

    var pagination = new SimplePagination('.pagination-job-summary-list');

    var $content = $('.content-job-summary');

    var $listJobSummaryResult = $content.find(".list-job-summary-result");

    var tmplJobSummarySimple = $.templates("#template-job-summary-simple");
    var tmplJobSummarySimpleEmpty = $.templates("#template-job-summary-simple-empty");

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    base.setData = function (opts) {
        model = opts.model || {};
        url = opts.url;

        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.id = parseInt(urlProp[urlProp.length - 2]) || 0;
    };

    pagination.onPageChanged = function (pageIndex) {
        base.searchJobSummary(base.setJobSummaryResultList, pageIndex);
    };

    base.handleResponse = function (response, successHandler) {
        if (response && response.status && response.status.code === 1) {
            successHandler(response);
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.searchJobSummary = function (callback, pageIndex) {
        var filterData = {};

        var recordSize = 10;
        var offset = pageIndex ? pageIndex * recordSize : 0;

        var requestData = {
            "filter": filterData.keyword,
            "offset": offset,
            "recordSize": recordSize,
            "sorting": null,
            "advancedFilter": filterData
        };

        model.search = requestData;

        var settings = {
            "url": url.searchJobSummary,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.setJobSummaryResultList = function (response) {
        if (response && response.status && response.status.code === 1) {

            if (response.data.data.length > 0) {
                $listJobSummaryResult.empty();
                pagination.set(response.data);
                $.each(response.data.data, function (i, item) {
                    item.viewJobCandidateFullProfileSubscriptionEndDate = moment(item.viewJobCandidateFullProfileSubscriptionEndDate, 'YYYY-MM-DD').add(timezone, 'hour').format('DD MMM YYYY');
                });

                if (model.search.offset === 0) {
                    base.clearJobSummaryResultList();
                }

                var htmlJobSummarySimples = tmplJobSummarySimple.render(response.data.data);
                $listJobSummaryResult.append(htmlJobSummarySimples);
                model.jobSummaryResultList = model.jobSummaryResultList.concat(response.data.data);
            } else {
                $listJobSummaryResult.append(tmplJobSummarySimpleEmpty);
            }
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.clearJobSummaryResultList = function () {
        $listJobSummaryResult.empty();
        model.jobSummaryResultList = [];
    };

    base.handleResponse = function (response, successHandler) {
        if (response && response.status && response.status.code === 1) {
            successHandler(response);
        } else {
            swal({
                icon: "warning",
                title: "Fail",
                text: response.status.message
            }).then(function () {});
        }
    };

    base.setUserEvent = function () {
        $listJobSummaryResult.on('click', '.btn-renew', function () {
            model.selectedJobApplicationId = $(this).closest('.card-job-summary-simple').data('entity-id');
        });
        $listJobSummaryResult.on('click', '.card-job-summary-simple', function (e) {
            var isRenewButton = $(e.target).hasClass('btn-renew');
            var isDisabled = $(this).hasClass('cursor-not-allowed');

            if (isRenewButton === false && isDisabled === false) {
                var redirectUrl = $(this).data('href');
                window.location.href = redirectUrl;
            }
        });
    };

    base.getModel = function () {
        return model;
    };

    var init = function init() {
        base.setData(opts);
        base.setUserEvent();
        base.searchJobSummary(base.setJobSummaryResultList);
    };

    init();
};

var pageJobSummary = new PageJobSummary({
    'url': {
        'searchJobSummary': '/api/pro/job/summary'
    }
});

