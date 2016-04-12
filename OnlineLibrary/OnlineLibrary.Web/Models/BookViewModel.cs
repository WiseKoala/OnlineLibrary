using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public string FrontCover { get; set; }
        public DateTime PublishDate { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}