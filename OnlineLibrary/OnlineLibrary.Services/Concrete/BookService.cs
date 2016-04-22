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

namespace OnlineLibrary.Services.Concrete
{
    public class BookService : IBookService
    {
        private ApplicationDbContext _dbContext;

        public BookService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int GetAmountOfAvailableCopies(int bookId)
        {
            // Obtain book.
            Book book = _dbContext.Books
                .Include(b => b.BookCopies)
                .Single(b => b.Id == bookId);

            // Count the number of not available book copies.
            int notAvailableBookCopies = (from bc in book.BookCopies
                                          join l in _dbContext.Loans
                                          on bc.Id equals l.BookCopyId
                                          where l.Status == LoanStatus.Approved || l.Status == LoanStatus.Loaned
                                          select bc).Count();

            // Return difference between the total number of book copies
            // and not available ones.
            return book.BookCopies.Count() - notAvailableBookCopies;
        }

        public DateTime? GetEarliestAvailableDate(int bookId)
        {
            // Obtain book with book copies.
            Book book = _dbContext.Books
                                  .Include(b => b.BookCopies)
                                  .Single(b => b.Id == bookId);

            // Determine the loan with earliest return date.
            Loan earliestLoan = (from bc in book.BookCopies
                                 join l in _dbContext.Loans
                                 on bc.Id equals l.BookCopyId
                                 where l.ExpectedReturnDate != null
                                 orderby l.ExpectedReturnDate
                                 select l).FirstOrDefault();

            return earliestLoan?.ExpectedReturnDate;
        }
    }
}
