using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class LibrarianController : Controller
    {
        // GET: Librarian
        [Authorize(Roles = "Librarian, SysAdmin, SuperAdmin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}