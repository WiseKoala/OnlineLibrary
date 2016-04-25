using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using OnlineLibrary.DataAccess.Abstract;

namespace OnlineLibrary.Services.Concrete
{
    public class RoleManagementService : RoleManager<Role>, IDisposable
    {
        private ILibraryDbContext _dbContext;

        public RoleManagementService(ILibraryDbContext dbContext, RoleStore<Role> store)
            : base(store)
        {
            _dbContext = dbContext;
        }

        public List<IdentityRole> GetRoleList()
        {
            var roles = _dbContext.Roles.Where(r => r.Name != UserRoles.SuperAdmin).ToList();

            return roles;
        }
    }
}