var SimplePagination = function (selector) {
    var base = this;
    $.templates({
        tmplSimplePagination: "#tmp-simple-pagination"
    });

    var model = {
        hasMorePrevPage: false,
        hasMorePage : false,
        hasNextPage: false,
        hasPreviousPage: false,
        pageIndex: 0,
        pageSize: 0,
        totalPage: 0,
        totalCount: 0,
        set: function (model) {
            $.observable(this).setProperty(model);
            var totalPage = Math.floor(this.totalCount / this.pageSize) + (Math.ceil(this.totalCount % this.pageSize) > 0?1:0);
            $.observable(this).setProperty('totalPage', totalPage);
            var hasMorePage = (this.pageIndex + 1 + 2) <= totalPage;
            $.observable(this).setProperty('hasMorePage', hasMorePage);
            var hasMorePrevPage = (this.pageIndex + 1 - 2) >= 0;
            $.observable(this).setProperty('hasMorePrevPage', hasMorePrevPage);
            //console.log(this);

        },
        setPageIndex: function (pageIndex) {
            console.log(pageIndex);
            base.onPageChanged && base.onPageChanged(pageIndex);
        }
    };

    Object.defineProperty(base, 'model', {
        get() {
            return model;
        }
    });

    base.set = function (param) {
        model.set(param);
    }

    if (selector) {
        $.templates.tmplSimplePagination.link(selector, model);
    }

    return this;
};