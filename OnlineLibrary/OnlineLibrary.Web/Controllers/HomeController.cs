using OnlineLibrary.Web.Infrastructure.Abstract;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using OnlineLibrary.Web.Models;
using System.Collections.Generic;

namespace OnlineLibrary.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if(HttpContext.User.Identity.IsAuthenticated && Session["UserName"] == null)
            {
                InitializeUserNameSessionVariable();
            }

            var books = DbContext.Books.Include(b => b.Authors);
            var booksList = new List<BookViewModel>();
            foreach (var book in books)
            {
                booksList.Add(new BookViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    PublishDate = book.PublishDate,
                    FrontCover = book.FrontCover,
                    Authors = string.Join(", ", book.Authors.Select(a => string.Join(" ", a.FirstName, (a.MiddleName ?? ""), a.LastName)))
                });
            }
            return View(booksList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}