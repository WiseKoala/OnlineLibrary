using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.HomeViewModels
{
    public class BooksListViewModel
    {
        public BooksListViewModel()
        {
            Books = new List<BookViewModel>();
        }

        public int NumberOfPages { get; set; }
        public IEnumerable<BookViewModel> Books { get; set; }

    }
}