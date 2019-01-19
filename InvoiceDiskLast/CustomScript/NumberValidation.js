
    $('.quantityG').keypress(function (event) {
        if (event.which != 46 && (event.which < 47 || event.which > 59)) {
            event.preventDefault();
            if ((event.which == 46) && ($(this).indexOf('.') != -1)) {
                event.preventDefault();
            }
        }
    });

  

$('.rateG ').keypress(function (event) {
    if (event.which != 46 && (event.which < 47 || event.which > 59)) {
        event.preventDefault();
        if ((event.which == 46) && ($(this).indexOf('.') != -1)) {
            event.preventDefault();
        }
    }
});
$('.FTotalG').keypress(function (event) {
    if (event.which != 46 && (event.which < 47 || event.which > 59)) {
        event.preventDefault();
        if ((event.which == 46) && ($(this).indexOf('.') != -1)) {
            event.preventDefault();
        }
    }
});




$('#Amount').keyup(function () {
    if (event.which != 46 && (event.which < 47 || event.which > 59)) {
        event.preventDefault();
        if ((event.which == 46) && ($(this).indexOf('.') != -1)) {
            event.preventDefault();
        }
    }
});


$(document).on('keypress', '.rateG', function (event) {

    if (event.which != 46 && (event.which < 47 || event.which > 59)) {
        event.preventDefault();
        if ((event.which == 46) && ($(this).indexOf('.') != -1)) {
            event.preventDefault();
        }
    }

});


$(document).on('keypress', '.FTotalG', function (event) {

    if (event.which != 46 && (event.which < 47 || event.which > 59)) {
        event.preventDefault();
        if ((event.which == 46) && ($(this).indexOf('.') != -1)) {
            event.preventDefault();
        }
    }

});




$(document).on('keypress', '.quantityG', function (event) {

    if (event.which != 46 && (event.which < 47 || event.which > 59)) {
        event.preventDefault();
        if ((event.which == 46) && ($(this).indexOf('.') != -1)) {
            event.preventDefault();
        }
    }
});
