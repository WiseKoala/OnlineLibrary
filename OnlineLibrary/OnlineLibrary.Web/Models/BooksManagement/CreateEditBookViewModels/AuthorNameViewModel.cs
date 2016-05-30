using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    [AtLeastOnePropertySet(ErrorMessage = "Author must have at least one name field completed.")]
    public class AuthorNameViewModel
    {
        [DisplayName("First Name")]
        [MaxLength(50)]
        [RegularExpression("([A-Z])['a-zA-Z.,-]+", ErrorMessage = "Author's first name is not in a correct format.")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        [MaxLength(50)]
        [RegularExpression("([A-Z])['a-zA-Z.,-]+", ErrorMessage = "Author's middle name is not in a correct format.")]
        public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        [MaxLength(50)]
        [RegularExpression("([A-Z])['a-zA-Z.,-]+", ErrorMessage = "Author's last name is not in a correct format.")]
        public string LastName { get; set; }
    }
}