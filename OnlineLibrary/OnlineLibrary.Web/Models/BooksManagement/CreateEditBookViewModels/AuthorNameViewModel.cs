using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class AuthorNameViewModel
    {
        [DisplayName("First Name")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        [MaxLength(50)]
        public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        [MaxLength(50)]
        public string LastName { get; set; }
    }
}