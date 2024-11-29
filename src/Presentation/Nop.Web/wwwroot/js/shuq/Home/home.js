var PageHome = function (opts) {
    var base = this;
    var model = opts.model;
    var url = opts.url;

    var eatsPageIndex = 1;
    var martPageIndex = 1;
    var tmplCardEatsList = $.templates({
        markup: "#template-card-eats-list",
        allowCode: true
    });
    var tmplCardMartList = $.templates({
        markup: "#template-card-mart-list",
        allowCode: true
    });
    var $listEatsList = $(".eats-item-box");
    var $listMartList = $(".mart-item-box");

    $('#recipeCarousel').carousel({
        interval: 5000
    })

    var items = document.querySelectorAll('.carousel .carousel-item')

    items.forEach((x) => {
        const minPerSlide = 4
        var next = x.nextElementSibling
        for (var i = 1; i < minPerSlide; i++) {
            if (!next) {
                next = items[0]
            }
            let cloneChild = next.cloneNode(true)
            x.appendChild(cloneChild.children[0])
            next = next.nextElementSibling
        }
    })

    base.setUserEvent = function () {
        $("#btnEatsLoad").click(function () {
            var result = "";

            var filterData = {
                keyword: ""
            };

            var recordSize = 4;
            var offset = eatsPageIndex ? eatsPageIndex * recordSize : 0;

            var requestData = {
                "filter": filterData.keyword,
                "offset": offset,
                "recordSize": recordSize,
                "sorting": null,
                "advancedFilter": filterData
            };

            $.ajax({
                type: "POST",
                url: url.getFeaturedEatsProduct,
                headers: {
                    "Content-Type": "application/json"
                },
                dataType: 'json',
                data: JSON.stringify(requestData),
                success: function (item) {
                    if (item.data.length > 0) {
                        var htmlCardEatsList = tmplCardEatsList.render(item.data);
                        $listEatsList.append(htmlCardEatsList);
                        if (item.data.length < 10)
                            $("#btnEatsLoad").addClass("hidden");
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.log(xhr);
                }
            });

            eatsPageIndex++;
        });

        $("#btnMartLoad").click(function () {
            var result = "";

            var filterData = {
                keyword: ""
            };

            var recordSize = 4;
            var offset = martPageIndex ? martPageIndex * recordSize : 0;

            var requestData = {
                "filter": filterData.keyword,
                "offset": offset,
                "recordSize": recordSize,
                "sorting": null,
                "advancedFilter": filterData
            };

            $.ajax({
                type: "POST",
                url: url.getFeaturedMartProduct,
                headers: {
                    "Content-Type": "application/json"
                },
                dataType: 'json',
                data: JSON.stringify(requestData),
                success: function (item) {
                    if (item.data.length > 0) {
                        var htmlCardMartList = tmplCardMartList.render(item.data);
                        $listMartList.append(htmlCardMartList);
                        if (item.data.length < 10)
                            $("#btnMartLoad").addClass("hidden");
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.log(xhr);
                }
            });

            martPageIndex++;
        });
    }

    var init = function () {
        base.setUserEvent();
    };
    init();
};
var pageHome = new PageHome({
    'url': {
        'getFeaturedEatsProduct': '/api/shuq/homeAPI/featured-eats',
        'getFeaturedMartProduct': '/api/shuq/homeAPI/featured-mart'
    }
});