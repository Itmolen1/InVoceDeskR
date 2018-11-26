$(document).ready(function () {
    debugger;
    $('#txtPassword').keyup(function () {
        $('#result').html(checkStrength($('#txtPassword').val()))
    })
    function checkStrength(password) {
        var strength = 0

        if (password.length < 1) {
            $('#result').removeClass().css('color', 'red');
            $('#result').hide();
            return 'Required'
        }

       else if (password.length < 6) {
            $('#result').removeClass()
            $('#result').addClass('short').css('color','red')
            $('#result').show();
            return 'Too short'
        }      

        if (password.length > 7) strength += 1
        // If password contains both lower and uppercase characters, increase strength value.
        if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) strength += 1
        // If it has numbers and characters, increase strength value.
        if (password.match(/([a-zA-Z])/) && password.match(/([0-9])/)) strength += 1
        // If it has one special character, increase strength value.
        if (password.match(/([!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1
        // If it has two special characters, increase strength value.
        if (password.match(/(.*[!,%,&,@,#,$,^,*,?,_,~].*[!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1
        // Calculated strength value, we can return messages
        // If value is less than 2
        if (strength < 2) {
            $('#result').removeClass()
            $('#result').addClass('weak').css('color','red')
            return 'Weak'
        } else if (strength == 2) {
            $('#result').removeClass()
            $('#result').addClass('good').css('color','skyblue')
            return 'Good'
        } else {
            $('#result').removeClass()
            $('#result').addClass('strong').css('color','Green')
            return 'Strong'
        }
    }
   
});