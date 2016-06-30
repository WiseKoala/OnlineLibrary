using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class DuplicateBookServiceModel
    {
        public DuplicateBookServiceModel()
        {
            Authors = new List<DuplicateAuthorServiceModel>();
        }

        private string _title;

        public string Title
        {
            get { return _title; }

            set
            {
                _title = Regex.Replace(value, @"[^A-Za-z]", string.Empty);
            }
        }
        public int Id { get; set; }
        public DateTime PublishDate { get; set; }
        public IEnumerable<DuplicateAuthorServiceModel> Authors { get; set; }
    }
}