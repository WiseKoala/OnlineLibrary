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