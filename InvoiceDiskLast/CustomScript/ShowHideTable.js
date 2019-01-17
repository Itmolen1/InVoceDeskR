 $('.TableClose').click(function () {       
        var Count = $('.tbodyGood tr').length;
        if (parseInt(Count) > 0) {
            var SelectID = $('#mainrow #SelectProductQutationG').val();
            if (parseInt(SelectID) > 0 && parseInt($('#QuantityG').val()) > 0)
            {
                $.toast({
                    heading: 'cannot hide',
                    text: 'Table already in use',
                    icon: 'info',
                    loader: true,        // Change it to false to disable loader
                    loaderBg: '#FF0000'  // To change the background
                });
            }
            else {                
                $('#TableGood').hide();
                $('#TableGoodLink').show();
            }
        }
       
        });

        $('#TableGoodLink').click(function () {
            $('#TableGood').show();
            $('#TableGoodLink').hide();
        });

        $('#TableServiceLink').click(function () {
            $('#ServiceTable').show();
            $('#TableServiceLink').hide();
        });

        $('.TableCloseService').click(function () {
          
            var Count = $('.tbodyService tr').length;
            if (parseInt(Count) > 0) {
                var SelectID = $('#mainrow1 #SelectProductQutationS').val();
                if (parseInt(SelectID) > 0 && parseInt($('#QuantityS').val()) > 0)
                {                    
                    $.toast({
                        heading: 'cannot hide',
                        text: 'Table already in use',
                        icon: 'info',
                        loader: true,        // Change it to false to disable loader
                        loaderBg: '#FF0000'  // To change the background
                    });
                }
                else {
                    $('#ServiceTable').hide();
                    $('#TableServiceLink').show();
                }
            }
        });