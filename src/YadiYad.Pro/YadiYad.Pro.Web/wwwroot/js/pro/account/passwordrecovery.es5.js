'use strict';

var PagePasswordRecovery = function PagePasswordRecovery(opts) {
    //model and scoped object

    var $content = $('.content-password-recovery');
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
var pagePasswordRecovery = new PagePasswordRecovery({
    'url': {}
});

