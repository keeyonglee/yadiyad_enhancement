'use strict';

var PageConfirmedConsultationOrder = function PageConfirmedConsultationOrder(opts) {
    var base = this;
    var selector = ".content-consultation-candidates";
    var $content = $(selector);
    var url = opts.url;
    var msg = opts.msg;
    var timezone = new Date().getTimezoneOffset() / 60 * -1;
    var pagination = new SimplePagination();
    var dtRefundRequest = null;
    var dtDepositRequest = null;
    var dtPayoutRequest = null;

    pagination.onPageChanged = function (pageIndex) {
        base.getConsultationInvitationList(base.setConsultationInvitationList, pageIndex);
    };

    var model = opts.model || {
        serviceChargeRate: 0.75,
        selectedConsultationInvitation: null,
        applicants: {
            totalCount: 0,
            data: [],
            selected: false
        },
        pagination: pagination,
        setCandidateResult: function setCandidateResult(result) {
            pagination.set(result);
            if (result && result.data) {

                $.each(result.data, function (i, item) {
                    item.createdOnText = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');
                    if (item.appointmentStartDate && item.appointmentEndDate) {
                        item.appointmentDateText = moment(item.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('YYYY-MM-DD (ddd)');
                        item.appointmentStartTimeText = moment(item.appointmentStartDate, 'YYYY-MM-DDTHH:mm').format('h:mmA');
                        item.appointmentEndTimeText = moment(item.appointmentEndDate, 'YYYY-MM-DDTHH:mm').format('h:mmA');
                    } else {
                        $.each(item.consultantAvailableTimeSlots, function (i, item) {
                            item.startDateText = moment(item.startDate, 'YYYY-MM-DDTHH:mm').format('dddd');
                            item.startTimeText = moment(item.startDate, 'YYYY-MM-DDTHH:mm').format('hh:mm A');
                            item.endTimeText = moment(item.endDate, 'YYYY-MM-DDTHH:mm').format('hh:mm A');
                        });
                    }
                });
            }

            $.observable(this.applicants).setProperty({
                totalCount: 0,
                data: [],
                selected: false
            });
            $.observable(this.applicants).setProperty(result);

            if (this.applicants.data && this.applicants.data.length > 0) {
                this.selectApplicant(this.applicants.data[0]);
            }

            return false;
        },
        showApplicantDetail: function showApplicantDetail() {
            $.observable(this.selectedConsultationInvitation).setProperty("showMore", true);

            return false;
        },
        showReviewForm: function showReviewForm() {
            var model = this;
            $.observable(this.selectedConsultationInvitation).setProperty("isReviewFormShown", true);

            $('input.rating[type=number], div.rating').each(function () {
                $(this).rating({
                    'iconLib': '',
                    'inactiveIcon': 'far fa-star',
                    'activeIcon': 'fa fa-star',
                    'additionalClasses': 'cursor-pointer'
                }).on('change', function () {
                    var key = $(this).attr('name');
                    var value = $(this).val();
                    $.observable(model.selectedConsultationInvitation).setProperty(key, value);
                });
            });
            return false;
        },
        publishReview: function publishReview() {
            var data = {
                reviewText: this.selectedConsultationInvitation.reviewText,
                knowledgenessRating: this.selectedConsultationInvitation.knowledgenessRating,
                relevanceRating: this.selectedConsultationInvitation.relevanceRating,
                respondingRating: this.selectedConsultationInvitation.respondingRating,
                clearnessRating: this.selectedConsultationInvitation.clearnessRating,
                professionalismRating: this.selectedConsultationInvitation.professionalismRating
            };
            base.reviewConsultationInvitation(this.selectedConsultationInvitation.id, data, base.reviewConsultationInvitationResponse);

            return false;
        },
        selectApplicant: function selectApplicant(applicant) {
            var selectedApplicants = $.grep(this.applicants.data, function (item) {
                return item.selected === true;
            });

            if (selectedApplicants.length === 1) {
                $.observable(selectedApplicants[0]).setProperty("selected", false);
            }

            var selectingApplicants = $.grep(this.applicants.data, function (item) {
                return item.id === applicant.id;
            });

            if (selectingApplicants.length === 1) {
                $.observable(selectingApplicants[0]).setProperty("selected", true);
                $.observable(this).setProperty('selectedConsultationInvitation', null);
                $.observable(this).setProperty('selectedConsultationInvitation', selectingApplicants[0]);
                base.updateOrganizationConsultationInvitationRead(selectingApplicants[0]);
            }
            dtRefundRequest = null;
            base.loadRefundRequestTable();
            base.loadDepositRequestTable();
            base.loadPayoutRequestTable();

            $('input.rating[type=number], div.rating').each(function () {
                $(this).rating({
                    'iconLib': '',
                    'inactiveIcon': 'far fa-star',
                    'activeIcon': 'fa fa-star'
                });
            });

            $(".submit-refund").on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();

                swal({
                    icon: "warning",
                    title: 'Are You Sure to Create Refund Request?',
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
                    }
                }).then(function (isConfirm) {
                    if (isConfirm) {
                        base.consultationRefund(base.onConsultationRefunded);
                    }
                });
            });

            return false;
        },
        stopSelectApplicantRefund: function stopSelectApplicantRefund() {
            $.observable(this.selectedConsultationInvitation).setProperty('canRefund', false);
        }
    };

    //getter
    Object.defineProperty(base, 'model', {
        get: function get() {
            return model;
        }
    });

    //jsViews
    $.templates({
        tmplConsultationJobApplicant: "#tmpl-consultation-job-applicant"
    });

    var setData = function setData() {};

    //backend interaction
    base.getConsultationInvitationList = function (callback, pageIndex) {
        var filterData = {
            keyword: ""
        };

        var recordSize = 10;
        var offset = pageIndex ? pageIndex * recordSize : 0;
        var data = {
            offset: offset,
            recordSize: recordSize,
            filter: filterData.keyword,
            advancedFilter: {
                consultationInvitationStatuses: [4, 5, 6, 7, 8, 9, 10]
            }
        };
        var settings = {
            "url": url.getConsultationInvitationList,
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.reviewConsultationInvitation = function (id, data, callback) {
        var settings = {
            "url": url.reviewConsultationInvitation.format({
                id: id
            }),
            "method": 'post',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.consultationRefund = function (callback) {
        var settings = {
            "url": url.submitRefund.format({
                id: model.selectedConsultationInvitation.id
            }),
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    base.updateOrganizationConsultationInvitationRead = function (applicants) {
        if (applicants.isOrganizationRead === true) {
            return;
        }

        var settings = {
            "url": url.updateOrganizationConsultationInvitationRead.format({
                id: applicants.id
            }),
            "background": true,
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(function (response) {
            console.log(response);
        });
    };

    //front-end response/action
    base.setConsultationInvitationList = function (response) {
        base.handleResponse(response, function (response) {
            model.setCandidateResult(response.data);
        });
    };

    base.reviewConsultationInvitationResponse = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Review Submitted Successfully"
            }).then(function () {
                base.getConsultationInvitationList(base.setConsultationInvitationList);
            });
        });
    };

    base.onConsultationRefunded = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Refund Request Created Successfully"
            }).then(function () {
                base.loadRefundRequestTable();
            });
        });
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

    base.loadPayoutRequestTable = function () {
        //if (dtPayoutRequest) {
        //    dtPayoutRequest.destroy();
        //}

        var cols = [{ "data": "createdDateTime" },
        /*  { "data": "payoutCycle" },*/
        { "data": "formattedAmount" }, { "data": "statusName" }, { "data": "lastRemark" }, { "data": "lastRemark" }];

        dtPayoutRequest = $content.find(".tbl-payout-request").DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": msg.keywordSearch,
                "lengthMenu": "_MENU_",
                "info": msg.info,
                "search": "",
                "emptyTable": "No Payout Request is created"
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getPayouRequests.format({
                    id: model.selectedConsultationInvitation.id
                }),
                "type": "post",
                "data": function data(_data) {
                    _data.customFilter = {};
                },
                "dataFilter": function dataFilter(data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;

                    dataList.forEach(function (item) {
                        var urlGetPayouRequest = url.getPayouRequest.replace(/{{id}}/g, item.id);
                        item.action = '<input class="btn btn-primary w-auto" value="View" type="button"' + 'data-toggle="modal" data-target="#modal-payout-request"   ' + 'data-keyboard="false" data-backdrop="static"              ' + 'href="' + urlGetPayouRequest + '" />                      ';

                        if (item.status === 2) {
                            var urlGetInvoiceStatement = url.getInvoiceStatement.replace(/{{id}}/g, item.invoiceId);
                            item.action += '<a class="text-primary" target="_blank"' + 'href="' + urlGetInvoiceStatement + '">[Invoice]</a>                      ';
                        }

                        item.createdDateTime = moment.utc(item.createdOnUTC).local().format("DD/MM/YYYY");
                        var endDate = moment.utc(item.endDate).local();

                        item.payoutCycle = endDate.format("YYYY MMM") + " #" + (endDate.toDate().getDate() <= 15 ? 1 : 2);
                        item.formattedAmount = item.fee.toFixed(2);

                        if (item.remarks && item.remarks.length) {
                            var lastRemark = item.remarks[item.remarks.length - 1];
                            item.lastRemark = lastRemark.actorName + ': ' + lastRemark.remark;
                        }
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]]
        });
    };

    base.loadDepositRequestTable = function () {
        //if (dtDepositRequest) {
        //    dtDepositRequest.destroy();
        //}

        var cols = [{ "data": "formattedRequestDate" }, { "data": "formattedDueDate" }, { "data": "formattedAmount" }, { "data": "statusText" }, { "data": "action" }];

        var $table = $content.find(".tbl-deposit-request");

        if (!$table || !$table.length) {
            return;
        }

        dtDepositRequest = $table.DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": msg.keywordSearch,
                "lengthMenu": "_MENU_",
                "info": msg.info,
                "search": "",
                "emptyTable": "No Deposit Request is created"
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getDepositRequests.format({
                    id: model.selectedConsultationInvitation.id
                }),
                "type": "post",
                "data": function data(_data2) {
                    _data2.customFilter = {};
                },
                "dataFilter": function dataFilter(data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {

                        if (item.status === 1) {
                            var urlGetDepositRequestStatement = url.getDepositRequestStatement.replace(/{{id}}/g, item.id);
                            item.action = '<a class="text-primary" target="_blank"' + 'href="' + urlGetDepositRequestStatement + '"><i class="fas fa-file-alt"></i> Statement</a>                      ';

                            if (item.serviceChargeInvoiceId) {
                                var urlGetInvoiceStatement = url.getInvoiceStatement.replace(/{{id}}/g, item.serviceChargeInvoiceId);

                                item.action += '<a class="text-primary" target="_blank"' + 'href="' + urlGetInvoiceStatement + '">[Invoice]</a>';
                            }
                        } else {
                            var urlPayDepositRequest = url.payDepositRequest;
                            item.action = '<input class="btn btn-primary w-auto btn-pay-deposit-request" value="Pay" type="button"' + 'data-toggle="modal" data-target="#modal-job-application-pay"   ' + 'data-keyboard="false" data-backdrop="static"              ' + 'data-id="' + item.id + '"        ' + 'href="' + urlPayDepositRequest + '" />                      ';
                        }

                        item.formattedRequestDate = moment.utc(item.requestDate).local().format("DD/MM/YYYY");
                        item.formattedDueDate = moment.utc(item.dueDate).local().format("DD/MM/YYYY");
                        item.formattedAmount = item.amount.toFixed(2);
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]]
        });

        $table.on('click', '.btn-pay-deposit-request', function () {
            var $this = $(this);
            var id = $this.data('id');
            model.depositRequestId = id;
        });
    };

    base.loadRefundRequestTable = function () {
        //if (dtRefundRequest) {
        //    dtRefundRequest.destroy();
        //}

        var cols = [{ "data": "createdDateTime" }, { "data": "refundNumber" }, { "data": "formattedAmount" }, { "data": "formattedServiceCharge" }, { "data": "statusName" }, { "data": "action" }];

        var $table = $content.find(".tbl-refund-request");

        if (!$table || !$table.length) {
            return;
        }

        if (dtRefundRequest) {
            dtRefundRequest.ajax.reload();
            return;
        }

        dtRefundRequest = $table.DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": msg.keywordSearch,
                "lengthMenu": "_MENU_",
                "info": msg.info,
                "search": "",
                "emptyTable": "No Refund Request is created"
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getRefundRequests.format({
                    id: model.selectedConsultationInvitation.id
                }),
                "type": "post",
                "data": function data(_data3) {
                    _data3.customFilter = {};
                },
                "dataFilter": function dataFilter(data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        item.action = '';
                        if (item.status === 1) {
                            var urlGetRefundStatement = url.getRefundStatement.replace(/{{id}}/g, item.id);
                            item.action += '<a class="text-primary" target="_blank"' + 'href="' + urlGetRefundStatement + '">[Refund]</a>                      ';
                        }

                        item.createdDateTime = moment.utc(item.createdOnUTC).local().format("DD/MM/YYYY");
                        item.formattedAmount = 'RM ' + item.amount.toFixed(2);
                        item.formattedServiceCharge = 'RM ' + item.serviceCharge.toFixed(2);
                    });

                    if (dataList.length > 0) {
                        model.stopSelectApplicantRefund();
                    }

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]]
        });
    };

    //set user event
    var setUserEvent = function setUserEvent() {};

    //initilize page
    var init = function init() {
        setUserEvent();
        setData();
        $.templates.tmplConsultationJobApplicant.link(selector, model);
        base.getConsultationInvitationList(base.setConsultationInvitationList);
    };

    init();
};

var pageConfirmedConsultationOrder = new PageConfirmedConsultationOrder({
    url: {
        'getConsultationInvitationList': '/api/pro/consultationinvitation/organization/list',
        'reviewConsultationInvitation': '/api/pro/consultationinvitation/{{id}}/review',
        'getRefundStatement': '/pro/statement/Pdf/Refund/{{id}}',
        'getRefundRequests': '/api/pro/refundrequest/consultationinvitation/{{id}}',
        'submitRefund': '/api/pro/consultationinvitation/{{id}}/refund',
        'getPayouRequest': '/pro/payoutrequest/{{id}}',
        'getPayouRequests': '/api/pro/payoutrequest/consultationinvitation/{{id}}',
        'getDepositRequests': '/api/pro/depositrequest/consultationinvitation/{{id}}',
        'getDepositRequestStatement': '/pro/statement/Pdf/Deposit/{{id}}',
        'getInvoiceStatement': '/pro/statement/Pdf/Invoice/{{id}}',
        'updateOrganizationConsultationInvitationRead': '/api/pro/consultationinvitation/{{id}}/org/read'
    },
    'msg': {
        'keywordSearch': "search by keyword",
        'info': 'this is info'
    }
});

