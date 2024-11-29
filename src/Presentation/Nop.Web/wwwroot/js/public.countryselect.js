/*
** nopCommerce country select js functions
*/
+function ($) {
    'use strict';
    if ('undefined' == typeof (jQuery)) {
        throw new Error('jQuery JS required');
    }
    function countrySelectHandler() {
        var $this = $(this);
        var selectedItem = $this.val();
        var stateProvince = $($this.data('stateprovince'));
        var loading = $($this.data('loading'));
        loading.show();
        $.ajax({
            cache: false,
            type: "GET",
            url: $this.data('url'),
            data: { 
              'countryId': selectedItem,
              'addSelectStateItem': "true" 
            },
            success: function (data, textStatus, jqXHR) {
                stateProvince.html('');
                $.each(data,
                    function (id, option) {
                        stateProvince.append($('<option></option>').val(option.id).html(option.name));
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
    if ($(document).has('[data-trigger="country-select"]')) {
        $('select[data-trigger="country-select"]').change(countrySelectHandler);
        if ($('select[data-trigger="state-select"]').val() === "0") {
            $('select[data-trigger="country-select"]').val(131);
            $('select[data-trigger="country-select"]').change();
        }
        //$('select[data-trigger="country-select"]').prop("disabled", true);
    }
    $.fn.countrySelect = function () {
        this.each(function () {
            $(this).change(countrySelectHandler);
        });
    }
}(jQuery); 