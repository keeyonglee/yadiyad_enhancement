var PageServiceReceives = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.serviceReceives = [];

    var pagination = new SimplePagination('.pagination-service-receives-list');

    var tmplCardServiceReceivesEmpty = $.templates("#template-card-service-receives-empty");
    var tmplCardServiceReceives = $.templates("#template-card-service-receives-list");
    var tmplCardServiceReceivesListEmpty = $.templates("#template-card-service-receives-list-empty");
    var tmplCardServiceReceivesResponse = $.templates("#template-card-service-receives-list-response");
    var tmplCardServiceReceivesResponseEmpty = $.templates("#template-card-service-receives-list-response-empty");

    var $content = $('.content-service-search');
    var $listServiceReceives = $content.find(".list-service-receives-list");
    var $detail = $('.content-service-receives-list-response');

    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    pagination.onPageChanged = function (pageIndex) {
        base.loadServiceReceivesData(base.setServiceReceives, pageIndex);
    }

    base.loadServiceReceivesData = function (callback, pageIndex) {
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

        model.search = requestData;

        var settings = {
            "url": url.getServiceReceives,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            'data': JSON.stringify(requestData)
        };

        $.ajax(settings).done(callback);
    };

    base.updateServiceSellerRead = function (selectedServiceApplication, callback) {
        if (selectedServiceApplication.providerIsRead) {
            return true;
        }
        selectedServiceApplication.providerIsRead = true;

        var settings = {
            "url": url.updateServiceSellerRead.format(selectedServiceApplication),
            "method": 'PUT',
            "headers": {
                "Content-Type": "application/json"
            },
            background: true
        };

        $.ajax(settings).done(callback);
    };

    base.updateServiceSellerReadResponse = function () {

    }

    base.setServiceReceives = function (response) {
        if (response
            && response.status
            && response.status.code === 1) {

            if (response.data.data.length > 0) {
                $listServiceReceives.empty();
                pagination.set(response.data);

                $.each(response.data.data, function (i, item) {
                    if (item.tenureStart !== null) {
                        item.tenureStart = moment(item.tenureStart, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }
                    if (item.tenureEnd !== null) {
                        item.tenureEnd = moment(item.tenureEnd, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
                    }
                    if (item.tenureStart !== null) {
                        item.startDate = moment(item.startDate, 'YYYY-MM-DD').add(0, 'hour').format('DD MMM YYYY');
                    }

                    item.createdOnUTC = moment(item.createdOnUTC, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');

                });

                $content.bindText(response.data);

                var htmlCardServiceReceives = tmplCardServiceReceives.render(response.data.data);
                $listServiceReceives.append(htmlCardServiceReceives);
                model.serviceReceives = model.serviceReceives.concat(response.data.data);

                model.selectedServiceReceives = response.data.data[0];
                base.showServiceReceiveDetails();

                $content.find(".btn-decline").on("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    swal({
                        icon: "warning",
                        title: 'Are You Sure to Decline?',
                        buttons: {
                            cancel: {
                                text: "Cancel",
                                value: false,
                                visible: true,
                                className: "btn-fail",
                                closeModal: true
                            },
                            confirm: {
                                text: "Confirm",
                                value: true,
                                visible: true,
                                className: "btn-success",
                                closeModal: true
                            }
                        }
                    }).then(function (isConfirm) {
                        if (isConfirm) {
                            base.declineService(base.responseDeclineService);
                        }
                    });
                });

                $content.find(".btn-accept").on("click", function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    swal({
                        icon: "warning",
                        title: 'Are You Sure to Accept?',
                        buttons: {
                            cancel: {
                                text: "Cancel",
                                value: false,
                                visible: true,
                                className: "btn-fail",
                                closeModal: true
                            },
                            confirm: {
                                text: "Confirm",
                                value: true,
                                visible: true,
                                className: "btn-success",
                                closeModal: true
                            }
                        }
                    }).then(function (isConfirm) {
                        if (isConfirm) {
                            base.acceptService(base.responseAcceptService);
                        }
                    });
                });
            } else {
                $content.replaceWith(tmplCardServiceReceivesEmpty);

                $listServiceReceives.append(tmplCardServiceReceivesListEmpty);
                $content.find('.content-service-receives-list-response').replaceWith(tmplCardServiceReceivesResponseEmpty);
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


    base.showServiceReceiveDetails = function () {
        if (model.selectedServiceReceives) {
            var htmlCardServiceReceivesResponse = tmplCardServiceReceivesResponse.render(model.selectedServiceReceives);
            $content.find('.content-service-receives-list-response').replaceWith(htmlCardServiceReceivesResponse);
            base.updateServiceSellerRead(model.selectedServiceReceives, base.updateServiceSellerReadResponse);

            var $areaExpertise = $('.area-expertise');
            model.selectedServiceReceives.serviceProfile.serviceExpertises.forEach(function (item) {
                $areaExpertise.append("<span class='badge badge-pill badge-primary'>" + item.name + "</span>&nbsp;");
            });
        }
    };

    base.setServiceReceivesResponse = function (target) {
        var serviceReceivesId = $(target).data('entity-id');
        var serviceReceivesModel = $.grep(model.serviceReceives, function (item) {
            return item.id === serviceReceivesId
        });

        $(target).parent().children('.selected').removeClass('selected');
        $(target).addClass('selected');

        model.selectedServiceReceives = serviceReceivesModel[0];
        base.showServiceReceiveDetails();
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

    base.acceptService = function (callback) {
        var serviceReceivesId = model.selectedServiceReceives.id;
        var data = {
            id: serviceReceivesId
        };
        var settings = {
            "url": url.acceptServiceApplication,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responseAcceptService = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Service Request Accepted Succesfully",
                //text: 
            }).then(function () {
                location.reload();
            });
        });
    };

    base.declineService = function (callback) {
        var serviceReceivesId = model.selectedServiceReceives.id;
        var data = {
            id: serviceReceivesId
        };
        var settings = {
            "url": url.declineServiceApplication,
            "method": 'POST',
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify(data)
        };

        $.ajax(settings).done(callback);
    };

    base.responseDeclineService = function (response) {
        base.handleResponse(response, function (response) {
            swal({
                icon: "success",
                title: "Repropose Request Declined Successfully",
                //text: 
            }).then(function () {
                location.reload();
            });
        });
    };

    base.setUserEvent = function () {
        $listServiceReceives.on('click', '.card-service-receives-list', function () {
            base.setServiceReceivesResponse(this);

            $content.find(".btn-decline").on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                swal({
                    icon: "warning",
                    title: 'Are You Sure to Decline?',
                    buttons: {
                        cancel: {
                            text: "Cancel",
                            value: false,
                            visible: true,
                            className: "btn-fail",
                            closeModal: true
                        },
                        confirm: {
                            text: "Confirm",
                            value: true,
                            visible: true,
                            className: "btn-success",
                            closeModal: true
                        }
                    }
                }).then(function (isConfirm) {
                    if (isConfirm) {
                        base.declineService(base.responseDeclineService);
                    }
                });
            });

            $content.find(".btn-accept").on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                swal({
                    icon: "warning",
                    title: 'Are You Sure to Accept?',
                    buttons: {
                        cancel: {
                            text: "Cancel",
                            value: false,
                            visible: true,
                            className: "btn-fail",
                            closeModal: true
                        },
                        confirm: {
                            text: "Confirm",
                            value: true,
                            visible: true,
                            className: "btn-success",
                            closeModal: true
                        }
                    }
                }).then(function (isConfirm) {
                    if (isConfirm) {
                        base.acceptService(base.responseAcceptService);
                    }
                });
            });
        });
    };

    base.getModel = function () {
        return model;
    };

    var init = function () {
        base.loadServiceReceivesData(base.setServiceReceives);
        base.setUserEvent();
    };

    init();
};

var pageServiceReceives = new PageServiceReceives({
    'url': {
        'getServiceReceives': '/api/pro/serviceApplication/receives',
        'acceptServiceApplication': '/api/pro/serviceApplication/accept',
        'declineServiceApplication': '/api/pro/serviceApplication/decline',
        'updateServiceSellerRead': '/api/pro/serviceApplication/{{id}}/seller/read'
    }
});