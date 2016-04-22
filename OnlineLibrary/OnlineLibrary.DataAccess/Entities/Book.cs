using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.DataAccess.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string FrontCover { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        public virtual ICollection<Author> Authors { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; }
        public virtual ICollection<BookCopy> BookCopies { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}