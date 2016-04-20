$(document).ready(function () {
    $('#loan').click(function () {

        $.ajax({

            url: '/BookDetails/CreateLoanRequest/',

<<<<<<< Updated upstream
        request.done(function () {
            // Implemented toaster functionality with success message
            toastr.options =
                {
                    "closeButton": true,
                    "onclick": null,
                    "positionClass": "toast-bottom-right",
                    "timeOut": 30000,           // 30 seconds
                    "extendedTimeOut": 30000    // another 30 seconds, if user hovers mouse over notification
                }
            toastr.success('Please wait for a confirmation from our librarian before coming to the library to pick up the book.', 'Loan Request Sent')
            // alert("success");
        });

        request.fail(function (jqXHR, textStatus) {
            // Implemented toaster functionality with error message
            toastr.options =
                {
                    "closeButton": true,
                    "onclick": null,
                    "positionClass": "toast-bottom-right",
                    "timeOut": 30000,           // 30 seconds
                    "extendedTimeOut": 60000    // another 30 seconds, if user hovers mouse over notification
                }
            toastr.error('Please try one more time or contact our library\'s system administrator if the error persists.', 'Error Sending Loan Request')
            // alert("fail");
        });
=======
            data: { 'id': $('#loan').data("book-id") },

            success: function (response) {

                if (response.error != null)
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
                    toastr.error('Please try one more time or contact our library\'s system administrator if the error persists.', 'Error Sending Loan Request')

                }
                else
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
                    toastr.success('Please wait for a confirmation from our librarian before coming to the library to pick up the book.', 'Loan Request Sent')

                }
            },
>>>>>>> Stashed changes

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
});