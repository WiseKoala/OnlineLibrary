using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OnlineLibrary.Services.Concrete
{
    public class UserManagementService : UserManager<User>
    {
        public UserManagementService(IUserStore<User> store)
            : base(store)
        {
        }

        public static UserManagementService Create(IdentityFactoryOptions<UserManagementService> options, IOwinContext context)
        {
            var manager = new UserManagementService(new UserStore<User>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public static string GetUsernameById(IOwinContext context, string id)
        {
            var manager = new UserManagementService(new UserStore<User>(context.Get<ApplicationDbContext>()));
            var dbContext = context.Get<ApplicationDbContext>();
            var UserName = dbContext.Users.Where(u => u.Id == id).Select(u => String.Concat(u.FirstName, u.LastName));
            return UserName.ToString();
        }

        public static User GetUserByName(IOwinContext context, string name)
        {
            var manager = new UserManagementService(new UserStore<User>(context.Get<ApplicationDbContext>()));
            var dbContext = context.Get<ApplicationDbContext>();
            User user = dbContext.Users.Where(u => u.UserName == name).FirstOrDefault();
            return user;
        }

        public static string GetTheUsernameByUsersName(IOwinContext context, string UsersName)
        {
            var manager = new UserManagementService(new UserStore<User>(context.Get<ApplicationDbContext>()));
            var dbContext = context.Get<ApplicationDbContext>();
            User user = GetUserByName(context, UsersName);
            string Username = String.Empty;
            if (user != null)
            {
                Username = UsersName;
                string firstName = Regex.Replace(user.FirstName, ".*?: ", String.Empty);
                string lastName = Regex.Replace(user.LastName, ".*?: ", String.Empty);
                Username = firstName + " " + lastName;
            }
            return Username;
        }
    }
}