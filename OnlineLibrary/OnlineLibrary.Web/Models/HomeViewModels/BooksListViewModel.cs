using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.HomeViewModels
{
    public class BooksListViewModel
    {
        public BookSearchViewModel SearchData { get; set; }
        public IEnumerable<BookViewModel> Books { get; set; }
    }
}