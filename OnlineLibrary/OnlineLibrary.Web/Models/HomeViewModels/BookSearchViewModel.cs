using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Models.HomeViewModels
{
    public class BookSearchViewModel
    {
        public BookSearchViewModel()
        {
            Categories = new List<SelectListItem>();
            Subcategories = new List<SelectListItem>();
        }

        public string Title { get; set; }

        public string Author { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public DateTime? PublishDate { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        [Display(Name = "Subcategory")]
        public int? SubcategoryId { get; set; }

        public IEnumerable<SelectListItem> Subcategories { get; set; }

        public int PageNumber { get; set; } = 1;
    }
}