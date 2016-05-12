$(document).ready(function () {
    $('.remove-book').click(function () {

        var id = $(this).data("bookId");

        $.ajax({

            url: $(this).data("url"),

            data: { 'id': id },

            method: "POST",

            success: function (response) {

                var thisItem = $("tr").find("[data-book-id='" + id + "']");

                thisItem.closest('tr').find('td').fadeOut(1000, function () {
                    $(this).closest('tr').remove();
                });

                thisItem.parent("tr").remove();

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