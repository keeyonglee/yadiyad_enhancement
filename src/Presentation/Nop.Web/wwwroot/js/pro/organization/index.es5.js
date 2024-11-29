'use strict';

var PageOrganizationProfileIndex = function PageOrganizationProfileIndex(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model;
    var $content = $('.content-organization-profile');
    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    base.setDisplayProperty = function (data) {
        data = data.data;
        model = opts.model || {};
        model.id = data.customerId;
        var reg = /{{([\d|\w]{1,})}}/g;

        data.dateEstablishedText = moment(data.dateEstablished, 'YYYY-MM-DDTHH:mm').add(0, 'hour').format('DD MMM YYYY');
        data.isListedCompany = data.isListedCompany ? "Yes" : "No";

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
    };

    base.getOrganizationProfileData = function (callback) {
        var settings = {
            "url": url.getOrganizationProfile,
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
            location.href = url.editOrganizationProfilePage;
        });
    };

    base.getOrganizationProfileData(base.setDisplayProperty);
    base.setUserEvent();
};

var pageOrganizationProfileIndex = new PageOrganizationProfileIndex({
    'url': {
        'getOrganizationProfile': '/api/pro/organization',
        /*'editOrganizationProfilePage': '/pro/organization/details'*/
        'editOrganizationProfilePage': '/pro/organization/ProfileEdit'
    }
});

