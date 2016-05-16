using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Web;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.BooksManagement;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using OnlineLibrary.DataAccess;

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

        [HttpPost]
        public ActionResult DeleteBookCopy(int id)
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
                                              Id = a.Id,
                                              FirstName = a.FirstName,
                                              MiddleName = a.MiddleName,
                                              LastName = a.LastName
                                           }).ToList(),

                                          BookCategories = m.SubCategories.Select( sc => new CategoryViewModel
                                          {
                                              Id = sc.CategoryId,
                                              Name = sc.Category.Name,

                                              BookSubCategories = new List<SubCategoryViewModel>
                                              {
                                                  new SubCategoryViewModel
                                                  {
                                                      Id = sc.Id,
                                                      Name = sc.Name
                                                  }
                                              }

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
        public ActionResult CreateEdit(CreateEditBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
                    FrontCover = SaveImage(model.Image)
                };

                // Add book copies.
                foreach (var bookCopy in model.BookCopies)
                {
                    book.BookCopies.Add(new BookCopy
                    {
                        Condition = bookCopy.BookCondition
                    });
                }

                // Add authors.
                foreach (var author in model.Authors)
                {
                    // Try to find author with the same name.
                    Author existingAuthor = DbContext.Authors
                                                     .FirstOrDefault(a => a.FirstName == author.FirstName 
                                                     && a.MiddleName == author.MiddleName 
                                                     && a.LastName == author.LastName);

                    if (existingAuthor != null)
                    {
                        book.Authors.Add(existingAuthor);
                    }
                    else
                    {
                        book.Authors.Add(new Author
                        {
                            FirstName = author.FirstName,
                            MiddleName = author.MiddleName,
                            LastName = author.LastName
                        });
                    }
                }

                // Save book.
                DbContext.Books.Add(book);
                DbContext.SaveChanges();
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
                if (model.Image != null)
                {
                    book.FrontCover = SaveImage(model.Image);
                }
                
                // Update book copies.
                foreach (var bookCopyModel in model.BookCopies)
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

                // Update authors.
                foreach (var authorModel in model.Authors)
                {
                    Author author = DbContext.Authors.Find(authorModel.Id);

                    if (author != null)
                    {
                        // Update existing author.
                        author.FirstName = authorModel.FirstName;
                        author.MiddleName = authorModel.MiddleName;
                        author.LastName = authorModel.LastName;
                    }
                    else
                    {
                        // Create and add new author.
                        Author newAuthor = new Author()
                        {
                            FirstName = authorModel.FirstName,
                            MiddleName = authorModel.MiddleName,
                            LastName = authorModel.LastName
                        };
                        book.Authors.Add(newAuthor);
                    }
                }

                DbContext.SaveChanges();
            }

            return RedirectToAction("CreateEdit", new { id = model.Id });
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