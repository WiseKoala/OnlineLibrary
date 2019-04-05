using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Concrete;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests
{
    [TestFixture]
    public class IsCategoryRemovableTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void init()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1 }
            }
            .AsQueryable();

            // Set subcategories with categories and without.
            var subcateogories = new List<SubCategory>
            {
                new SubCategory { Id = 1, CategoryId = categories.First().Id },
                new SubCategory { Id = 2 }
            }
            .AsQueryable();

            var categoriesSet = Substitute.For<DbSet<Category>, IQueryable<Category>>();
            ((IQueryable<Category>)categoriesSet).Provider.Returns(categories.Provider);
            ((IQueryable<Category>)categoriesSet).Expression.Returns(categories.Expression);
            ((IQueryable<Category>)categoriesSet).ElementType.Returns(categories.ElementType);
            ((IQueryable<Category>)categoriesSet).GetEnumerator().Returns(categories.GetEnumerator());

            var subcategoriesSet = Substitute.For<DbSet<SubCategory>, IQueryable<SubCategory>>();
            ((IQueryable<SubCategory>)subcategoriesSet).Provider.Returns(subcateogories.Provider);
            ((IQueryable<SubCategory>)subcategoriesSet).Expression.Returns(subcateogories.Expression);
            ((IQueryable<SubCategory>)subcategoriesSet).ElementType.Returns(subcateogories.ElementType);
            ((IQueryable<SubCategory>)subcategoriesSet).GetEnumerator().Returns(subcateogories.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.Categories.Returns(categoriesSet);
            _dbContext.SubCategories.Returns(subcategoriesSet);
        }
        [Test]
        public void Should_RetrunFalse_When_CategoryHasSubcategories()
        {
            // Arrange.
            var sut = new CategoryService(_dbContext);
            var categoryId = 1;
            bool expectedResult = false; 

            // Act.
            var result = sut.IsCategoryRemovable(categoryId);

            // Assert.
            Assert.AreEqual(result, expectedResult);
        }

        [Test]
        public void Should_RetrunTrue_When_CategoryHasNoSuncategories()
        {
            // Arrange.
            var sut = new CategoryService(_dbContext);
            var categoryId = 2;
            bool expectedResult = true;

            // Act.
            var result = sut.IsCategoryRemovable(categoryId);

            // Assert.
            Assert.AreEqual(result, expectedResult);
        }
    }
}
