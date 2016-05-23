using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.CategoriesManagement;

namespace OnlineLibrary.Web.Controllers
{
    public class CategoriesManagementController : BaseController
    {
        private ICategoryService _categoryService;

        public CategoriesManagementController(ILibraryDbContext dbContext, ICategoryService categoryService)
            : base(dbContext)
        {
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            var model = DbContext.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .OrderBy(c => c.Name)
                .ToList();

            return View(model);
        }

        [HttpPost]
        public JsonResult CreateCategory(string name)
        {
            Category category = _categoryService.CreateCategory(name);

            return Json(new
            {
                Id = category.Id,
                Name = category.Name
            });
        }
    }
}