$(document).ready(function () {
    $("#SearchData_SelectedCategoryId").change(function (e) {
        var url = $(this).data("subcategoriesUrl");

        var settings = {
            url: url,
            data: { categoryId: $(this).val() },
            method: "GET",
            success: function (subcategories) {
                // Clear select list.
                var subcategoriesSelectList = $("#SearchData_SelectedSubcategoryId");
                subcategoriesSelectList.empty();

                // Add 'Choose subcategory' option.
                var option = $(document.createElement("option"));
                option.text("Choose subcategory...");
                subcategoriesSelectList.append(option);

                // Add option elements for all subcategories.
                for (var i = 0; i < subcategories.length; i++) {
                    var option = $(document.createElement("option"));
                    option.val(subcategories[i].Id);
                    option.text(subcategories[i].Name);

                    subcategoriesSelectList.append(option);
                }
            }
        };

        $.ajax(settings);
    });
});