'use strict';

var PageIndividualProfileIndex = function PageIndividualProfileIndex(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model;
    var $content = $('.content-individual-profile');
    var $areaInterest = $('.area-interest');
    var timezone = new Date().getTimezoneOffset() / 60 * -1;

    base.setDisplayProperty = function (data) {
        data = data.data;
        model = opts.model || {};
        model.id = data.customerId;
        var reg = /{{([\d|\w]{1,})}}/g;

        data.dateOfBirthText = moment(data.dateOfBirth, 'YYYY-MM-DDTHH:mm').add(timezone, 'hour').format('DD MMM YYYY');

        if (data.billingAddress !== null) {
            data.billingAddress1 = data.billingAddress.address1;
            data.billingAddress2 = data.billingAddress.address2;
            data.billingCityName = data.billingAddress.cityName;
            data.billingStateProvinceName = data.billingAddress.stateProvinceName;
            data.billingZipPostalCode = data.billingAddress.zipPostalCode;
            data.billingCountryName = data.billingAddress.countryName;
        };

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

        data.interestHobbies.forEach(function (item) {
            $areaInterest.append("<div>" + item.name + "</div>");
        });

        var assignFacebookIframeId = function assignFacebookIframeId() {
            var t = $(".fb_dialog_content").children()[0];
            $(t).attr('id', 'driver-facebook-iframe');

            setTimeout(function () {
                startDriverTour();
            }, 1000);
        };

        var startDriverTour = function startDriverTour() {
            driver.defineSteps([stepProfileArea, stepSeekFreelanceJob, stepSellServices, stepBuyServices, stepOnlineOffline, stepMainMenu, stepFacebookMessengerButton]);
            driver.start();
        };

        if (!data.isTourCompleted) {
            setTimeout(function () {
                assignFacebookIframeId();
            }, data.setDelay);
            //startDriverTour();
        }
    };

    base.getIndividualProfileData = function (callback) {
        var settings = {
            "url": url.getIndividualProfile,
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
            location.href = url.editIndividualProfilePage;
        });
    };

    base.getIndividualProfileData(base.setDisplayProperty);
    base.setUserEvent();
};

var pageIndividualProfileIndex = new PageIndividualProfileIndex({
    'url': {
        'getIndividualProfile': '/api/pro/individual',
        /*'editIndividualProfilePage': '/pro/individual/details'*/
        'editIndividualProfilePage': '/pro/individual/ProfileEdit'
    }
});

