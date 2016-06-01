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
    internal class UpdateCategoryTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            // Test Category.
            var testCategory = new Category { Id = 2, Name = "Engineering" };

            // Categories.
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "History" },
                testCategory
            }
            .AsQueryable();

            DbSet<Category> categoriesSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();
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
            ((IQueryable<Category>)categoriesSet).Provider.Returns(categories.Provider);
            ((IQueryable<Category>)categoriesSet).Expression.Returns(categories.Expression);
            ((IQueryable<Category>)categoriesSet).ElementType.Returns(categories.ElementType);
            ((IQueryable<Category>)categoriesSet).GetEnumerator().Returns(categories.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.Categories.Returns(categoriesSet);
        }

        [Test]
        public void Should_UpdateCategory()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var categoryIdToUpdate = 2;
            var newCategoryName = "Architecture";

            // Act.
            var updatedCategory = categoryService.UpdateCategory(categoryIdToUpdate, newCategoryName);

            // Assert.
            Assert.AreEqual(newCategoryName, updatedCategory.Name);
        }

        [Test]
        public void Should_ThrowKeyNotFoundException_Given_InvalidCategoryId()
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var categoryIdToUpdate = -1;
            var newCategoryName = "NewName";

            var testDelegate = new TestDelegate(() => 
                categoryService.UpdateCategory(categoryIdToUpdate, newCategoryName));

            // Assert.
            Assert.Throws<KeyNotFoundException>(testDelegate);
        }

        [TestCase("")] // Empty name.
        [TestCase("New NameNew NameNew NameNew NameNew NameNew NameNew Name")] // Too long name.
        public void Should_ThrowArgumentException_Given_InvalidName(string newCategoryName)
        {
            // Arrange.
            var categoryService = new CategoryService(_dbContext);
            var categoryIdToUpdate = 2;

            var testDelegate = new TestDelegate(() =>
                categoryService.UpdateCategory(categoryIdToUpdate, newCategoryName));

            // Assert.
            Assert.Throws<ArgumentException>(testDelegate);
        }
    }
}