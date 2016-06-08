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
        public ActionResult Index()
        {
            InitializeUserNameSessionVariable();

            // Retreive list of books.
            var booksList = DbContext.Books
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

            // Retreive list of categories.
            var categories = DbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            })
            .ToList();

            // Craft the view model object.
            var model = new BooksListViewModel()
            {
                Books = booksList,
                SearchData = new BookSearchViewModel { Categories = categories }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(BooksListViewModel model)
        {
            BookSearchViewModel searchModel = model.SearchData;
            var searchServiceModel = new BookSearchServiceModel()
            {
                Author = searchModel.Author,
                Description = searchModel.Description,
                ISBN = searchModel.ISBN,
                PublishDate = searchModel.PublishDate,
                Title = searchModel.Title,
                CategoryId= searchModel.CategoryId,
                SubcategoryId = searchModel.SubcategoryId
            };

            IEnumerable<Book> foundBooks = _bookService.Find(searchServiceModel);
            var booksViewModel = foundBooks.Select(b => new BookViewModel
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

            model.Books = booksViewModel;

            return View(model);
        }
    }
}