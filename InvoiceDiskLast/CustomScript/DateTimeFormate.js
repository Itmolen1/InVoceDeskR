

    $(document).ready(function () {
        $('#mainrow1 .ServiceDate').datepicker({
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            dateFormat: 'yy/mm/dd'
        }).datepicker;

        if ($(this).hasClass('birthdate')) {
            $(this).datepicker('option', 'yearRange', '1980:c');
            $(this).datepicker('option', 'defaultDate', '-10y');
        }


        var date = new Date();
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        if (day < 10) {
            day = "0" + day;
        }
        if (month < 10) {
            month = "0" + month;
        }
        var date = month + "/" + day + "/" + year;
     
        $('#mainrow1 .ServiceDate').val(date);
    });

$(document).ready(function () {
    $('.FromDate').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'yy/mm/dd'
    }).datepicker();
    if ($(this).hasClass('birthdate')) {
        $(this).datepicker('option', 'yearRange', '1980:c');
        $(this).datepicker('option', 'defaultDate', '-10y');
    }
});

$(document).ready(function () {
    $('.DueDate').datepicker({
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'yy/mm/dd'
    }).datepicker();
    if ($(this).hasClass('birthdate')) {
        $(this).datepicker('option', 'yearRange', '1980:c');
        $(this).datepicker('option', 'defaultDate', '-10y');
    }
});

$('body').on('focus', ".ss", function () {

    $(this).closest('tr').find(':input').removeClass('hasDatepicker');
    $(this).datepicker({
        changeMonth: true,
        changeYear: true,
        gotoCurrent: true,
        dateFormat: 'dd/mm/yy',
    });
    if ($(this).hasClass('birthdate')) {
        $(this).datepicker('option', 'yearRange', '1980:c');
        $(this).datepicker('option', 'defaultDate', '-10y');
    }
});

