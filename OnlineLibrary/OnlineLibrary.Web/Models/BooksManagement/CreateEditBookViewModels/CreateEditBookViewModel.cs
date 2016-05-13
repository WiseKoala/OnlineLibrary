using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class CreateEditBookViewModel
    {
        public CreateEditBookViewModel()
        {
            BookCategories = new List<CategoryViewModel>();
            BookCopies = new List<BookCopyViewModel>();
            Authors = new List<BookAuthorViewModel>();
        }
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string FrontCover { get; set; }

        public HttpPostedFileBase Image { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        public List<BookAuthorViewModel> Authors { get; set; }
        public List<CategoryViewModel> BookCategories { get; set; }
        public List<BookCopyViewModel> BookCopies { get; set; }

    }
}