$(document).ready(function () {

    function SearchViewModel() {
        var self = this;

        self.title = ko.observable();
        self.author = ko.observable();
        self.publishDate = ko.observable();
        self.categoryId = ko.observable();
        self.isbn = ko.observable();
        self.description = ko.observable();
        self.subcategoryId = ko.observable();
        self.pageNumber = ko.observable(1);
    }

    function BooksViewModel() {
        var self = this;
        self.books = ko.observableArray([]);
        self.numberOfPages = ko.observable();

        self.searchData = new SearchViewModel();
    }

    // Activate knockout.js
    var viewModel = new BooksViewModel();
    ko.applyBindings(viewModel);

    function loadBooks(data) {

        var settings = {
            url: $("#booksList").data("getBooksUrl"),
            data: data,
            type: 'GET',
            success: function (result) {
                viewModel.books.removeAll();
                for (var i = 0; i < result.Books.length; i++) {
                    viewModel.books.push(result.Books[i]);
                }
                viewModel.numberOfPages(result.NumberOfPages);
            }
        };
        $.ajax(settings);
    }

    loadBooks(null);

    function LoadPage(data) {
        loadBooks(data);

        $("html, body").animate({ scrollTop: 0 }, "slow");
        $("#booksList").fadeOut(100, function () {
            $("#booksList").fadeIn(1000);
        });
    }

    // Establish Variables
    var History = window.History;

    function LoadDataBasedOnState() {
        var state = History.getState(); // Note: We are using History.getState() instead of event.state
        console.log('statechange:', state.data, state.title, state.url);

        LoadPage(state.data);
    }
    LoadDataBasedOnState();

    (function (window, undefined) {

        // Bind to State Change
        History.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
            LoadDataBasedOnState();
            var state = History.getState(); // Note: We are using History.getState() instead of event.state

            ko.mapping.fromJS(state.data, {}, viewModel.searchData);

            var pageNumber;
            if (state.data.pageNumber) {
                pageNumber = state.data.pageNumber;
            }
            else {
                pageNumber = 1;
            }

            viewModel.searchData.pageNumber(pageNumber);
        });

    })(window);

    function switchToPage(pageNumber) {
        viewModel.searchData.pageNumber(pageNumber);

        var searchFormJson = JSON.parse(ko.toJSON(viewModel.searchData));
        var searchFormText = $.param(searchFormJson);

        History.pushState(searchFormJson, null, "?" + searchFormText);
    }

    $("#previous-page").click(function (e) {
        switchToPage(viewModel.searchData.pageNumber() - 1);
    });

    $("#next-page").click(function (e) {
        switchToPage(viewModel.searchData.pageNumber() + 1);
    });

    $("#search-button").click(function (e) {
        viewModel.searchData.pageNumber(1);

        var searchFormJson = ko.toJS(viewModel.searchData);
        var searchFormText = $.param(searchFormJson);

        History.pushState(searchFormJson, null, "?" + searchFormText);
    });

    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

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

