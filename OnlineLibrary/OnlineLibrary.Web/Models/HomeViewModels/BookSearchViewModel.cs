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
        public string Title { get; set; }

        public string Author { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public DateTime? PublishDate { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }

        [Display(Name = "Categories")]
        public IEnumerable<SelectListItem> Categories { get; set; }

        public int? SelectedCategoryId { get; set; }

        public int? SelectedSubcategoryId { get; set; }
    }
}