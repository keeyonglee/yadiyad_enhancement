var PagePasswordRecovery = function (opts) {
    //model and scoped object

    var $content = $('.content-password-recovery');
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
var pagePasswordRecovery = new PagePasswordRecovery({
    'url': {
    }
});