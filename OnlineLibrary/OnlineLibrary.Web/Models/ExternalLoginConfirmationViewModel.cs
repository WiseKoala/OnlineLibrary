using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}