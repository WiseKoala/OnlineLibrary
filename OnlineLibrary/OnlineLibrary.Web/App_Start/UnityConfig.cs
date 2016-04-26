using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Concrete;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using OnlineLibrary.Services.Concrete;

namespace OnlineLibrary.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // Register types.
            container.RegisterType<ILibraryDbContext, LibraryDbContext>(new ContainerControlledLifetimeManager());

            // Services infrastructure.
            container.RegisterType<IRoleStore<Role, string>, RoleStore<Role>>(new InjectionConstructor(container.Resolve<ILibraryDbContext>()));
            container.RegisterType<IUserStore<User>, UserStore<User>>(new InjectionConstructor(container.Resolve<ILibraryDbContext>()));
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(o => HttpContext.Current.GetOwinContext().Authentication));

            // Services.
            container.RegisterType<IBookService, BookService>();
            container.RegisterType<ILibrarianService, LibrarianService>();
            container.RegisterType<RoleManagementService>();
            container.RegisterType<SignInService>();
            var userManagementService = GetUserManagementServiceInstance(container);
            container.RegisterInstance<UserManagementService>(userManagementService);
        }

        private static UserManagementService GetUserManagementServiceInstance(IUnityContainer container)
        {
            var manager = new UserManagementService(container.Resolve<ILibraryDbContext>(), container.Resolve<UserStore<User>>());
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

            return manager;
        }
    }
}
