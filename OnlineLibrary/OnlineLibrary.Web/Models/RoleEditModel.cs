﻿using OnlineLibrary.DataAccess.Entities;
using System.Collections.Generic;

namespace OnlineLibrary.Web.Models
{
    public class RoleEditModel
    {
        public Role Role { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }
    }
}