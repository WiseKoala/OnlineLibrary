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
    }
}
