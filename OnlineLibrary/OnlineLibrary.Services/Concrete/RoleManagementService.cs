using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLibrary.Services.Concrete
{
    public class RoleManagementService : RoleManager<Role>, IDisposable
    {
        public RoleManagementService(RoleStore<Role> store) : base(store)
        {
        }

        public static RoleManagementService Create(IdentityFactoryOptions<RoleManagementService> options, IOwinContext context)
        {
            return new RoleManagementService(new RoleStore<Role>(context.Get<ApplicationDbContext>()));
        }

        public static List<IdentityRole> GetRoleList(IOwinContext context)
        {
            var dbContext = context.Get<ApplicationDbContext>();

            var roles = dbContext.Roles.Where(r => r.Name != UserRoles.SuperAdmin).ToList();

            return roles;
        }
    }
}