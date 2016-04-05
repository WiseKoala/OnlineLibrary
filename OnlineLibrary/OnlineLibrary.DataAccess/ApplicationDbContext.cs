using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<OnlineLibrary.DataAccess.Entities.Role> IdentityRoles { get; set; }
    }
}
