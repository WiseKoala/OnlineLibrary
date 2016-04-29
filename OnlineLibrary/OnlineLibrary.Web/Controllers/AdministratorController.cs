using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Services.Concrete;
using OnlineLibrary.Web.Infrastructure.Abstract;
using System.Threading.Tasks;
using System.Web.Mvc;
using OnlineLibrary.Common.Infrastructure;

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
            return View();
        }

        [HttpPost]
        [Route("power")]
        public async Task<ActionResult> Authorize(string SuperAdminPassword)
        {

            if (string.IsNullOrEmpty(SuperAdminPassword))
            {
                ModelState.AddModelError("", "Password can not be empty.");

                return View();
            }
            else
            {
                var result = await _signInService.PasswordSignInAsync(LibraryConstants.SuperAdminUserName, SuperAdminPassword, isPersistent: false, shouldLockout: false);

                if (result == SignInStatus.Success)
                {
                    return RedirectToAction("Index", "Role");
                }

                ModelState.AddModelError("", "The provided password was incorrect.");

                return View();
            }
        }
    }
}