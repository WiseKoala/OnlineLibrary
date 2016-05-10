using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OnlineLibrary.Web.Models.RoleViewModels
{
    public class RoleViewModel
    {
        // List of users with their own role
        public List<UserAndRole> UsersAndTheirRoles { get; set; }

        // The list of roles
        public IEnumerable<IdentityRole> AllRoles { get; set; }
    }
}