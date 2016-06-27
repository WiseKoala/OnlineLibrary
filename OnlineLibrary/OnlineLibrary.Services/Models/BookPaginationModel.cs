using OnlineLibrary.DataAccess.Entities;
using System.Collections.Generic;

namespace OnlineLibrary.Services.Models
{
    public class BookPaginationModel
    {
        public BookPaginationModel()
        {
            Books = new List<Book>();
        }
        public int NumberOfBooks { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
