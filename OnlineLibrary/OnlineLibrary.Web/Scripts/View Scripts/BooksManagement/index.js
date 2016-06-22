$(document).ready(function () {

    function BooksViewModel() {
        var self = this;

        self.books = ko.observableArray([]);
    }

    // Activate knockout.js
    var viewModel = new BooksViewModel();
    ko.applyBindings(viewModel);

    function loadData() {
        var settings = {
            url: $("#booksList").first().data("getBooksUrl"),
            method: "GET",
            data: { pageNumber: 1 },
            success: function (books) {
                for (var i = 0; i < books.length; i++) {
                    viewModel.books.push(books[i]);
                }
            },
            error: function () {
                alert('Error');
            }
        };

        $.ajax(settings);
    }

    loadData();
});