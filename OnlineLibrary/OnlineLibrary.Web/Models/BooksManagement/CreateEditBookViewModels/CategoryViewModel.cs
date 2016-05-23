using System.Collections.Generic;
using System.Web.Mvc;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SubCategoryViewModel Subcategory { get; set; }
        public IEnumerable<SelectListItem> Subcategories { get; set; }
    }
}