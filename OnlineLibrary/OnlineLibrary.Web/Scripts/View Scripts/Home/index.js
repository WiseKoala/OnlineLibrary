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

        var settings = {
            url: $("#booksList").data("getBooksUrl"),
            data: $("#searchFilters").serialize(),
            type: 'GET',
            success: function (result) {
                ko.mapping.fromJS(result, {}, viewModel);
            }
        };
        $.ajax(settings);
    }

    // Set initial page number in request form.
    $("#page-number").val(viewModel.FirstPage());
    loadBooks();

    function switchToPage(pageNumber) {
        var a = viewModel.NumberOfPages();
        if (viewModel.CurrentPage() == pageNumber) {
            LoadPage(pageNumber);
        }
        location.hash = pageNumber;
    }

    $("#previous-page").click(function (e) {
        switchToPage(parseInt(viewModel.CurrentPage()) - 1);
    });

    $("#next-page").click(function (e) {
        switchToPage(parseInt(viewModel.CurrentPage()) + 1);
    });

    $("#search-button").click(function (e) {
        switchToPage(parseInt(viewModel.FirstPage()));
    });

    Sammy(function () {
        this.get('#:CurrentPage', function () {
            LoadPage(this.params.CurrentPage);
        });
    }).run();

    function LoadPage(pageNumber) {
        viewModel.CurrentPage(pageNumber);
        $("#page-number").val(pageNumber);
        loadBooks();

        $("html, body").animate({ scrollTop: 0 }, "slow");
        $("#booksList").fadeOut(100, function () {
            $(".booksList").fadeIn(1000);
        });
    }

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

