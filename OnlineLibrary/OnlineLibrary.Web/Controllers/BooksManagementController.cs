using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.BooksManagement;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;

namespace OnlineLibrary.Web.Controllers
{
    public class BooksManagementController : BaseController
    {
        private IBookService _bookService;

        public BooksManagementController(ILibraryDbContext dbContext, IBookService bookService)
            : base(dbContext)
        {
            _bookService = bookService;
        }

        public ActionResult Index()
        {
            var books = DbContext.Books
                                .Select(b => new BookManagementViewModel
                                {
                                    Id = b.Id,
                                    FrontCover = b.FrontCover,
                                    ISBN = b.ISBN,
                                    PublishDate = b.PublishDate,
                                    Title = b.Title
                                })
                                .ToList();

            return View(books);
        }

        [HttpPost]
        public ActionResult DeleteBookCopy(int id)
        {
            BookCopy removedBookCopy = null;
            try
            {
                removedBookCopy = _bookService.DeleteBookCopy(id);
            }
            catch (BookCopyNotAvailableException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message });
            }

            return Json(removedBookCopy, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult CreateEdit(int id = 2)
        {
            var model = DbContext.Books.Where(b => b.Id == id)
                                       .Include(b => b.SubCategories)
                                       .Include(b => b.BookCopies)
                                       .Select(m => new CreateEditBookViewModel
                                       {
                                           Id = m.Id,
                                           Title = m.Title,
                                           ISBN = m.ISBN,
                                           Description = m.Description,
                                           PublishDate = m.PublishDate,
                                           FrontCover = m.FrontCover,
                                           BookCopies = m.BookCopies.Select(bc => new BookCopyViewModel
                                           {
                                               Id = bc.Id,
                                               BookCondition = bc.Condition
                                           }).ToList(),
                                           SubCategories = m.SubCategories.Select(sc => new SubCategoryViewModel
                                           {
                                               Id = sc.Id,
                                               Name = sc.Name
                                           }).ToList()
                                       })
                                       .SingleOrDefault();

            // Create the book if doesn't exist.
            if (model == null)
            {
                model = new CreateEditBookViewModel();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteBook(int id)
        {
            Book removedBook = null;

            try
            {
                removedBook = _bookService.DeleteBook(id);
            }
            catch (BookNotAvailableException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message });
            }

            return Json(removedBook, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult CreateEdit(CreateEditBookViewModel model)
        {
            return RedirectToAction("Index");
        }

        public JsonResult ListBookConditions()
        {
            var bookConditionNames = Enum.GetNames(typeof(BookCondition));

            var dict = new Dictionary<string, int>();
            foreach (var name in Enum.GetNames(typeof(BookCondition)))
            {
                dict.Add(name, (int)Enum.Parse(typeof(BookCondition), name));
            }
            var bookConditions = dict
                .Select(kvp => new { Value = kvp.Value, Name = kvp.Key })
                .ToList();

            return Json(bookConditions, JsonRequestBehavior.AllowGet);
        }
    }
}