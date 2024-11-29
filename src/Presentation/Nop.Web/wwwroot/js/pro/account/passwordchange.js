var PagePasswordChange = function (opts) {
    //model and scoped object
    var base = this;
    var url = opts.url;

    var model = opts.model || {};
    var $content = $('.content-account-password-change');
    var $form = $content.find('form');

    var initComponent = function () {
        app.initFormComponent($form);
    };

    var setUserEvent = function () {
    };

    //inititalize page
    var init = function () {
        setUserEvent();
        initComponent();
    };

    init();
};

//set init param
var pagePasswordChange = new PagePasswordChange({
    'url': {
    }
});