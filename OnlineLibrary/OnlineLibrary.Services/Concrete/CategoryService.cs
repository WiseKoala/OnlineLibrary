using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;

namespace OnlineLibrary.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private ILibraryDbContext _dbContext;

        public CategoryService(ILibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Category CreateCategory(string name)
        {
            Category category = _dbContext.Categories.Add(new Category { Name = name });
            _dbContext.SaveChanges();

            return category;
        }

        public SubCategory CreateSubCategory(int categoryId, string name)
        {
            Category category = _dbContext.Categories.SingleOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            else
            {
                // Try to find subcategory with the same name.
                SubCategory subCategory = _dbContext.SubCategories
                    .FirstOrDefault(sc => sc.Name.ToLower() == name.ToLower());

                if (subCategory != null)
                {
                    throw new ArgumentException("Subcategory with such name already exists.");
                }

                // Create new subcategory.
                subCategory = new SubCategory
                {
                    Name = name
                };

                category.SubCategories.Add(subCategory);
                _dbContext.SaveChanges();

                return subCategory;
            }
        }
    }
}
