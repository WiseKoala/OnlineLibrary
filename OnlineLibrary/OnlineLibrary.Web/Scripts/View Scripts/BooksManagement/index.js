$(document).ready(function () {

    function BooksViewModel() {
        var self = this;

        self.books = ko.observableArray([]);
        self.currentPage = ko.observable(1);
        self.totalPages = ko.observable();
    }

    // Activate knockout.js
    var viewModel = new BooksViewModel();
    ko.applyBindings(viewModel);

    function loadData(pageNumber) {
        var settings = {
            url: $("#booksList").first().data("getBooksUrl"),
            method: "GET",
            data: { pageNumber: pageNumber },
            success: function (data) {
                viewModel.books.removeAll();

                for (var i = 0; i < data.books.length; i++) {
                    viewModel.books.push(data.books[i]);
                }

                viewModel.totalPages(data.totalPages);
            },
            error: function () {
                alert('Error');
            }
        };

        $.ajax(settings);
    }

    loadData(viewModel.currentPage());

    $("#prevButton").click(function (e) {
        loadData(viewModel.currentPage() - 1);
        viewModel.currentPage(viewModel.currentPage() - 1);
        $("html, body").animate({ scrollTop: 0 }, "slow");
    });

    $("#nextButton").click(function (e) {
        loadData(viewModel.currentPage() + 1);
        viewModel.currentPage(viewModel.currentPage() + 1);
        $("html, body").animate({ scrollTop: 0 }, "slow");
    });
});