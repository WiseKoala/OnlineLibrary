using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.DataAccess
{
    class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var roles = new List<Role>
            {
                new Role() { Id = Guid.NewGuid().ToString(), Name = "User" },
                new Role() { Id = Guid.NewGuid().ToString(), Name = "Admin" },
                new Role() { Id = Guid.NewGuid().ToString(), Name = "Librarian" }
            };
            var roleManager = new RoleManager<Role>(new RoleStore<Role>(context));
            roles.ForEach(r => roleManager.Create(r));

            context.SaveChanges();
        }
    }
}
