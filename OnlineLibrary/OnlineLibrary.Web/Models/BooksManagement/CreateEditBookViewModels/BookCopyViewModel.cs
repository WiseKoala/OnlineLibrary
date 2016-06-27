using OnlineLibrary.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class BookCopyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Condition field is required for a book copy.")]
        [Display(Name = "Condition")]
        public BookCondition BookCondition { get; set; }
        public bool IsToBeDeleted { get; set; }
        public bool IsLost { get; set; }
    }
}