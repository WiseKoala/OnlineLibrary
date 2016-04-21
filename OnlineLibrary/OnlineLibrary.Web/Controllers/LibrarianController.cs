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

namespace OnlineLibrary.Web.Controllers
{
    public class LibrarianController : BaseController
    {
        private ILibrarianService _librarianService;

       // [Authorize(Roles = "Librarian")]
        public ActionResult Index()
        {
            _librarianService = new LibrarianService(DbContext);

            var result =
            from r in DbContext.LoanRequests
            join b in DbContext.Books on r.BookId equals b.Id
            join u in DbContext.Users on r.UserId equals u.Id
            select new LoanRequestViewModel
            {
                LoanRequestId = r.Id,
                BookTitle = b.Title,
                UserName = u.UserName
            };

            var loanRequestsViewModel = result.ToList();

            var approvedLoans = _librarianService.GetApprovedLoans()
                .Include(l => l.User)
                .Include(l => l.Book)
                .ToList();
            var approvedLoansViewModel = new List<ApprovedLoanViewModel>();

            foreach (var approvedLoan in approvedLoans)
            {
                approvedLoansViewModel.Add(new ApprovedLoanViewModel
                {
                    BookTitle = approvedLoan.Book.Title,
                    UserName = approvedLoan.User.UserName
                });
            }

            var loanViewModel = new LoansViewModel
            {
                LoansRequestViewModels = loanRequestsViewModel,
                ApprovedLoansViewModels = approvedLoansViewModel
            };

            return View(loanRequestsViewModel);
        }

        [HttpPost]
        public ActionResult ApproveLoanRequest(int bookCopyId,int loanRequestId)
        {
            _librarianService = new LibrarianService(DbContext);
            _librarianService.ApproveLoanRequest(bookCopyId, loanRequestId);

            return RedirectToActionPermanent("Index");
        }

        [HttpPost]
        public ActionResult RejectLoanRequest( int loanRequestId )
        {
            _librarianService = new LibrarianService(DbContext);
            _librarianService.RejectLoanRequest(loanRequestId);
            return RedirectToActionPermanent("Index");
        }
            
    }
}