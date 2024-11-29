'use strict';

var PageSignUp = function PageSignUp(opts) {
    //model and scoped object
    var base = this;
    var url = opts.url;

    var model = opts.model || {};

    var $content = $('.content-account-sign-up');
    var $form = $content.find('form');
    var $contentAccountType = $content.find('.content-account-type');
    var $contentSignUpForm = $content.find('.content-sign-up-form');
    var $chkAccountType = $contentAccountType.find('.chk-account-type');
    var $btnNext = $contentAccountType.find('.btn-next');
    var $btnBack = $contentSignUpForm.find('.btn-back');

    var initComponent = function initComponent() {
        app.initFormComponent($form);
    };

    var setUserEvent = function setUserEvent() {
        $chkAccountType.on('click', function () {
            $btnNext.attr('disabled', false);
        });

        $contentAccountType.find('.btn-next').on('click', function () {
            var selectedAccountType = $chkAccountType.val();

            if (selectedAccountType) {
                $contentAccountType.addClass('hidden');
                $contentSignUpForm.removeClass('hidden');
            }
        });

        $chkAccountType.on('change', function () {
            //var selectedAccountType = $contentAccountType.find('.chk-account-type');.val();
            var selectedAccountType = $('input[name="Type"]:checked').val();

            if (selectedAccountType) {
                $contentAccountType.addClass('hidden');
                $contentSignUpForm.removeClass('hidden');

                $("#signUpAs").html(selectedAccountType);
            }
        });

        $btnBack.on('click', function () {
            $('.validation-summary-errors').empty();
            app.clearForm($form);
            $btnNext.attr('disabled', true);
            $contentAccountType.removeClass('hidden');
            $contentSignUpForm.addClass('hidden');
        });
    };

    //inititalize page
    var init = function init() {
        setUserEvent();
        initComponent();
    };

    init();
};

//set init param
var pageSignUp = new PageSignUp({
    'url': {}
});

