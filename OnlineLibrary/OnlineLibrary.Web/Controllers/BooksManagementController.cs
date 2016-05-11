using System.Linq;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.BooksManagement;

namespace OnlineLibrary.Web.Controllers
{
    public class BooksManagementController : BaseController
    {
        public BooksManagementController(ILibraryDbContext dbContext)
            : base(dbContext)
        {
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
    }
}