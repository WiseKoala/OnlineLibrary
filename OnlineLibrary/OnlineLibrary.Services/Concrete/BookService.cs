using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.DataAccess.Infrastructure;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Models;
using OnlineLibrary.Services.Models.BookServiceModels;

namespace OnlineLibrary.Services.Concrete
{
    public class BookService : IBookService
    {
        private ILibraryDbContext _dbContext;
        private ILibrarianService _librarianService;

        public BookService(ILibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BookService(ILibraryDbContext dbContext, ILibrarianService librarianService)
        {
            _dbContext = dbContext;
            _librarianService = librarianService;
        }

        public int GetNumberOfAvailableCopies(int bookId)
        {
            var count = _dbContext.Books.Include(b => b.Loans)
                                    .Include(b => b.BookCopies)
                                    .SingleOrDefault(b => b.Id == bookId)
                                    .BookCopies
                                    .Count(bc => !bc.IsLost &&
                                                 !(bc.Loans.Any(l => l.Status == LoanStatus.Approved ||
                                                                     l.Status == LoanStatus.InProgress)));
            return count;
        }

        public DateTime? GetEarliestAvailableDate(int bookId)
        {
            // Determine the loan with earliest return date.
            Loan earliestLoan = (from b in _dbContext.Books
                                 join bc in _dbContext.BookCopies
                                 on b.Id equals bc.BookId
                                 join l in _dbContext.Loans
                                 on bc.Id equals l.BookCopyId
                                 where b.Id == bookId && l.ExpectedReturnDate != null
                                 orderby l.ExpectedReturnDate
                                 select l)
                                 .FirstOrDefault();

            return earliestLoan?.ExpectedReturnDate;
        }

        public string GetConditionDescription(BookCondition cond)
        {
            switch (cond)
            {
                case BookCondition.New: return "New";
                case BookCondition.Fine: return "Fine";
                case BookCondition.VeryGood: return "Very Good";
                case BookCondition.Good: return "Good";
                case BookCondition.Fair: return "Fair";
                case BookCondition.Poor: return "Poor";
                default: return string.Empty;
            }
        }

        public bool IsBookCopyRemovable(int id)
        {
            var bookCopyExists = _dbContext.BookCopies.Any(bc => bc.Id == id);

            if (!bookCopyExists)
            {
                throw new KeyNotFoundException("Book Copy not found.");
            }

            // The book copy is removable if there are no loans with it's ID.
            return !_dbContext.Loans.Any(l => l.BookCopyId == id);
        }

        public BookCopy DeleteBookCopy(int id)
        {
            // Determine if there're any loans for the specified book copy.
            bool isRemovable = IsBookCopyRemovable(id);

            if (!isRemovable)
            {
                throw new BookCopyNotAvailableException("The specified book copy is unavailable for removal.");
            }
            else
            {
                SetNullOnBookCopyCascade(id);

                // Delete book copy.
                var bookCopy = _dbContext.BookCopies.Find(id);
                _dbContext.BookCopies.Remove(bookCopy);

                return bookCopy;
            }
        }

        public Book DeleteBook(int id, string path)
        {
            var book = _dbContext.Books
                .Include(b => b.BookCopies)
                .Where(b => b.Id == id)
                .SingleOrDefault();

            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }
            else
            {
                if (!IsBookAvailable(book.Id))
                {
                    throw new BookNotAvailableException("The specified book is currently involved in some loans.");
                }

                foreach (var bookCopy in book.BookCopies)
                {
                    SetNullOnBookCopyCascade(bookCopy.Id);
                }

                File.Delete(path);

                _dbContext.Books.Remove(book);
                _dbContext.SaveChanges();

                return book;
            }
        }

        private bool IsBookAvailable(int id)
        {
            var bookExists = _dbContext.Books.Any(b => b.Id == id);

            if (!bookExists)
            {
                throw new KeyNotFoundException("Book Copy not found.");
            }

            bool anyLoans = _dbContext.Loans.Where(l => l.BookId == id).Any();

            // Book is available if there're no loans.
            return !anyLoans;
        }

