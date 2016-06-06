using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.HomeViewModels
{
    public class BookSearchViewModel
    {
        public string Title { get; set; }

        public string Author { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }
    }
}