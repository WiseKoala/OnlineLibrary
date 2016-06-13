using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;
using OnlineLibrary.Web.Models.HomeViewModels;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;

namespace OnlineLibrary.Web.Controllers
{
    public class BookDetailsController : BaseController
    {
        private IBookService _bookService;

        public BookDetailsController(ILibraryDbContext dbContext, IBookService bookService)
            : base(dbContext)
        {
            _bookService = bookService;
        }

        public ActionResult Index(int id)
        {
            var book = DbContext.Books.Include(b => b.BookCopies).First(b => b.Id == id);

            var query = book.BookCopies;
            string conditionStr = "None";
            if (query.Any())
            {
                conditionStr = query.GroupBy(e => e.Condition)
                   .OrderBy(e => e.Key)
                   .Select(e => string.Concat(e.Count(), " ", _bookService.GetConditionDescription(e.Key))).ToList()
                   .Aggregate((current, next) => current + ", " + next);
            }
                   

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
                Categories = book.SubCategories.Select(sc => new Models.CategoryViewModel
                {
                    Category = sc.Category.Name,
                    SubCategory = sc.Name
                }),
                AvailableCopies = _bookService.GetNumberOfAvailableCopies(id),
                EarliestDateAvailable = _bookService.GetEarliestAvailableDate(id)
            };
            return View(book_view);
        }

        public JsonResult CreateLoanRequest(int id)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var AvailableCopies = _bookService.GetNumberOfAvailableCopies(id);
                var userLoanRequestsNumber = DbContext.Loans.Where(lr => lr.UserId == userId && lr.BookId == id && lr.Status == LoanStatus.Pending).Count();

                if (userLoanRequestsNumber >= AvailableCopies)
                {
                    return Json(new { error = "no more copies" }, JsonRequestBehavior.AllowGet);
                }

                var book = DbContext.Books.Where(b => b.Id == id).Single();

                var loan = new Loan();

                loan.BookId = book.Id;
                loan.UserId = User.Identity.GetUserId();
                DbContext.Loans.Add(loan);
                DbContext.SaveChanges();
                return Json(new { id = id, success = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}