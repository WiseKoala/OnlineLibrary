using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models
{
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public List<string> IdsToAdd { get; set; } = new List<string>();
        public List<string> IdsToDelete { get; set; } = new List<string>();
    }
}