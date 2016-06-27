using System.Collections.Generic;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class CategoryServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SubCategoryServiceModel Subcategory { get; set; }
        public bool IsRemoved { get; set; }
    }
}