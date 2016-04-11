using OnlineLibrary.Web.Infrastructure.Abstract;
using System;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if(HttpContext.User.Identity.IsAuthenticated && Session["UserName"] == null)
            {
                InitializeUserNameSessionVariable();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}