using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.CategoriesManagement;

namespace OnlineLibrary.Web.Controllers
{
    public class CategoriesManagementController : BaseController
    {
        public CategoriesManagementController(ILibraryDbContext dbContext)
            : base(dbContext)
        { }

        public ActionResult Index()
        {
            var model = DbContext.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return View(model);
        }
    }
}