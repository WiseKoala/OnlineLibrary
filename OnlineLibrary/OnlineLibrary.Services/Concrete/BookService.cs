using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using OnlineLibrary.Common.Exceptions;
using System.Text;
using System.Threading.Tasks;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using System.Data.Entity;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.DataAccess.Infrastructure;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Models;
using OnlineLibrary.Services.Models.BookServiceModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OnlineLibrary.Services.Concrete
{
    public class BookService : IBookService
    {
        private ILibraryDbContext _dbContext;
        private ILibrarianService _librarianService;

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

        public Book DeleteBook(int id)
        {
            var book = _dbContext.Books.Include(b => b.BookCopies).Where(b => b.Id == id).SingleOrDefault();

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

                File.Delete(book.FrontCover);

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
                foreach(var author in authors)
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
            var foundBooks = books.OrderBy( b => b.Id)
                                  .Skip((model.PageNumber - 1) * model.ItemPerPage)
                                  .Take(model.ItemPerPage)
                                  .ToList();

            result.Books = foundBooks;

            return result;
        }

        public void CreateEditPreparations(CreateEditBookServiceModel model, out Dictionary<string, string> modelErrors)
        {
            var book = _dbContext.Books.Find(model.Id);

            modelErrors = new Dictionary<string, string>();

            // Format and trim model fields.
            model.ISBN = FormatISBN(model.ISBN);
            model.Description = model.Description.Trim();
            FormatAuthorNames(model.Authors);

            // Remove elements that have to be deleted.
            RemoveMarkedBookCopiesFromModel(model);
            RemoveMarkedAuthors(model);
            RemoveMarkedBookCategories(model);
            RemoveDupliacteAuthors(model);
            RemoveDupliacteCategories(model);

            // Manually validate needed fields.
            ValidateISBN(model, modelErrors);
        }

        #region RemoveMarkedElements

        private void RemoveMarkedBookCopiesFromModel(CreateEditBookServiceModel model)
        {
            if (model.BookCopies.Any())
            {
                var bookCopiesToBeDeleted = model.BookCopies.Where(bc => bc.IsToBeDeleted);

                foreach (var bookCopy in bookCopiesToBeDeleted)
                {
                    if (bookCopy.IsToBeDeleted
                        && !_dbContext.BookCopies.Any(bc => bc.Id == bookCopy.Id))
                    {
                        // Remove the marked book copies from model only if they are not in the database.
                        model.BookCopies.Remove(bookCopy);
                    }
                }
            }            
        }

        public void RemoveMarkedBookCopiesFromDatabase(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors)
        {
            if (model.BookCopies.Any())
            {
                foreach (var bookCopy in model.BookCopies)
                {
                    if (bookCopy.IsToBeDeleted && !IsNewBookCopy(bookCopy.Id))
                    {
                        // Remove the marked book copies only if they are in the database.
                        try
                        {
                            DeleteBookCopy(bookCopy.Id);
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

                _dbContext.SaveChanges();
            }
        }
                
        private void RemoveMarkedAuthors(CreateEditBookServiceModel model)
        {
            // Remove Authors that have to be deleted.
            if (model.Authors.Any())
            {
                foreach (var author in model.Authors)
                {
                    if (author.IsRemoved)
                    {
                        model.Authors.Remove(author);
                    }
                }
            }
        }

        private void RemoveMarkedBookCategories(CreateEditBookServiceModel model)
        {
            // Remove categories that have to be deleted.
            if (model.BookCategories.Any())
            {
                foreach (var category in model.BookCategories)
                {
                    if (category.IsRemoved)
                    {
                        model.BookCategories.Remove(category);
                    }
                }
            }
        }

        #endregion RemoveMarkedElements

        #region RemoveDuplicates

        private void RemoveDupliacteAuthors(CreateEditBookServiceModel model)
        {
            RemoveDuplicateAuthorsFromModel(model);
            RemoveDuplicateAuthorsFromBook(model);
        }

        private void RemoveDuplicateAuthorsFromModel(CreateEditBookServiceModel model)
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

        private void RemoveDuplicateAuthorsFromBook(CreateEditBookServiceModel model)
        {
            // Remove duplicate authors from book if book is already created.
            if (model.Id > 0)
            {
                var book = _dbContext.Books.Find(model.Id);
                if (book != null)
                {
                    if (model.Authors.Any())
                    {
                        foreach (var author in model.Authors)
                        {
                            var authorsToRemove = book.Authors.Where(ba => ba.Id != author.Id && ba.FirstName == author.AuthorName.FirstName
                                                             && ba.MiddleName == author.AuthorName.MiddleName
                                                             && ba.LastName == author.AuthorName.LastName).Select(ba => ba).ToList();

                            foreach (var authorToRemove in authorsToRemove)
                            {
                                // Remove duplicate author from book.
                                book.Authors.Remove(authorToRemove);
                            }
                        }
                    }
                }
            }
        }

        private void RemoveDupliacteCategories(CreateEditBookServiceModel model)
        {
            RemoveDuplicateCategoriesFromModel(model);
            RemoveDuplicateCategoriesFromBook(model);
        }

        private void RemoveDuplicateCategoriesFromBook(CreateEditBookServiceModel model)
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

        private void RemoveDuplicateCategoriesFromModel(CreateEditBookServiceModel model)
        {
            // Remove duplicate authors from book if book is already created.
            if (model.Id > 0)
            {
                var book = _dbContext.Books.Find(model.Id);
                if (book != null)
                {
                    if (model.Authors.Any())
                    {
                        foreach (var author in model.Authors)
                        {
                            var authorsToRemove = book.Authors.Where(ba => ba.Id != author.Id && ba.FirstName == author.AuthorName.FirstName
                                                             && ba.MiddleName == author.AuthorName.MiddleName
                                                             && ba.LastName == author.AuthorName.LastName).Select(ba => ba).ToList();

                            foreach (var authorToRemove in authorsToRemove)
                            {
                                // Remove duplicate author from book.
                                book.Authors.Remove(authorToRemove);
                            }
                        }
                    }
                }
            }
        }

        #endregion RemoveDuplicates

        #region ManualValidation
        private void ValidateISBN(CreateEditBookServiceModel model, Dictionary<string, string> modelErrors)
        {
            var book = _dbContext.Books.Find(model.Id);

            // Validate ISBN to be unique if it has changed or is new.
            if (book == null || (book != null && model.ISBN != book.ISBN))
            {
                if (!IsValidISBN(model.ISBN))
                {
                    modelErrors.Add("ISBN", "A book with this ISBN already exists");
                }
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
            Book book;

            // Create new book.
            book = new Book()
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
            Book book = _dbContext.Books.Find(model.Id);

            if(model.Title != book.Title)
            {
                book.Title = model.Title;
            }

            if(model.Description != book.Description)
            {
                book.Description = model.Description;
            }

            if(model.ISBN != book.ISBN)
            {
                book.ISBN = model.ISBN;
            }

            if(model.PublishDate != book.PublishDate)
            {
                book.PublishDate = model.PublishDate;
            }

            // Delete old book cover image if new image was added.
            if (!string.IsNullOrEmpty(model.OldImagePath))
            {
                DeleteOldBookCoverImage(model.OldImagePath);
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

            // Mark book copies as lost if needed.
            if (model.BookCopies.Any())
            {
                foreach (var bookCopy in model.BookCopies)
                {
                    if (bookCopy.IsLost != book.BookCopies.FirstOrDefault(bc => bc.Id == bookCopy.Id).IsLost)
                        _librarianService.ChangeIsLostStatus(bookCopy.Id, bookCopy.IsLost);
                }
            }

            // Update book copies.
            foreach (var bookCopyModel in model.BookCopies)
            {
                var bookCopy = _dbContext.BookCopies.Find(bookCopyModel.Id);

                if (bookCopy != null && bookCopy.Condition != bookCopyModel.BookCondition)
                {
                    // Update existing book copy.
                    bookCopy.Condition = bookCopyModel.BookCondition;
                }
                else
                {
                    // Create and add new book copy.
                    var newBookCopy = new BookCopy()
                    {
                        Condition = bookCopyModel.BookCondition,
                        BookId = book.Id
                    };
                    _dbContext.BookCopies.Add(newBookCopy);
                }
            }

            // Update authors.
            foreach (var authorModel in model.Authors)
            {
                // Variable "author" is checking whether the author with same name is already in the database.
                // It selects the item or returns null if not found.
                var authorByName = _dbContext.Authors
                    .FirstOrDefault(a => a.FirstName == authorModel.AuthorName.FirstName
                    && a.MiddleName == authorModel.AuthorName.MiddleName
                    && a.LastName == authorModel.AuthorName.LastName);

                var bookAuthor = book.Authors
                    .FirstOrDefault(a => a.FirstName == authorModel.AuthorName.FirstName
                    && a.MiddleName == authorModel.AuthorName.MiddleName
                    && a.LastName == authorModel.AuthorName.LastName);

                // Select author from database with id coresponding to current author iterated from model.
                var authorById = _dbContext.Authors.Find(authorModel.Id);

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

                    book.Authors.Add(newAuthor);
                }

                // If user edits existing author name, previous author name is removed.
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

            book.SubCategories.Clear();

            var categories = model.BookCategories;
            foreach (var category in categories)
            {
                if (!book.SubCategories.Any(sc => sc.Id == category.Subcategory.Id))
                {
                    // If the book has no such subcategory, add it.
                    var adedSubcategory = _dbContext.SubCategories.Find(category.Subcategory.Id);
                    book.SubCategories.Add(adedSubcategory);
                }
                else
                {
                    // If the book has such category, remove it from model.
                    model.BookCategories.Remove(category);
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

        #region ImageHelepers

        private void DeleteOldBookCoverImage(string oldImagePath)
        {
            // Get the image relative save path without tilda sign at the beginning.
            string imageSavePath = ConfigurationManager.AppSettings["BookCoverRelativePath"]
                                                       .Substring(1);

            // Delete the file if the old image path refers to a valid image relative save path.
            if (oldImagePath.IndexOf(imageSavePath) > -1)
            {
                string relativeImagePath = "~" + oldImagePath
                    .Substring(oldImagePath.IndexOf(imageSavePath));

                File.Delete(relativeImagePath);
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
            string imageSavePath = null;

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
                    imageSavePath = Path.Combine(pathToImages, fileName);
                    SaveStreamToPath(image.InputStream, imageSavePath);
                }
            }

            return imageSavePath;
        }

        private void SaveStreamToPath(Stream stream, string savePath)
        {
            using (var file = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                ((MemoryStream)stream).WriteTo(file);
            }
        }

        #endregion ImageHelepers
    }
}