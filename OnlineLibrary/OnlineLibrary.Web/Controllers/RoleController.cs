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
using OnlineLibrary.Services.Concrete;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Concrete;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        private UserManagementService _userService;
        private RoleManagementService _roleService;

        public RoleController(ILibraryDbContext dbContext, UserManagementService userService, RoleManagementService roleService)
            : base(dbContext)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public ActionResult Index()
        {
            if (HasAdminPrivileges(User))
            {              
                if (!IsUserNameSessionVariableSet())
                {
                    InitializeUserNameSessionVariable();
                }

                // Initializing the model to be passed to the view.
                var model = new RoleViewModel();

                // Initializing model components.
                List<IdentityRole> roles = _roleService.GetRoleList();
                model.Roles = roles;

                List<User> users = _userService.GetUserList();
                model.UserNames = new List<string>();

                // Creating a temporary variable to store the information for the model.
                string userNames;

                // Iterating through the roles to get the usernames for each role.
                foreach (var role in roles)
                {
                    if (role.Users == null || !role.Users.Any())
                    {
                        // Show a message if there are no users in a role.
                        userNames = "No users have this role.";
                    }
                    else
                    {
                        // Get all users in this role.
                        var allUsersInRole = role.Users.Select(userInRole => userInRole.UserId);

                        // Get the usernames list for the users in this role.
                        var userNamesList = users.Where(user => allUsersInRole.Contains(user.Id)).Select(user => user.UserName);

                        // Join the usernames list as a string.
                        userNames = string.Join(", ", userNamesList);
                    }
                    
                    // Add the information about usernames to the model.
                    model.UserNames.Add(userNames);
                }

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (HasAdminPrivileges(User))
            {
                Role role = await _roleService.FindByIdAsync(id);
                string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
                IEnumerable<User> members = _userService.Users.Where(x => memberIDs.Any(y => y == x.Id));
                IEnumerable<User> nonMembers = _userService.Users.Where( u => u.UserName != "Admin").Except(members);

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
            return IsFirstLogin() || user.IsInRole(UserRoles.SysAdmin) || user.IsInRole(UserRoles.SuperAdmin);
        }

        private async Task<bool> RemoveUserCurrentRoles(string userId)
        {
            var currentUserRoles = await _userService.GetRolesAsync(userId);
            IdentityResult result = await _userService.RemoveFromRoleAsync(userId, currentUserRoles.Single());

            return result.Succeeded;
        }

        private async Task<bool> AddUserToRole(string userId, RoleModificationModel model)
        {
            bool removeResult = await RemoveUserCurrentRoles(userId);
            IdentityResult addResult = await _userService.AddToRoleAsync(userId, model.RoleName);

            return removeResult && addResult.Succeeded;
        }

        private async Task<bool> RemoveUserFromRole(string userId, RoleModificationModel model)
        {
            IdentityResult removeResult = await _userService.RemoveFromRoleAsync(userId, model.RoleName);
            IdentityResult addResult = await _userService.AddToRoleAsync(userId, UserRoles.User);

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
                var currentUser = _userService.FindByName(currentUserName);
                var identity = await _userService.CreateIdentityAsync(
                    currentUser, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
            }

            return true;
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

        #endregion Helper Methods
    }
}