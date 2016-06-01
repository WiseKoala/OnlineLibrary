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

            $("#categoriesList input").change(categoriesRadioButtonsChange);

            // Set first radio input as checked.
            $("#categoriesList label").first().addClass("active");
            $("#categoriesList input").first().prop("checked", true);
            $("#categoriesList input").trigger("change");

            // Bind edit buttons.
            $("button.btn-edit-category").click(editCategoryClick);
            $("button.save-category-changes").click(categorySaveChangesClick);
            $("button.save-category-changes").mousedown(function (e) {
                $(this).data("mouseDown", true);
            });
            $("button.save-category-changes").mouseup(function (e) {
                $(this).data("mouseDown", false);
            });
            $("input[name='categoryName']").blur(categoryInputBlur);
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);
        }

        $.ajax(settings);
    })();

    function categoriesRadioButtonsChange() {
        var categoryId = parseInt($("input[name=category]:checked", "#categoriesList").val());
        var url = $("#categoriesList").data("subcategoryUrl");
        
        var settings = {};
        settings.success = function (data) {
            //viewModel.subCategories(data);

            ko.mapping.fromJS(data, {}, viewModel.subCategories);

            // Bind edit buttons.
            $("button.btn-edit-subcategory").click(editSubcategoryClick);
            $("button.save-subcategory-changes").click(subcategorySaveChangesClick);
            $("button.save-subcategory-changes").mousedown(function (e) {
                $(this).data("mouseDown", true);
            });
            $("button.save-subcategory-changes").mouseup(function (e) {
                $(this).data("mouseDown", false);
            });
            $("input[name='subcategoryName']").blur(subcategoryInputBlur);
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
            $("#categoriesList input").change(categoriesRadioButtonsChange); // TO FIX.
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

    var currentlyEditingName;

    // Categories.
    function editCategoryClick() {
        var root = $(this).closest("label");

        enterCategoryEditMode(root);
    }

    function enterCategoryEditMode(rootElement) {
        // Save category name.
        currentlyEditingName = rootElement.children(".category-name-caption").first().text();

        // Show edit category controls.
        var saveCategoryControls = rootElement.children("span.edit-category-controls").first();
        saveCategoryControls.css("display", "inline-block");

        // Set focus to the input element.
        var inputElement = saveCategoryControls.children("input");
        inputElement.first().focus();

        // Hide control buttons and caption.
        rootElement.children("span.control-buttons").first().hide();
        rootElement.children("span.category-name-caption").first().hide();
    }

    function categorySaveChangesClick() {
        var root = $(this).closest("label");

        // New name.
        var newCategoryName = $(this).siblings("input[name='categoryName']").first().val();

        // Extract category ID.
        var categoryId = parseInt(root.find("input[name='category']").val());

        // Extract URL.
        var updateCategoryUrl = $("#categoriesList").data("categoryUpdateUrl");

        var settings = {};
        settings.type = "POST";
        settings.url = updateCategoryUrl;
        settings.data = {
            categoryId: categoryId,
            newName: newCategoryName
        };
        settings.success = function (data) {
            exitCategoryEditMode(root);

            // Show notification.
            toastr.success("Category <b>" + data.Name + "</b> has been successfully updated.");
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);

            restoreCategoryName(root);
            exitCategoryEditMode(root);
        }

        $.ajax(settings);
    }

    function categoryInputBlur() {
        var root = $(this).closest("label");

        // Is mouse down over button 'Save Changes'.
        var isMouseDown = root.find("button.save-category-changes").first().data("mouseDown");
        
        if (!isMouseDown) {
            // Restore category name.
            restoreCategoryName(root);

            exitCategoryEditMode(root);
        }        
    }

    function restoreCategoryName(rootElement) {
        rootElement.find(".category-name-caption").first().text(currentlyEditingName);
        rootElement.find("input[name='categoryName']").first().val(currentlyEditingName);
    }

    function exitCategoryEditMode(rootElement) {
        // Show caption.
        rootElement.children("span.control-buttons").first().css("display", "inline-block");
        rootElement.children("span.category-name-caption").first().css("display", "inline-block");

        // Hide edit category controls.
        rootElement.children("span.edit-category-controls").first().hide();
    }

    // Subcategories.
    function editSubcategoryClick() {
        var root = $(this).closest("li");

        enterSubcategoryEditMode(root);
    }

    function enterSubcategoryEditMode(rootElement) {
        // Save subcategory name.
        currentlyEditingName = rootElement.children(".subcategory-name-caption").first().text();

        // Show edit subcategory controls.
        var saveCategoryControls = rootElement.children("span.edit-subcategory-controls").first();
        saveCategoryControls.css("display", "inline-block");

        // Set focus to the input element.
        var inputElement = saveCategoryControls.children("input");
        inputElement.first().focus();

        // Hide control buttons and caption.
        rootElement.children("span.control-buttons").first().hide();
        rootElement.children("span.subcategory-name-caption").first().hide();
    }

    function subcategorySaveChangesClick() {
        var root = $(this).closest("li");

        // New name.
        var newSubcategoryName = $(this).siblings("input[name='subcategoryName']").first().val();

        // Extract subcategory ID.
        var subCategoryId = parseInt(root.attr("data-id"));

        // Extract URL.
        var updateSubCategoryUrl = $("#subcategoriesList").data("subcategoryUpdateUrl");

        var settings = {};
        settings.type = "POST";
        settings.url = updateSubCategoryUrl;
        settings.data = {
            subCategoryId: subCategoryId,
            newName: newSubcategoryName
        };
        settings.success = function (data) {
            exitSubcategoryEditMode(root);

            // Show notification.
            toastr.success("Subcategory <b>" + data.Name + "</b> has been successfully updated.");
        };
        settings.error = function (jqXHR) {
            toastr.error(jqXHR.responseJSON.error);

            restoreSubcategoryName(root);
            exitSubcategoryEditMode(root);
        }

        $.ajax(settings);
    }

    function subcategoryInputBlur() {
        var root = $(this).closest("li");

        // Is mouse down over button 'Save Changes'.
        var isMouseDown = root.find("button.save-subcategory-changes").first().data("mouseDown");

        if (!isMouseDown) {
            restoreSubcategoryName(root);
            exitSubcategoryEditMode(root);
        }
    }

    function restoreSubcategoryName(rootElement) {
        rootElement.find(".subcategory-name-caption").first().text(currentlyEditingName);
        rootElement.find("input[name='subcategoryName']").first().val(currentlyEditingName);
    }

    function exitSubcategoryEditMode(rootElement) {
        // Show caption.
        rootElement.children("span.control-buttons").first().css("display", "inline-block");
        rootElement.children("span.subcategory-name-caption").first().css("display", "inline-block");

        // Hide edit category controls.
        rootElement.children("span.edit-subcategory-controls").first().hide();
    }
});