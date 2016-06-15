using System.Collections.Generic;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.DataAccess.Entities
{
    public class BookCopy
    {
        public BookCopy()
        {
            Loans = new List<Loan>();
        }

        public int Id { get; set; }
        public int BookId { get; set; }
        public BookCondition Condition { get; set; }
        public bool IsLost { get; set; }

        public virtual Book Book { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}