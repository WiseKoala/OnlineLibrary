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
                                    LibrarianName = l.UserName
                                })
                                .ToList();

            // Populate the view model object.
            var model = new UserLoansViewModel();
            // Active loans.
            model.PendingLoans = allLoans.Where(l => l.Status == LoanStatus.Pending);
            model.ApprovedLoans = allLoans.Where(ul => ul.Status == LoanStatus.Approved);
            model.InProgressLoans = allLoans.Where(ul => ul.Status == LoanStatus.InProgress);
            // History loans.
            model.RejectedLoans = historyLoans.Where(l => l.Status == HistoryStatus.Rejected);
            model.CompletedLoans = historyLoans.Where(ul => ul.Status == HistoryStatus.Completed);
            model.LostBookLoans = historyLoans.Where(ul => ul.Status == HistoryStatus.LostBook);
            model.CancelledLoans = historyLoans.Where(l => l.Status == HistoryStatus.Cancelled);

            return View(model);
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