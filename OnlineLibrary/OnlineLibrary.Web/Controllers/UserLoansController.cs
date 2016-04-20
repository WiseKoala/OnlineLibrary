using Microsoft.AspNet.Identity;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.UserLoansViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class UserLoansController : BaseController
    {
        public ActionResult MyLoans()
        {
            // Initializing view model objects.
            var model = new UserLoansViewModel();
            var currentLoansList = new List<CurrentUserLoansViewModel>();
            var pendingLoansList = new List<PendingUserLoansViewModel>();

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
                            select new CurrentUserLoansViewModel() { Title = b.Title, ExpectedReturnDate = l.ExpectedReturnDate, Status = l.Status };

            
            var userLoanRequests = from lr in DbContext.LoanRequests
                                   where lr.UserId == userId
                                   join b in DbContext.Books
                                   on lr.BookId equals b.Id
                                   select new PendingUserLoansViewModel() { BookTitle = b.Title };


            // Populating view model objects.
            foreach (var item in userLoans)
            {
                currentLoansList.Add(item);
            }

            foreach (var item in userLoanRequests)
            {
                pendingLoansList.Add(item);
            }

            model.CurrentLoans = userLoans.ToList();
            model.PendingLoans = userLoanRequests.ToList();

            // Returning the view.
            return View(model);
        }
    }
}