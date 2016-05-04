using System;
using System.Data.Entity;
using System.Linq;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using System.Web;

namespace OnlineLibrary.Services.Concrete
{
    public class LibrarianService : ILibrarianService
    {
        private ILibraryDbContext _dbContext;

        public LibrarianService(ILibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ApproveLoanRequest(int bookCopyId, int loanId, int daysNumberForLateApprovedLoans)
        {
            // Find loan by ID.
            Loan loanToApprove = _dbContext.Loans.Find(loanId);

            // Check if ID of BookCopy corresponds to the ID of book.
            bool bookCopyIdCorresponds = _dbContext.Books
                .Include(b => b.BookCopies)
                .Single(b => b.Id == loanToApprove.BookId)
                .BookCopies
                .Any(bc => bc.Id == bookCopyId);

            // Check if the book copy is available for loan.
            bool isNotAvaialble = 
                _dbContext.Loans
                .Where(l => l.BookCopyId == bookCopyId
                    && (l.Status == LoanStatus.Approved || l.Status == LoanStatus.InProgress))
                .Any();

            if (isNotAvaialble)
            {
                throw new BookCopyNotAvailableException();
            }

            if (bookCopyIdCorresponds)
            {
                // Set status to Approved.
                loanToApprove.BookCopyId = bookCopyId;
                loanToApprove.Status = LoanStatus.Approved;
                loanToApprove.BookPickUpLimitDate = DateTime.Now.AddDays(daysNumberForLateApprovedLoans);
                loanToApprove.ApprovingDate = DateTime.Now;

                _dbContext.SaveChanges();
            }
            else
            {
                throw new InvalidBookCopyIdException();
            }
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