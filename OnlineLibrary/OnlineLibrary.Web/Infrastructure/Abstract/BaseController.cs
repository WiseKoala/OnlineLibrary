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

        protected BaseController(ILibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected ILibraryDbContext DbContext
        {
            get { return _dbContext; }
        }

        private bool IsNeededToSetSessionVariable()
        {
            return User.Identity.IsAuthenticated && SessionHelper.UserNameSessionVariable == null;
        }

        protected void InitializeUserNameSessionVariable()
        {
            if (IsNeededToSetSessionVariable())
            {
                InitializeUserNameSessionVariable(string.Empty, string.Empty);
            }
        }

        protected void InitializeUserNameSessionVariable(string firstName, string lastName)
        {
            if (IsNeededToSetSessionVariable())
            {
                string userName = string.Empty;
                if (!string.IsNullOrEmpty(User.Identity.Name))
                {
                    var user = _dbContext.Users.Where(u => u.UserName == User.Identity.Name).Single();
                    userName = user.FirstName + " " + user.LastName;
                }
                else if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
                {
                    userName = firstName + " " + lastName;
                }

                Session["UserName"] = userName;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                // Get current user.
                var user = _dbContext.Users.Where(u => u.UserName == User.Identity.Name).Single();
            }
            catch
            {
                // Delete cookies and abandon session if the logged in user is not in the database or doesn't exist.
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Session.Abandon();
            }

            base.OnActionExecuting(filterContext);
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