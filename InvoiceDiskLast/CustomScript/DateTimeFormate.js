
    $(document).ready(function () {
        $('.ServiceDate').datepicker({
            changeMonth: true,
            changeYear: true,
            gotoCurrent: true,
            datetime: new Date(),
            dateFormat: 'dd/mm/yy',
        }).datepicker("setDate", new Date());
        if ($(this).hasClass('birthdate')) {
            $(this).datepicker('option', 'yearRange', '1980:c');
            $(this).datepicker('option', 'defaultDate', '-10y');
        }
    });


 $(document).ready(function () {
        $('.sdate').datepicker({
            changeMonth: true,
            changeYear: true,
            gotoCurrent: true,
            datetime: new Date(),
            dateFormat: 'dd/mm/yy',
        });
        if ($(this).hasClass('birthdate')) {
            $(this).datepicker('option', 'yearRange', '1980:c');
            $(this).datepicker('option', 'defaultDate', '-10y');
        }
    });



$(document).ready(function () {
    $('.PurchaseDate').datepicker({
        changeMonth: true,
        changeYear: true,
        gotoCurrent: true,
        showTimepicker: false
        //dateFormat: 'dd/mm/yy',
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
        gotoCurrent: true,
        showTimepicker: false
        //dateFormat: 'dd/mm/yy',
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



