'use strict';

var PagePasswordChange = function PagePasswordChange(opts) {
    //model and scoped object
    var base = this;
    var url = opts.url;

    var model = opts.model || {};
    var $content = $('.content-account-password-change');
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
var pagePasswordChange = new PagePasswordChange({
    'url': {}
});

