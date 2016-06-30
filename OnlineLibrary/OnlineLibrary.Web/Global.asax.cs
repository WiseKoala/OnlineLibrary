using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OnlineLibrary.Services.Models.BookServiceModels;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;

namespace OnlineLibrary.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperConfig.RegisterMappings();
        }

        protected void Session_Start()
        { }
    }
}