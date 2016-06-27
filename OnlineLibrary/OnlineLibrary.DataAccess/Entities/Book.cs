using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.DataAccess.Entities
{
    public class Book
    {
        public Book()
        {
            Authors = new List<Author>();
            SubCategories = new List<SubCategory>();
            BookCopies = new List<BookCopy>();
            Loans = new List<Loan>();
        }

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

        public override int GetHashCode()
        {
            return this.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Book)
            {
                return ((Book)obj).Id == this.Id;
            }
            return false;
        }
    }
}