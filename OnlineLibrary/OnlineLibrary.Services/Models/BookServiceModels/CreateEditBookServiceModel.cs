using System;
using System.Collections.Generic;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class CreateEditBookServiceModel
    {
        public CreateEditBookServiceModel()
        {
            BookCopies = new List<BookCopyServiceModel>();
            Authors = new List<BookAuthorServiceModel>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ISBN { get; set; }

        public FrontCoverServiceModel BookCover { get; set; }

        public DateTime PublishDate { get; set; }

        public IList<BookAuthorServiceModel> Authors { get; set; }

        public IList<BookCopyServiceModel> BookCopies { get; set; }

        public IList<CategoryServiceModel> BookCategories { get; set; }

        public string OldImagePath { get; set; }
    }
}