        /// <summary>
        /// Sets NULL for history records for the book copy.
        /// </summary>
        /// <param name="bookCopyId">ID of book copy</param>
        private void SetNullOnBookCopyCascade(int bookCopyId)
        {
            var historyRecords = _dbContext.History
                .Where(h => h.BookCopyId == bookCopyId)
                .ToList();
            historyRecords.ForEach(h => h.BookCopyId = null);
            _dbContext.SaveChanges();
        }

        public string FormatISBN(string ISBN)
        {
            // Give ISBN the default value if it is empty.
            ISBN = ISBN ?? Common.Infrastructure.LibraryConstants.undefinedISBN;

            // Format ISBN if it is not the default unknown ISBN value.
            if (!string.Equals(ISBN, Common.Infrastructure.LibraryConstants.undefinedISBN))
            {
                // Remove spaces and slashes.
                ISBN = ISBN.Trim().Replace("-", string.Empty);
                ISBN = ISBN.Replace(Common.Infrastructure.LibraryConstants.stringSpace, string.Empty);
            }

            return ISBN;
        }

        public void FormatAuthorNames(IList<BookAuthorServiceModel> authors)
        {
            if (authors.Any())
            {
                foreach (var author in authors)
                {
                    FormatString(author.AuthorName.FirstName);
                    FormatString(author.AuthorName.MiddleName);
                    FormatString(author.AuthorName.LastName);
                }
            }
        }

        private string FormatString(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                // Trim and remove extra space characters.
                text = text.Trim();
                Regex regex = new Regex("[ ]{2,}");
                text = regex.Replace(text, Common.Infrastructure.LibraryConstants.stringSpace);
            }

            return text;
        }

        public BookPaginationModel Find(BookSearchServiceModel model)
        {
            // Find by basic fields.
            var books = _dbContext.Books
                .WhereIf(model.PublishDate != null, b => b.PublishDate == model.PublishDate)
                .WhereIf(model.ISBN != null, b => b.ISBN.Contains(model.ISBN));

            // Find by title.
            if (model.Title != null)
            {
                string[] words = model.Title.Split(' ');
                var booksByTitle = _dbContext.Books.Where(b => words.Any(w => b.Title.Contains(w)));
                books = books.Intersect(booksByTitle);
            }

            // Find by description.
            if (model.Description != null)
            {
                string[] words = model.Description.Split(' ');
                var booksByDescription = _dbContext.Books.Where(b => words.Any(w => b.Description.Contains(w)));
                books = books.Intersect(booksByDescription);
            }

            // Find by authors.
            if (model.Author != null)
            {
                string[] words = model.Author.Split(' ');

                var booksByAuthors = _dbContext.Authors
                    .Where(a => words.Any(w =>
                           (a.FirstName != null && a.FirstName.Contains(w))
                        || (a.MiddleName != null && a.MiddleName.Contains(w))
                        || (a.LastName != null && a.LastName.Contains(w))))
                    .SelectMany(a => a.Books);

                books = booksByAuthors.Intersect(books);
            }

            // Find by categories.
            if (model.CategoryId != null && model.SubcategoryId == null)
            {
                var booksByCategory = (from c in _dbContext.Categories
                                       join sc in _dbContext.SubCategories.Include(sc => sc.Books)
                                       on c.Id equals sc.CategoryId
                                       where c.Id == model.CategoryId
                                       select sc.Books)
                                       .SelectMany(lb => lb)
                                       .Distinct();

                books = booksByCategory.Intersect(books);
            }

            // Find by subcategories.
            if (model.SubcategoryId != null)
            {
                var booksBySubcategory = _dbContext.SubCategories
                    .Include(sc => sc.Books)
                    .Where(sc => sc.Id == model.SubcategoryId)
                    .SelectMany(sc => sc.Books)
                    .Distinct();

                books = booksBySubcategory.Intersect(books);
            }

            var result = new BookPaginationModel();

            // Set the number of all books.
            result.NumberOfBooks = books.Count();

            // Take books of a specific page.
            var foundBooks = books.OrderBy(b => b.Id)
                                  .Skip((model.PageNumber - 1) * model.ItemPerPage)
                                  .Take(model.ItemPerPage)
                                  .ToList();

            result.Books = foundBooks;

            return result;
        }

