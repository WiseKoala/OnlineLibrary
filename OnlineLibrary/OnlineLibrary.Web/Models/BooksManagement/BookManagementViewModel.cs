using System;

namespace OnlineLibrary.Web.Models.BooksManagement
{
    public class BookManagementViewModel
    {
        public int Id { get; set; }
        public string FrontCover { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string PublishDate { get; set; }
    }
}