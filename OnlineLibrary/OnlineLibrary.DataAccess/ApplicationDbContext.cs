using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;
using System.Data.Entity;

namespace OnlineLibrary.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Role> IdentityRoles { get; set; }
    }
}