using System;
using System.Data.Entity;
using System.Linq;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;

namespace OnlineLibrary.Services.Concrete
{
    public class LibrarianService : ILibrarianService
    {
        private ILibraryDbContext _dbContext;

        public LibrarianService(ILibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool TryApproveLoanRequest(int bookCopyId, int loanId)
        {
            // Find loan by ID.
            Loan loanToApprove = _dbContext.Loans.Find(loanId);

            // Check if ID of BookCopy corresponds to the ID of book.
            bool bookCopyIdCorresponds = _dbContext.Books
                .Include(b => b.BookCopies)
                .Single(b => b.Id == loanToApprove.BookId)
                .BookCopies
                .Any(bc => bc.Id == bookCopyId);

            if (bookCopyIdCorresponds)
            {
                // Set status to Approved.
                loanToApprove.Status = LoanStatus.Approved;

                _dbContext.SaveChanges();
            }

            return bookCopyIdCorresponds;
        }

        public void RejectLoanRequest(int loanId)
        {
            // Find loan by ID.
            Loan loanToReject = _dbContext.Loans.Find(loanId);

            // Set status to Rejected.
            loanToReject.Status = LoanStatus.Rejected;

            _dbContext.SaveChanges();
        }

        public void PerformLoan(int loanId)
        {
            // Find loan by ID.
            Loan loan = _dbContext.Loans.Find(loanId);

            // Set status, start date and expected return date.
            loan.Status = LoanStatus.InProgress;
            loan.StartDate = DateTime.Today;
            loan.ExpectedReturnDate = DateTime.Today.AddDays(14);

            _dbContext.SaveChanges();
        }
    }
}