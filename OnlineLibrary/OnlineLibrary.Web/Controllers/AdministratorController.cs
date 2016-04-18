using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    public class AdministratorController : BaseController
    {

        [HttpGet]
        public ActionResult Authorize()
        {
            return View(new SuperAdminViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Authorize(SuperAdminViewModel model)
        {
            var result = await SignInManager.PasswordSignInAsync("Admin", model.Password, isPersistent: false, shouldLockout: false);

            if (result == SignInStatus.Success)
                return RedirectToAction("Index", "Role");

            return RedirectToAction("Authorize", "Administrator");
        }
    }
}