using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.Web.Models.RoleViewModels
{
    public class UserAndRole
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}