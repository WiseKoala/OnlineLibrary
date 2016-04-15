using Microsoft.AspNet.Identity;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using OnlineLibrary.DataAccess;
using Microsoft.Owin.Security;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        public ActionResult Index()
        {
            if (HasAdminPrivileges(User))
            {
                List<Role> roles = RoleManager.Roles.Include( r => r.Users).ToList();                
                List<User> users = UserManager.Users.ToList();

                return View(new RoleViewModel
                {
                    Users = users,
                    Roles = roles
                });
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (HasAdminPrivileges(User))
            {
                Role role = await RoleManager.FindByIdAsync(id);
                string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
                IEnumerable<User> members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
                IEnumerable<User> nonMembers = UserManager.Users.Except(members);

                return View(new RoleEditModel
                {
                    Role = role,
                    Members = members,
                    NonMembers = nonMembers
                });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            if (HasAdminPrivileges(User))
            {
                if (ModelState.IsValid)
                {
                    if (await EditUsersRoles(model))
                        return RedirectToAction("Index");
                    else
                        return View("Error");
                }
                return View("Error", new string[] { "Role Not Found" });
            }
            return RedirectToAction("Index", "Home");
        }

        #region Helper Methods

        private bool HasAdminPrivileges(IPrincipal user)
        {
            return AccountController.IsFirstLogin || user.IsInRole(UserRoles.SysAdmin);
        }

        private async Task<bool> RemoveUserCurrentRoles(string userId)
        {
            var currentUserRoles = await UserManager.GetRolesAsync(userId);
            IdentityResult result = await UserManager.RemoveFromRoleAsync(userId, currentUserRoles.Single());

            return result.Succeeded;
        }

        private async Task<bool> AddUserToRole(string userId, RoleModificationModel model)
        {
            bool removeResult = await RemoveUserCurrentRoles(userId);
            IdentityResult addResult = await UserManager.AddToRoleAsync(userId, model.RoleName);

            return removeResult && addResult.Succeeded;
        }

        private async Task<bool> RemoveUserFromRole(string userId, RoleModificationModel model)
        {
            IdentityResult removeResult = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
            IdentityResult addResult = await UserManager.AddToRoleAsync(userId, UserRoles.User);

            return removeResult.Succeeded && addResult.Succeeded;
        }

        private async Task<bool> EditUsersRoles(RoleModificationModel model)
        {
            // Change roles.
            foreach (string userId in model.IdsToAdd ?? new List<string>())
            {
                if (!await AddUserToRole(userId, model))
                {
                    return false;
                }
            }
            foreach (string userId in model.IdsToDelete ?? new List<string>())
            {
                if (!await RemoveUserFromRole(userId, model))
                {
                    return false;
                }
            }

            // If current user has changed his/her role.
            if (model.IdsToAdd.Contains(User.Identity.GetUserId()) ||
                model.IdsToDelete.Contains(User.Identity.GetUserId()))
            {
                // Save current user name.
                var currentUserName = User.Identity.Name;

                // Sign the user out.
                AuthenticationManager.SignOut();

                // Sign the user back.
                if (AccountController.IsFirstLogin == true)
                {
                    AccountController.IsFirstLogin = false;
                }
                var currentUser = UserManager.FindByName(currentUserName);
                var identity = await UserManager.CreateIdentityAsync(
                    currentUser, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
            }

            return true;
        }

        #endregion Helper Methods
    }
}