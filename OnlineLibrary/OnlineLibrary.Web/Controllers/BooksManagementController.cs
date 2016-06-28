using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Models.BookServiceModels;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.BooksManagement;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize(Roles = UserRoles.SysAdmin)]
    public class BooksManagementController : BaseController
    {
        private IBookService _bookService;
        private ILibrarianService _librarianService;

        public BooksManagementController(ILibraryDbContext dbContext, IBookService bookService, ILibrarianService librarianService)
            : base(dbContext)
        {
            _bookService = bookService;
            _librarianService = librarianService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetBooks(int pageNumber = 1)
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);

            var books = DbContext.Books
                                 .OrderBy(b => b.Id)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList()
                                 .Select(b => new BookManagementViewModel
                                 {
                                     Id = b.Id,
                                     FrontCover = Url.Content(b.FrontCover),
                                     ISBN = b.ISBN,
                                     PublishDate = b.PublishDate.ToShortDateString(),
                                     Title = b.Title
                                 });

            int totalPages = (int)Math.Ceiling(DbContext.Books.Count() / (double)pageSize);

            return Json(new { books = books, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateEdit(int id)
        {
            // Get all data needed for this action.
            var subcategories = DbContext.SubCategories.ToList();
            var categories = DbContext.Categories.ToList();
            var book = _bookService.GetBook(id);

            CreateEditBookViewModel model = new CreateEditBookViewModel();

            // If book != null is edit else is create.
            if (book != null)
            {
                model = new CreateEditBookViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    Description = book.Description,
                    PublishDate = book.PublishDate,
                    BookCover = new FrontCoverViewModel
                    {
                        FrontCover = book.FrontCover ?? string.Empty
                    },

                    BookCopies = book.BookCopies.Select(bc => new BookCopyViewModel
                    {
                        Id = bc.Id,
                        BookCondition = bc.Condition,
                        IsLost = bc.IsLost
                    }).ToList(),

                    Authors = book.Authors.Select(a => new BookAuthorViewModel
                    {
                        Id = a.Id,
                        AuthorName = new AuthorNameViewModel
                        {
                            FirstName = a.FirstName,
                            MiddleName = a.MiddleName,
                            LastName = a.LastName
                        }
                    }).ToList(),
                    // Get book categories and subcategories for this book.
                    BookCategories = book.SubCategories.Select(sc => new CategoryViewModel
                    {
                        Id = sc.CategoryId,
                        Name = sc.Category.Name,
                        Subcategory = new SubCategoryViewModel
                        {
                            Id = sc.Id,
                            Name = sc.Name
                        }
                    }).ToList()
                };

                PrepareDropdowns(model);
            }
            // If, is initializing some data for view.
            else
            {
                model.BookCover = new FrontCoverViewModel();
                model.BookCategories = new List<CategoryViewModel>()
                {
                    new CategoryViewModel
                    {
                         Categories = SetCategoriesDropDownItems(categories)
                    }
                };
            }

            model.AllBookConditions = _bookService.PopulateWithBookConditions();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateEdit(CreateEditBookViewModel model)
        {
            if (model.Authors != null && model.Authors.Any())
            {
                // Remove ModelState Errors for each author marked as removed.
                for (int i = 0; i < model.Authors.Count; i++)
                {
                    if (model.Authors[i].IsRemoved)
                    {
                        if (ModelState.ContainsKey(string.Concat("Authors[", i, "].IsRemoved")))
                        {
                            var keysToRemove = ModelState
                                .Where(ms => ms.Key.StartsWith(string.Concat("Authors[", i)))
                                .Select(ms => ms.Key)
                                .ToList();

                            foreach (var key in keysToRemove)
                            {
                                ModelState[key].Errors.Clear();
                            }
                        }
                    }
                }
            }

            if (model.BookCategories != null)
            {
                // Remove ModelState Error for each book category marked as removed.
                for (int i = 0; i < model.BookCategories.Count(); i++)
                {
                    if (model.BookCategories[i].IsRemoved)
                    {
                        if (ModelState.ContainsKey(string.Concat("BookCategories[", i, "].IsRemoved")))
                        {
                            var keysToRemove = ModelState
                                .Where(ms => ms.Key.StartsWith(string.Concat("BookCategories[", i)))
                                .Select(ms => ms.Key)
                                .ToList();

                            foreach (var key in keysToRemove)
                            {
                                ModelState[key].Errors.Clear();
                            }
                        }
                    }
                }
            }

            var serviceModel = Mapper.Map<CreateEditBookServiceModel>(model);
            var modelErrors = new Dictionary<string, string>();

            model.AllBookConditions = _bookService.PopulateWithBookConditions();
            PrepareDropdowns(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _bookService.CreateEditPreparations(serviceModel, out modelErrors);

            // Update model State and return the view if it's not valid.
            foreach (var error in modelErrors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Remove the needed data from database.
            _bookService.RemoveDataFromDatabase(serviceModel, modelErrors);
            
            // Update model State and return the view if it's not valid.
            foreach (var error in modelErrors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Call Create / Edit service method.
            string absolutePathToImages = Server.MapPath(
                ConfigurationManager.AppSettings["BookCoverRelativePath"]);

            _bookService.CreateEdit(serviceModel, absolutePathToImages);

            var bookCover = model.BookCover;

            model = Mapper.Map<CreateEditBookServiceModel, CreateEditBookViewModel>(serviceModel);

            model.BookCover = bookCover;

            model.AllBookConditions = _bookService.PopulateWithBookConditions();
            PrepareDropdowns(model);

            // On successful book create return to the list page.
            if (model.Id <= 0)
            {
                return RedirectToAction("Index", "BooksManagement");
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteBook(int id)
        {
            Book removedBook = null;
            try
            {
                removedBook = _bookService.GetBook(id);
                removedBook = _bookService.DeleteBook(id, Path.Combine(Server.MapPath(removedBook.FrontCover)));
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

        public JsonResult ListBookConditions()
        {
            var bookConditions = _bookService.PopulateWithBookConditions()
                .Select(name => new
                {
                    Value = (int)Enum.Parse(typeof(BookCondition), name.Key.ToString()),
                    Name = name.Value
                })
                .ToList();

            return Json(bookConditions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListBookCategories()
        {
            var categories = DbContext.Categories
                                      .Select(sc => new
                                      {
                                          Value = sc.Id,
                                          Name = sc.Name
                                      })
                                      .ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListBookSubcategories(int categoryId)
        {
            var subCategories = DbContext.SubCategories
                                         .Where(sc => sc.CategoryId == categoryId)
                                         .Select(sc => new
                                         {
                                             Value = sc.Id,
                                             Name = sc.Name
                                         })
                                         .ToList();

            return Json(subCategories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateISBN(string ISBN)
        {
            try
            {
                if (ISBN != Common.Infrastructure.LibraryConstants.undefinedISBN && ISBN != string.Empty)
                {
                    if (_bookService.IsValidISBN(ISBN))
                    {
                        return Json(new { success = true, valid = true }, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        return Json(new { success = true, invalid = true }, JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }
            }
            catch
            {
                return Json(new { error = true }, JsonRequestBehavior.DenyGet);
            }
        }

        #region Helpers

        /// <summary>
        ///  Get subcategories for a specific category.
        /// </summary>
        /// <param name="categoryId">Specific subcategory.</param>
        private IList<SelectListItem> SetSubCategoriesDropDownItems(IList<SubCategory> subcategories, int categoryId)
        {
            return subcategories.Where(sc => sc.CategoryId == categoryId)
                                .Select(sc => new SelectListItem
                                {
                                    Text = sc.Name,
                                    Value = sc.Id.ToString(),
                                }).ToList();
        }

        private IList<SelectListItem> SetSelectedCategory(IList<SelectListItem> categories, int selectedId)
        {
            return categories.Select(c =>
                              {
                                  c.Selected = c.Value == selectedId.ToString();
                                  return c;
                              }).ToList();
        }

        private IList<SelectListItem> SetSelectedSubCategory(IList<SelectListItem> subCategories, int selectedId)
        {
            return subCategories.Select(c =>
            {
                c.Selected = c.Value == selectedId.ToString();
                return c;
            }).ToList();
        }

        private IList<SelectListItem> SetCategoriesDropDownItems(IList<Category> categories)
        {
            var dropDownItems = new List<SelectListItem>();
            dropDownItems.Add(new SelectListItem { Text = "Choose a category", Value = "0" });

            dropDownItems.AddRange(categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList());

            return dropDownItems;
        }

        private void PrepareDropdowns(CreateEditBookViewModel model)
        {
            if (model.BookCategories != null && model.BookCategories.Any())
            {
                var categories = DbContext.Categories.ToList();
                var subcategories = DbContext.SubCategories.ToList();

                //  Get all categories for each book's category.
                for (int i = 0; i < model.BookCategories.Count(); i++)
                {
                    var categoriesDropDownItems = SetCategoriesDropDownItems(categories);
                    model.BookCategories[i].Categories = SetSelectedCategory(categoriesDropDownItems, model.BookCategories[i].Id);
                }

                // Get all subcategories for each  book's category.
                var modelCategories = model.BookCategories.ToList();

                foreach (var category in modelCategories)
                {
                    if (!category.IsRemoved && category.Subcategory != null)
                    {
                        var subcategoriesDropDownItems = SetSubCategoriesDropDownItems(subcategories, category.Id);
                        category.Subcategories = SetSelectedSubCategory(subcategoriesDropDownItems, category.Subcategory.Id);
                    }
                }
            }
        }

        #endregion Helpers
    }
}