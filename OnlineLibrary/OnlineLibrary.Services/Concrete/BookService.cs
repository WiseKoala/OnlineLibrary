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

        public int GetAmountOfAvailableCopies(int bookId)
        {
            // Count the number of not available book copies.
            int notAvailableBookCopies = (from b in _dbContext.Books
                                          join bc in _dbContext.BookCopies
                                          on b.Id equals bc.BookId
                                          join l in _dbContext.Loans
                                          on bc.Id equals l.BookCopyId
                                          where b.Id == bookId &&
                                          (l.Status == LoanStatus.Approved || l.Status == LoanStatus.InProgress)
                                          select bc).Count();

            // All book copies.
            int numberOfBookCopies = (from b in _dbContext.Books
                                      join bc in _dbContext.BookCopies
                                      on b.Id equals bc.BookId
                                      select bc).Count();

            // Return difference between the total number of book copies
            // and not available ones.
            return numberOfBookCopies - notAvailableBookCopies;
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
