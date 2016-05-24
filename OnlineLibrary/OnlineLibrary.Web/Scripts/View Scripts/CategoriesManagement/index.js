$(document).ready(function () {

    function LoansViewModel() {

        var self = this;
        self.categories = ko.observableArray([]);
        self.subCategories = ko.observableArray([]);
    }

    // Activate knockout.js
    var viewModel = new LoansViewModel();
    ko.applyBindings(viewModel);

    (function loadCategories() {
        var url = $("#categoriesList").data("categoriesUrl");

        var settings = {};
        settings.type = "GET";
        settings.url = url;
        settings.success = function (data) {
            viewModel.categories(data);
            $("#categoriesList input").on("change", bindCategoriesRadioButtons);
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    })();

    // Bind
    function bindCategoriesRadioButtons() {
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
    };

    $("#addCategory").click(function () {
        var categoryName = $("#newCategoryName").val();
        var url = $(this).data("addCategoryUrl");

        var settings = {};
        settings.type = "POST";
        settings.url = url;
        settings.data = {
            name: categoryName
        };
        settings.success = function (data) {
            // Clear input.
            $("#newCategoryName").val("");
            toastr.success("Category " + data.Name + " has been successfully created.");

            viewModel.categories.push(data);
            $("#categoriesList input").on("change", bindCategoriesRadioButtons); // TO FIX.
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    });
});