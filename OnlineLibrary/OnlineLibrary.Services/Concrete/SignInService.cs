using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineLibrary.DataAccess.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineLibrary.Services.Concrete
{
    public class SignInService : SignInManager<User, string>
    {
        public SignInService(UserManagementService userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((UserManagementService)UserManager);
        }
    }
}