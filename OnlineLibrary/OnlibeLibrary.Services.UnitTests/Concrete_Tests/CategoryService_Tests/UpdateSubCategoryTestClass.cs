using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Concrete;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    class UpdateSubCategoryTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            // Test Category.
            var testSubCategory = new SubCategory { Id = 2, Name = "Classics" };

            // Categories.
            var categories = new List<SubCategory>
            {
                new SubCategory { Id = 1, Name = "Modern Arts" },
                testSubCategory
            }
            .AsQueryable();

            DbSet<SubCategory> categoriesSet = Substitute.For<DbSet<SubCategory>, IQueryable<SubCategory>>();
            categoriesSet.Find(Arg.Any<object>()).Returns(callinfo =>
            {
                object[] idValues = callinfo.Arg<object[]>();
                if (idValues != null && idValues.Length == 1)
                {
                    int requestedId = (int)idValues[0];
                    return categories.SingleOrDefault(c => c.Id == requestedId);
                }

                return null;
            });
            ((IQueryable<SubCategory>)categoriesSet).Provider.Returns(categories.Provider);
            ((IQueryable<SubCategory>)categoriesSet).Expression.Returns(categories.Expression);
            ((IQueryable<SubCategory>)categoriesSet).ElementType.Returns(categories.ElementType);
            ((IQueryable<SubCategory>)categoriesSet).GetEnumerator().Returns(categories.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.SubCategories.Returns(categoriesSet);
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
        public void Should_ThrowKeyNotFoundException_Given_InvalidCategoryId()
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
