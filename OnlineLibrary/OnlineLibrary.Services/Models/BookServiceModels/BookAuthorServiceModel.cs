namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class BookAuthorServiceModel
    {
        public int Id { get; set; }
        public AuthorNameServiceModel AuthorName { get; set; }
        public bool IsRemoved { get; set; }
    }
}