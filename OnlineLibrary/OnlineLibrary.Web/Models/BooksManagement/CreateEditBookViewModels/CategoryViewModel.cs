using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SubCategoryViewModel Subcategory { get; set; }
        public IEnumerable<SelectListItem> Subcategories{ get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}