        public void CreateEditPreparations(CreateEditBookServiceModel model, out Dictionary<string, string> modelErrors)
        {
            modelErrors = new Dictionary<string, string>();

            // Format and trim model fields.
            model.ISBN = FormatISBN(model.ISBN);
            model.Description = model.Description.Trim();
            FormatAuthorNames(model.Authors);

            // Remove elements that have to be deleted.
            RemoveMarkedBookCopiesFromModel(model);
            RemoveMarkedAuthorsFromModel(model);
            RemoveMarkedBookCategoriesFromModel(model);
            RemoveDupliacteAuthors(model);

            // Manually validate needed fields.
            ValidateISBN(model, modelErrors);
            ValidateAuthorsNumber(model, modelErrors);
            ValidateBookCopiesNumber(model, modelErrors);
            ValidateBookCategoriesNumber(model, modelErrors);
        }

        #region RemoveMarkedElements

        private void RemoveMarkedBookCopiesFromModel(CreateEditBookServiceModel model)
        {
            // Remove Book copies only from model only if not in book.
            if (model.BookCopies.Any())
            {
                var book = GetBook(model.Id);
                var bookCopiesToBeDeleted = model.BookCopies.Where(bc => bc.IsToBeDeleted).ToList();

                foreach (var bookCopy in bookCopiesToBeDeleted)
                {
                    if (book == null || (book != null && !book.BookCopies.Any(a => a.Id == bookCopy.Id)))
                    {
                        model.BookCopies.Remove(bookCopy);
                    }
                }
            }
        }

        private void RemoveMarkedAuthorsFromModel(CreateEditBookServiceModel model)
        {
            // Remove Authors only from model only if not in book.
            if (model.Authors != null && model.Authors.Any())
            {
                var book = GetBook(model.Id);
                var authorsToBeRemoved = model.Authors.Where(a => a.IsRemoved).ToList();

                foreach (var author in authorsToBeRemoved)
                {
                    if (book == null || (book != null && !book.Authors.Any(a => a.Id == author.Id)))
                    {
                        model.Authors.Remove(author);
                    }
                }
            }
        }

        private void RemoveMarkedBookCategoriesFromModel(CreateEditBookServiceModel model)
        {
            // Remove marked categories only from model only if not in book.
            if (model.BookCategories.Any())
            {
                var book = GetBook(model.Id);
                var categoriesToBeRemoved = model.BookCategories.Where(c => c.IsRemoved).ToList();

                foreach (var category in categoriesToBeRemoved)
                {
                    if (book == null || (book != null && !book.SubCategories
                                                         .Any(sc => sc.Id == category.Subcategory.Id)))
                    {
                        model.BookCategories.Remove(category);
                    }
                }
            }
        }

        public void RemoveDataFromDatabase(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors)
        {
            var bookCopiesToBeRemoved = model.BookCopies.Where(bc => bc.IsToBeDeleted).ToList();
            var allBookCopiesAreRemovable = true;

            foreach (var bookCopy in bookCopiesToBeRemoved)
            {
                if (!IsBookCopyRemovable(bookCopy.Id))
                {
                    allBookCopiesAreRemovable = false;
                    break;
                }
            }

            if (allBookCopiesAreRemovable)
            {
                bool saveChanges = true;

                RemoveBookCopies(model, modelErrors, saveChanges);

                RemoveAuthors(model, saveChanges);

                RemoveBookCategories(model, saveChanges);
            }
        }

