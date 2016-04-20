using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Concrete;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class LibrarianController : BaseController
    {
        private ILibrarianService _librarianService;

        //[Authorize(Roles = "Librarian, SysAdmin, SuperAdmin")]
        public ActionResult Index()
        {
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

            return View(result.ToList());
        }

        [HttpPost]
        public ActionResult ApproveLoanRequest(string bookId,string loanRequestId)
        {
            return null;
        }

        [HttpPost]
        public ActionResult RejectLoanRequest( string loanRequestId )
        {
            return null;
        }
            
    }
}