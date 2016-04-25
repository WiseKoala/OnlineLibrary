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
            string userName = firstName + lastName;

            Session["UserName"] = userName;
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