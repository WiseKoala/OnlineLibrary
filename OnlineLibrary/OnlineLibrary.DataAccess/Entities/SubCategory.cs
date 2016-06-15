using System.Collections.Generic;

namespace OnlineLibrary.DataAccess.Entities
{
    public class SubCategory
    {
        public SubCategory()
        {
            Books = new List<Book>();
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}