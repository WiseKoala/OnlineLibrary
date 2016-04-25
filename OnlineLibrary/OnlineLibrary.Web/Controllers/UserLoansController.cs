using Microsoft.AspNet.Identity;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.UserLoansViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;

namespace OnlineLibrary.Web.Controllers
{
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
                            select new CurrentUserLoansViewModel() { Title = b.Title, ExpectedReturnDate = l.ExpectedReturnDate, Status = l.Status, BookId = b.Id };

            
            var userLoanRequests = from lr in DbContext.LoanRequests
                                   where lr.UserId == userId
                                   join b in DbContext.Books
                                   on lr.BookId equals b.Id
                                   select new PendingUserLoansViewModel() { BookTitle = b.Title, BookId = b.Id };

            var rejectedUserLoans = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.Rejected);
            var returnedUserLoans = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.Returned);
            var lostUserBooks = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.Lost);
            var approvedUserLoans = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.Approved);
            var currentUserLoans = userLoans.Where(ul => ul.Status == DataAccess.Enums.LoanStatus.Loaned);
            
            // Populating view model object.
            model.PendingLoans = userLoanRequests;
            model.RejectedLoans = rejectedUserLoans;
            model.ReturnedLoans = returnedUserLoans;
            model.LostBooks = lostUserBooks;
            model.ApprovedLoans = approvedUserLoans;
            model.CurrentLoans = currentUserLoans;

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

            var loan = DbContext.LoanRequests.Where(l => l.UserId == userId && l.BookId == id).FirstOrDefault();

            if (loan != null)
            {
                DbContext.LoanRequests.Remove(loan);
                DbContext.SaveChanges();

                return RedirectToAction("MyLoans");
            }
            else
                return View("Error");
        }
    }
}