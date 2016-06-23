$(document).ready(function () {

    function BooksViewModel() {
        var self = this;

        self.books = ko.observableArray([]);
        self.currentPage = ko.observable(1);
        self.totalPages = ko.observable();

        self.CopyBookIdToConfirmButton = function (book) {
            $("#removeBookConfirmButton").data("bookIdToRemove", book.Id);
        };

        self.rowFadeOut = function (element) {
            $(element).fadeOut(1000);
        }
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

    function switchToPage(pageNumber) {
        location.hash = pageNumber;
    }

    $("#prevButton").click(function (e) {
        switchToPage(parseInt(viewModel.currentPage()) - 1);
    });

    $("#nextButton").click(function (e) {
        switchToPage(parseInt(viewModel.currentPage()) + 1);
    });

    Sammy(function () {
        this.get('#:currentPage', function () {
            loadData(this.params.currentPage);
            viewModel.currentPage(this.params.currentPage);
            $("html, body").animate({ scrollTop: 0 }, "slow");
            
            $("#booksList tbody").fadeOut(100, function () {
                $("#booksList tbody").fadeIn(1000);
            });
        })
    }).run();

    $('#removeBookConfirmButton').click(function () {
        var settings = {
            url: $(this).data("url"),
            data: { 'id': $(this).data("bookIdToRemove") },
            method: "POST",
            success: function (removedBook) {
                // Remove book from the model object.
                viewModel.books.remove(function (book) {
                    return book.Id == removedBook.Id;
                });

                // Show toastr notification.
                toastr.options =
                    {
                        "closeButton": true,
                        "onclick": null,
                        "positionClass": "toast-bottom-right",
                        "timeOut": 5000,
                        "extendedTimeOut": 10000
                    }
                toastr.success('The book with ISBN ' + removedBook.ISBN +
                    ' has been successfully deleted. All it\'s book copies were also removed.', 'Success!');
            },

            error: function (jqXHR) {
                toastr.options = {
                    "closeButton": true,
                    "onclick": null,
                    "positionClass": "toast-bottom-right",
                    "timeOut": 5000,
                    "extendedTimeOut": 10000
                }

                if (jqXHR.status == 404 || jqXHR.status == 400) {
                    toastr.error(jqXHR.responseJSON.error, jqXHR.statusText);
                }
                else {
                    toastr.error("An error has occured.", "Error.");
                }
            }
        };
        $.ajax(settings);
    });
});