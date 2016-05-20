﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web;
using OnlineLibrary.Web.Infrastructure.CustomAttributes;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class CreateEditBookViewModel
    {
        public CreateEditBookViewModel()
        {
            BookCopies = new List<BookCopyViewModel>();
            Authors = new List<BookAuthorViewModel>();
            AllBookSubcategories = new List<SubCategoryViewModel>();
            SelectedSubcategories = new List<int>();
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
        [MaxLength(13, ErrorMessage = "ISBN field is too long.")]
        public string ISBN { get; set; }

        public FrontCoverViewModel BookCover { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DateRange]
        public DateTime PublishDate { get; set; }

        [NotEmptyList(ErrorMessage = "There has to be at least one author.")]
        public IList<BookAuthorViewModel> Authors { get; set; }
        public IList<BookCopyViewModel> BookCopies { get; set; }
        public IEnumerable<SubCategoryViewModel> AllBookSubcategories { get; set; }
        public IList<int> SelectedSubcategories { get; set; }

    }
}