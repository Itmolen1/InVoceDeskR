function CharacterLenthCount() {
    debugger;
    var count = $('#InvoiceDescription').val().length;

    if (count > 150) {
        $('#lenthC').removeClass('badge-info');
        $('#lenthC').addClass('badge-danger');
        $('#lenthC').text(count + " excedded");


    }
    else if (count < 150) {
        $('#lenthC').removeClass('badge-danger');
        $('#lenthC').addClass('badge-info');
        $('#lenthC').text(count);

    }
}