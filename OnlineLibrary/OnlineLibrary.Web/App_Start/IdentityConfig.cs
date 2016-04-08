using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess;
using System.Web.Mvc;
using System.Web;
using OnlineLibrary.Services.Concrete;

namespace OnlineLibrary.Web
{
    public static class IdentityHelpers
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            UserManagementService mgr = HttpContext.Current.GetOwinContext().GetUserManager<UserManagementService>();
            return new MvcHtmlString(mgr.FindByIdAsync(id).Result.UserName);
        }
    }
}