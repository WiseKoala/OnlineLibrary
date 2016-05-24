$(document).ready(function () {

    function LoansViewModel() {

        var self = this;
        self.subCategories = ko.observableArray([]);
    }

    // Activate knockout.js
    var viewModel = new LoansViewModel();
    ko.applyBindings(viewModel);

    $("#categoriesList input").on("change", function () {
        var categoryId = parseInt($("input[name=categories]:checked", "#categoriesList").val());
        var url = $("#categoriesList").data("subcategoryUrl");

        var settings = {};
        settings.type = "GET";
        settings.url = url;
        settings.data = {
            categoryId: categoryId
        };
        settings.success = function (data) {
            viewModel.subCategories(data);
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    });
});