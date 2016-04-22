$( document ).ready(function() {
    $(".approve").click(function () {
        var requestId = $(this).data('loanRequestId');
        $("#approve").find('input[id="loanRequestId"]').val(requestId);
    });
});

$(document).ready(function () {
    $(".reject").click(function () {
        var requestId = $(this).data('loanRequestId');
        $("#reject").find('input[id="loanRequestId"]').val(requestId);
    });
});
