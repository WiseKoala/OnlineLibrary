using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web;
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
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string FrontCover { get; set; }

        // Object for loading image 
        public HttpPostedFileBase Image { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        public IList<BookAuthorViewModel> Authors { get; set; }
        public IList<BookCopyViewModel> BookCopies { get; set; }
        public IEnumerable<SubCategoryViewModel> AllBookSubcategories { get; set; }
        public IList<int> SelectedSubcategories { get; set; }
    }
}