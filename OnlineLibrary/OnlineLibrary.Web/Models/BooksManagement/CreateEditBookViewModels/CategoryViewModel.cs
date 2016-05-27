using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public SubCategoryViewModel Subcategory { get; set; }
        public IEnumerable<SelectListItem> Subcategories{ get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public bool IsRemoved { get; set; }
    }
}