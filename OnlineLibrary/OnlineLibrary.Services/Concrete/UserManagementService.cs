using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Services.Abstract;

namespace OnlineLibrary.Services.Concrete
{
    public class UserManagementService : UserManager<User>
    {
        private ILibraryDbContext _dbContext;

        public UserManagementService(ILibraryDbContext dbContext, IUserStore<User> store)
            : base(store)
        {
            _dbContext = dbContext;
        }

        public string GetUsernameById(string id)
        {
            var UserName = _dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => String.Concat(u.FirstName, u.LastName));

            return UserName.ToString();
        }

        public User GetUserByName(string name)
        {
            var user = _dbContext.Users
                .Where(u => u.UserName == name)
                .FirstOrDefault();

            return user;
        }

        public string GetTheUsernameByUsersName(string UsersName)
        {
            var user = GetUserByName(UsersName);

            string Username = String.Empty;

            if (user != null)
            {
                Username = UsersName;
                string firstName = Regex.Replace(user.FirstName, ".*?: ", String.Empty);
                string lastName = Regex.Replace(user.LastName, ".*?: ", String.Empty);
                Username = firstName + " " + lastName;
            }

            return Username;
        }

        public List<User> GetUserList()
        {
            var users = _dbContext.Users.Where(u => u.UserName != "Admin").ToList();

            return users;
        }
    }
}