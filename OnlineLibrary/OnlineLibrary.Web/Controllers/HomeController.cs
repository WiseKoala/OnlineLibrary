using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Models;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.HomeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

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

            var model = new BookSearchViewModel();
            model.Categories = GetCategories();

            return View(model);
        }

        [HttpGet]
        public JsonResult GetBooks(BookSearchViewModel searchModel)
        {
            int itemPerPage = Int32.Parse(ConfigurationManager.AppSettings["PageSize"]);

            var searchServiceModel = new BookSearchServiceModel()
            {
                Author = searchModel.Author,
                Description = searchModel.Description,
                ISBN = searchModel.ISBN,
                PublishDate = searchModel.PublishDate,
                Title = searchModel.Title,
                CategoryId = searchModel.CategoryId,
                SubcategoryId = searchModel.SubcategoryId,
                ItemPerPage = itemPerPage,
                PageNumber = searchModel.PageNumber,
            };

            var findResult = _bookService.Find(searchServiceModel);
            var booksList = findResult.Books.Select(b => new BookViewModel
            {
                Id = b.Id,
                ISBN = b.ISBN,
                Title = b.Title,
                PublishDate = b.PublishDate.ToShortDateString(),
                FrontCover = Url.Content(b.FrontCover),
                Authors = b.Authors.Select(a =>
                            string.Join(" ", a.FirstName, (a.MiddleName ?? ""), a.LastName)),
                Categories = b.SubCategories.Select(sc => new CategoryViewModel
                {
                    Category = sc.Category.Name,
                    SubCategory = sc.Name
                }),
                Description = b.Description,
                BookLink = Url.RouteUrl("Default", new { controller = "BookDetails", action = "Show", id = b.Id })
            }).ToList();


            var bookListViewModel = new BooksListViewModel()
            {
                NumberOfPages = (int)Math.Ceiling((double)findResult.NumberOfBooks / itemPerPage),
                Books = booksList,
            };

            return Json(bookListViewModel, JsonRequestBehavior.AllowGet);
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