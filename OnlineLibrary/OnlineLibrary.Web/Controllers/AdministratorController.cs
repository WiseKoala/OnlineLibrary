using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Services.Concrete;
using OnlineLibrary.Web.Infrastructure.Abstract;
using System.Threading.Tasks;
using System.Web.Mvc;
using OnlineLibrary.Common.Infrastructure;

namespace OnlineLibrary.Web.Controllers
{
    public class AdministratorController : BaseController
    {
        private SignInService _signInService;

        public AdministratorController(ILibraryDbContext dbContext, SignInService signInService)
            : base(dbContext)
        {
            _signInService = signInService;
        }

        
    }
}