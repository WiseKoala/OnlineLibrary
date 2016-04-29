using OnlineLibrary.Web.Infrastructure.Abstract;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using OnlineLibrary.Web.Models;
using System.Collections.Generic;
using OnlineLibrary.DataAccess.Abstract;

namespace OnlineLibrary.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ILibraryDbContext dbContext)
            : base(dbContext)
        {

        }

        public ActionResult Index()
        {
            if (!IsUserNameSessionVariableSet())
            {
                InitializeUserNameSessionVariable();
            }

            // Obtain list of books from the database.
            var booksList = DbContext.Books
                .Include(b => b.Authors)
                .Include(b => b.SubCategories)
                .Include("SubCategories.Category")
                .ToList()
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublishDate = b.PublishDate,
                    FrontCover = b.FrontCover,
                    Authors = b.Authors.Select(a =>
                        string.Join(" ", a.FirstName, (a.MiddleName ?? ""), a.LastName)),
                    Categories = b.SubCategories.Select(sc => new CategoryViewModel
                    {
                        Category = sc.Category.Name,
                        SubCategory = sc.Name
                    }),
                    Description = b.Description
                })
                .ToList();

            return View(booksList);
        }
    }
}