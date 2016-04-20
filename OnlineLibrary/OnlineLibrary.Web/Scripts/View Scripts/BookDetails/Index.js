$(document).ready(function () {
    $('#loan').click(function () {

        var request = $.ajax({
            url: '/BookDetails/CreateLoanRequest/',
            data: { 'id': $('#loan').data("book-id") }
        });

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

    });
});