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
            // Extract user ID.
            string userId = User.Identity.GetUserId();

            // Retreive active loans.
            var allLoans = (from l in DbContext.Loans
                            join b in DbContext.Books
                            on l.BookId equals b.Id
                            where l.UserId == userId
                            select new CurrentUserLoanViewModel()
                            {
                                Title = b.Title,
                                ExpectedReturnDate = l.ExpectedReturnDate,
                                Status = l.Status,
                                BookPickUpLimitDate = l.BookPickUpLimitDate,
                                BookId = b.Id,
                            })
                            .ToList();

            // Retreive history loans.
            var historyLoans = (from h in DbContext.History
                                join b in DbContext.Books
                                on h.ISBN equals b.ISBN
                                join l in DbContext.Users
                                on h.LibrarianUserName equals l.UserName
                                where h.UserName == DbContext.Users.Where(u => u.Id == userId).FirstOrDefault().UserName
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
                                    LibrarianName = l.UserName,
                                    SeenByUser = h.SeenByUser
                                })
                                .ToList();

            // Populate the view model object.
            var model = new UserLoansViewModel();
            // Active loans.
            model.PendingLoans = allLoans.Where(l => l.Status == LoanStatus.Pending);
            model.ApprovedLoans = allLoans.Where(l => l.Status == LoanStatus.Approved);
            model.InProgressLoans = allLoans.Where(l => l.Status == LoanStatus.InProgress);
            // History loans.
            model.NotSeenRejectedLoans = historyLoans.Where(h => h.Status == HistoryStatus.Rejected && h.SeenByUser == false);
            model.AllRejectedLoans = historyLoans.Where(h => h.Status == HistoryStatus.Rejected);
            model.CompletedLoans = historyLoans.Where(h => h.Status == HistoryStatus.Completed);
            model.LostBookLoans = historyLoans.Where(h => h.Status == HistoryStatus.LostBook);
            model.CancelledLoans = historyLoans.Where(h => h.Status == HistoryStatus.Cancelled);

            return View(model);
        }

        [HttpPost]
        public ActionResult HideRejectedLoanNotification(int loanId)
        {
            string userName = User.Identity.GetUserName();

            var historyEntry = DbContext.History
                .Where(h => h.Id == loanId && h.UserName == userName && h.Status == HistoryStatus.Rejected)
                .FirstOrDefault();

            if (historyEntry != null)
            {
                historyEntry.SeenByUser = true;
                DbContext.SaveChanges();

                return RedirectToAction("MyLoans");
            }
            else
            {
                return View("Error");
            }
        }
    }
}