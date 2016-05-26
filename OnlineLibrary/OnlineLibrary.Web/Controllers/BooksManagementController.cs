using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.BooksManagement;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize(Roles = UserRoles.SysAdmin)]
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

        [HttpGet]
        public ActionResult CreateEdit(int id)
        {
            // Get all data needed for this action.
            var subcategories = DbContext.SubCategories.ToList();
            var categories = DbContext.Categories.ToList();
            var book = GetBook(id);

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
                        BookCondition = bc.Condition
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

                //  Get all categories for each book's category.
                for (int i = 0; i < model.BookCategories.Count(); i++)
                {
                    var categoriesDropDownItems = SetCategoriesDropDownItems(categories);
                    model.BookCategories[i].Categories = SetSelectedCategory(categoriesDropDownItems, model.BookCategories[i].Id);
                }

                // Get all subcategories for each  book's category.
                for (int i = 0; i < model.BookCategories.Count(); i++)
                {
                    var subcategoriesDropDownItems = SetSubCategoriesDropDownItems(subcategories, model.BookCategories[i].Id);
                    model.BookCategories[i].Subcategories = SetSelectedSubCategory(subcategoriesDropDownItems, model.BookCategories[i].Subcategory.Id);
                }
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

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateEdit(CreateEditBookViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            // If book is new.
            if (model.Id < 1)
            {
                // Create new book.
                var book = new Book()
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    ISBN = model.ISBN,
                    PublishDate = model.PublishDate,
                    FrontCover = SaveImage(model.BookCover.Image)
                };

                // Add book copies.
                foreach (var bookCopy in model.BookCopies)
                {
                    if (!bookCopy.IsToBeDeleted)
                    {
                        book.BookCopies.Add(new BookCopy
                        {
                            Condition = bookCopy.BookCondition
                        });
                    }
                }

                // Add authors.
                foreach (var author in model.Authors)
                {
                    // Try to find author with the same name.
                    Author existingAuthor = DbContext.Authors
                                                     .FirstOrDefault(a => a.FirstName == author.AuthorName.FirstName
                                                     && a.MiddleName == author.AuthorName.MiddleName
                                                     && a.LastName == author.AuthorName.LastName);

                    if (existingAuthor != null && !author.IsRemoved)
                    {
                        book.Authors.Add(existingAuthor);
                    }
                    else if (!author.IsRemoved)
                    {
                        book.Authors.Add(new Author
                        {
                            FirstName = author.AuthorName.FirstName,
                            MiddleName = author.AuthorName.MiddleName,
                            LastName = author.AuthorName.LastName
                        });
                    }
                }

                foreach (var category in model.BookCategories)
                {
                    if (!category.IsRemoved)
                    {
                        var bookSubcategory = DbContext.SubCategories
                                                         .Find(category.Subcategory.Id);
                                                                                
                        book.SubCategories.Add(bookSubcategory);
                    }
                }

                // Save book.
                DbContext.Books.Add(book);
                DbContext.SaveChanges();

                return RedirectToAction("Index", "BooksManagement");
            }
            else
            {
                // Update book.
                Book book = DbContext.Books.Find(model.Id);
                book.Title = model.Title;
                book.Description = model.Description;
                book.ISBN = model.ISBN;
                book.PublishDate = model.PublishDate;

                // Update image only if was uploaded.
                if (model.BookCover.Image != null)
                {
                    book.FrontCover = SaveImage(model.BookCover.Image);
                }

                // Delete book copy from database if element passed to model through HttpPost contains the IsToBeDeleted = true field
                bool DbContextChanged = false;

                foreach (var bookcopy in model.BookCopies)
                {
                    if (bookcopy.IsToBeDeleted == true && bookcopy.Id != 0)
                    {
                        try
                        {
                            _bookService.DeleteBookCopy(bookcopy.Id);
                            DbContextChanged = true;
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
                    }
                }

                if (DbContextChanged)
                {
                    DbContext.SaveChanges();
                }

                // Update book copies

                foreach (var bookCopyModel in model.BookCopies)
                {
                    if (bookCopyModel.IsToBeDeleted == false)
                    {
                        BookCopy bookCopy = DbContext.BookCopies.Find(bookCopyModel.Id);

                        if (bookCopy != null)
                        {
                            // Update existing book copy.
                            bookCopy.Condition = bookCopyModel.BookCondition;
                        }
                        else
                        {
                            // Create and add new book copy.
                            BookCopy newBookCopy = new BookCopy()
                            {
                                Condition = bookCopyModel.BookCondition,
                                BookId = book.Id
                            };
                            DbContext.BookCopies.Add(newBookCopy);
                            DbContext.SaveChanges();
                        }
                    }
                }

                // Update authors.
                foreach (var authorModel in model.Authors)
                {
                    Author author = DbContext.Authors.Find(authorModel.Id);

                    if (author != null)
                    {
                        // If author was removed on the page.
                        if (authorModel.IsRemoved)
                        {
                            book.Authors.Remove(author);
                        }
                        else
                        {
                            // Update existing author.
                            author.FirstName = authorModel.AuthorName.FirstName;
                            author.MiddleName = authorModel.AuthorName.MiddleName;
                            author.LastName = authorModel.AuthorName.LastName;
                        }
                    }
                    else
                    {
                        if (!authorModel.IsRemoved)
                        {
                            // Try to find author with the same name.
                            Author existingAuthor = DbContext.Authors
                                                             .FirstOrDefault(a => a.FirstName == authorModel.AuthorName.FirstName
                                                             && a.MiddleName == authorModel.AuthorName.MiddleName
                                                             && a.LastName == authorModel.AuthorName.LastName);

                            if (existingAuthor != null)
                            {
                                book.Authors.Add(existingAuthor);
                            }
                            else
                            {
                                // Create and add new author.
                                Author newAuthor = new Author()
                                {
                                    FirstName = authorModel.AuthorName.FirstName,
                                    MiddleName = authorModel.AuthorName.MiddleName,
                                    LastName = authorModel.AuthorName.LastName
                                };
                                book.Authors.Add(newAuthor);
                            }
                        }
                    }
                }

                book.SubCategories.Clear();
                var subcategories = DbContext.SubCategories.ToList();

                foreach (var category in model.BookCategories)
                {
                    if( !category.IsRemoved )
                    {
                        var addedCategory = subcategories.Find(sc => sc.Id == category.Subcategory.Id);
                        book.SubCategories.Add(addedCategory);
                    }
                } 

                DbContext.SaveChanges();

                return RedirectToAction("CreateEdit", "BooksManagement", new { id = model.Id });
            }
        }

        [HttpPost]
        public ActionResult DeleteBookCopy(int id = 2)
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

        private string SaveImage(HttpPostedFileBase image)
        {
            string imageRelativeSavePath = null;

            if (image != null)
            {
                var allowedContentTypes = new[]
                {
                    "image/jpeg", "image/png"
                };

                // If content type is allowed, save the image.
                if (allowedContentTypes.Contains(image.ContentType))
                {
                    string contentPath = "~/Content/Images/Books/front-covers";
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string imageAbsoluteSavePath = Path.Combine(Server.MapPath(contentPath), fileName);
                    image.SaveAs(imageAbsoluteSavePath);

                    imageRelativeSavePath = string.Concat(contentPath, '/', fileName);
                }
            }

            return imageRelativeSavePath;
        }

        [HttpPost]
        public ActionResult DeleteBook(int id)
        {
            Book removedBook = null;

            try
            {
                removedBook = _bookService.DeleteBook(id);

                DeleteFileFromServer(removedBook.FrontCover);
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

        [AllowAnonymous]
        public JsonResult ListBookConditions()
        {
            var bookConditionNames = Enum.GetNames(typeof(BookCondition));

            var bookConditions = bookConditionNames
                .Select(name => new
                {
                    Value = (int)Enum.Parse(typeof(BookCondition), name),
                    Name = name
                })
                .ToList();

            return Json(bookConditions, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        public JsonResult ListBookSubcategories(int categoryId )
        {
            var subCategories = DbContext.SubCategories
                                         .Where( sc => sc.CategoryId == categoryId)
                                         .Select(sc => new
                                         {
                                             Value = sc.Id,
                                             Name = sc.Name
                                         })
                                         .ToList();

            return Json(subCategories, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        private void DeleteFileFromServer(string path)
        {
            System.IO.File.Delete(Server.MapPath(path));
        }

        private Book GetBook(int id)
        {
           return DbContext.Books.Where(b => b.Id == id)
                                 .Include(b => b.BookCopies)
                                 .Include(b => b.Authors)
                                 .Include(b => b.SubCategories)
                                 .SingleOrDefault();
        }

        private IList<SelectListItem> SetCategoriesDropDownItems(IList<Category> categories)
        {
           return categories.Select(c => new SelectListItem
                             {
                                 Text = c.Name,
                                 Value = c.Id.ToString()
                             }).ToList();
        }

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

        #endregion
    }
}