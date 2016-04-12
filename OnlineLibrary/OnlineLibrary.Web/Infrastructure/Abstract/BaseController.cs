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

        protected bool IsUserNameSessionVariableSet()
        {
            return User.Identity.IsAuthenticated && UserName.UserNameSessionVariable == null;
        }

        protected void InitializeUserNameSessionVariable()
        {
            InitializeUserNameSessionVariable(string.Empty, string.Empty);
        }

        protected void InitializeUserNameSessionVariable(string firstName, string lastName)
        {
            string UserName = string.Empty;
            if(!string.IsNullOrEmpty(User.Identity.Name))
            {
                UserName = UserManagementService.GetTheUsernameByUsersName(HttpContext.GetOwinContext(), User.Identity.Name);
            }
            else if(!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
            {
                UserName = firstName + lastName;
            }
            Abstract.UserName.UserNameSessionVariable = UserName;
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