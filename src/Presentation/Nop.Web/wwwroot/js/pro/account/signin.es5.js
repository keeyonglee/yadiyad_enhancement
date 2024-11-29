'use strict';

var PageSignIn = function PageSignIn(opts) {
    //model and scoped object

    var $content = $('.content-account-sign-in');
    var $form = $content.find('form');

    var initComponent = function initComponent() {
        app.initFormComponent($form);
    };

    var setUserEvent = function setUserEvent() {};

    //inititalize page
    var init = function init() {
        setUserEvent();
        initComponent();
    };

    init();
};

//set init param
var pageSignIn = new PageSignIn({
    'url': {}
});

