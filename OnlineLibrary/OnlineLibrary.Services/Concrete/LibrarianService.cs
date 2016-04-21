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

namespace OnlineLibrary.Services.Concrete
{
    public class LibrarianService : ILibrarianService
    {
        private ApplicationDbContext _dbContext;

        public LibrarianService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ApproveLoanRequest(int bookCopyId, int loanRequestId)
        {
            var loans = CreateLoans(bookCopyId, loanRequestId);
            SaveLoans(loans,loanRequestId);
        }

        public void RejectLoanRequest(int loanRequestId)
        {
            //var loans =
            //(from r in DbContext.LoanRequests
            // join b in DbContext.Books on r.BookId equals b.Id
            // join u in DbContext.Users on r.UserId equals u.Id
            // where r.Id == loanRequestId
            // select new
            // {
            //     bookId = r.BookId,
            //     UserId = u.Id,
            //     Status = LoanStatus.Rejected

            // }).ToList().Select(a => new Loan
            // {
            //     UserId = a.UserId,
            //     Status = a.Status,
            //   //  BookId = a.bookId
            // });

            //foreach (var loan in loans)
            //{
            //    DbContext.Loans.Add(loan);
            //}

            var request = _dbContext.LoanRequests.Find(loanRequestId);
            _dbContext.LoanRequests.Remove(request);

            _dbContext.SaveChanges();
        }

        public IQueryable<Loan> GetApprovedLoans()
        {
            var result =
            from l in _dbContext.Loans
            where l.Status == LoanStatus.Approved
            select l;

            return result.AsQueryable();
        }

        private void SaveLoans(IEnumerable loans, int loanRequestId)
        {
            foreach (Loan loan in loans)
            {
                _dbContext.Loans.Add(loan);
            }

            var request = _dbContext.LoanRequests.Find(loanRequestId);
            _dbContext.LoanRequests.Remove(request);

            _dbContext.SaveChanges();
        }

        private IEnumerable CreateLoans(int bookCopyId, int loanRequestId)
        {
            var loans =
            (from r in _dbContext.LoanRequests
             join b in _dbContext.Books on r.BookId equals b.Id
             join u in _dbContext.Users on r.UserId equals u.Id
             where r.Id == loanRequestId
             select new
             {
                 BookCopyId = bookCopyId,
                 UserId = u.Id,
                 Status = LoanStatus.Approved,
             }).ToList().Select(a => new Loan
             {
                 BookCopyId = a.BookCopyId,
                 UserId = a.UserId,
                 Status = a.Status
             });
            return loans;
        }

    }
}
