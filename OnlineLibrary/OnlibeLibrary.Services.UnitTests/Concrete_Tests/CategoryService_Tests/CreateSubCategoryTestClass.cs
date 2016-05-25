using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Concrete;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    internal class CreateSubCategoryTestClass
    {
        private ILibraryDbContext _dbContext;
        private DbSet<Category> _categoriesSet;
        private DbSet<SubCategory> _subCategoriesSet;
        private Category _categoryToTest;

        [OneTimeSetUp]
        public void Init()
        {
            var subCategoriesForCategory1 = Substitute.For<ICollection<SubCategory>>();

            // Complex.
            var subCategories = new List<SubCategory>
            {
                new SubCategory { Id = 1, Name = "Classics", Category = _categoryToTest }
            };

            var subCategoriesForCategory2 = Substitute.For<ICollection<SubCategory>>();
            subCategoriesForCategory2.GetEnumerator().Returns(subCategories.GetEnumerator());

            _categoryToTest = new Category { Id = 2, Name = "Programming", SubCategories = subCategoriesForCategory2 };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "History", SubCategories = subCategoriesForCategory1 },
                _categoryToTest
            }
            .AsQueryable();

            // Prepare DbSets
            _categoriesSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();
            ((IQueryable<Category>)_categoriesSet).Provider.Returns(categories.Provider);
            ((IQueryable<Category>)_categoriesSet).Expression.Returns(categories.Expression);
            ((IQueryable<Category>)_categoriesSet).ElementType.Returns(categories.ElementType);
            ((IQueryable<Category>)_categoriesSet).GetEnumerator().Returns(categories.GetEnumerator());

            _subCategoriesSet = Substitute.For<DbSet<SubCategory>>();

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.Categories.Returns(_categoriesSet);
            _dbContext.SubCategories.Returns(_subCategoriesSet);
        }

        [Test]
        public void Should_CreateNewSubCategory_Given_NewUniqueName()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var categoryId = 2;
            var subCategoryName = "Python";
            var category = _categoriesSet.Single(c => c.Id == categoryId);

            // Act.
            categoryService.CreateSubCategory(categoryId, subCategoryName);

            // Assert.
            category.SubCategories.Received(1).Add(Arg.Any<SubCategory>());
            _dbContext.Received(1).SaveChanges();
        }

        [Test]
        public void Should_CreateNewSubCategory_Given_NameBelongingToOtherSubcategoryFromDifferentCategory()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var categoryId = 1;
            var subCategoryName = "Classics";
            var category = _categoriesSet.Single(c => c.Id == categoryId);

            // Act.
            categoryService.CreateSubCategory(categoryId, subCategoryName);

            // Assert.
            category.SubCategories.Received(1).Add(Arg.Any<SubCategory>());
            _dbContext.Received(1).SaveChanges();
        }

        [Test]
        public void Should_ThrowArgumentException()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var categoryId = 2;
            var subCategoryName = "Classics";

            // Act.
            var sutDelegate = new TestDelegate(() => 
                categoryService.CreateSubCategory(categoryId, subCategoryName));

            // Assert.
            Assert.Throws<ArgumentException>(sutDelegate);
        }

        [Test]
        public void Should_ThrowKeyNotFoundException()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var categoryId = -1;
            var subCategoryName = "New Category";

            // Act.
            var sutDelegate = new TestDelegate(() =>
                categoryService.CreateSubCategory(categoryId, subCategoryName));

            // Assert.
            Assert.Throws<KeyNotFoundException>(sutDelegate);
        }
    }
}