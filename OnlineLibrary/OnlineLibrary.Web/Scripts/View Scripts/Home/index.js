$(document).ready(function () {

    function BooksViewModel() {
        var self = this;
        self.Books = ko.observableArray([]);
        self.FirstPage = ko.observable(1);
        self.NumberOfPages = ko.observable();
        self.CurrentPage = ko.observable(1);
    }

    // Activate knockout.js
    var viewModel = new BooksViewModel();
    ko.applyBindings(viewModel);

    function loadBooks() {
        $.ajax(
             {
                 url: '/Home/GetBooks',
                 dataType: 'json',
                 data: $("#searchFilters").serialize(),
                 type: 'GET',
                 success: function (result) {
                     ko.mapping.fromJS(result, {}, viewModel);
                 }
             });
    }

    // Set initial page number in request form.
    $("#page-number").val(viewModel.FirstPage());

    function switchToPage(pageNumber) {
        location.hash = pageNumber;
    }

    $("#previous-page").click(function (e) {
        switchToPage(parseInt(viewModel.CurrentPage()) - 1);
    });

    $("#next-page").click(function (e) {
        switchToPage(parseInt(viewModel.CurrentPage()) + 1);
    });

    $("#search-button").click(function () {
        switchToPage(viewModel.FirstPage());
    });

    Sammy(function () {
        this.get('#:CurrentPage', function () {

            viewModel.CurrentPage(this.params.CurrentPage);
            $("#page-number").val(this.params.CurrentPage);
            loadBooks();

            $("html, body").animate({ scrollTop: 0 }, "slow");
            $(".BookList").fadeOut(100, function () {
                $(".BookList").fadeIn(1000);
            });
        })
    }).run();

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

