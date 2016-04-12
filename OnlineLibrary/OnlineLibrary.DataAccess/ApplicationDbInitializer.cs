using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace OnlineLibrary.DataAccess
{
    internal class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var roles = new List<Role>
            {
                new Role() { Id = Guid.NewGuid().ToString(), Name = UserRoles.User },
                new Role() { Id = Guid.NewGuid().ToString(), Name = UserRoles.SysAdmin },
                new Role() { Id = Guid.NewGuid().ToString(), Name = UserRoles.Librarian }
            };
            var roleManager = new RoleManager<Role>(new RoleStore<Role>(context));
            roles.ForEach(r => roleManager.Create(r));

            context.SaveChanges();
        }
    }
}