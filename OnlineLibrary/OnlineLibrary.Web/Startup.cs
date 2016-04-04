using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineLibrary.Web.Startup))]
namespace OnlineLibrary.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
