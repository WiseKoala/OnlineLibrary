using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.BooksManagement;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class BooksManagementController : BaseController
    {
        public BooksManagementController(ILibraryDbContext dbContext)
            :base(dbContext) {}

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateEdit(int id = 2)

        {
            var model = DbContext.Books.Where(b => b.Id == id)
                                       .Include(b => b.SubCategories)
                                       .Include(b => b.BookCopies)
                                       .Include(b => b.Authors)
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
                                          Authors = m.Authors.Select(a => new BookAuthorViewModel
                                          {
                                              FirstName = a.FirstName,
                                              MiddleName = a.MiddleName,
                                              LastName = a.LastName
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
        public ActionResult CreateEdit( CreateEditBookViewModel model )
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