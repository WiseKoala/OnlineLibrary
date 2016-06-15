$(document).ready(function () {

    //Don't dismiss the modal window by default.
    var dismissModal = false;

    $("#approveLoanButton").click(function () {

        var approveForm = $("#approveForm");
        $.ajax({
            method: "POST",
            url: approveForm.find('input[id="approveUrl"]').val(),
            data: {
                bookCopyId: approveForm.find('input[id="bookCopyId"]').val(),
                loanId: approveForm.find('input[id="loanId"]').val()
            },
            async: false,
            success: function (response) {
                dismissModal = false;

                if (response.error != null) {
                    $("#approveLoanErrorAlert").removeClass("hidden");
                    $("#approveLoanError").text(response.error);
                    return false;
                }
                else {
                    toastr.options =
                    {
                        "closeButton": true,
                        "onclick": null,
                        "positionClass": "toast-bottom-right",
                        "timeOut": 5000,
                        "extendedTimeOut": 10000
                    };
                    toastr.success("The book has been successfully marked as loaned.", "Success.");

                    var itemToRemove = $('tr button[data-loan-id="' + $("#approveForm").find('input[id="loanId"]').val() + '"]').closest("tr");
                    itemToRemove.fadeOut(1000, function () { itemToRemove.remove(); });

                    dismissModal = true;
                }
            },
            error: function (jqXHR) {
                dismissModal = false;
                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        }

                toastr.error("An error has occured.", "Error.");
                dismissModal = true;
            }
        });

        if (!dismissModal) {
            return false;
        }

    });

    // ===== Tabs functionality.
    function LoansViewModel() {

        var self = this;
        self.pending = ko.observableArray([]);
        self.approved = ko.observableArray([]);
        self.inProgress = ko.observableArray([]);
        self.history = ko.observable({});

        self.rejectedVisible = ko.computed(function () {
            return self.history().Rejected && self.history().Rejected.length > 0;
        });

        self.completedVisible = ko.computed(function () {
            return self.history().Completed && self.history().Completed.length > 0;
        });

        self.lostBookVisible = ko.computed(function () {
            return self.history().LostBook && self.history().LostBook.length > 0;
        });

        self.cancelledVisible = ko.computed(function () {
            return self.history().Cancelled && self.history().Cancelled.length > 0;
        });
    }

    // Activate knockout.js
    var viewModel = new LoansViewModel();
    ko.applyBindings(viewModel);

    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

        // Extract the selected tab.
        var tabAnchor = $(e.target);

        // Initialize the settings object.
        var settings = {};
        settings.type = "GET";
        settings.complete = function (jqXHR) {
            // Find active tab.
            var tabName = $("ul#loans-tabs > li.active > a").attr("href");
            switch (tabName) {
                case "#pending":
                    viewModel.pending(jqXHR.responseJSON);
                    break;
                case "#approved":
                    viewModel.approved(jqXHR.responseJSON);
                    break;
                case "#inProgress":
                    viewModel.inProgress(jqXHR.responseJSON);
                    break;
                case "#history":
                    viewModel.history(jqXHR.responseJSON);
                    break;
                default:
                    console("Error");
                    break;
            }

            performBinding();
        };

        // Determine the URL to make request to.
        var requestUrl = tabAnchor.data("requestUrl");
        if (tabAnchor.attr("href") === "#history") {
            settings.url = requestUrl;
        }
        else {
            var status = tabAnchor.attr("data-status");
            settings.url = requestUrl + "?status=" + status;
        }

        // Make the AJAX request to the server.
        $.ajax(settings);
    });

    // Trigger the event manually in order to load the tab content.
    $('a[data-toggle="tab"]:first').trigger("show.bs.tab");

    $('#performLoan button[type="submit"]').click(function () {
        $.ajax({
            url: $(this).attr("data-url"),
            data: { loanId: $("#performLoan").find('input[id="loanId"]').val() },
            method: "POST",
            success: function (response) {
                var itemToRemove = $('tr button[data-loan-id="' + $("#performLoan").find('input[id="loanId"]').val() + '"]').closest("tr");
                itemToRemove.fadeOut(1000, function () { itemToRemove.remove(); });

                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        };
                toastr.success("The book has been successfully marked as loaned.", "Success.");
            },
            error: function (jqXHR) {
                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        }

                if (jqXHR.status == 404) {
                    toastr.error(jqXHR.responseJSON.error, jqXHR.statusText);
                }
                else {
                    toastr.error("An error has occured.", "Error.");
                }
            }
        });
    });

    $('#cancelApprovedLoan button[type="submit"]').click(function () {
        $.ajax({
            url: $(this).attr("data-url"),
            data: { loanId: $("#cancelApprovedLoan").find('input[id="loanId"]').val() },
            method: "POST",
            success: function (response) {
                var itemToRemove = $('tr button[data-loan-id="' + $("#cancelApprovedLoan").find('input[id="loanId"]').val() + '"]').closest("tr");
                itemToRemove.fadeOut(1000, function () { itemToRemove.remove(); });

                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        };
                toastr.success("The loan has been successfully cancelled.", "Success.");
            },
            error: function (jqXHR) {
                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        }

                if (jqXHR.status == 404) {
                    toastr.error(jqXHR.responseJSON.error, jqXHR.statusText);
                }
                else {
                    toastr.error("An error has occured.", "Error.");
                }
            }
        });
    });

    $('#returnLoanedBook button[type="submit"]').click(function () {
        var settings = {
            url: $(this).attr("data-url"),
            data: {
                loanId: $("#returnLoanedBookOptions").find('input[id="loanId"]').val(),
                bookCondition: $("#returnLoanedBookOptions .form-group").find('select[id="BookCondition"]').val()
            },
            method: "POST",
            success: function (response) {
                var itemToRemove = $('tr button[data-loan-id="' + $("#returnLoanedBookOptions").find('input[id="loanId"]').val() + '"]').closest("tr");
                itemToRemove.fadeOut(1000, function () { itemToRemove.remove(); });

                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        };
                toastr.success("The loaned book has been successfully marked as returned.", "Success.");

                resetBookConditionSelect();
            },
            error: function (jqXHR) {
                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        }

                if (jqXHR.status == 404) {
                    toastr.error(jqXHR.responseJSON.error, jqXHR.statusText);
                }
                else {
                    toastr.error("An error has occured.", "Error.");
                }

                resetBookConditionSelect();
            }
        };

        $.ajax(settings);
    });

    function resetBookConditionSelect() {
        var selectList = $("#returnLoanedBookOptions .form-group").find('select[id="BookCondition"]');
        selectList.prop("selectedIndex", 0);
    }

    $('#lostLoanedBook button[type="submit"]').click(function () {
        $.ajax({
            url: $(this).attr("data-url"),
            data: { loanId: $("#lostLoanedBook").find('input[id="loanId"]').val() },
            method: "POST",
            success: function (response) {
                var itemToRemove = $('tr button[data-loan-id="' + $("#lostLoanedBook").find('input[id="loanId"]').val() + '"]').closest("tr");
                itemToRemove.fadeOut(1000, function () { itemToRemove.remove(); });

                toastr.options =
                        {
                            "closeButton": true,
                            "onclick": null,
                            "positionClass": "toast-bottom-right",
                            "timeOut": 5000,
                            "extendedTimeOut": 10000
                        };
                toastr.success("The loaned book has been successfully marked as lost.", "Success.");
            },
            error: function (jqXHR) {
                toastr.options =
                {
                    "closeButton": true,
                    "onclick": null,
                    "positionClass": "toast-bottom-right",
                    "timeOut": 5000,
                    "extendedTimeOut": 10000
                };

                if (jqXHR.status == 404) {
                    toastr.error(jqXHR.responseJSON.error, jqXHR.statusText);
                }
                else {
                    toastr.error("An error has occured.", "Error.");
                }
            }
        });
    });


    function performBinding() {
        $(".approve").click(function () {
            var loanId = $(this).data('loanId');
            $("#approveForm").find('input[id="loanId"]').val(loanId);
        });

        $(".reject").click(function () {
            var loanId = $(this).data('loanId');
            $("#reject").find('input[id="loanId"]').val(loanId);
        });

        $(".passLoanIdForLoan").click(function () {
            var loanId = $(this).data('loanId');
            $("#performLoan").find('input[id="loanId"]').val(loanId);
        });

        $(".passLoanIdForCancel").click(function () {
            var loanId = $(this).data('loanId');
            $("#cancelApprovedLoan").find('input[id="loanId"]').val(loanId);
        });

        $(".return").click(function () {

            // Set loan id into the hidden form input in the model window.
            var loanId = $(this).data('loanId');
            $("#returnLoanedBookOptions").find('input[id="loanId"]').attr("value", loanId);

            // Obtain book condition by loan ID.
            var url = $("#loansInProgress").data("bookConditionByLoanUrl");

            var settings = {
                url: url,
                method: "GET",
                data: { loanId: loanId },
                success: function (data) {
                    $("#returnLoanedBookOptions").find("#currentBookConditionValue").text(data.bookCondition);
                },
                error: function (jqXHR) {
                    toastr.options =
                    {
                        "closeButton": true,
                        "onclick": null,
                        "positionClass": "toast-bottom-right",
                        "timeOut": 5000,
                        "extendedTimeOut": 10000
                    };
                    toastr.error(jqXHR.responseJSON.error, "Error");
                }
            };

            $.ajax(settings);
        });

        $(".lost").click(function () {
            var loanId = $(this).data('loanId');
            $("#lostLoanedBook").find('input[id="loanId"]').attr("value", loanId);
        });
    }
});