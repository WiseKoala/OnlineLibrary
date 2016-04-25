$(document).ready(function () {
    $('#loan').click(function () {

        $.ajax({

            url: '/BookDetails/CreateLoanRequest/',

            data: { 'id': $('#loan').data("book-id") },

            success: function (response) {

                if (response.error == "no more copies")
                {

                    // There was an error in the controller.
                    toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 30000,           // 30 seconds
                            "extendedTimeOut": 60000    // another 30 seconds, if user hovers mouse over notification
                        }
                    toastr.error('There are no more available copies that you can request loans for.', 'No more copies available');

                }
                else if(response.success != null)
                {
                    // The controller returned with success.

                    toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 30000,           // 30 seconds
                            "extendedTimeOut": 30000    // another 30 seconds, if user hovers mouse over notification
                        }
                    toastr.info('Please wait for a confirmation from our librarian before coming to the library to pick up the book.', 'Loan Request Sent');

                }
                else if (response.error != null) {

                    // There is an error because no more available copies can be loaned
                    toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 30000,           // 30 seconds
                            "extendedTimeOut": 60000    // another 30 seconds, if user hovers mouse over notification
                        }
                    toastr.error('Please try one more time or contact our library\'s system administrator if the error persists.', 'Error Sending Loan Request');

                }
            },

            error: function (response) {
                // The url or controller was not found.

                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 30000,           // 30 seconds
                            "extendedTimeOut": 60000    // another 30 seconds, if user hovers mouse over notification
                        }
                toastr.error('Please try one more time or contact our library\'s system administrator if the error persists.', 'Error Sending Loan Request')

            }
        });
    });

    $('[data-toggle-tooltip="tooltip"]').tooltip();

});