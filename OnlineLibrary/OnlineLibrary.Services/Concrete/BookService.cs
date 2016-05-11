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
            // Determine not available book copies.
            var notAvailableBookCopies = from b in _dbContext.Books
                                         join bc in _dbContext.BookCopies
                                         on b.Id equals bc.BookId
                                         join l in _dbContext.Loans
                                         on bc.Id equals l.BookCopyId
                                         where b.Id == bookId 
                                            && (l.Status == LoanStatus.Approved || l.Status == LoanStatus.InProgress)
                                         select bc;

            // Obtain all book copies for the specified book.
            var allBookCopies = from b in _dbContext.Books
                                join bc in _dbContext.BookCopies
                                on b.Id equals bc.BookId
                                where b.Id == bookId
                                select bc;

            // Calculate the difference between the total number of book copies
            // and not available ones.
            int numberOfAvailableBookCopies = allBookCopies
                .Except(notAvailableBookCopies)
                .Count();

            return numberOfAvailableBookCopies;
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
            // The book copy is removable if there are no loans with it's ID.
            return !_dbContext.Loans.Any(l => l.BookCopyId == id);
        }

        public BookCopy DeleteBookCopy(int id)
        {
            // Determine if there're any loans for the specified book copy.
            bool isUnavailable = IsBookCopyRemovable(id);

            if (isUnavailable)
            {
                throw new BookCopyNotAvailableException("The specified book copy is unavailable for removal.");
            }
            else
            {
                var bookCopy = _dbContext.BookCopies.Find(id);
                _dbContext.BookCopies.Remove(bookCopy);

                return bookCopy;
            }
        }

        public Book DeleteBook(int id)
        {
            var book = _dbContext.Books.Include(b => b.BookCopies).Where(b => b.Id == id).SingleOrDefault();

            if (book != null)
            {
                foreach (var bookCopy in book.BookCopies)
                {
                    // If one of the book copies is unavailable for removal
                    // then the book book becomes unavailable for removal.
                    if (!IsBookCopyRemovable(bookCopy.Id))
                    {
                        throw new BookNotAvailableException("The specified book has book copies that are currently involved in loans.");
                    }
                }

                _dbContext.Books.Remove(book);
            }

            return book;
        }

    }
}
