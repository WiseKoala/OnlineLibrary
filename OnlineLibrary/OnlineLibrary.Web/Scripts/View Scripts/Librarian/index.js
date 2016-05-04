$(document).ready(function () {
    $("#approveLoanButton").click(function () {
        var approveForm = $("#approveForm");
        var settings = {
            method: "POST",
            url: approveForm.find('input[id="approveUrl"]').val(),
            data: {
                bookCopyId: approveForm.find('input[id="bookCopyId"]').val(),
                loanId: approveForm.find('input[id="loanId"]').val()
            }
        }
        // Make AJAX request.
        $.ajax(settings)
            .done(function (response) {
                if (response.error) {
                    $("#approveLoanErrorAlert").removeClass("hidden");
                    $("#approveLoanError").text(response.error);
                }
                else {
                    window.location.replace(approveForm.find('input[id="redirectUrl"]').val());
                }

                //return false;
            });

        return false;
    });

    // ===== Tabs functionality.
    function LoansViewModel() {
        this.pending = ko.observableArray([]);
        this.approved = ko.observableArray([]);
        this.inProgress = ko.observableArray([]);
        this.history = ko.observableArray([]);
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
            var status = tabAnchor.data("status");
            settings.url = requestUrl + "?status=" + status;
        }

        // Make the AJAX request to the server.
        $.ajax(settings);
    });

    // Trigger the event manually in order to load the tab content.
    $('a[data-toggle="tab"]:first').trigger("show.bs.tab");
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
}
