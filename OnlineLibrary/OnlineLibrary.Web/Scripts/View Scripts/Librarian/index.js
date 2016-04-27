$( document ).ready(function() {
    $(".approve").click(function () {
        var loanId = $(this).data('loanId');
        $("#approve").find('input[id="loanId"]').val(loanId);
    });
});

$(document).ready(function () {
    $(".reject").click(function () {
        var loanId = $(this).data('loanId');
        $("#reject").find('input[id="loanId"]').val(loanId);
    });
});
