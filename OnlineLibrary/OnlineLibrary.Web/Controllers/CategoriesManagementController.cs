using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.CategoriesManagement;
using OnlineLibrary.Common.Exceptions;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize(Roles = UserRoles.SysAdmin)]
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
            return View();
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
                return Json(new { error = ex.Message }, JsonRequestBehavior.DenyGet);
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

        public JsonResult GetCategories()
        {
            var categories = DbContext.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
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

        [HttpPost]
        public JsonResult DeleteBookCategory(int categoryId)
        {
            try
            {
                _categoryService.DeleteBookCategory(categoryId);
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (BookCategoryIsNotRemovableException)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Cateogry is not removable" }, JsonRequestBehavior.AllowGet);
            }
            catch(BookCategoryNotFoundException)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Category not found"}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteBookSubcategory(int subcategoryId)
        {
            try
            {
                _categoryService.DeleteBookSubcategory(subcategoryId);
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (BookSubcateogryIsNotRemovableException)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Subcategory is not removable" }, JsonRequestBehavior.AllowGet);
            }
            catch (BookSubcategoryNotFoundException)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = "Subcategory not found" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCategory(int categoryId, string newName)
        {
            try
            {
                Category updatedCategory = _categoryService.UpdateCategory(categoryId, newName);

                return Json(new
                {
                    Id = updatedCategory.Id,
                    Name = updatedCategory.Name
                });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateSubCategory(int subCategoryId, string newName)
        {
            try
            {
                SubCategory updatedSubCategory = 
                    _categoryService.UpdateSubCategory(subCategoryId, newName);

                return Json(new
                {
                    Id = updatedSubCategory.Id,
                    Name = updatedSubCategory.Name
                });
            }
            catch (KeyNotFoundException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}