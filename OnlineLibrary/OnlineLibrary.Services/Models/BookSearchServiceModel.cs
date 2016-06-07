using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Services.Models
{
    public class BookSearchServiceModel
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime? PublishDate { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }
    }
}
