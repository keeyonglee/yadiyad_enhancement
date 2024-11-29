//pure js
'use strict';

String.prototype.format = function (obj) {
    var args = arguments;

    if (typeof obj === 'object') {
        var reg = /{{([\d|\w]{1,})}}/g;

        var text = this;
        var result;
        while ((result = reg.exec(text)) !== null) {
            var replacingText = result[0];
            var property = result[1];
            var value = obj[property];
            text = text.replace(replacingText, value === 0 ? 0 : value || '');
        }

        return text;
    }

    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] !== 'undefined' ? args[number] : match;
    });
};

window.location.getUrlQueryParams = function () {
    return this.toString().getUrlQueryParams();
};

String.prototype.getUrlQueryParams = function () {
    var queryParams = {};
    //create an anchor tag to use the property called search
    var anchor = document.createElement('a');
    //assigning url to href of anchor tag
    anchor.href = this;
    //search property returns the query string of url
    var queryStrings = anchor.search.substring(1);
    var params = queryStrings.split('&');

    for (var i = 0; i < params.length; i++) {
        var pair = params[i].split('=');
        var key = decodeURIComponent(pair[0].toLowerCase());

        if (key.indexOf("[]") > -1) {
            key = key.replace("[]", "");
            queryParams[key] = queryParams[key] || [];
            queryParams[key].push(decodeURIComponent(pair[1]));
        } else {
            queryParams[key] = decodeURIComponent(pair[1]);
        }
    }
    return queryParams;
};

window.location.getUrlPathParams = function () {
    var params = [];
    var url = window.location.href.replace('//', '');
    var urlStartIndex = url.indexOf('/');
    var urlEndLength = url.indexOf('?');
    if (urlEndLength === -1) {
        urlEndLength = url.length;
    }
    if (urlStartIndex === -1) {
        urlStartIndex = url.length - 1;
    }

    var relativeURL = url.substring(urlStartIndex, urlEndLength);

    params = relativeURL.split('/').filter(function (x) {
        return x !== "";
    });;

    return params;
};
JSON.flatten = function (data) {
    var result = {};

    function recurse(cur, prop) {
        if (Object(cur) !== cur) {
            result[prop] = cur;
        } else if (Array.isArray(cur)) {
            for (var i = 0, l = cur.length; i < l; i++) recurse(cur[i], prop + "[" + i + "]");
            if (l == 0) result[prop] = [];
        } else {
            var isEmpty = true;
            for (var p in cur) {
                isEmpty = false;
                recurse(cur[p], prop ? prop + "." + p : p);
            }
            if (isEmpty && prop) result[prop] = {};
        }
    }
    recurse(data, "");
    return result;
};
JSON.unflatten = function (data) {
    "use strict";
    if (Object(data) !== data || Array.isArray(data)) return data;
    var regex = /\.?([^.\[\]]+)|\[(\d+)\]/g,
        resultholder = {};
    for (var p in data) {
        var cur = resultholder,
            prop = "",
            m;
        while (m = regex.exec(p)) {
            cur = cur[prop] || (cur[prop] = m[2] ? [] : {});
            prop = m[2] || m[1];
        }
        cur[prop] = data[p];
    }
    return resultholder[""] || resultholder;
};

//global applicate layer scrope
var app = new function () {
    var base = this;

    base.mode = 'debug';

    base.title = {
        success: "Success",
        error: "Error"
    };

    base.storage = {};

    var onImgError = function onImgError(e) {
        e.target.className += " hidden";
    };

    base.onImgError = onImgError;
}();

var debug = new function () {
    var base = this;
    base.alert = function (data) {
        alert(data);
    };
    base.log = function (data) {
        console.log(data);
    };
    base['break'] = function () {
        debugger;
    };

    if (app.mode !== 'debug') {
        for (var prop in this) {
            if (typeof this[prop] === 'function') {
                this[prop] = function () {};
            }
        }
    }
}();

