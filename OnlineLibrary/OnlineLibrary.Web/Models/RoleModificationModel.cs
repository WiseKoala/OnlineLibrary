using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Web.Models
{
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }

        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}