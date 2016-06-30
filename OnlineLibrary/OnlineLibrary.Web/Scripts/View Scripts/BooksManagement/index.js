$(document).ready(function () {

    function BooksViewModel() {
        var self = this;

        self.books = ko.observableArray([]);
        self.currentPage = ko.observable(1);
        self.totalPages = ko.observable();

        self.CopyBookIdToConfirmButton = function (book) {
            $("#removeBookConfirmButton").data("bookIdToRemove", book.Id);
        };
    }

    // Activate knockout.js
    var viewModel = new BooksViewModel();
    ko.applyBindings(viewModel);

        var settings = {
            url: $("#booksList").first().data("getBooksUrl"),
            method: "GET",
            data: { pageNumber: parseInt(pageNumber) || 1 },
            success: function (data) {
                if (data.totalPages < pageNumber) {
                    viewModel.currentPage(1);
                    loadData(1);
                }
                else {
                    viewModel.books.removeAll();

                    for (var i = 0; i < data.books.length; i++) {
                        viewModel.books.push(data.books[i]);
                    }

                    viewModel.totalPages(data.totalPages);
                }
            },
            error: function () {
                // Show toastr notification.
                toastr.options =
                    {
                        "closeButton": true,
                        "onclick": null,
                        "positionClass": "toast-bottom-right",
                        "timeOut": 5000,
                        "extendedTimeOut": 10000
                    }
                toastr.error('An error has occured. Try to refresh the page. If the error persists - contact the website adminsitrator.', 'Error!');
            }
        };

        $.ajax(settings);
    }

    loadData(viewModel.currentPage());

    function switchToPage(pageNumber) {
        location.hash = (parseInt(pageNumber) || 1);
    }

    $("#prevButton").click(function (e) {
        switchToPage(parseInt(viewModel.currentPage()) - 1);
    });

    $("#nextButton").click(function (e) {
        switchToPage(parseInt(viewModel.currentPage()) + 1);
    });
    
    Sammy(function () {
        this.get('#:currentPage', function () {

            var correctedPage = parseInt(this.params.currentPage) || 1;

            if (correctedPage < 1) {
                correctedPage = 1;
            }

            //if (this.params.currentPage != correctedPage) {
            //    window.setTimeout(function () {
            //        context.app.location_proxy.unbind();
            //        context.app.setLocation('#/');
            //        context.app.last_location = '#/';
            //        context.app.location_proxy.bind();
            //    }, 55); 
            //    return false;
            //}

            loadData(correctedPage);
            viewModel.currentPage(correctedPage);

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
                if (viewModel.currentPage() == viewModel.totalPages() && viewModel.currentPage() != 1) {
                    location.hash = viewModel.currentPage() - 1;
                }
                else {
                    loadData(viewModel.currentPage());
                }

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