using OnlineLibrary.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.DataAccess.Abstract;

namespace OnlineLibrary.Services.Concrete
{
    public class LibrarianService : ILibrarianService
    {
        private ILibraryDbContext _dbContext;

        public LibrarianService(ILibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ApproveLoanRequest(int bookCopyId, int loanRequestId)
        {
            var approvedLoan = CreateLoan(bookCopyId, loanRequestId,LoanStatus.Approved);
            SaveLoan(approvedLoan,loanRequestId);
        }

        public void RejectLoanRequest(int loanRequestId)
        {
            var rejectLoan = CreateLoan(null, loanRequestId, LoanStatus.Rejected);
            SaveLoan(rejectLoan, loanRequestId);
        }

        private Loan CreateLoan(int? bookCopyId, int loanRequestId, LoanStatus loanStatus)
        {
            var loan =
            (from r in _dbContext.LoanRequests
             join b in _dbContext.Books on r.BookId equals b.Id
             join u in _dbContext.Users on r.UserId equals u.Id
             where r.Id == loanRequestId
             select new
             {
                 BookCopyId = bookCopyId,
                 bookId = b.Id,
                 UserId = u.Id,
                 Status = loanStatus,
             })
             .ToList()
             .Select(a => new Loan
             {
                 BookId = a.bookId,
                 BookCopyId = a.BookCopyId,
                 UserId = a.UserId,
                 Status = a.Status
             })
             .SingleOrDefault();

            return loan;
        }

        private void SaveLoan(Loan loan, int loanRequestId)
        {
            _dbContext.Loans.Add(loan);

            var request = _dbContext.LoanRequests.Find(loanRequestId);
            _dbContext.LoanRequests.Remove(request);

            _dbContext.SaveChanges();
        }

        public void PerformLoan(int loanId)
        {
            Loan loan = _dbContext.Loans.Find(loanId);
            loan.Status = LoanStatus.InProgress;
            loan.StartDate = DateTime.Today;
            loan.ExpectedReturnDate = DateTime.Today.AddDays(14);
            _dbContext.SaveChanges();
        }
    }
}
