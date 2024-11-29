'use strict';

var PageServiceProfileIndex = function PageServiceProfileIndex(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model;
    var $content = $('.content-service-profile');
    var $grpOnsite = $content.find('.form-group-onsite');
    var $grpCharges = $content.find('.form-group-charges');
    var $grpConsultation = $content.find('.form-group-consultation');
    var $grpFreelancing = $content.find('.form-group-freelancing');
    var $grpPartTime = $content.find('.form-group-part-time');
    var $areaExpertise = $('.area-expertise');
    var $areaLocation = $('.area-location');

    base.setData = function (opts) {
        model = opts.model || {};
        url = opts.url;

        var urlPage = window.location.href;
        var urlProp = urlPage.split('/');
        model.id = parseInt(urlProp[urlProp.length - 1]) || 0;
    };

    base.setDisplayProperty = function (data) {
        data = data.data;

        var reg = /{{([\d|\w]{1,})}}/g;
        $content.find("[text]").each(function () {
            var text = $(this).attr('text');
            var result;
            while ((result = reg.exec(text)) !== null) {
                var replacingText = result[0];
                var property = result[1];
                text = text.replace(replacingText, data[property] === 0 ? 0 : data[property] || '');
            }

            $(this).text(text);
        });

        var serviceTypeId = data.serviceTypeId;
        $grpCharges.addClass('hidden');
        $grpConsultation.addClass('hidden');
        $grpFreelancing.addClass('hidden');
        $grpPartTime.addClass('hidden');

        switch (serviceTypeId) {
            case 1:
                $grpFreelancing.removeClass('hidden');
                break;
            case 2:
                $grpPartTime.removeClass('hidden');
                break;
            case 3:
                $grpConsultation.removeClass('hidden');
                break;
        }

        if (serviceTypeId) {
            $grpCharges.removeClass('hidden');
        }

        data.serviceExpertises.forEach(function (item) {
            $areaExpertise.append("<div>" + item.name + "</div>");
        });
    };

    base.getServiceProfileData = function (callback) {
        var settings = {
            "url": url.getServiceProfile.format(model),
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };
        $.ajax(settings).done(callback);
    };

    base.setUserEvent = function () {
        $content.find(".btn-edit").on("click", function (e) {
            e.preventDefault();
            e.stopPropagation();
            location.href = url.editServiceProfilePage.format(model);
        });
    };

    base.setData(opts);
    base.getServiceProfileData(base.setDisplayProperty);
    base.setUserEvent();
};

var pageServiceProfileIndex = new PageServiceProfileIndex({
    'url': {
        'getServiceProfile': '/api/pro/service/{{id}}',
        'editServiceProfilePage': '/pro/service/details/{{id}}'
    }
});

