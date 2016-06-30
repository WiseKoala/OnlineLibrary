using System;
using System.Collections.Generic;
using OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels;

namespace OnlineLibrary.Web.Models.BooksManagement
{
    public class DuplicateBookViewModel
    {
        public DuplicateBookViewModel()
        {
            Authors = new List<AuthorNameViewModel>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public IEnumerable<AuthorNameViewModel> Authors { get; set; }
    }
}