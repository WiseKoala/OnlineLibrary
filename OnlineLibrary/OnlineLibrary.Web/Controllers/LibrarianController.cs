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
using OnlineLibrary.Common.Exceptions;

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
            return View();
        }

        public JsonResult ListActive(LoanStatus status)
        {
            var model = DbContext.Loans
                                 .Include(l => l.Book)
                                 .Include(u => u.User)
                                 .Where(l => l.Status == status)
                                 .Select(l => new LoanViewModel
                                 {
                                     LoanId = l.Id,
                                     BookTitle = l.Book.Title,
                                     UserName = l.User.UserName
                                 })
                                 .ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApproveLoanRequest(int bookCopyId, int loanId)
        {
            try
            {
                _librarianService.ApproveLoanRequest(bookCopyId, loanId);

                return Json(new { success = "Loan approved!" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (InvalidBookCopyIdException)
            {
                return Json(new { error = "BookCopyId doesn't correspond to the BookId" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (BookCopyNotAvailableException)
            {
                return Json(new { error = "This book copy is not available for loan" },
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