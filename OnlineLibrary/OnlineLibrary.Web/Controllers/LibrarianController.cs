using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.LibrarianLoansViewModels;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Net;

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

        [Authorize(Roles = "Librarian, System administrator")]
        public ActionResult Index()
        {
            var model = new LibrarianIndexViewModel()
            {
                PendingStatusValue = (byte)LoanStatus.Pending,
                ApprovedStatusValue = (byte)LoanStatus.Approved,
                InProgressStatusValue = (byte)LoanStatus.InProgress
            };

            return View(model);
        }

        public JsonResult ListActive(LoanStatus status)
        {
            var model = DbContext.Loans
                                 .Include(l => l.Book)
                                 .Include(u => u.User)
                                 .Where(l => l.Status == status)
                                 .OrderBy(lr => lr.ApprovingDate)
                                 .ToList()
                                 .Select(l => new LoanViewModel
                                 {
                                     LoanId = l.Id,
                                     BookTitle = l.Book.Title,
                                     UserName = l.User.UserName,
                                     ApprovingDate = l.ApprovingDate.HasValue ? l.ApprovingDate.Value.ToShortDateString() : "unknown",
                                     IsApprovedLoanLate = l.BookPickUpLimitDate.HasValue && (l.BookPickUpLimitDate < DateTime.Now)
                                 });

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListHistory()
        {
            var allHistory = DbContext.History
                                      .ToList()
                                      // Perform projection here, because of the use of DateTime methods
                                      // which cannot be translated to SQL.
                                      .Select(h => new HistoryLoanViewModel
                                      {
                                          ISBN = h.ISBN,
                                          BookCopyId = h.BookCopyId,
                                          UserName = h.UserName,
                                          Status = h.Status,
                                          StartDate = h.StartDate.HasValue ? h.StartDate.Value.ToShortDateString() : "unknown",
                                          ExpectedReturnDate = h.ExpectedReturnDate.HasValue ? h.ExpectedReturnDate.Value.ToShortDateString() : "unknown",
                                          ActualReturnDate = h.ActualReturnDate.HasValue ? h.ActualReturnDate.Value.ToShortDateString() : "unknown"
                                      });

            // Fill the model object.
            var model = new HistoryLoansViewModel
            {
                Rejected = allHistory.Where(h => h.Status == HistoryStatus.Rejected),
                Completed = allHistory.Where(h => h.Status == HistoryStatus.Completed),
                LostBook = allHistory.Where(h => h.Status == HistoryStatus.LostBook),
                Cancelled = allHistory.Where(h => h.Status == HistoryStatus.Cancelled),
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApproveLoanRequest(int bookCopyId, int loanId)
        {
            try
            {
                int daysNumberForLateApprovedLoans;
                int.TryParse(ConfigurationManager.AppSettings["DaysNumberForLateApprovedLoans"], out daysNumberForLateApprovedLoans);
                _librarianService.ApproveLoanRequest(bookCopyId, loanId, daysNumberForLateApprovedLoans);

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
            try
            {
                var librarian = DbContext.Users.Where(u => u.UserName == User.Identity.Name).Single();
                _librarianService.RejectLoanRequest(loanId, librarian);
                return Json(new { success = 1 });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message });
            }
            catch
            {
                return Json(new { error = 1 });
            }
        }

        [HttpPost]
        public ActionResult PerformLoan(int loanId)
        {
            try
            {
                _librarianService.PerformLoan(loanId);
                return Json(new { success = 1 });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message });
            }
            catch
            {
                return Json(new { error = 1 });
            }
        }

        [HttpPost]
        public ActionResult ReturnBook(int loanId)
        {
            try
            {
                var librarian = DbContext.Users.Where(u => u.UserName == User.Identity.Name).Single();
                _librarianService.ReturnBook(loanId, librarian);
                return Json(new { success = 1 });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message });
            }
            catch
            {
                return Json(new { error = 1 });
            }
        }

        [HttpPost]
        public ActionResult LostBook(int loanId)
        {
            try
            {
                var librarian = DbContext.Users.Where(u => u.UserName == User.Identity.Name).Single();
                _librarianService.LostBook(loanId, librarian);
                return Json(new { success = 1 });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message });
            }
            //catch
            //{
            //    return Json(new { error = 1 });
            //}
        }

        [HttpPost]
        public ActionResult CancelApprovedLoan(int loanId)
        {
            try
            {
                var librarian = DbContext.Users.Where(u => u.UserName == User.Identity.Name).Single();
                _librarianService.CancelApprovedLoan(loanId, librarian);
                return Json(new { success = 1 });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message });
            }
            catch
            {
                return Json(new { error = 1 });
            }
        }
    }
}