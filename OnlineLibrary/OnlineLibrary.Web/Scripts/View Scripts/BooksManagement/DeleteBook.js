$(document).ready(function () {
    var id = 0;
    $(".pass-id-to-remove").click(function(){
        id = $(this).data("bookId");
    });

    $('.remove-book').click(function () {

        $.ajax({

            url: $(this).data("url"),

            data: { 'id': id },

            method: "POST",

            success: function (response) {

                var trId = "book" + id;
                var thisTR = $("tr#" + trId);

                thisTR.fadeOut(1000, function () {
                    thisTR.remove();
                });

                    toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,           
                            "extendedTimeOut": 10000   
                        }
                    toastr.success('The book with ISBN ' + response.ISBN + ' has been successfully deleted. All it\'s book copies were also removed.', 'Success!');
            },

            error: function (jqXHR) {
                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,           
                            "extendedTimeOut": 10000   
                        }

                if (jqXHR.status == 404 || jqXHR.status == 400) {
                    toastr.error(jqXHR.responseJSON.error, jqXHR.statusText);
                }
                else {
                    toastr.error("An error has occured.", "Error.");
                }                
            }
        });
    });
});