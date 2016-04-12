using Microsoft.AspNet.Identity.Owin;
using OnlineLibrary.Services.Concrete;
using System.Web;
using System.Web.Mvc;

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