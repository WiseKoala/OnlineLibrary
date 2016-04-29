using Microsoft.AspNet.Identity;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.UserLoansViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;

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

            // Creating queries to fill view models objects.
            var userLoans = from l in DbContext.Loans
                            where l.UserId == userId
                            join bc in DbContext.BookCopies
                            on l.BookCopyId equals bc.Id
                            join b in DbContext.Books
                            on bc.BookId equals b.Id
                            orderby l.Status, l.ExpectedReturnDate
                            select new CurrentUserLoanViewModel() { Title = b.Title, ExpectedReturnDate = l.ExpectedReturnDate, Status = l.Status, BookPickUpLimitDate = l.BookPickUpLimitDate, BookId = b.Id };

            var historyUserLoans = from h in DbContext.History
                                   where h.UserName == DbContext.Users.Where(u => u.Id == userId).FirstOrDefault().UserName
                                   join bc in DbContext.BookCopies
                                   on h.BookCopyId equals bc.Id
                                   join b in DbContext.Books
                                   on bc.BookId equals b.Id
                                   join l in DbContext.Users
                                   on h.LibrarianUserName equals l.UserName
                                   orderby h.Status, h.ExpectedReturnDate
                                   select new HistoryUserLoanViewModel() { BookTitle = b.Title, Status = h.Status, StartDate = h.StartDate, ExpectedReturnDate = h.ExpectedReturnDate, ActualReturnDate = h.ActualReturnDate, InitialBookCondition = h.InitialBookCondition, FinalBookCondition = h.FinalBookCondition, LibrarianName = l.UserName };

            var userLoansWithNoBookCopy = from l in DbContext.Loans
                                          where l.UserId == userId && l.BookCopyId == null
                                          join b in DbContext.Books
                                          on l.BookId equals b.Id
                                          select new CurrentUserLoanViewModel() { BookId = b.Id, Status = l.Status, Title = b.Title };

            var pendingUserLoans = userLoansWithNoBookCopy.Where(l => l.Status == DataAccess.Enums.LoanStatus.Pending);
            var rejectedUserLoans = userLoansWithNoBookCopy.Where(l => l.Status == DataAccess.Enums.LoanStatus.Rejected);
            var returnedUserLoans = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.Completed);
            var lostUserBooks = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.LostBook);
            var approvedUserLoans = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.Approved);
            var currentUserLoans = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.InProgress);
            
            // Populating view model object.
            model.PendingLoans = pendingUserLoans;
            model.RejectedLoans = rejectedUserLoans;
            model.ReturnedLoans = returnedUserLoans;
            model.LostBooks = lostUserBooks;
            model.ApprovedLoans = approvedUserLoans;
            model.CurrentLoans = currentUserLoans;
            model.HistoryLoans = historyUserLoans;

            // Returning the view.
            return View(model);
        }

        [HttpPost]
        public ActionResult CancelApprovedLoan(int id)
        {
            var userId = User.Identity.GetUserId();

            var loan = DbContext.Loans.Where(l => l.UserId == userId && l.BookId == id && l.Status == DataAccess.Enums.LoanStatus.Approved).FirstOrDefault();

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
        public ActionResult HideRejectedLoanNotification(int id)
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
    }
}