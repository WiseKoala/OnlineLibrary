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

            // Obtain list of books from the database.
            var booksList = DbContext.Books
                .Include(b => b.Authors)
                .Include(b => b.SubCategories)
                .Include("SubCategories.Category")
                .ToList()
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
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

            var model = new BooksListViewModel()
            {
                Books = booksList
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
                Title = searchModel.Title
            };

            IEnumerable<Book> foundBooks = _bookService.Find(searchServiceModel);
            var booksViewModel = foundBooks.Select(b => new BookViewModel
                                 {
                                     Id = b.Id,
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