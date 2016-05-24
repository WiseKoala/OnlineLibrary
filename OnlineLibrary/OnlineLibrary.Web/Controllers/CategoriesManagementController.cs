using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            try
            {
                Category category = _categoryService.CreateCategory(name);

                return Json(new
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }
            catch (ArgumentException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult CreateSubCategory(int categoryId, string name)
        {
            try
            {
                SubCategory subCategory = _categoryService.CreateSubCategory(categoryId, name);

                return Json(new
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name
                });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message }, JsonRequestBehavior.DenyGet);
            }
            catch (ArgumentException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = ex.Message }, JsonRequestBehavior.DenyGet);
            }
        }

        public JsonResult GetSubCategories(int categoryId)
        {
            try
            {
                IEnumerable<SubCategory> subCategories = _categoryService.GetSubCategories(categoryId);

                // Project to JSON object.
                var subCategoriesJson = subCategories
                    .Select(sc => new
                    {
                        Id = sc.Id,
                        Name = sc.Name
                    })
                    .ToList();

                return Json(subCategoriesJson, JsonRequestBehavior.AllowGet);
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}