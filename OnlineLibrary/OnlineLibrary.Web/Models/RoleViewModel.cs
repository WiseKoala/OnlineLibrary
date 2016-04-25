using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using System.Collections.Generic;

namespace OnlineLibrary.Web.Models
{
    public class RoleViewModel
    {
        // The user names for roles (in the same order as in the Roles list below)
        public List<string> UserNames { get; set; }

        // The list of roles
        public List<IdentityRole> Roles { get; set; }
    }
}