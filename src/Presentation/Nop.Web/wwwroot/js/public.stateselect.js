/*
** nopCommerce state select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' === typeof (jQuery)) {
        throw new Error('jQuery JS required');
    }
    function stateSelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var city = $($this.data('city'));
        var loading = $($this.data('loading'));
        var cityOption = $('#cityname');

        loading.show();
        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: { 
              'stateId': selectedItem,
              'addSelectStateItem': "true" 
            },
            success: function (data, textStatus, jqXHR) {
                city.html('');
                cityOption.html('');
                $.each(data,
                    function (id, option) {
                        city.append($('<option></option>').val(option.name).html(option.name));
                        cityOption.append($('<option></option>').val(option.name).html(option.name));

                    });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to retrieve states.');
            },
            complete: function (jqXHR, textStatus) {
                loading.hide();
            }
        });
    }
    if ($(document).has('[data-trigger="state-select"]')) {
        $('select[data-trigger="state-select"]').change(stateSelectHandler);
        let cityEl = $('select[name="City"]');
        let availableCity = cityEl.data("available-city");


        //$('select[data-trigger="state-select"]').val(131);
        //$('select[data-trigger="state-select"]').change();
        //$('select[data-trigger="state-select"]').prop("disabled", true);
    }
    $.fn.stateSelect = function () {
        this.each(function () {
            $(this).change(stateSelectHandler);
        });
    }
}(jQuery); 