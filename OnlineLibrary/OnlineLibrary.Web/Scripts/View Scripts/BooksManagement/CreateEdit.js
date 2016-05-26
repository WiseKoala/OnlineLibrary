$(document).ready(function () {

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

    $("#addCategory").click(function () {
        // Retreive all sub categories.
        var ajaxData = {
            method: "POST",
            complete: function (jqXhr) {
                var subCategories = jqXhr.responseJSON;
                var newIndex = 0;
                var lastSubcategorySequenceNumber = parseInt($("#bookSubcategories .book-subcategory").last().data("sequenceId"));

                if (!isNaN(lastSubcategorySequenceNumber)) {
                    newIndex = lastSubcategorySequenceNumber + 1;
                }

                // Add subcategory dropdown for current choosen category.
                var subcategoryDiv = $(document.createElement("div"));
                subcategoryDiv.addClass("book-subcategory");
                subcategoryDiv.attr("data-sequence-id", newIndex);

                var formGroup = $(document.createElement("div"));
                formGroup.addClass("form-group");

                var label = $(document.createElement("label"));
                label.text("Subcategory");

                var select = $(document.createElement("select"));
                select.attr("name", "SelectedSubcategories[" + newIndex + "]");
                select.addClass("form-control");

                for (var i = 0; i < subCategories.length; i++) {
                    var option = $(document.createElement("option"));
                    option.val(subCategories[i].Value);
                    option.text(subCategories[i].Name);
                    select.append(option);
                }

                formGroup.append(label);
                formGroup.append(select);
                subcategoryDiv.append(formGroup);
                $("#bookSubcategories").append(subcategoryDiv);
            }
        };
        $.ajax(window.location.origin + "/BooksManagement/ListBookSubCategories", ajaxData);
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