        private void RemoveBookCopies(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors, bool saveChanges)
        {
            if (model.BookCopies.Any())
            {
                var bookCopiesToBeRemoved = model.BookCopies
                    .Where(bc => bc.IsToBeDeleted)
                    .ToList();

                foreach (var bookCopy in bookCopiesToBeRemoved)
                {
                    if (!IsNewBookCopy(bookCopy.Id))
                    {
                        // Remove the marked book copy from database and then remove it from model.
                        try
                        {
                            DeleteBookCopy(bookCopy.Id);
                            model.BookCopies.Remove(bookCopy);
                        }
                        catch (BookCopyNotAvailableException)
                        {
                            modelErrors.Add("Book Copy", "Book copy with Id # = " + bookCopy.Id + " is currently involved in loans. Unable to remove it.");
                        }
                        catch (KeyNotFoundException)
                        {
                            modelErrors.Add("Book Copy", "Book copy with Id # = " + bookCopy.Id + " is not found. Someone may have already deleted it. Please reload page to see latest changes.");
                        }
                    }
                }

                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
        }

        private void RemoveAuthors(CreateEditBookServiceModel model, bool saveChanges)
        {
            if (model.Authors.Any())
            {
                var book = GetBook(model.Id);

                var authorsToBeRemoved = model.Authors
                    .Where(a => a.IsRemoved)
                    .ToList();

                foreach (var author in authorsToBeRemoved)
                {
                    // Remove the marked auhots from database and then remove it from model.
                    var dbAuthor = _dbContext.Authors.Find(author.Id);
                    book.Authors.Remove(dbAuthor);
                    model.Authors.Remove(author);
                }
                
                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
        }

        private void RemoveBookCategories(CreateEditBookServiceModel model, bool saveChanges)
        {
            if (model.BookCategories.Any())
            {
                var book = GetBook(model.Id);
                var categoriesToBeRemoved = model.BookCategories
                    .Where(c => c.IsRemoved)
                    .ToList();

                foreach (var category in categoriesToBeRemoved)
                {
                    // Remove the marked category from database and then remove it from model.
                    var dbSubcategory = _dbContext.SubCategories.Find(category.Subcategory.Id);

                    book.SubCategories.Remove(dbSubcategory);
                    model.BookCategories.Remove(
                        model.BookCategories.SingleOrDefault(c => c.Subcategory.Id == dbSubcategory.Id));
                }

                if (saveChanges)
                {
                    _dbContext.SaveChanges();
                }
            }
        }

        #endregion RemoveMarkedElements

        #region RemoveDuplicates

        private void RemoveDupliacteAuthors(CreateEditBookServiceModel model)
        {
            // Remove duplicate authors from model.
            if (model.Authors.Any())
            {
                model.Authors = model.Authors
                    .GroupBy(a => new
                    {
                        a.AuthorName.FirstName,
                        a.AuthorName.MiddleName,
                        a.AuthorName.LastName
                    })
                    .Select(g => g.First())
                    .ToList();
            }
        }

        #endregion RemoveDuplicates

        #region ManualValidation

        private void ValidateISBN(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors)
        {
            var book = GetBook(model.Id);

            // Validate ISBN to be unique if it has changed or is new.
            if (book == null || (book != null && model.ISBN != book.ISBN))
            {
                if (!IsValidISBN(model.ISBN))
                {
                    modelErrors.Add("ISBN", "A book with this ISBN already exists");
                }
            }
        }

        private void ValidateAuthorsNumber(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors)
        {
            // Validate authors number to be less than limit.
            var limit = int.Parse(ConfigurationManager.AppSettings["ListLimitNumber"]);

            if (model.Authors.Where(a => !a.IsRemoved).Count() > limit)
            {
                modelErrors.Add("Authors", "Authors number is too large.");
            }

            // Validate authors number to be at least one.
            if (!model.Authors.Any())
            {
                modelErrors.Add("Authors", "There has to be at least one author.");
            }
        }

        private void ValidateBookCopiesNumber(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors)
        {
            var limit = int.Parse(ConfigurationManager.AppSettings["ListLimitNumber"]);

            if (model.BookCopies.Where(bc => !bc.IsToBeDeleted).Count() > limit)
            {
                modelErrors.Add("Book Copies", "Book Copies number is too large.");
            }
        }

        private void ValidateBookCategoriesNumber(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors)
        {
            // Validate book categories number to be less than limit.
            var limit = int.Parse(ConfigurationManager.AppSettings["ListLimitNumber"]);

            if (model.BookCategories.Where(bc => !bc.IsRemoved).Count() > limit)
            {
                modelErrors.Add("Book Categories", "Book Categories number is too large.");
            }

            // Validate book categories number to be at least one.
            if (!model.BookCategories.Any())
            {
                modelErrors.Add("BookCategories",
                "There has to be at least one book category.");
            }
        }

        #endregion ManualValidation

        public void CreateEdit(CreateEditBookServiceModel model, string imagePath)
        {
            if (model.Id <= 0)
            {
                // Create.
                Create(model, imagePath);
            }
            else
            {
                // Edit.
                Edit(model, imagePath);
            }
        }

        #region CreateEditHelpers

        private void Create(CreateEditBookServiceModel model, string imagePath)
        {
            // Create new book.
            var book = new Book()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                ISBN = model.ISBN,
                PublishDate = model.PublishDate
            };

            // Add front cover.
            if (!string.IsNullOrEmpty(model.BookCover.FrontCover))
            {
                book.FrontCover = SaveImageFromUrl(model.BookCover.FrontCover, imagePath);
            }
            else
            {
                book.FrontCover = SaveImage(model.BookCover.Image, imagePath);
            }

            // Add book copies.
            foreach (var bookCopy in model.BookCopies)
            {
                book.BookCopies.Add(new BookCopy
                {
                    Condition = bookCopy.BookCondition
                });
            }

            // Add authors.
            foreach (var author in model.Authors)
            {
                // Try to find author with the same name and link to existing author.
                Author existingAuthor = _dbContext.Authors
                            .FirstOrDefault(a => a.FirstName == author.AuthorName.FirstName
                            && a.MiddleName == author.AuthorName.MiddleName
                            && a.LastName == author.AuthorName.LastName);

                if (existingAuthor != null)
                {
                    book.Authors.Add(existingAuthor);
                }
                else
                {
                    book.Authors.Add(new Author
                    {
                        FirstName = author.AuthorName.FirstName,
                        MiddleName = author.AuthorName.MiddleName,
                        LastName = author.AuthorName.LastName
                    });
                }
            }

            // Add categories.
            foreach (var category in model.BookCategories)
            {
                var bookSubcategory = _dbContext.SubCategories
                                                 .Find(category.Subcategory.Id);

                book.SubCategories.Add(bookSubcategory);
            }

            // Save book.
            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();
        }

        private void Edit(CreateEditBookServiceModel model, string imagePath)
        {
            // Find and update book.
            Book book = GetBook(model.Id);
            
            if (model.Title != book.Title)
            {
                book.Title = model.Title;
            }
            
            if (model.Description != book.Description)
            {
                book.Description = model.Description;
            }

            if (model.ISBN != book.ISBN)
            {
                book.ISBN = model.ISBN;
            }

            if (model.PublishDate != book.PublishDate)
            {
                book.PublishDate = model.PublishDate;
            }

            // Delete old book cover image if new image was added.
            if (!string.IsNullOrEmpty(model.OldImagePath))
            {
                DeleteOldBookCoverImage(model.OldImagePath, imagePath);
            }

            // Save image from Url address in case image is imported.
            if (!string.IsNullOrEmpty(model.BookCover.FrontCover)
                && model.BookCover.FrontCover.StartsWith("http"))
            {
                book.FrontCover = SaveImageFromUrl(model.BookCover.FrontCover, imagePath);
            }

            // Update image if was uploaded.
            if (model.BookCover.Image != null)
            {
                book.FrontCover = SaveImage(model.BookCover.Image, imagePath);
            }

            if (model.BookCopies.Any())
            {
                // Mark book copies as lost if needed.
                foreach (var bookCopy in model.BookCopies)
                {
                    if (bookCopy.IsLost != (book.BookCopies.FirstOrDefault(bc => bc.Id == bookCopy.Id)?.IsLost ?? false))
                        _librarianService.ChangeIsLostStatus(bookCopy.Id, bookCopy.IsLost);
                }

                // Update book copies if needed.
                foreach (var bookCopyModel in model.BookCopies)
                {
                    var bookCopy = _dbContext.BookCopies.Find(bookCopyModel.Id);

                    if (bookCopy != null && bookCopy.Condition != bookCopyModel.BookCondition)
                    {
                        // Update existing book copy.
                        bookCopy.Condition = bookCopyModel.BookCondition;
                    }
                    else if (bookCopy == null)
                    {
                        // Create and add new book copy.
                        var newBookCopy = new BookCopy()
                        {
                            Condition = bookCopyModel.BookCondition,
                            BookId = book.Id
                        };

                        book.BookCopies.Add(newBookCopy);
                        _dbContext.SaveChanges();
                    }
                }
            }

            // Update authors.
            foreach (var authorModel in model.Authors)
            {
                // See if the book already has such an author.
                var bookAuthor = book.Authors
                    .FirstOrDefault(a => a.FirstName == authorModel.AuthorName.FirstName
                    && a.MiddleName == authorModel.AuthorName.MiddleName
                    && a.LastName == authorModel.AuthorName.LastName);

                if (bookAuthor == null)
                {
                    // Check whether the author with same name already exists.
                    var authorByName = _dbContext.Authors
                        .FirstOrDefault(a => a.FirstName == authorModel.AuthorName.FirstName
                        && a.MiddleName == authorModel.AuthorName.MiddleName
                        && a.LastName == authorModel.AuthorName.LastName);

                    if (authorByName != null)
                    {
                        book.Authors.Add(authorByName);
                    }
                    else
                    {
                        // Create and add new author.
                        Author newAuthor = new Author()
                        {
                            FirstName = authorModel.AuthorName.FirstName,
                            MiddleName = authorModel.AuthorName.MiddleName,
                            LastName = authorModel.AuthorName.LastName
                        };

                        _dbContext.Authors.Add(newAuthor);
                        book.Authors.Add(newAuthor);
                    }

                    // Select author from database with id coresponding to current author iterated from model.
                    var authorById = _dbContext.Authors.Find(authorModel.Id);

                    // If user edits existing author name, previous author is removed.
                    if (authorById != null)
                    {
                        if (authorById.FirstName != authorModel.AuthorName.FirstName
                            || authorById.MiddleName != authorModel.AuthorName.MiddleName
                            || authorById.LastName != authorModel.AuthorName.LastName)
                        {
                            book.Authors.Remove(authorById);
                        }
                    }
                }
            }

            // Update book categories if needed.
            foreach (var category in model.BookCategories)
            {
                if (!book.SubCategories.Any(sc => sc.Id == category.Subcategory.Id))
                {
                    // If the book has no such subcategory, add it.
                    var addedSubcategory = _dbContext.SubCategories.Find(category.Subcategory.Id);
                    book.SubCategories.Add(addedSubcategory);
                }
            }

            _dbContext.SaveChanges();
        }

        #endregion CreateEditHelpers

        public bool IsNewBookCopy(int bookCopyId)
        {
            return bookCopyId == 0;
        }

        public bool IsValidISBN(string ISBN)
        {
            ISBN = FormatISBN(ISBN);

            // ISBN is valid if it is unique or if it is the default unknown ISBN.
            return string.Equals(ISBN, Common.Infrastructure.LibraryConstants.undefinedISBN)
                    || !_dbContext.Books.Where(b => b.ISBN == ISBN).Any();
        }

        public Book GetBook(int id)
        {
            return _dbContext.Books.Where(b => b.Id == id)
                                  .Include(b => b.BookCopies)
                                  .Include(b => b.Authors)
                                  .Include(b => b.SubCategories)
                                  .SingleOrDefault();
        }

        public Dictionary<BookCondition, string> PopulateWithBookConditions()
        {
            var bookConditions = new Dictionary<BookCondition, string>();

            foreach (var cond in Enum.GetValues(typeof(BookCondition)))
            {
                bookConditions.Add((BookCondition)cond, GetConditionDescription((BookCondition)cond));
            }

            return bookConditions;
        }

        public bool IsBookDuplicate(DuplicateBookServiceModel model)
        {
            var books = _dbContext
                .Books.Include(b => b.Authors)
                .Where(b => b.Id != model.Id)
                .Select(b => new DuplicateBookServiceModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishDate = b.PublishDate,
                    Authors = b.Authors.Select(a => new DuplicateAuthorServiceModel
                    {
                        FirstName = a.FirstName,
                        MiddleName = a.MiddleName,
                        LastName = a.LastName
                    })
                })
            .ToList();

            bool isTitleDuplicate = false;
            bool isAuthorDuplicate = false;
            bool isPublishDateDuplicate = false;

            foreach (var book in books)
            {
                isTitleDuplicate = false;
                isAuthorDuplicate = false;
                isPublishDateDuplicate = false;

                // Trim book title.
                book.Title = Regex.Replace(book.Title, @"[^A-Za-z]", string.Empty);

                isTitleDuplicate = string.Equals(book.Title, model.Title, StringComparison.InvariantCultureIgnoreCase);

                if (isTitleDuplicate)
                {
                    isAuthorDuplicate = book.Authors.Any(author =>
                        model.Authors.Any(a =>
                            string.Equals(author.FirstName, a.FirstName, StringComparison.InvariantCultureIgnoreCase)
                            && string.Equals(author.MiddleName, a.MiddleName, StringComparison.InvariantCultureIgnoreCase)
                            && string.Equals(author.LastName, a.LastName, StringComparison.InvariantCultureIgnoreCase)));

                    if (isAuthorDuplicate)
                    {
                        break;
                    }

                    isPublishDateDuplicate = book.PublishDate.Year.Equals(model.PublishDate.Year);

                    if (isPublishDateDuplicate)
                    {
                        break;
                    }
                }
            }

            if (isTitleDuplicate && isAuthorDuplicate || isTitleDuplicate && isPublishDateDuplicate)
            {
                return true;
            }

            return false;
        }

