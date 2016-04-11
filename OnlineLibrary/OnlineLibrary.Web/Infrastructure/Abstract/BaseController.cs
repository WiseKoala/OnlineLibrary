using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineLibrary.Services.Concrete;
using System;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Infrastructure.Abstract
{
    public abstract class BaseController : Controller
    {
        private SignInService _signInManager;
        private UserManagementService _userManager;

        protected BaseController()
        {
        }

        protected SignInService SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<SignInService>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        protected UserManagementService UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManagementService>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected RoleManagementService RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<RoleManagementService>();
            }
        }

        protected void InitializeUserNameSessionVariable(string firstName = "", string lastName = "")
        {
            string UserName = String.Empty;
            if(HttpContext.User.Identity.Name != null && HttpContext.User.Identity.Name != String.Empty)
            {
                UserName = UserManagementService.GetTheUsernameByUsersName(HttpContext.GetOwinContext(), HttpContext.User.Identity.Name);
            }
            else if(firstName != String.Empty || lastName != String.Empty)
            {
                UserName = firstName + lastName;
            }
            Session["UserName"] = UserName;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion Helpers
    }
}