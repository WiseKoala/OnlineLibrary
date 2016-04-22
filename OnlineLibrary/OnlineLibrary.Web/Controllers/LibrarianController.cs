using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Concrete;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using OnlineLibrary.Web.Models.LibrarianLoansViewModels;

namespace OnlineLibrary.Web.Controllers
{
    public class LibrarianController : BaseController
    {
        private ILibrarianService _librarianService;

        [Authorize(Roles = "Librarian, System administrator, Super administrator")]
        public ActionResult Index()
        {
            // CR: should be injected
            _librarianService = new LibrarianService(DbContext);
            var model = new LoansViewModel();

            // Obtain loan requests.
            var loanRequests = DbContext.LoanRequests
                 .Include(lr => lr.User)
                 .Include(lr => lr.Book)
                 .Select(lr => new LoanViewModel
                 {
                     LoanId = lr.Id,
                     BookTitle = lr.Book.Title,
                     UserName = lr.User.UserName
                 })
                 .ToList();

            // Obtain loans.
            var loans = DbContext.Loans
                 .Include(lr => lr.User)
                 .Include(lr => lr.Book)
                 .Select(lr => new LoanViewModel
                 {
                     LoanId = lr.Id,
                     BookTitle = lr.Book.Title,
                     UserName = lr.User.UserName,
                     Status = lr.Status
                 })
                 .ToList();

            model.PendingLoans = loanRequests;
            model.ApprovedLoans = loans.Where(l => l.Status == LoanStatus.Approved);
            model.LoanedBooks = loans.Where(l => l.Status == LoanStatus.Loaned);
            model.RejectedLoans = loans.Where(l => l.Status == LoanStatus.Rejected);
            model.ReturnedBooks = loans.Where(l => l.Status == LoanStatus.Returned);
            model.LostBooks = loans.Where(l => l.Status == LoanStatus.Lost);

            return View(model);
        }

        [HttpPost]
        public ActionResult ApproveLoanRequest(int bookCopyId,int loanRequestId)
        {
            _librarianService = new LibrarianService(DbContext);
            _librarianService.ApproveLoanRequest(bookCopyId, loanRequestId);

            return RedirectToActionPermanent("Index");
        }

        [HttpPost]
        public ActionResult RejectLoanRequest( int loanRequestId )
        {
            _librarianService = new LibrarianService(DbContext);
            _librarianService.RejectLoanRequest(loanRequestId);
            return RedirectToActionPermanent("Index");
        }
        
        [HttpPost]
        public ActionResult PerformLoan(int loanId)
        {
            _librarianService = new LibrarianService(DbContext);
            _librarianService.PerformLoan(loanId);
            return RedirectToActionPermanent("Index");
        }

        [HttpPost]
        public ActionResult ReturnBook(int loanId)
        {
            //CR: any change in the domain should go through Service layer
            Loan loan = DbContext.Loans.Find(loanId);
            loan.Status = LoanStatus.Returned;
            DbContext.SaveChanges();

            return RedirectToActionPermanent("Index");
        }

        [HttpPost]
        public ActionResult LostBook(int loanId)
        {
            //CR: same as above
            Loan loan = DbContext.Loans.Find(loanId);
            loan.Status = LoanStatus.Lost;
            DbContext.SaveChanges();

            return RedirectToActionPermanent("Index");
        }
    }
}