
var PagePaymentDetailsList = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.paymentList = [];
    var pagination = new SimplePagination('.pagination-list-payment-list');

    var tmplCardPaymentList = $.templates("#template-card-payment-list");
    var tmplCardPaymentListEmpty = $.templates("#template-card-payment-list-empty");
    var tmplCardPaymentListResponse = $.templates("#template-card-payment-detail");
    var tmplCardPaymentListResponseEmpty = $.templates("#template-card-payment-detail-empty");

    var $content = $('.content-payment-search');
    var $listPaymentList = $content.find(".list-payment-list");
    var $form = $content.find('#form-payment-details-filter');


    pagination.onPageChanged = function (pageIndex) {
        base.loadPaymentListData(base.setPaymentList, pageIndex);
    }

    //back-end interation
    base.loadPaymentListData = function (callback, pageIndex) {
        var filterData = {
            keyword: ""
        };

        var recordSize = 10;
        var offset = pageIndex ? pageIndex * recordSize : 0;

        var requestData = {
            "filter": filterData.keyword,
            "offset": offset,
            "recordSize": recordSize,
            "sorting": null,
            "advancedFilter": filterData
        };
        filterData.orderDate = $('#inputPaymentDetailsOrderDate').val();

        model.search = requestData;
        var settings = {
            "url": url.getPaymentList,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    //front-end response
    base.setPaymentList = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {

            if (response.data.data.length > 0) {
                $content.bindText(response.data);

                var htmlCardPaymentList = tmplCardPaymentList.render(response.data.data);
                $listPaymentList.empty();
                pagination.set(response.data);

                $listPaymentList.append(htmlCardPaymentList);
                model.paymentList = model.paymentList.concat(response.data.data);

                model.selectedPayment = response.data.data[0]
                base.refreshPaymentDetail(model.selectedPayment);
            }
            else {
                $listPaymentList.empty();
                pagination.set(response.data);

                $listPaymentList.append(tmplCardPaymentListEmpty);
                $content.find('.content-payment-detail').replaceWith(tmplCardPaymentListResponseEmpty);

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

    base.setConsultationListResponse = function (target) {
        var paymentListId = $(target).data('entity-id');
        var paymentListModel = $.grep(model.paymentList, function (item) {
            return item.id === paymentListId
        });
        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        if (model.selectedPayment.invoiceDate == null && model.selectedPayment.invoiceNumber == null) {
            model.selectedPayment.invoiceDate = "-";
            model.selectedPayment.invoiceNumber = "-";
        }

        model.selectedPayment = paymentListModel[0]
        base.refreshPaymentDetail(model.selectedPayment);
    };

    base.refreshPaymentDetail = function (model) {
        var htmlCardPaymentListResponse = tmplCardPaymentListResponse.render(model);
        $content.find('.content-payment-detail').replaceWith(htmlCardPaymentListResponse);
    };

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

    //user event handle
    base.setUserEvent = function () {
        $listPaymentList.on('click', '.card-payment-list', function () {
            base.setConsultationListResponse(this);
        });

        $('#btnPaymentDetailsSearch').on('click', function () {
            base.loadPaymentListData(base.setPaymentList);

        })
    };

    //page initilize
    var init = function () {
        base.loadPaymentListData(base.setPaymentList);
        base.setUserEvent();
        app.initFormComponent($form);

    };

    init();
};

var pagePaymentDetailsList = new PagePaymentDetailsList({
    'url': {
        'getPaymentList': '/api/pro/order/paymentlist',
    }
});