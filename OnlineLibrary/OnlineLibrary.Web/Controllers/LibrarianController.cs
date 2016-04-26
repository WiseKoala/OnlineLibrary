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
using OnlineLibrary.DataAccess.Abstract;

namespace OnlineLibrary.Web.Controllers
{
    public class LibrarianController : BaseController
    {
        private ILibrarianService _librarianService;

        public LibrarianController(ILibraryDbContext dbContext, ILibrarianService librarianService)
            : base(dbContext)
        {
            _librarianService = librarianService;
        }

        [Authorize(Roles = "Librarian, System administrator, Super administrator")]
        public ActionResult Index()
        {
            var model = new LoansViewModel();

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

            model.PendingLoans = loans.Where(l => l.Status == LoanStatus.Pending);
            model.ApprovedLoans = loans.Where(l => l.Status == LoanStatus.Approved);
            model.LoanedBooks = loans.Where(l => l.Status == LoanStatus.InProgress);
            model.RejectedLoans = loans.Where(l => l.Status == LoanStatus.Rejected);
            model.ReturnedBooks = loans.Where(l => l.Status == LoanStatus.Completed);
            model.LostBooks = loans.Where(l => l.Status == LoanStatus.LostBook);

            return View(model);
        }

        [HttpPost]
        public ActionResult ApproveLoanRequest(int bookCopyId, int loanId)
        {
            bool result = _librarianService.TryApproveLoanRequest(bookCopyId, loanId);

            if (result)
            {
                return RedirectToActionPermanent("Index");
            }
            else
            {
                return Json(new { error = "BookCopyId doesn't correspond to the BookId" }, 
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RejectLoanRequest(int loanId)
        {
            _librarianService.RejectLoanRequest(loanId);
            return RedirectToActionPermanent("Index");
        }
        
        [HttpPost]
        public ActionResult PerformLoan(int loanId)
        {
            _librarianService.PerformLoan(loanId);
            return RedirectToActionPermanent("Index");
        }

        [HttpPost]
        public ActionResult ReturnBook(int loanId)
        {
            Loan loan = DbContext.Loans.Find(loanId);
            loan.Status = LoanStatus.Completed;
            DbContext.SaveChanges();

            return RedirectToActionPermanent("Index");
        }

        [HttpPost]
        public ActionResult LostBook(int loanId)
        {
            Loan loan = DbContext.Loans.Find(loanId);
            loan.Status = LoanStatus.LostBook;
            DbContext.SaveChanges();

            return RedirectToActionPermanent("Index");
        }
    }
}