using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineLibrary.Services.Concrete;
using System;
using System.Web;
using System.Web.Mvc;
using OnlineLibrary.DataAccess;
using System.Linq;
using OnlineLibrary.DataAccess.Entities;
using System.Data.Entity;
using OnlineLibrary.DataAccess.Abstract;

namespace OnlineLibrary.Web.Infrastructure.Abstract
{
    public abstract class BaseController : Controller
    {
        private ILibraryDbContext _dbContext;
        private SignInService _signInService;
        private UserManagementService _userManagementService;
        private RoleManagementService _roleManagementService;

        protected BaseController(ILibraryDbContext dbContext, 
            SignInService signInService, 
            UserManagementService userManagementService,
            RoleManagementService roleManagementService)
        {
            _dbContext = dbContext;
            _signInService = signInService;
            _userManagementService = userManagementService;
            _roleManagementService = roleManagementService;
        }

        protected ILibraryDbContext DbContext
        {
            get { return _dbContext; }
        }

        protected SignInService SignInManager
        {
            get
            {
                return _signInService ?? HttpContext.GetOwinContext().Get<SignInService>();
            }
            private set
            {
                _signInService = value;
            }
        }

        protected UserManagementService UserManager
        {
            get
            {
                return _userManagementService;
            }
        }

        protected RoleManagementService RoleManager
        {
            get
            {
                return _roleManagementService;
            }
        }

        protected bool IsUserNameSessionVariableSet()
        {
            return User.Identity.IsAuthenticated && SessionHelper.UserNameSessionVariable == null;
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

            Session["UserName"] = UserName;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManagementService != null)
                {
                    _userManagementService.Dispose();
                    _userManagementService = null;
                }

                if (_signInService != null)
                {
                    _signInService.Dispose();
                    _signInService = null;
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

        protected bool IsFirstLogin()
        {
            bool isFirstUserLogin = false;

            if (UserManager.Users.Count() == 2)
            {
                // Retrieve users into memory.
                var users = UserManager.Users.ToList();

                // Check if there're any users in the role users
                // that don't have the last sign out date set.
                isFirstUserLogin = users.Any(u => 
                    UserManager.IsInRole(u.Id, UserRoles.User) 
                    && u.LastSignOutDate == null);
            }

            return isFirstUserLogin;
        }

        #endregion Helpers
    }
}