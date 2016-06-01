using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Abstract;
using System.Configuration;
using OnlineLibrary.Common.Exceptions;

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
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            // Verify max length.
            int maxLength = Convert.ToInt32(ConfigurationManager.AppSettings["CategorySubcategoryMaxLength"]);
            if (name.Length > maxLength)
            {
                throw new ArgumentException($"Category name is too long. Maximum length is {maxLength} characters");
            }

            // Try to find category with the same name.
            string trimmedName = name.Trim();
            bool duplicateExists = _dbContext.Categories
                .Any(c => c.Name.ToLower() == trimmedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException("Category with such name already exists.");
            }

            Category category = _dbContext.Categories.Add(new Category { Name = name });
            _dbContext.SaveChanges();

            return category;
        }

        public SubCategory CreateSubCategory(int categoryId, string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Subcategory name cannot be empty.");
            }

            // Verify max length.
            int maxLength = Convert.ToInt32(ConfigurationManager.AppSettings["CategorySubcategoryMaxLength"]);
            if (name.Length > maxLength)
            {
                throw new ArgumentException($"Subcategory name is too long. Maximum length is {maxLength} characters");
            }

            Category category = _dbContext.Categories.SingleOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            else
            {
                // Try to find subcategory with the same name that belongs
                // to the specified category.
                string trimmedName = name.Trim();
                bool duplicateExists = category.SubCategories
                    .Any(sc => sc.Name.ToLower() == trimmedName.ToLower());

                if (duplicateExists)
                {
                    throw new ArgumentException("Subcategory with such name already exists.");
                }

                // Create new subcategory.
                SubCategory subCategory = _dbContext.SubCategories.Add(new SubCategory { Name = name });

                // Add to the corresponding category.
                category.SubCategories.Add(subCategory);
                _dbContext.SaveChanges();

                return subCategory;
            }
        }

        public void DeleteBookCategory(int id)
        {
            if (!IsCategoryRemovable(id))
            {
                throw new BookCategoryIsNotRemovableException("Book category has books or subcategories");
            }

            var removedCategory = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            _dbContext.Categories.Remove(removedCategory);
            _dbContext.SaveChanges();
    
        }

        public bool IsCategoryRemovable(int categoryId)
        {
            // Book can't have just category without subcategory, it's enough to check just the following condition.
            return !_dbContext.SubCategories.Any(sc => sc.CategoryId == categoryId);              
        }

        public IEnumerable<SubCategory> GetSubCategories(int categoryId)
        {
            Category category = _dbContext.Categories.Find(categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            return category.SubCategories
                .OrderBy(sc => sc.Name)
                .ToList();
        }
    }
}