        #region ImageHelepers

        private void DeleteOldBookCoverImage(string oldImagePath, string pathToImages)
        {
            // Get the image relative save path without tilda sign at the beginning.
            string imageSavePath = ConfigurationManager.AppSettings["BookCoverRelativePath"]
                                                       .Substring(1);

            // Delete the file if the old image path refers to a valid image relative save path.
            if (oldImagePath.IndexOf(imageSavePath) > -1)
            {
                string imageTitle = oldImagePath.Substring(oldImagePath.LastIndexOf("/"));
                string absoluteDeletePath = string.Concat(pathToImages, "\\", imageTitle);

                File.Delete(absoluteDeletePath);
            }
        }

        private string SaveImageFromUrl(string url, string imagePath)
        {
            // Prepare save path parts.
            string contentPath = ConfigurationManager.AppSettings["BookCoverRelativePath"];
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            string imageAbsoluteSavePath = Path.Combine(imagePath, fileName);

            // Save image.
            var webClient = new WebClient();
            webClient.DownloadFile(url, imageAbsoluteSavePath);

            // Craft the relative image save path.
            string imageRelativeSavePath = string.Concat(contentPath, '/', fileName);

            return imageRelativeSavePath;
        }

        private string SaveImage(HttpPostedFileServiceModel image, string pathToImages)
        {
            string imageRelativeSavePath = null;

            if (image != null)
            {
                var allowedContentTypes = new[]
                {
                    "image/jpeg", "image/png"
                };

                // If content type is allowed, save the image.
                if (allowedContentTypes.Contains(image.ContentType))
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string imageSavePath = Path.Combine(pathToImages, fileName);
                    SaveStreamToPath(image.InputStream, imageSavePath);

                    string relativePathToImageFolder = ConfigurationManager.AppSettings["BookCoverRelativePath"];
                    imageRelativeSavePath = string.Concat(relativePathToImageFolder, "/", fileName);
                }
            }

            return imageRelativeSavePath;
        }

        private void SaveStreamToPath(Stream stream, string savePath)
        {
            using (var memoryStream = new MemoryStream())
            using (var file = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(memoryStream);
                memoryStream.WriteTo(file);
            }
        }

        #endregion ImageHelepers
    }
}