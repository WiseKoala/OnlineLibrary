using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Services.Concrete;

namespace OnlineLibrary.Web.Controllers
{
    public class AdministratorController : BaseController
    {
        private SignInService _signInService;

        public AdministratorController(ILibraryDbContext dbContext, SignInService signInService)
            : base(dbContext)
        {
            _signInService = signInService;
        }

        [HttpGet]
        [Route("power")]
        public ActionResult Authorize()
        {
            return View(new SuperAdminViewModel());
        }

        [HttpPost]
        [Route("power")]
        public async Task<ActionResult> Authorize(SuperAdminViewModel model)
        {
            var result = await _signInService.PasswordSignInAsync("Admin", model.Password, isPersistent: false, shouldLockout: false);

            if (result == SignInStatus.Success)
                return RedirectToAction("Index", "Role");

            return RedirectToAction("Authorize", "Administrator");
        }
    }
}