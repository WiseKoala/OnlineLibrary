using OnlineLibrary.DataAccess;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models
{
    public class RoleViewModel
    {
        public List<User> Users { get; set; }
        private List<Role> _roles;

        public List<Role> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

    }
}