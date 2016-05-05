using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using System;
using System.Data.Entity;
using System.Linq;

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
            var loanToApprove = _dbContext.Loans.Find(loanId);

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

        public void RejectLoanRequest(int loanId, User librarian)
        {
            // Find loan by ID.
            var loanToReject = _dbContext.Loans
                                        .Include(l => l.User)
                                        .Include(l => l.Book)
                                        .Where(l => l.Id == loanId)
                                        .SingleOrDefault();
            if (loanToReject != null)
            {
                // Set status to Rejected.
                loanToReject.Status = LoanStatus.Rejected;

                // Add rejeted loan to history.
                var historyLoan = new History
                {
                    ISBN = loanToReject.Book.ISBN,
                    LibrarianUserName = librarian.UserName,
                    Status = HistoryStatus.Rejected,
                    UserName = loanToReject.User.UserName
                };

                _dbContext.History.Add(historyLoan);

                _dbContext.SaveChanges();
            }
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

        public void CancelApprovedLoan(int loanId, User librarian)
        {
            // Find the needed loan.
            var loan = _dbContext.Loans
                                .Include(l => l.Book)
                                .Include(l => l.User)
                                .Where(l => l.Id == loanId).SingleOrDefault();

            if (loan != null)
            {
                // Create History Loan.
                var historyLoan = new History
                {
                    ISBN = loan.Book.ISBN,
                    BookCopyId = loan.BookCopyId,
                    LibrarianUserName = librarian.UserName,
                    UserName = loan.User.UserName,
                    Status = HistoryStatus.Cancelled
                };

                // Add history loan to History.
                _dbContext.History.Add(historyLoan);

                // Delete the loan from Loans.
                var loanToRemove = _dbContext.Loans.Where(l => l.Id == loanId).Single();
                _dbContext.Loans.Remove(loanToRemove);

                _dbContext.SaveChanges();
            }
        }

        public void ReturnBook(int loanId, User librarian /*, BookCondition finalBookCondition*/)
        {
            var loan = _dbContext.Loans
                                .Include(l => l.Book)
                                .Include(l => l.User)
                                .Include(l => l.BookCopy)
                                .Where(l => l.Id == loanId)
                                .SingleOrDefault();

            //var bookCopy = _dbContext.BookCopies.Find(loan.BookCopy);
            //if (bookCopy != null)
            //{
            //    bookCopy.Condition = finalBookCondition;
            //}

            var historyLoan = new History
            {
                UserName = loan.User.UserName,
                ISBN = loan.Book.ISBN,
                BookCopyId = loan.BookCopyId,
                StartDate = loan.StartDate,
                ActualReturnDate = DateTime.Now,
                ExpectedReturnDate = loan.ExpectedReturnDate,
                //FinalBookCondition = finalBookCondition,
                InitialBookCondition = loan.BookCopy.Condition,
                LibrarianUserName = librarian.UserName,
                Status = HistoryStatus.Completed
            };

            _dbContext.History.Add(historyLoan);

            _dbContext.Loans.Remove(loan);

            _dbContext.SaveChanges();
        }

        public void LostBook(int loanId, User librarian)
        {
            var loan = _dbContext.Loans
                                .Include(l => l.Book)
                                .Include(l => l.User)
                                .Include(l => l.BookCopy)
                                .Where(l => l.Id == loanId)
                                .SingleOrDefault();

            var historyLoan = new History
            {
                UserName = loan.User.UserName,
                ISBN = loan.Book.ISBN,
                BookCopyId = loan.BookCopyId,
                StartDate = loan.StartDate,
                ActualReturnDate = DateTime.Now,
                ExpectedReturnDate = loan.ExpectedReturnDate,
                InitialBookCondition = loan.BookCopy.Condition,
                LibrarianUserName = librarian.UserName,
                Status = HistoryStatus.LostBook
            };

            _dbContext.History.Add(historyLoan);

            _dbContext.Loans.Remove(loan);

            _dbContext.SaveChanges();
        }
    }
}