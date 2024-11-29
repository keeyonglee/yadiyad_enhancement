"use strict";

var PageService = function PageService(opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model || {};
    model.categoryExpertiseList = [];

    var tmplCardCategory = $.templates("#template-card-category");
    var tmplCardExpertise = $.templates("#template-card-expertise");

    base.setCategoryExpertiseListResponse = function (target) {
        model.categoryExpertiseList = target.data;

        var otherObj = model.categoryExpertiseList.filter(function (obj) {
            return obj.name.toLowerCase() === 'others';
        });

        model.categoryExpertiseList = model.categoryExpertiseList.filter(function (obj) {
            return obj.name.toLowerCase() !== 'others';
        });

        if (otherObj.length > 0) {
            model.categoryExpertiseList.push(otherObj[0]);
        }

        model.categoryExpertiseList = model.categoryExpertiseList.map(function (item) {
            item.expertises = item.expertises.sort(function (a, b) {
                var textA = a.name.toUpperCase();
                var textB = b.name.toUpperCase();
                return textA < textB ? -1 : textA > textB ? 1 : 0;
            });
            var data = item.expertises.reduce(function (r, e) {
                // get first letter of name of current element
                var group = e.name[0];
                // if there is no property in accumulator with this letter create it
                if (!r[group]) r[group] = { group: group, children: [e] };
                // if there is push current element to children array for that letter
                else r[group].children.push(e);
                // return accumulator
                return r;
            }, {});

            // since data at this point is an object, to get array of values
            // we use Object.values method
            var result = Object.values(data);

            item.expertises = result;
            return item;
        });

        var htmlCardCategory = tmplCardCategory.render(model.categoryExpertiseList);
        $('#v-pills-tab-content').replaceWith(htmlCardCategory);
        var htmlCardExpertise = tmplCardExpertise.render(model.categoryExpertiseList);
        $('#v-pills-tabContent-content').replaceWith(htmlCardExpertise);
    };

    base.loadCategoryExpertiseListData = function (callback) {
        var settings = {
            "url": url.getJobServiceCategoryExpertise,
            "method": 'GET',
            "headers": {
                "Content-Type": "application/json"
            }
        };

        $.ajax(settings).done(callback);
    };

    //initital
    var init = function init() {
        base.loadCategoryExpertiseListData(base.setCategoryExpertiseListResponse);
    };

    init();
};

var pageService = new PageService({
    'url': {
        'getJobServiceCategoryExpertise': '/api/pro/source/JobServiceCategoryExpertise'
    }
});

