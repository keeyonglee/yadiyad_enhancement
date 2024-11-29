var PageIndividualDashboardIndex = function (opts) {
    var base = this;
    var url = opts.url;
    var model = opts.model;
    var $content = $('.content-individual-profile');

    base.setDisplayProperty = function (data) {
        data = data.data;
        model = opts.model || {};
        var reg = /{{([\d|\w]{1,})}}/g;

        var jobTable = $(".tbl-job-engagement").DataTable();
        var consultantTable = $(".tbl-consultation-engagement").DataTable();
        data.jobEngagementCount = jobTable.rows().count();
        data.consultationEngagementCount = consultantTable.rows().count();

        $content.find("[text]")
            .each(function () {
                var text = $(this).attr('text');
                var result;
                while ((result = reg.exec(text)) !== null) {
                    var replacingText = result[0];
                    var property = result[1];
                    text = text.replace(replacingText, data[property] === 0 ? 0 : (data[property] || ''));
                }

                $(this).text(text);
            });
    };

    base.loadJobTable = function () {
        var cols = [
            { "data": "engagementId" },
            { "data": "engagementDate" },
            { "data": "engagementTitle" },
            { "data": "engagementStatusText" },
            { "data": "depositStatus" },
            { "data": "totalDepositAmount" },
            { "data": "depositReserve" }
        ];

        var table = $(".tbl-consultation-engagement").DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": "Search by Keyword",
                "lengthMenu": "_MENU_",
                "info": "This is info",
                "search": ""
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getJobEngagement,
                "type": "post",
                "data": function (data) {
                    data.customFilter = {};
                },
                "dataFilter": function (data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        item.engagementDate = moment.utc(item.engagementDate).local().format("DD/MM/YYYY");
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]],
        });


    }

    base.loadConsultantTable = function (){
        var cols = [
            { "data": "engagementId" },
            { "data": "engagementDate" },
            { "data": "engagementStatusText" },
            { "data": "depositStatus" },
            { "data": "totalDepositAmount" },
            { "data": "depositReserve" }
        ];

        $(".tbl-job-engagement").DataTable({
            "dom": '' + 'rt<"clear">',
            "language": {
                "searchPlaceholder": "Search by Keyword",
                "lengthMenu": "_MENU_",
                "info": "This is info",
                "search": ""
            },
            "processing": false,
            "serverSide": true,
            "autoWidth": true,
            "scrollX": true,
            "columns": cols,
            "ordering": false,
            "order": [[0, 'asc']],
            "ajax": {
                "url": url.getConsultationEngagement,
                "type": "post",
                "data": function (data) {
                    data.customFilter = {};
                },
                "dataFilter": function (data) {
                    var result = $.parseJSON(data);
                    var dataList = result.data.data;
                    dataList.forEach(function (item) {
                        item.engagementDate = moment.utc(item.engagementDate).local().format("DD/MM/YYYY");
                    });

                    return JSON.stringify(result.data);
                }
            },
            "lengthMenu": [[10, 25, 50, -1], [10, 25, 50]],
        });
    }

    base.getDashboardCardData = function (callback) {
        var settings = {
            "url": url.getDashboardData,
            "method": 'get',
            "headers": {
                "Content-Type": "application/json"
            }
        };
        $.ajax(settings).done(callback);
    };


    var init = function () {
        base.loadJobTable();
        base.loadConsultantTable();
        base.getDashboardCardData(base.setDisplayProperty);
    }

    init();
};

var PageIndividualDashboardIndex = new PageIndividualDashboardIndex({
    'url': {
        'getDashboardData': '/api/pro/account/dashboard',
        'getJobEngagement': '/api/pro/account/JobEngagement',
        'getConsultationEngagement': '/api/pro/account/ConsultantEngagement'
    }
});