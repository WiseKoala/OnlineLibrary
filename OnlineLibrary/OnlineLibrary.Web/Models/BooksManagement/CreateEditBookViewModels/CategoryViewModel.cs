using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<SubCategoryViewModel> BookSubCategories { get; set; }
    }
}