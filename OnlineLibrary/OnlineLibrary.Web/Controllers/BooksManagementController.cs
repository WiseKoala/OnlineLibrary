using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.BooksManagement;

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
    }
}