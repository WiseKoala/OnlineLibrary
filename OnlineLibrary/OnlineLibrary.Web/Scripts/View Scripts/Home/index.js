$(document).ready(function () {

    function BooksViewModel() {
        var self = this;
        self.books = ko.observableArray([]);
        self.numberOfPages = ko.observable();
        self.pageNumber = ko.observable(1);

        // Search filters.
        self.title = ko.observable();
        self.author = ko.observable();
        self.publishDate = ko.observable();
        self.category = ko.observable();
        self.isbn = ko.observable();
        self.description = ko.observable();
        self.subcategory = ko.observable();
    }

    // Activate knockout.js
    var viewModel = new BooksViewModel();
    ko.applyBindings(viewModel);

    function loadBooks() {

        var settings = {
            url: $("#booksList").data("getBooksUrl"),
            data: {
                title: viewModel.title(),
                author: viewModel.author(),
                publishDate: viewModel.publishDate(),
                categoryId: viewModel.category(),
                isbn: viewModel.isbn(),
                description: viewModel.description(),
                subcategoryId: viewModel.subcategory(),
                pageNumber: viewModel.pageNumber()
            },
            type: 'GET',
            success: function (result) {
                for (var i = 0; i < result.Books.length; i++) {
                    viewModel.books.push(result.Books[i]);
                }
                viewModel.numberOfPages(result.NumberOfPages);
            }
        };
        $.ajax(settings);
    }

    function fillUrlFromViewModel() {
        var url =
            "title=" + viewModel.title() + "&author=" + viewModel.author() + "&publishDate=" + viewModel.publishDate() +
            "&category=" + viewModel.category() + "&isbn=" + viewModel.isbn() + "&description=" + viewModel.description() +
            "&subcategory=" + viewModel.subcategory() + "&pageNumber=" + viewModel.pageNumber();

        location.hash = url;
    }

    function switchToPage(pageNumber) {
        fillUrlFromViewModel();

        if (viewModel.pageNumber() == pageNumber) {
            LoadPage(pageNumber);
        }
    }

    $("#previous-page").click(function (e) {
        switchToPage(viewModel.pageNumber() - 1);
    });

    $("#next-page").click(function (e) {
        switchToPage(viewModel.pageNumber() + 1);
    });

    $("#search-button").click(function (e) {
        switchToPage(1);
    });

    Sammy(function () {
        this.get(/\#title=(.*)&author=(.*)&publishDate=(.*)&category=(.*)&isbn=(.*)&description=(.*)&subcategory=(.*)&pageNumber=(.*)/,
            function (context) {
                var queryStringParameters = this.params['splat'];

                viewModel.title(queryStringParameters[0]);
                viewModel.author(queryStringParameters[1]);
                viewModel.publishDate(queryStringParameters[2]);
                viewModel.category(queryStringParameters[3]);
                viewModel.isbn(queryStringParameters[4]);
                viewModel.description(queryStringParameters[5]);
                viewModel.subcategory(queryStringParameters[6]);
                viewModel.pageNumber(queryStringParameters[7]);

                LoadPage();
            });
    }).run();

    location.hash = "title=&author=&publishDate=&category=&isbn=&description=&subcategory=&pageNumber=1";

    function LoadPage() {
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

