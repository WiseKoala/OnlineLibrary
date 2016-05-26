using System.Collections.Generic;

namespace OnlineLibrary.DataAccess.Entities
{
    public class Category
    {
        public Category()
        {
            SubCategories = new List<SubCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}