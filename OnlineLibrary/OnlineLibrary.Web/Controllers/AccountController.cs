using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineLibrary.Common.Infrastructure;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Concrete;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Infrastructure.ActionResults;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private UserManagementService _userService;
        private SignInService _signInService;

        public AccountController(ILibraryDbContext dbContext, UserManagementService userService, SignInService signInService)
            : base (dbContext)
        {
            _userService = userService;
            _signInService = signInService;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Get the current URL if null use the Home page.
            string returnUrl = Request.UrlReferrer?.AbsolutePath ?? Url.Action("Index","Home");

            // Using object cast for send the model
            return View((object)returnUrl);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogOff();
            }

            // Request a redirect to the external login provider
            return new ChallengeResult(
                provider,
                Url.Action("ExternalLoginCallback", new { returnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            // Sign in the user with this external login provider if the user already has a login
            var result = await _signInService.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (returnUrl == "/o/oauth2/auth")
                        returnUrl = "/";
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    return RedirectToAction("ExternalLoginConfirmation", new { returnUrl = returnUrl });           
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginConfirmation(string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                string firstName = info.ExternalIdentity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.GivenName).Value;
                string lastName = info.ExternalIdentity.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Surname).Value;
                var user = new User { UserName = info.Email, Email = info.Email, FirstName = firstName, LastName = lastName };

                var createUserResult = await _userService.CreateAsync(user);
                if (createUserResult.Succeeded)
                {
                    // Add login.
                    var addUserResult = await _userService.AddLoginAsync(user.Id, info.Login);
                    // Add user to the 'User' role.
                    _userService.AddToRole(user.Id, UserRoles.User);

                    if (addUserResult.Succeeded)
                    {
                        await _signInService.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        if(IsFirstLogin())
                        {
                            return RedirectToAction("Index", "Role");
                        }
                        return RedirectToLocal(returnUrl);
                    }
                }
            }
           return RedirectToLocal(returnUrl);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();

            // Save sign out date.
            var userToUpdate = _userService.FindById(User.Identity?.GetUserId());
            if (userToUpdate != null)
            {
                userToUpdate.LastSignOutDate = DateTime.Now;
                await _userService.UpdateAsync(userToUpdate);
            }

            return RedirectToLocal("Home/Index");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }


        [HttpGet]
        [Route("power")]
        [AllowAnonymous]
        public ActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        [Route("power")]
        [AllowAnonymous]
        public async Task<ActionResult> Authorize(string superAdminPassword)
        {

            if (string.IsNullOrEmpty(superAdminPassword))
            {
                ModelState.AddModelError("", "Password can not be empty.");

                return View();
            }
            else
            {
                var result = await _signInService.PasswordSignInAsync(LibraryConstants.SuperAdminUserName, superAdminPassword, isPersistent: false, shouldLockout: false);

                if (result == SignInStatus.Success)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "The provided password was incorrect.");

                return View();
            }
        }

        public bool IsFirstLogin()
        {
            bool isFirstUserLogin = false;

            if (DbContext.Users.Count() == 2)
            {
                // Retrieve users into memory.
                var users = DbContext.Users.ToList();

                // Check if there're any users in the role users
                // that don't have the last sign out date set.
                isFirstUserLogin = users.Any(u =>
                    _userService.IsInRole(u.Id, UserRoles.User)
                    && u.LastSignOutDate == null);
            }

            return isFirstUserLogin;
        }
    }
}