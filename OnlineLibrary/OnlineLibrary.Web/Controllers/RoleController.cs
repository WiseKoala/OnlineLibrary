using Microsoft.AspNet.Identity;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Services.Concrete;
using OnlineLibrary.Web.Infrastructure.Abstract;
using OnlineLibrary.Web.Models.RoleViewModels;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

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
                InitializeUserNameSessionVariable();

                var model = new RoleViewModel();

                model.AllRoles = _roleService.GetRoleList().OrderBy(r => r.Name);
                model.UsersAndTheirRoles = new List<UserAndRole>();

                foreach (var user in _userService.GetUserList())
                {
                    string userName = DbContext.Users.Where(u => u.Id == user.Id).Single().UserName;

                    string roleId = user.Roles.Single().RoleId;

                    string roleName = DbContext.Roles.Where(r => r.Id == roleId).Single().Name;

                    model.UsersAndTheirRoles.Add(
                        new UserAndRole
                        {
                            UserName = userName,
                            RoleName = roleName
                        });
                }

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Index(string userName, string roleName)
        {
            if (HasAdminPrivileges(User))
            {
                if (ModelState.IsValid)
                {
                    var user = DbContext.Users
                                        .Include(u => u.Roles)
                                        .Where(u => u.UserName == userName).SingleOrDefault();

                    if (user != null)
                    {
                        string roleId = user.Roles.Single().RoleId;

                        var oldRoleName = DbContext.Roles.Where(r => r.Id == roleId).Single().Name;

                        if (await _userService.RemoveUserFromRole(user.Id, oldRoleName) && await _userService.AddUserToRole(user.Id, roleName))
                        {
                            return RedirectToAction("Index", "Role");
                        }
                        else
                        {
                            return View("Error", new string[] { "Role or User was Not Found" });
                        }
                    }

                    return View("Error", new string[] { "User was Not Found" });
                }

                return View("Error", new string[] { "Role or User was Not Found" });
            }

            return RedirectToAction("Index", "Home");
        }

        #region Helper Methods

        private bool HasAdminPrivileges(IPrincipal user)
        {
            return IsFirstLogin() || user.IsInRole(UserRoles.SysAdmin) || user.IsInRole(UserRoles.SuperAdmin);
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