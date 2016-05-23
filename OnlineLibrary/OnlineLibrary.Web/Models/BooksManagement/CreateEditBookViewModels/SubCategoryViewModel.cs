using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class SubCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [Display(Name = "Subcategory")]
        public string Name { get; set; }
        public CategoryViewModel Category { get; set; }

    }
}