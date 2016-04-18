using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Infrastructure.Abstract
{
    public static class SessionHelper
    {
        public static string UserNameSessionVariable {
            get
            {
                return HttpContext.Current.Session["UserName"]?.ToString();
            }
        }
    }
}