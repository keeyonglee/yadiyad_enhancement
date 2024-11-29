var PageSignIn = function (opts) {
    //model and scoped object

    var $content = $('.content-account-sign-in');
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
var pageSignIn = new PageSignIn({
    'url': {
    }
});