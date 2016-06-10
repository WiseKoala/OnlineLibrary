$(document).ready(function () {

    $(".datepicker").datepicker({
        dateFormat: "mm/dd/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-160:+0"
    });

    $("#SearchData_CategoryId").change(function (e) {
        var url = $(this).data("subcategoriesUrl");

        var settings = {
            url: url,
            data: { categoryId: $(this).val() },
            method: "GET",
            success: function (subcategories) {
                // Clear select list.
                var subcategoriesSelectList = $("#SearchData_SubcategoryId");
                subcategoriesSelectList.empty();

                // Add 'Choose subcategory' option.
                var option = $(document.createElement("option"));
                option.text("Any");
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

    $("#toggleSearch").click(function () {
        if ($(this).find("span").hasClass("glyphicon-chevron-down")) {
            $(this).find("span").removeClass("glyphicon glyphicon-chevron-down");
            $(this).find("span").addClass("glyphicon glyphicon-chevron-up");
        }
        else {
            $(this).find("span").removeClass("glyphicon glyphicon-chevron-up");
            $(this).find("span").addClass("glyphicon glyphicon-chevron-down");
        }
    });
});