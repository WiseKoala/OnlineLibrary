using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Concrete;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    class UpdateSubCategoryTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            // Test subcategory.
            var testSubCategory = new SubCategory
            {
                Id = 2,
                Name = "Classics",
                Category = new Category { Id = 1, Name = "Arts",
                    SubCategories = new List<SubCategory> {
                        new SubCategory { Id = 2, Name = "Classics" },
                        new SubCategory { Id = 5, Name = "Martial Arts" } } }
            };

            // Categories.
            var subCategories = new List<SubCategory>
            {
                new SubCategory { Id = 1, Name = "Modern Arts" },
                testSubCategory
            }
            .AsQueryable();

            DbSet<SubCategory> subcategoriesSet = Substitute.For<DbSet<SubCategory>, IQueryable<SubCategory>>();
            subcategoriesSet.Find(Arg.Any<object>()).Returns(callinfo =>
            {
                object[] idValues = callinfo.Arg<object[]>();
                if (idValues != null && idValues.Length == 1)
                {
                    int requestedId = (int)idValues[0];
                    return subCategories.SingleOrDefault(c => c.Id == requestedId);
                }

                return null;
            });
            ((IQueryable<SubCategory>)subcategoriesSet).Provider.Returns(subCategories.Provider);
            ((IQueryable<SubCategory>)subcategoriesSet).Expression.Returns(subCategories.Expression);
            ((IQueryable<SubCategory>)subcategoriesSet).ElementType.Returns(subCategories.ElementType);
            ((IQueryable<SubCategory>)subcategoriesSet).GetEnumerator().Returns(subCategories.GetEnumerator());
            subcategoriesSet.Include(Arg.Any<string>()).Returns(subcategoriesSet);

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.SubCategories.Returns(subcategoriesSet);
        }

        [Test]
        public void Should_UpdateSubCategory()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var subCategoryIdToUpdate = 2;
            var newSubCategoryName = "New Name";

            // Act.
            var updatedCategory = categoryService.UpdateSubCategory(subCategoryIdToUpdate, newSubCategoryName);

            // Assert.
            Assert.AreEqual(newSubCategoryName, updatedCategory.Name);
        }

        [Test]
        public void Should_ThrowArgumentException_Given_DuplicateSubcategoryNameInCategory()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var subCategoryId = 2;
            var newName = "Martial Arts";

            // Act.
            var sutDelegate = new TestDelegate(() =>
                categoryService.UpdateSubCategory(subCategoryId, newName));

            // Assert.
            Assert.Throws<ArgumentException>(sutDelegate);
        }

        [Test]
        [TestCase("")] // Empty name.
        [TestCase("New NameNew NameNew NameNew NameNew NameNew NameNew Name")] // Too long name.
        public void Should_ThrowArgumentException_Given_InvalidSubcategoryName(string newName)
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var subCategoryId = 2;

            // Act.
            var sutDelegate = new TestDelegate(() =>
                categoryService.UpdateSubCategory(subCategoryId, newName));

            // Assert.
            Assert.Throws<ArgumentException>(sutDelegate);
        }

        [Test]
        public void Should_ThrowKeyNotFoundException_Given_InvalidSubCategoryId()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var subCategoryIdToUpdate = -1;
            var newSubCategoryName = "NewName";

            var testDelegate = new TestDelegate(() =>
                categoryService.UpdateSubCategory(subCategoryIdToUpdate, newSubCategoryName));

            // Assert.
            Assert.Throws<KeyNotFoundException>(testDelegate);
        }
    }
}
