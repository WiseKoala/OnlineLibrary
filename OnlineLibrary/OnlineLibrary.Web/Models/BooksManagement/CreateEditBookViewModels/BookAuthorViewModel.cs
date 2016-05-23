namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class BookAuthorViewModel
    {
        public int Id { get; set; }

        public AuthorNameViewModel AuthorName { get; set; }

        public bool IsRemoved { get; set; }
    }
}