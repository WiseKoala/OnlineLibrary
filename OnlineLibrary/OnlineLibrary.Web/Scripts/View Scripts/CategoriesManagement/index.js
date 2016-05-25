$(document).ready(function () {

    function LoansViewModel() {

        var self = this;
        self.categories = ko.observableArray([]);
        self.subCategories = ko.observableArray([]);
    }

    // Activate knockout.js
    var viewModel = new LoansViewModel();
    ko.applyBindings(viewModel);
    
    // Set Toastr options.
    toastr.options = {
        "closeButton": true,
        "positionClass": "toast-bottom-right",
        "timeOut": 30000,           // 30 seconds
        "extendedTimeOut": 60000    // another 30 seconds, if user hovers mouse over notification
    };

    (function loadCategories() {
        var url = $("#categoriesList").data("categoriesUrl");

        var settings = {};
        settings.type = "GET";
        settings.url = url;
        settings.success = function (data) {
            viewModel.categories(data);

            // Create the settings object.
            var settings = { 
                success: function (data) {
                    viewModel.subCategories(data);
                }
            };
            $("#categoriesList input").on("change", settings, bindCategoriesRadioButtonsFirstTime);

            // Set first radio input as checked.
            $("#categoriesList label").first().addClass("active");
            $("#categoriesList input").first().prop("checked", true);
            $("#categoriesList input").trigger("change");
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    })();

    // Bind
    function bindCategoriesRadioButtonsFirstTime(settings) {
        var categoryId = parseInt($("input[name=category]:checked", "#categoriesList").val());
        var url = $("#categoriesList").data("subcategoryUrl");
        
        // The success callback should be provided by the caller.
        settings.type = "GET";
        settings.url = url;
        settings.data = {
            categoryId: categoryId
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        };

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
            toastr.success("Category <b>" + data.Name + "</b> has been successfully created.");

            viewModel.categories.push(data);

            // Build settings object.
            var settings = {
                success: function (data) {
                    $("#subcategoriesList").fadeOut(300, function () {
                        viewModel.subCategories(data);
                    });
                    $("#subcategoriesList").fadeIn(300);
                }
            };
            $("#categoriesList input").on("change", settings, bindCategoriesRadioButtons); // TO FIX.
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    });

    $("#addSubcategory").click(function () {
        var selectedCategoryId = parseInt($("input[name=category]:checked", "#categoriesList").val());
        var subcategoryName = $("#newSubcategoryName").val();
        var url = $(this).data("url");

        var settings = {};
        settings.type = "POST";
        settings.url = url;
        settings.data = {
            categoryId: selectedCategoryId,
            name: subcategoryName
        };
        settings.success = function (data) {
            // Clear input.
            $("#newSubcategoryName").val("");

            // Show notification.
            toastr.success("Subcategory <b>" + data.Name + "</b> has been successfully created.");

            // Store data in view model.
            viewModel.subCategories.push(data);
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    });
});