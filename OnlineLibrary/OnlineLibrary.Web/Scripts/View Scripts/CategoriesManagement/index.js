$(document).ready(function () {
    $("#categoriesList input").on("change", function () {
        alert($("input[name=categories]:checked", "#categoriesList").val());
    });
});