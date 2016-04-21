using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;
using System.Data.Entity;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Concrete;
using Microsoft.AspNet.Identity;

namespace OnlineLibrary.Web.Controllers
{
    public class BookDetailsController : BaseController
    {
        private IBookService _bookService;

        public ActionResult Index(int id)
        {
            if (!IsUserNameSessionVariableSet())
            {
                InitializeUserNameSessionVariable();
            }

            var book = DbContext.Books.Include(b => b.BookCopies).First(b => b.Id == id);
           // var bookcopies = new List<BookCopy>();
            int[] condition =
            {
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.New).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Fine).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.VeryGood).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Good).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Fair).Count()),
                (DbContext.BookCopies.Where(n => n.BookId == id && n.Condition == DataAccess.Enums.BookCondition.Poor).Count())
            };

            string[] arr = Enumerable.Repeat(string.Empty, Enum.GetValues(typeof(BookCondition)).Length).ToArray();
            string conditionStr = string.Empty;

            if (condition[0] != 0) { arr[0] = condition[0] + " New"; };
            if (condition[1] != 0) { arr[1] = condition[1] + " Fine"; };
            if (condition[2] != 0) { arr[2] = condition[2] + " Very Good"; };
            if (condition[3] != 0) { arr[3] = condition[3] + " Good"; };
            if (condition[4] != 0) { arr[4] = condition[4] + " Fair"; };
            if (condition[5] != 0) { arr[5] = condition[5] + " Poor"; };

            foreach (var s in arr)
            { if (s.Length != 0) { conditionStr = conditionStr + s + ", "; } };
           if (conditionStr.Length > 3) { conditionStr = conditionStr.Substring(0, conditionStr.Length - 2); };

            // Manually instantiate the service.
            _bookService = new BookService(DbContext);

            var book_view = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                PublishDate = book.PublishDate,
                FrontCover = book.FrontCover,
                Authors = book.Authors.Select(a =>
                        string.Join(" ", a.FirstName, (a.MiddleName ?? ""), a.LastName)),
                Description = book.Description,
                ISBN = book.ISBN,
                NrOfBooks = DbContext.BookCopies.Count(n => n.BookId == id),
                HowManyInThisCondition = conditionStr,
                Categories = book.SubCategories.Select(sc => new CategoryViewModel
                {
                    Category = sc.Category.Name,
                    SubCategory = sc.Name
                }),
                AvailableCopies = _bookService.GetAmountOfAvailableCopies(id),
                EarliestDateAvailable = _bookService.GetEarliestAvailableDate(id)
            };
            return View(book_view);
        }

        public JsonResult CreateLoanRequest(int id)
        {
            try
            {
                var _bookService = new BookService(DbContext);
                var userId = User.Identity.GetUserId();
                var AvailableCopies = _bookService.GetAmountOfAvailableCopies(id);
                var userLoanRequestsNumber = DbContext.LoanRequests.Where(lr => lr.UserId == userId && lr.BookId == id).Count();

                if (userLoanRequestsNumber > AvailableCopies)
                {
                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
                }

                var book = DbContext.Books.Where(b => b.Id == id).Single();

                var loanRequest = new LoanRequest();

                loanRequest.BookId = book.Id;
                loanRequest.UserId = User.Identity.GetUserId();
                DbContext.LoanRequests.Add(loanRequest);
                DbContext.SaveChanges();
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
