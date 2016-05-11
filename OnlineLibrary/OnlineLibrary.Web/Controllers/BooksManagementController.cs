using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class BooksManagementController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}