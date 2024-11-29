'use strict';

var SessionHandler = function SessionHandler(settings) {
    //setting
    var defaultConfig = {
        activityEvents: 'click keypress scroll wheel mousewheel mousemove',
        dialogDisplayLimit: 60,
        idleTimeLimit: 1740,
        idleCheckTime: 2,
        sessionTimeoutMsg: "Your session is about to expire in {0} seconds.",
        logoutUrl: '/logout',
        sessionExtensionUrl: '/token/refresh',
        retrieveSessionExpiryDateTimeUrl: '/token/exp'
    };

    var opts = $.extend(defaultConfig, settings);

    //active variable
    var self = this;
    var idleCounter = opts.idleTimeLimit;
    var sessionExpiredCounter = opts.idleTimeLimit;
    var sessionChecker = null;
    var sessionExpiryDateTime = new Date(9999, 12, 31);
    var tabTitle = $(document).attr("title");

    var init = function init() {
        store.set('idleTimerLastActivity', $.now());
        store.set('idleTimerLoggedOut', false);
        retrieveSessionExpiryDateTime(function (exp) {
            setSessionExpiryDateTime(exp);
        });
    };

    var setSessionExpiryDateTime = function setSessionExpiryDateTime(exp) {
        sessionExpiryDateTime = new Date(0);
        sessionExpiryDateTime.setUTCSeconds(exp);
        monitorSession();
    };

    var retrieveSessionExpiryDateTime = function retrieveSessionExpiryDateTime(callback) {
        var settings = {
            "url": opts.retrieveSessionExpiryDateTimeUrl,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(function (response) {
            callback(response.data.exp);
        });
    };

    var getSessionLeftTime = function getSessionLeftTime() {
        return (sessionExpiryDateTime.getTime() - new Date().getTime()) / 1000;
    };

    var getIdleTimeoutTime = function getIdleTimeoutTime() {
        return (new Date().getTime() - store.get('idleTimerLastActivity')) / 1000;
    };

    var monitorSession = function monitorSession() {
        sessionChecker = setInterval(function () {
            checkTimeout();
        }, opts.idleCheckTime * 1000);
        checkTimeout();
    };

    var checkTimeout = function checkTimeout() {
        idleCounter -= 1;

        var sessionLeftTime = getSessionLeftTime();
        var idleTimeoutTime = getIdleTimeoutTime();
        var idleTimerLoggedOut = store.get('idleTimerLoggedOut');

        if (sessionLeftTime <= 0 || idleTimerLoggedOut) {
            logout();
        } else if (sessionLeftTime < opts.dialogDisplayLimit + opts.idleCheckTime
        //|| idleCounter <= 0
         || idleTimeoutTime > opts.idleTimeLimit) {
            if (sessionChecker) {
                clearInterval(sessionChecker);
            }
            self.showDialog();
        }
    };

    var logout = function logout() {
        store.set('idleTimerLoggedOut', true);
        window.location.href = opts.logoutUrl;
    };

    var extendSession = function extendSession() {
        idleCounter = opts.idleTimeLimit;
        var settings = {
            "url": opts.sessionExtensionUrl,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(function (response) {
            setSessionExpiryDateTime(response.data.exp);
        });
    };

    self.showDialog = function () {
        var sessionLeftTime = getSessionLeftTime();
        var countdown = opts.dialogDisplayLimit > sessionLeftTime ? parseInt(sessionLeftTime) : opts.dialogDisplayLimit;
        clearInterval(sessionChecker);

        var countingDown = setInterval(function () {
            countdown -= 1;

            if (countdown > 0) {
                $(document).attr("title", "Left {0}s - ".format(countdown) + tabTitle);
                swal({
                    icon: "warning",
                    title: "Session Timeout",
                    text: opts.sessionTimeoutMsg.format(countdown),
                    buttons: {
                        cancel: {
                            text: "Logout",
                            value: false,
                            visible: true,
                            className: "btn-secondary",
                            closeModal: true
                        },
                        confirm: {
                            text: "Stay Login",
                            value: true,
                            visible: true,
                            className: "btn-primary",
                            closeModal: true
                        }
                    },
                    timer: 2 * 1000
                }).then(function (isConfirm) {
                    $(document).attr("title", tabTitle);
                    clearInterval(countingDown);
                    if (isConfirm) {
                        extendSession();
                    } else {
                        logout();
                    }
                });
            } else {
                clearInterval(countingDown);
            }
        }, 1000);
    };

    $('body').on(opts.activityEvents, function () {
        idleCounter = opts.idleTimeLimit;
        store.set('idleTimerLastActivity', $.now());
    });

    init();
};

