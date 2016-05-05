using Microsoft.AspNet.Identity;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.UserLoansViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize]
    public class UserLoansController : BaseController
    {
        public UserLoansController(ILibraryDbContext dbContext)
            : base (dbContext)
        {

        }

        public ActionResult MyLoans()
        {
            // Initializing view model objects.
            var model = new UserLoansViewModel();

            // Storing user id.
            string userId = User.Identity.GetUserId();

            // Create queries to fill view models objects.
            var loans = (from l in DbContext.Loans
                         where l.UserId == userId
                         join bc in DbContext.BookCopies
                         on l.BookCopyId equals bc.Id
                         join b in DbContext.Books
                         on bc.BookId equals b.Id
                         orderby l.Status, l.ExpectedReturnDate
                         select new CurrentUserLoanViewModel()
                         {
                             Title = b.Title,
                             ExpectedReturnDate = l.ExpectedReturnDate,
                             Status = l.Status,
                             BookPickUpLimitDate = l.BookPickUpLimitDate,
                             BookId = b.Id
                         })
                         .AsEnumerable();

            var loansWithNoBookCopy = (from l in DbContext.Loans
                                       where l.UserId == userId && l.BookCopyId == null
                                       join b in DbContext.Books
                                       on l.BookId equals b.Id
                                       select new CurrentUserLoanViewModel()
                                       {
                                           BookId = b.Id,
                                           Status = l.Status,
                                           Title = b.Title
                                       })
                                       .AsEnumerable();

            var historyLoans = (from h in DbContext.History
                                where h.UserName == DbContext.Users.Where(u => u.Id == userId).FirstOrDefault().UserName
                                join bc in DbContext.BookCopies
                                on h.BookCopyId equals bc.Id
                                join b in DbContext.Books
                                on bc.BookId equals b.Id
                                join l in DbContext.Users
                                on h.LibrarianUserName equals l.UserName
                                orderby h.Status, h.ExpectedReturnDate
                                select new HistoryUserLoanViewModel()
                                {
                                    HistoryLoanId = h.Id,
                                    BookTitle = b.Title,
                                    Status = h.Status,
                                    StartDate = h.StartDate,
                                    ExpectedReturnDate = h.ExpectedReturnDate,
                                    ActualReturnDate = h.ActualReturnDate,
                                    InitialBookCondition = h.InitialBookCondition,
                                    FinalBookCondition = h.FinalBookCondition,
                                    LibrarianName = l.UserName
                                })
                                .AsEnumerable();

            var pendingLoans = loansWithNoBookCopy.Where(l => l.Status == LoanStatus.Pending);
            var approvedLoans = loans.Where(ul => ul.Status == LoanStatus.Approved);
            var currentLoans = loans.Where(ul => ul.Status == LoanStatus.InProgress);

            var rejectedLoans = historyLoans.Where(l => l.Status == HistoryStatus.Rejected);
            var returnedLoans = historyLoans.Where(ul => ul.Status == HistoryStatus.Completed);
            var lostBookLoans = historyLoans.Where(ul => ul.Status == HistoryStatus.LostBook);
            var cancelledLoans = historyLoans.Where(l => l.Status == HistoryStatus.Cancelled);

            // Populate view model object.
            model.PendingLoans = pendingLoans;
            model.ApprovedLoans = approvedLoans;
            model.InProgressLoans = currentLoans;

            model.RejectedLoans = rejectedLoans;
            model.CompletedLoans = returnedLoans;
            model.LostBookLoans = lostBookLoans;
            model.CancelledLoans = cancelledLoans;

            return View(model);
        }

        [HttpPost]
        public ActionResult CancelApprovedLoan(int id)
        {
            var userId = User.Identity.GetUserId();

            var loan = DbContext.Loans.Where(l => l.UserId == userId && l.BookId == id && l.Status == LoanStatus.Approved).FirstOrDefault();

            if (loan != null)
            {
                DbContext.Loans.Remove(loan);
                DbContext.SaveChanges();

                return RedirectToAction("MyLoans");
            }
            else
                return View("Error");
        }

        [HttpPost]
        public ActionResult CancelPendingLoan(int id)
        {
            var userId = User.Identity.GetUserId();

            var loan = DbContext.Loans.Where(l => l.UserId == userId && l.BookId == id).FirstOrDefault();

            if (loan != null)
            {
                DbContext.Loans.Remove(loan);
                DbContext.SaveChanges();

                return RedirectToAction("MyLoans");
            }
            else
                return View("Error");
        }

        [HttpPost]
        public ActionResult HideRejectedLoanNotification(int loanId)
        {
            var userId = User.Identity.GetUserId();

            var loan = DbContext.Loans.Where(l => l.Id == loanId && l.UserId == userId).FirstOrDefault();

            if (loan != null)
            {
                DbContext.Loans.Remove(loan);
                DbContext.SaveChanges();

                return RedirectToAction("MyLoans");
            }
            else
                return View("Error");
        }
    }
}