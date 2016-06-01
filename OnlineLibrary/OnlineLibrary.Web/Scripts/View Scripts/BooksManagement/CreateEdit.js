
$(document).ready(function () {
    $("#add-category").click(function () {
  
        $(".deleteCategory").click(function () {
            var e = document.createElement("div");
            var value = $(this).attr("value");
        });

        // Retreive all sub categories.
        var ajaxData = {
            method: "GET",
            complete: function (jqXhr) {
                var categories = jqXhr.responseJSON;
                var mainDiv = $("#book-categories-subcategories");

                // Index for next category is equal with number of book-category-subcatery divs.
                var index = mainDiv.children(".book-category-subcategory").length;

                // New book-category-subcategory div.
                var categorySubcategryDiv = document.createElement("div");
                categorySubcategryDiv.className = "row book-category-subcategory"
                $(categorySubcategryDiv).css("margin-top", "10px");

                var isRemovedInput = document.createElement("input");
                isRemovedInput.className = "is-book-category-removed";
                isRemovedInput.name = "BookCategories[" + index + "].IsRemoved";
                isRemovedInput.type = "hidden";
                isRemovedInput.value = false;
                categorySubcategryDiv.appendChild(isRemovedInput);

                // New category div.
                var categoryDiv = document.createElement("div");
                categoryDiv.className = "col-md-3 book-category";

                // New subcateogry div.
                var subcategoryDiv = document.createElement("div");
                subcategoryDiv.className = "col-md-3 book-subcategory";

                var select = document.createElement("select");
                select.className = "form-control";
                select.id = "select-category";
                select.name = "BookCategories[" + index + "].Id";
                $(select).bind("change", addSubcategory);

                for (var i = 0; i < categories.length; i++) {
                    var option = document.createElement("option");
                    option.value = categories[i].Value
                    option.text = categories[i].Name
                    select.appendChild(option);

                }
                categoryDiv.appendChild(select);

                // New Delete category div/button
                var deleteCategoryDiv = document.createElement("div");
                deleteCategoryDiv.className = "col-md-2";
                var button = document.createElement("button");
                button.className = "btn btn-sm btn-danger remove-book-category";
                button.type = "button";
                $(button).bind("click", deleteCategory);
                var span = document.createElement("span");
                span.className = "glyphicon glyphicon-remove";
                button.appendChild(span);
                deleteCategoryDiv.appendChild(button);

                // Add all elements on mainDiv.
                categorySubcategryDiv.appendChild(categoryDiv);
                categorySubcategryDiv.appendChild(subcategoryDiv);
                categorySubcategryDiv.appendChild(deleteCategoryDiv);
                mainDiv.append(categorySubcategryDiv);
            }
        };
        $.ajax("/BooksManagement/ListBookCategories/", ajaxData);
    });

    function addSubcategory() {
        var id = parseInt( $(this).val() );
        var subcategoryDiv = $(this).parent().siblings(".book-subcategory");
        var selectName = this.name;
        var subcategoryName = selectName.replace("Id", "Subcategory.Id")

        var ajaxData = {
            method: "GET",
            data: { categoryId : id },
            success: function (data) {

                var subcategories = data;
                subcategoryDiv.empty()
                var select = document.createElement("select");
                select.className = "form-control";
                select.name = subcategoryName;

                for (var i = 0; i < subcategories.length; i++) {
                    var option = document.createElement("option");
                    option.value = subcategories[i].Value
                    option.text = subcategories[i].Name
                    select.appendChild(option);
                }
                subcategoryDiv.append(select);
            }
        };
        $.ajax("/BooksManagement/ListBookSubcategories/", ajaxData);
    }

    function deleteCategory() {
        $(this).parent().parent().children(".is-book-category-removed").val(true);
        var element = $(this).parent().parent();
        $(element).fadeOut(1000);
    }

    $(".select-category").change(addSubcategory);

    $(".remove-book-category").click(deleteCategory);

    // Implement datepicker for datetime inputs on this page.
    $('.datepicker').datepicker({
        dateFormat: "mm-dd-yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-160:+0"
    });

    $("#btnRemoveAuthorConfirm").click(function () {
        var authorIdToRemove = $(this).attr("data-author-to-remove-id");

        $("#bookAuthors table tbody tr[data-author-id='" + authorIdToRemove + "'] .is-removed").first().val(true);
        var authorRowToRemove = $("#bookAuthors table tbody tr[data-author-id='" + authorIdToRemove + "']").fadeOut(1000);
    });

    // Shows uploaded file name next to the Image Upload button
    $("#inputFile").click(function () {
        var inputs = document.querySelectorAll('.inputfile');
        Array.prototype.forEach.call(inputs, function (input) {

            var text = document.getElementById("textImageFileName")

            input.addEventListener('change', function (e) {
                var fileName = '';
                fileName = e.target.value.split('\\').pop();
                if (fileName)
                    text.innerHTML = fileName;
            });
        });
    });

   
    

    // Copies the author ID to modal window.
    function btnRemoveAuthorModal() {
        var authorId = $(this).closest(".book-author").data("authorId");
        $("#btnRemoveAuthorConfirm").attr("data-author-to-remove-id", authorId);
    };

    $(".btn-remove-author-modal").click(btnRemoveAuthorModal);

    $("#AddBookAuthor").click(function () {
        var authorsBody = $("#bookAuthors tbody");
        var newBookAuthorId = parseInt($("#bookAuthors tbody .book-author").last().data("authorId")) + 1;

        if (isNaN(newBookAuthorId)) {
            newBookAuthorId = 0;
        }

        // Create a div for new Author
        var newAuthorRow = document.createElement("tr");
        newAuthorRow.className = "book-author";
        newAuthorRow.setAttribute("data-author-id", newBookAuthorId);

        // Create inputs for Author data.
        // IsRemoved.
        var cell = document.createElement("td");

        var isRemovedInput = document.createElement("input");
        isRemovedInput.className = "is-removed";
        isRemovedInput.name = "Authors[" + newBookAuthorId + "].IsRemoved";
        isRemovedInput.type = "hidden";
        isRemovedInput.value = "false";

        // First Name.
        cell = document.createElement("td");

        var firstNameLabel = document.createElement("label");
        firstNameLabel.htmlFor = "Authors_" + newBookAuthorId + "__AuthorName.FirstName";
        firstNameLabel.innerHTML = "First Name";

        var firstNameInput = document.createElement("input");
        firstNameInput.name = "Authors[" + newBookAuthorId + "].AuthorName.FirstName";
        firstNameInput.id = "Authors_" + newBookAuthorId + "__AuthorName.FirstName";
        firstNameInput.type = "text";
        firstNameInput.className = "form-control";

        cell.appendChild(isRemovedInput);
        cell.appendChild(firstNameLabel);
        cell.appendChild(firstNameInput);
        newAuthorRow.appendChild(cell);

        // Middle Name.
        cell = document.createElement("td");

        var middleNameLabel = document.createElement("label");
        middleNameLabel.htmlFor = "Authors_" + newBookAuthorId + "__AuthorName.MiddleName";
        middleNameLabel.innerHTML = "Middle Name";

        var middleNameInput = document.createElement("input");
        middleNameInput.id = "Authors_" + newBookAuthorId + "__AuthorName.MiddleName";
        middleNameInput.name = "Authors[" + newBookAuthorId + "].AuthorName.MiddleName";
        middleNameInput.type = "text";
        middleNameInput.className = "form-control";

        cell.appendChild(middleNameLabel);
        cell.appendChild(middleNameInput);
        newAuthorRow.appendChild(cell);

        // Last Name.
        cell = document.createElement("td");

        var lastNameLabel = document.createElement("label");
        lastNameLabel.htmlFor = "Authors_" + newBookAuthorId + "__AuthorName.LastName";
        lastNameLabel.innerHTML = "Last Name";

        var lastNameInput = document.createElement("input");
        lastNameInput.id = "Authors_" + newBookAuthorId + "__AuthorName.LastName";
        lastNameInput.name = "Authors[" + newBookAuthorId + "].AuthorName.LastName";
        lastNameInput.type = "text";
        lastNameInput.className = "form-control";

        cell.appendChild(lastNameLabel);
        cell.appendChild(lastNameInput);
        newAuthorRow.appendChild(cell);

        // Button.
        cell = document.createElement("td");
        cell.id = "tdRemoveAuthorButton";

        var removeButton = document.createElement("button");
        removeButton.className = "btn btn-sm btn-danger btn-remove-author-modal";
        removeButton.type = "button";
        removeButton.setAttribute("data-toggle", "modal");
        removeButton.setAttribute("data-target", "#removeAuthorConfirmation");
        $(removeButton).click(btnRemoveAuthorModal);

        // Button icon.
        var removeButtonIcon = document.createElement("span");
        removeButtonIcon.className = "glyphicon glyphicon-remove";
        removeButton.appendChild(removeButtonIcon);

        cell.appendChild(removeButton);
        newAuthorRow.appendChild(cell);

        // Add new Author div to all authors
        authorsBody.append(newAuthorRow);
    })

    $(".passBookCopyForDelete").click(function () {
        rowid = $(this).data('bookCopyRowId');
    });

    $("#confirm-remove").click(function () {
        var trToHide = $("#bookCopy" + rowid);
        trToHide.fadeOut(1000, function () {
            trToHide.hide();
        })
        $("#bookCopy" + rowid + " .bookCopy .IsToBeDeleted").attr("value", "true");
    });

    $("#AddBookCopy").click(function () {
        // Retreive book conditions.
        var ajaxData = {
            method: "POST",
            complete: function (jqXHR) {
                var bookConditions = jqXHR.responseJSON;

                var newId = 0;

                var newBookCopyRowId = parseInt($(".passBookCopyForDelete").last().attr("data-book-copy-row-id")) + 1;

                if (isNaN(newBookCopyRowId)) {
                    newBookCopyRowId = 0;
                }

                var newTr = document.createElement("tr");
                newTr.id = "bookCopy" + newBookCopyRowId;

                var newTd = document.createElement("td");
                newTd.className = "col-sm-2";
                newTd.innerHTML = "Book Copy Id = new Id";

                var newTd2 = document.createElement("td");
                newTd2.className = "col-sm-3";

                var newTd3 = document.createElement("td");
                newTd3.className = "col-sm-7";

                var newDiv = document.createElement("div");
                newDiv.className = "bookCopy"

                var newInput = document.createElement("input");
                newInput.className = "IsToBeDeleted";
                newInput.type = "hidden";
                newInput.name = "BookCopies[" + newBookCopyRowId + "].IsToBeDeleted";
                newInput.value = "false";

                var bookCopySelect = document.createElement("select");
                bookCopySelect.id = "BookCopies_" + newBookCopyRowId + "__BookCondition";
                bookCopySelect.name = "BookCopies[" + newBookCopyRowId + "].BookCondition";
                bookCopySelect.className = "form-control";

                // Fill dropdown.
                for (var i = 0; i < bookConditions.length; i++) {
                    var optionElement = document.createElement("option");
                    optionElement.value = bookConditions[i].Value;
                    optionElement.innerHTML = bookConditions[i].Name;

                    bookCopySelect.appendChild(optionElement);
                }

                // Remove button
                var newRemoveButton = document.createElement("button");
                newRemoveButton.className = "btn btn-danger btn-sm passBookCopyForDelete";
                newRemoveButton.type = "button";
                newRemoveButton.setAttribute("data-toggle", "modal");
                newRemoveButton.setAttribute("data-target", "#deleteCopy");
                newRemoveButton.setAttribute("data-book-copy-row-id", newBookCopyRowId);

                // Button icon.
                var newRemoveButtonIcon = document.createElement("span");
                newRemoveButtonIcon.className = "glyphicon glyphicon-remove";
                newRemoveButton.appendChild(newRemoveButtonIcon);

                newDiv.appendChild(newInput);
                newDiv.appendChild(bookCopySelect);
                newTd2.appendChild(newDiv);
                newTd3.appendChild(newRemoveButton);
                newTr.appendChild(newTd);
                newTr.appendChild(newTd2);
                newTr.appendChild(newTd3);

                $("#bookCopies table").append(newTr);

                $(".passBookCopyForDelete").click(function () {
                    rowid = $(this).data('bookCopyRowId');
                });

                $("#confirm-remove").click(function () {
                    var trToHide = $("#bookCopy" + rowid);
                    trToHide.fadeOut(1000, function () {
                        trToHide.hide();
                    })
                    $("#bookCopy" + rowid + " .bookCopy .IsToBeDeleted").attr("value", "true");
                });
            }
        }
        $.ajax(window.location.origin + "/BooksManagement/ListBookConditions", ajaxData);
    });
});