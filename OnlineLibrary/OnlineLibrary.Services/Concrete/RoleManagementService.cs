using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLibrary.Services.Concrete
{
    public class RoleManagementService : RoleManager<Role>, IDisposable
    {
        private ILibraryDbContext _dbContext;

        public RoleManagementService(ILibraryDbContext dbContext, IRoleStore<Role, string> store)
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