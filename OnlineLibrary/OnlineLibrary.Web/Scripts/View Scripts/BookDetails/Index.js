$(document).ready(function () {
    $('#loan').click(function () {

        var request = $.ajax({
            url: '/BookDetails/CreateLoanRequest/',
            data: { 'id': $('#loan').data("book-id") }
        });

        request.done(function () {
            // ToDo: Implement toaster functionality with success message
            alert("success");
        });

        request.fail(function (jqXHR, textStatus) {
            // ToDo: Implement toaster functionality with error message
            alert("fail");
        });

    });
});