using System;
using System.Collections.Generic;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class DuplicateBookServiceModel
    {
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public IEnumerable<DuplicateAuthorServiceModel> Authors { get; set; }
    }
}