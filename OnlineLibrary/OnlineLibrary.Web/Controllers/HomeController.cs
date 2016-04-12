using OnlineLibrary.Web.Infrastructure.Abstract;
using System;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (!IsUserNameSessionVariableSet())
            {
                InitializeUserNameSessionVariable();
            }
            return View();
        }
    }
}