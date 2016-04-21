using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLibrary.Web.Models
{
    public class BookDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public string FrontCover { get; set; }
        public DateTime PublishDate { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int NrOfBooks { get; set; }
        public string HowManyInThisCondition { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public int AvailableCopies { get; set; }
        public DateTime? EarliestDateAvailable { get; set; }
    }
}