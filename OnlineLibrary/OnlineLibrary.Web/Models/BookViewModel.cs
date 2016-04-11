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
        public string Authors { get; set; }
        public DateTime PublishDate { get; set; }
    }
}