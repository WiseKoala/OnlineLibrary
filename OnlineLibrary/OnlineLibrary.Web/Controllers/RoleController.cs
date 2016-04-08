using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OnlineLibrary.Web.Infrastructure.Abstract;

namespace OnlineLibrary.Web.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        public ActionResult Index()
        {
            if (HasAdminPrivileges(User))
                return View(RoleManager.Roles.Include(x => x.Users).ToList());

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Edit(string id)
        {
            if( HasAdminPrivileges(User) )
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
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            if ( HasAdminPrivileges(User) )
            {
                IdentityResult result;
                if (ModelState.IsValid)
                {
                    foreach (string userId in model.IdsToAdd ?? new string[] { })
                    {
                        result = await UserManager.AddToRoleAsync(userId, model.RoleName);
                        if (!result.Succeeded)
                        {
                            return View("Error", result.Errors);
                        }
                    }
                    foreach (string userId in model.IdsToDelete ?? new string[] { })
                    {
                        result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
                        if (!result.Succeeded)
                        {
                            return View("Error", result.Errors);
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View("Error", new string[] { "Role Not Found" });
            }
            return RedirectToAction("Index", "Home");
        }

        private bool HasAdminPrivileges(IPrincipal user)
        {
            return AccountController.IsFirstLogin || user.IsInRole("System administrator");
        }
    }
}