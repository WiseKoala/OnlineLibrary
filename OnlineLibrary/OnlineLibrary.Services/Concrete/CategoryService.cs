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
using System.Data.Entity;

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
            VerifyCategoryName(name);

            Category category = _dbContext.Categories.Add(new Category
            {
                Name = name.Trim()
            });
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
                VerifySubCategoryName(name, category);

                name = name.Trim();

                // Create new subcategory.
                SubCategory subCategory = _dbContext.SubCategories.Add(new SubCategory
                {
                    Name = name.Trim()
                });

                // Add to the corresponding category.
                category.SubCategories.Add(subCategory);
                _dbContext.SaveChanges();

                return subCategory;
            }
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

        public Category UpdateCategory(int categoryId, string newName)
        {
            Category category = _dbContext.Categories.Find(categoryId);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            VerifyCategoryName(newName);

            // Update data.
            category.Name = newName.Trim();
            _dbContext.SaveChanges();

            return category;
        }

        public SubCategory UpdateSubCategory(int subcategoryId, string newName)
        {
            SubCategory subCategory = _dbContext.SubCategories
                                                .Include(sc => sc.Category)
                                                .SingleOrDefault(sc => sc.Id == subcategoryId);

            if (subCategory == null)
            {
                throw new KeyNotFoundException("Subcategory not found.");
            }
            else
            {
                Category category = subCategory.Category;
                VerifySubCategoryName(newName, category);
            }

            // Update data.
            subCategory.Name = newName.Trim();

            _dbContext.SaveChanges();

            return subCategory;
        }

        private void VerifyCategoryName(string name)
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
        }

        private static void VerifySubCategoryName(string name, Category category)
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

            // Try to find subcategory with the same name that belongs
            // to the specified category.
            string trimmedName = name.Trim();
            bool duplicateExists = category.SubCategories
                .Any(sc => sc.Name.ToLower() == trimmedName.ToLower());

            if (duplicateExists)
            {
                throw new ArgumentException("Subcategory with such name already exists.");
            }
        }

        public void DeleteBookCategory(int cateogryId)
        {
            if (!CategoryExists(cateogryId))
            {
                throw new BookCategoryNotFoundException("Book Category does't exist");
            }
            if (!IsCategoryRemovable(cateogryId))
            {
                throw new BookCategoryIsNotRemovableException("Book category has subcategories");
            }

            var removedCategory = _dbContext.Categories.Find(cateogryId);
            _dbContext.Categories.Remove(removedCategory);
            _dbContext.SaveChanges();

        }

        public bool IsCategoryRemovable(int categoryId)
        {
            // Book can't have just category without subcategory, it's enough to check just the following condition.
            return !_dbContext.SubCategories.Any(sc => sc.CategoryId == categoryId);
        }

        public bool CategoryExists(int categoryId)
        {
            if (_dbContext.Categories.Find(categoryId) != null)
            {
                return true;
            }
            return false;
        }

        public void DeleteBookSubcategory(int subcategoryId)
        {
            if (!SubcategoryExists(subcategoryId))
            {
                throw new BookSubcategoryNotFoundException("Subcategory doesn't exist");
            }
            if (!IsSubcategoryRemovable(subcategoryId))
            {
                throw new BookSubcateogryIsNotRemovableException("Subcategory has books");
            }

            var removedSubcategory = _dbContext.SubCategories.Find(subcategoryId);
            _dbContext.SubCategories.Remove(removedSubcategory);
            _dbContext.SaveChanges();
        }

        public bool IsSubcategoryRemovable(int subcategoryId)
        {
            return !_dbContext.Books.Any(b => b.SubCategories.Any(sc => sc.Id == subcategoryId));
        }

        public bool SubcategoryExists(int subcategoryId)
        {
            if (_dbContext.SubCategories.Find(subcategoryId) != null)
            {
                return true;
            }
            return false;
        }        
    }
}
