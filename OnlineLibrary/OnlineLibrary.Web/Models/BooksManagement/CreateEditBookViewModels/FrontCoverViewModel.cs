using System.ComponentModel.DataAnnotations;
using System.Web;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    [AtLeastOnePropertySet(ErrorMessage = "The Front Cover must be chosen.")]
    public class FrontCoverViewModel
    {
        [Display(Name = "Front Cover")]
        public string FrontCover { get; set; }

        [Display(Name = "Front Cover")]
        public HttpPostedFileBase Image { get; set; }
    }
}