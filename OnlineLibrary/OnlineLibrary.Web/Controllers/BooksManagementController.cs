using OnlineLibrary.Web.Models.BooksManagement;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class BooksManagementController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateEdit(int bookId)
        {

            return View();
        }
    }
}