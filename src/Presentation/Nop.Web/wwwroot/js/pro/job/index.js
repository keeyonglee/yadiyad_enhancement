var PageJobProfileIndex = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model;
    var $content = $('.content-job-profile');
    var $areaInterest = $('.area-interest');

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
        $content.find("[text]")
            .each(function () {
                var text = $(this).attr('text');
                var result;
                while ((result = reg.exec(text)) !== null) {
                    var replacingText = result[0];
                    var property = result[1];
                    text = text.replace(replacingText, data[property] === 0 ? 0 : (data[property] || ''));
                }

                $(this).text(text);
            });
    };


    base.getJobProfileData = function (callback) {
        var settings = {
            "url": url.getJobProfile.format(model),
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
            location.href = url.editJobProfilePage.format(model);
        });
    };

    base.setData(opts);
    base.getJobProfileData(base.setDisplayProperty);
    base.setUserEvent();
};

var pageJobProfileIndex = new PageJobProfileIndex({
    'url': {
        'getJobProfile': '/api/pro/job/{{id}}',
        'editJobProfilePage': '/pro/job/details/{{id}}'
    }
});