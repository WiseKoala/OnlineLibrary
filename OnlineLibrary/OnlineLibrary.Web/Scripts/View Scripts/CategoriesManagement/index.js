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

            ko.mapping.fromJS(data, {}, viewModel.categories);

            $("#categoriesList input").change(bindCategoriesRadioButtons);

            // Set first radio input as checked.
            $("#categoriesList label").first().addClass("active");
            $("#categoriesList input").first().prop("checked", true);
            $("#categoriesList input").trigger("change");

            // Bind edit category button.
            $("button.btn-edit-category").click(bindEditCategoryButtons);
            $("button.save-category-changes").click(bindSaveCategoryChangesButtons);
            $("input[name='categoryName']").blur(bindSaveCategoryChangesButtons);
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    })();

    function bindCategoriesRadioButtons() {
        var categoryId = parseInt($("input[name=category]:checked", "#categoriesList").val());
        var url = $("#categoriesList").data("subcategoryUrl");
        
        var settings = {};
        settings.success = function (data) {
            viewModel.subCategories(data);
        };
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

            viewModel.categories.splice(0, 0, data);

            // Build settings object.
            $("#categoriesList input").change(bindCategoriesRadioButtons); // TO FIX.
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
            viewModel.subCategories.splice(0, 0, data);
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    });

    function bindEditCategoryButtons() {
        var root = $(this).closest("label");
        
        // Show edit category controls.
        var saveCategoryControls = root.children("span.save-category-controls").first();
        saveCategoryControls.css("display", "inline-block");

        // Set focus to the input element.
        var inputElement = saveCategoryControls.children("input");
        inputElement.first().focus();

        // Hide caption.
        root.children("span.control-buttons").first().hide();
        root.children("span.category-name-caption").first().hide();
    };

    function bindSaveCategoryChangesButtons() {
        var root = $(this).closest("label");

        // Show caption.
        root.children("span.control-buttons").first().css("display", "inline-block");
        root.children("span.category-name-caption").first().css("display", "inline-block");

        // Hide edit category controls.
        root.children("span.save-category-controls").first().hide();
    }
});