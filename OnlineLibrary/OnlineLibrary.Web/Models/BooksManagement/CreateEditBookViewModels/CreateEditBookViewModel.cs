using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class CreateEditBookViewModel
    {
        public CreateEditBookViewModel()
        {
            BookCopies = new List<BookCopyViewModel>();
            Authors = new List<BookAuthorViewModel>();
            AllBookConditions = new Dictionary<BookCondition, string>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "The Title field is too long.")]
        public string Title { get; set; }

        [Required]
        [MinLength(20, ErrorMessage = "Description field must be more consistent.")]
        public string Description { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "ISBN field is too short.")]
        public string ISBN { get; set; }

        public FrontCoverViewModel BookCover { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DateRange]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "There has to be at least one author.")]
        public IList<BookAuthorViewModel> Authors { get; set; }

        public IList<BookCopyViewModel> BookCopies { get; set; }

        public IDictionary<BookCondition, string> AllBookConditions { get; set; }

        [Required(ErrorMessage = "There has to be at least one book category.")]
        public IList<CategoryViewModel> BookCategories { get; set; }

        public string OldImagePath { get; set; }
    }
}