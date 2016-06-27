using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using System.Data.Entity;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.Services.Models;
using OnlineLibrary.DataAccess.Infrastructure;

namespace OnlineLibrary.Services.Concrete
{
    public class BookService : IBookService
    {
        private ILibraryDbContext _dbContext;

        public BookService(ILibraryDbContext dbContext)
        {
            _dbContext = dbContext;
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
                _dbContext.SaveChanges();

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
                // Remove spaces and slashes
                ISBN = ISBN.Trim().Replace("-", string.Empty);
                ISBN = ISBN.Replace(Common.Infrastructure.LibraryConstants.stringSpace, string.Empty);
            }

            return ISBN;
        }

        public bool IsValidISBN(string ISBN)
        {
            ISBN = FormatISBN(ISBN);

            // ISBN is valid if it is unique or if it is the default unknown ISBN.
            return string.Equals(ISBN, Common.Infrastructure.LibraryConstants.undefinedISBN)
                    || !_dbContext.Books.Where(b => b.ISBN == ISBN).Any();

        }

        public IEnumerable<Book> Find(BookSearchServiceModel model)
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

            var foundBooks = books.ToList();

            return books;
        }
    }
}
