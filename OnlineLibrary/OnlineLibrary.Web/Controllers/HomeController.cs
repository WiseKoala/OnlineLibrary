using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Models;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.HomeViewModels;

namespace OnlineLibrary.Web.Controllers
{
    public class HomeController : BaseController
    {
        private IBookService _bookService;

        public HomeController(ILibraryDbContext dbContext, IBookService bookService)
            : base(dbContext)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public ActionResult Index(BooksListViewModel model)
        {
            InitializeUserNameSessionVariable();

            IEnumerable<BookViewModel> booksList = null;
            if (model.SearchData == null)
            {
                // Retreive list of all books.
                booksList = DbContext.Books
                .Include(b => b.Authors)
                .Include(b => b.SubCategories)
                .Include("SubCategories.Category")
                .ToList()
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    ISBN = b.ISBN,
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
            }
            else
            {
                BookSearchViewModel searchViewModel = model.SearchData;
                var searchServiceModel = new BookSearchServiceModel()
                {
                    Author = searchViewModel.Author,
                    Description = searchViewModel.Description,
                    ISBN = searchViewModel.ISBN,
                    PublishDate = searchViewModel.PublishDate,
                    Title = searchViewModel.Title,
                    CategoryId = searchViewModel.CategoryId,
                    SubcategoryId = searchViewModel.SubcategoryId
                };

                IEnumerable<Book> foundBooks = _bookService.Find(searchServiceModel);
                booksList = foundBooks.Select(b => new BookViewModel
                {
                    Id = b.Id,
                    ISBN = b.ISBN,
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
                });
            }

            // Craft the view model object.
            var viewModel = new BooksListViewModel()
            {
                Books = booksList,
                SearchData = new BookSearchViewModel
                {
                    Categories = GetCategories()
                }
            };

            if (model.SearchData != null)
            {
                viewModel.SearchData.Subcategories = model.SearchData.Subcategories = GetSubcategories(model.SearchData.CategoryId);
            }

            return View(viewModel);
        }

        private List<SelectListItem> GetCategories()
        {
            List<SelectListItem> categories = DbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            return categories;
        }

        private IEnumerable<SelectListItem> GetSubcategories(int? categoryId)
        {
            if (categoryId != null)
            {
                var subcategories = DbContext.SubCategories
                    .Where(sc => sc.CategoryId == categoryId)
                    .Select(sc => new SelectListItem { Text = sc.Name, Value = sc.Id.ToString() })
                    .ToList();
                return subcategories;
            }
            else
            {
                return Enumerable.Empty<SelectListItem>();
            }
        }
    }
}