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
        private ILibrarianService _librarianService;

        public BooksManagementController(ILibraryDbContext dbContext, IBookService bookService ,ILibrarianService librarianService)
            : base(dbContext)
        {
            _bookService = bookService;
            _librarianService = librarianService;
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

                //  Get all categories for each book's category.
                for (int i = 0; i < model.BookCategories.Count(); i++)
                {
                    var categoriesDropDownItems = SetCategoriesDropDownItems(categories);
                    model.BookCategories[i].Categories = SetSelectedCategory(categoriesDropDownItems, model.BookCategories[i].Id);
                }

                // Get all subcategories for each  book's category.
                for (int i = 0; i < model.BookCategories.Count(); i++)
                {
                    if (model.BookCategories[i].Subcategory != null)
                    {
                        var subcategoriesDropDownItems = SetSubCategoriesDropDownItems(subcategories, model.BookCategories[i].Id);
                        model.BookCategories[i].Subcategories = SetSelectedSubCategory(subcategoriesDropDownItems, model.BookCategories[i].Subcategory.Id);
                    }
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

            model.AllBookConditions = PopulateWithBookConditions();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateEdit(CreateEditBookViewModel model)
        {
            var subcategories = DbContext.SubCategories.ToList();
            model.AllBookConditions = PopulateWithBookConditions();

            var categories = DbContext.Categories.ToList();
            //  Get all categories for each book's category.
            for (int i = 0; i < model.BookCategories.Count(); i++)
            {
                var categoriesDropDownItems = SetCategoriesDropDownItems(categories);
                model.BookCategories[i].Categories = SetSelectedCategory(categoriesDropDownItems, model.BookCategories[i].Id);
            }

            // Get all subcategories for each  book's category.
            for (int i = 0; i < model.BookCategories.Count(); i++)
            {
                if (model.BookCategories[i].Subcategory != null)
                {
                    var subcategoriesDropDownItems = SetSubCategoriesDropDownItems(subcategories, model.BookCategories[i].Id);
                    model.BookCategories[i].Subcategories = SetSelectedSubCategory(subcategoriesDropDownItems, model.BookCategories[i].Subcategory.Id);
                }
            }

            model.ISBN = _bookService.FormatISBN(model.ISBN);
            
            if (!ModelState.IsValid)
            {
                var bookcopies = model.BookCopies.ToList();

                if (bookcopies.Count > 0)
                {
                    foreach (var bookcopy in bookcopies)
                    {
                        if (bookcopy.IsToBeDeleted == true)
                        {
                            model.BookCopies.Remove(bookcopy);
                        }
                    }
                }

                for (int i = 0; i < model.Authors.Count; i++)
                {
                    if (model.Authors[i].IsRemoved)
                    {
                        if (ModelState.ContainsKey(string.Concat("Authors[", i, "].IsRemoved")))
                        {
                            foreach (var state in ModelState.ToArray())
                            {
                                if (state.Key.StartsWith(string.Concat("Authors[", i)))
                                {
                                    ModelState.Remove(state);
                                }
                            }
                        }

                        if (model.Authors[i].IsRemoved == true)
                        {
                            model.Authors.Remove(model.Authors[i]);
                        }

                        if (!model.Authors.Any())
                        {
                            ModelState.AddModelError("Authors", "There has to be at least one author.");
                        }
                    }
                }

                for (int i = 0; i < model.BookCategories.Count(); i++)
                {
                    if (model.BookCategories[i].IsRemoved)
                    {
                        if (ModelState.ContainsKey(string.Concat("BookCategories[", i, "].IsRemoved")))
                        {
                            foreach (var state in ModelState.ToArray())
                            {
                                if (state.Key.StartsWith(string.Concat("BookCategories[", i)))
                                {
                                    ModelState.Remove(state);
                                }
                            }
                        }

                        if (model.BookCategories[i].IsRemoved == true)
                        {
                            model.BookCategories.Remove(model.BookCategories[i]);
                        }

                        if (!model.BookCategories.Any())
                        {
                            ModelState.AddModelError("Book Categories", "There has to be at least one Book Category.");
                        }
                    }
                }

                return View(model);
            }
            
            // If book is new.
            if (model.Id < 1)
            {
                if (!_bookService.IsValidISBN(model.ISBN))
                {
                    ModelState.AddModelError("ISBN", "A book with this ISBN already exists");
                    return View(model);
                }

                Book book;
                if (!String.IsNullOrEmpty(model.BookCover.FrontCover))
                {
                    // Create new book.
                    book = new Book()
                    {
                        Id = model.Id,
                        Title = model.Title,
                        Description = model.Description,
                        ISBN = model.ISBN,
                        PublishDate = model.PublishDate,
                        FrontCover = SaveImageFromUrl(model.BookCover.FrontCover)
                    };
                }
                else
                {
                    // Create new book.
                    book = new Book()
                    {
                        Id = model.Id,
                        Title = model.Title,
                        Description = model.Description,
                        ISBN = model.ISBN,
                        PublishDate = model.PublishDate,
                        FrontCover = SaveImage(model.BookCover.Image)
                    };
                }
                
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

                // Remove duplicate authors from model.
                var authors = model.Authors.ToList();

                foreach (var author in authors)
                {
                    var duplicateAuthors = model.Authors.Where(a => a.AuthorName.FirstName == author.AuthorName.FirstName
                                                     && a.AuthorName.MiddleName == author.AuthorName.MiddleName
                                                     && a.AuthorName.LastName == author.AuthorName.LastName && a.IsRemoved == false).ToList();
                    if (duplicateAuthors.Count() > 1)
                    {
                        for (var i = 1; i < duplicateAuthors.Count(); i++)
                        {
                            model.Authors.RemoveAt(i);
                        }
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

                // Delete old book cover image from database in case new image is added
                if (!String.IsNullOrEmpty(model.OldImagePath))
                {
                    if (model.OldImagePath.IndexOf("/Content/Images/Books/front-covers") != -1)
                    {
                        string correctedPath = "~" + model.OldImagePath.Substring(model.OldImagePath.IndexOf("/Content/Images/Books/front-covers"));
                        DeleteFileFromServer(correctedPath);
                    }
                }

                // Save image from Url address in case image is imported
                if (!String.IsNullOrEmpty(model.BookCover.FrontCover) && model.BookCover.FrontCover.StartsWith("http"))
                {
                    book.FrontCover = SaveImageFromUrl(model.BookCover.FrontCover);
                }
                
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
                            ModelState.AddModelError("Book Copy", "Book copy with Id # = " + bookcopy.Id + " is currently involved in loans. Unable to remove it.");
                            return View(model);
                        }
                        catch (KeyNotFoundException ex)
                        {
                            ModelState.AddModelError("Book Copy", "Book copy with Id # = " + bookcopy.Id + " is not found. Someone may have already deleted it. Please reload page to see latest changes.");
                            return View(model);
                        }
                    }
                    else
                    {
                        _librarianService.ChangeIsLostStatus(bookcopy.Id, bookcopy.IsLost);
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

                // Remove duplicate authors from model.
                var authors = model.Authors.ToList();

                foreach (var author in authors)
                {
                    // Identify all duplicate authors with the same name as current iterated author that is also not marked as removed.
                    var duplicateAuthors = model.Authors.Where(a => a.AuthorName.FirstName == author.AuthorName.FirstName
                                                     && a.AuthorName.MiddleName == author.AuthorName.MiddleName
                                                     && a.AuthorName.LastName == author.AuthorName.LastName && a.IsRemoved == false).ToList();
                    if (duplicateAuthors.Count() > 1)
                    {
                        for (var i = 1; i < duplicateAuthors.Count(); i++)
                        {
                            // Remove duplicate author from model.
                            model.Authors.RemoveAt(i);
                        }

                        var authorsToRemove = book.Authors.Where(ba => ba.Id != author.Id && ba.FirstName == author.AuthorName.FirstName
                                                                 && ba.MiddleName == author.AuthorName.MiddleName
                                                                 && ba.LastName == author.AuthorName.LastName).Select(ba => ba).ToList();

                        foreach (var authorToRemove in authorsToRemove)
                        {
                            // Remove duplicate author from book.
                            book.Authors.Remove(authorToRemove);
                        }
                    }
                }


                // Update authors.
                foreach (var authorModel in model.Authors)
                {
                    // Variable "author" is checking whether the author is already in the database
                    // It selects the item or returns null if not found
                    Author author = DbContext.Authors.FirstOrDefault(a => a.FirstName == authorModel.AuthorName.FirstName
                                                             && a.MiddleName == authorModel.AuthorName.MiddleName
                                                             && a.LastName == authorModel.AuthorName.LastName);
                    Author bookauthor = book.Authors.FirstOrDefault(a => a.FirstName == authorModel.AuthorName.FirstName
                                                             && a.MiddleName == authorModel.AuthorName.MiddleName
                                                             && a.LastName == authorModel.AuthorName.LastName);

                    // Select author from database with id coresponding to current author iterated from model.
                    Author authorById = DbContext.Authors.FirstOrDefault(a => a.Id == authorModel.Id);

                    if (author != null)
                    {
                        if (bookauthor != null)
                        {
                            // Checks if author was removed on the page.
                            if (authorModel.IsRemoved)
                            {
                                book.Authors.Remove(author);
                            }
                        }
                        else
                        {
                            book.Authors.Add(author);
                        }
                    }
                    else
                    {
                        if (!authorModel.IsRemoved)
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

                    // If user edits existing author name, previous author name is removed. 
                    if (authorById != null)
                    {
                        if (authorById.FirstName != authorModel.AuthorName.FirstName
                            || authorById.MiddleName != authorModel.AuthorName.MiddleName
                            || authorById.LastName != authorModel.AuthorName.LastName)
                        {
                            book.Authors.Remove(authorById);
                        }
                    }
                }

                DbContext.SaveChanges();

                book.SubCategories.Clear();

                foreach (var category in model.BookCategories)
                {
                    if (!category.IsRemoved && !book.SubCategories.Any(sc => sc.Id == category.Subcategory.Id ))
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

        private string SaveImageFromUrl(string url)
        {
                string contentPath = "~/Content/Images/Books/front-covers";
                string fileName = Guid.NewGuid().ToString() + ".jpg"; // + Path.GetExtension(image.FileName);
                string imageAbsoluteSavePath = Path.Combine(Server.MapPath(contentPath), fileName);
                string imageRelativeSavePath = string.Concat(contentPath, '/', fileName);
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, imageAbsoluteSavePath);
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

        public JsonResult ListBookConditions()
        {
            var bookConditions = PopulateWithBookConditions()
                .Select(name => new
                {
                    Value = (int)Enum.Parse(typeof(BookCondition), name.Key.ToString()),
                    Name = name.Value
                })
                .ToList();

            return Json(bookConditions, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<BookCondition, string> PopulateWithBookConditions()
        {
       
            var bookConditions = new Dictionary<BookCondition, string>();

            foreach (var cond in Enum.GetValues(typeof(BookCondition)))
            {
                bookConditions.Add((BookCondition)cond, _bookService.GetConditionDescription((BookCondition)cond));
            }

            return bookConditions;
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

        public JsonResult ValidateISBN(string ISBN)
        {
            try {
                if(ISBN != Common.Infrastructure.LibraryConstants.undefinedISBN && ISBN != string.Empty)
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
            var dropDownItems = new List<SelectListItem>();
            dropDownItems.Add(new SelectListItem { Text = "Choose a category", Value = "0" });

            dropDownItems.AddRange(categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList());

            return dropDownItems;
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