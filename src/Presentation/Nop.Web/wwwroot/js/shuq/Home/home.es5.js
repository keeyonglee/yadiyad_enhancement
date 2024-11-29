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

    $('.carousel .carousel-item').each(function () {
        var minPerSlide = 5;
        var next = $(this).next();
        if (!next.length) {
            next = $(this).siblings(':first');
        }
        next.children(':first-child').clone().appendTo($(this));

        for (var i = 0; i < minPerSlide; i++) {
            next = next.next();
            if (!next.length) {
                next = $(this).siblings(':first');
            }

            next.children(':first-child').clone().appendTo($(this));
        }
    });

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