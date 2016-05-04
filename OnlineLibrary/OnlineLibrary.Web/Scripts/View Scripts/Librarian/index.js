$( document ).ready(function() {
    $(".approve").click(function () {
        var loanId = $(this).data('loanId');
        $("#approveForm").find('input[id="loanId"]').val(loanId);
    });

    $(".reject").click(function () {
        var loanId = $(this).data('loanId');
        $("#reject").find('input[id="loanId"]').val(loanId);
    });

    $(".passLoanIdForLoan").click(function () {
        var loanId = $(this).data('loanId');
        $("#performLoan").find('input[id="loanId"]').val(loanId);
    });

    $(".passLoanIdForCancel").click(function () {
        var loanId = $(this).data('loanId');
        $("#cancelApprovedLoan").find('input[id="loanId"]').val(loanId);
    });

    $("#approveLoanButton").click(function () {
        var approveForm = $("#approveForm");
        var settings = {
            method: "POST",
            url: approveForm.find('input[id="approveUrl"]').val(),
            data: {
                bookCopyId: approveForm.find('input[id="bookCopyId"]').val(),
                loanId: approveForm.find('input[id="loanId"]').val()
            }
        }
        // Make AJAX request.
        $.ajax(settings)
            .done(function (response) {
                if (response.error) {
                    $("#approveLoanErrorAlert").removeClass("hidden");
                    $("#approveLoanError").text(response.error);
                }
                else {
                    window.location.replace(approveForm.find('input[id="redirectUrl"]').val());
                }

                //return false;
            });

        return false;
    });
});
