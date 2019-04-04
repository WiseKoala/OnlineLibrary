using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    class IsSubcategoryRemovableTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void init()
        {
            // Set subcategories with categories and without.
            var subcateogories = new List<SubCategory>
            {
                new SubCategory { Id = 1 },
                new SubCategory { Id = 2 }
            }
            .AsQueryable();

            var book = new Book
            {
                Id = 1,
                SubCategories = new List<SubCategory> { subcateogories.FirstOrDefault(sc => sc.Id == 1) }
            };

            var books = new List<Book>
            {
                book
            }
            .AsQueryable();

            var subcategoriesSet = Substitute.For<DbSet<SubCategory>, IQueryable<SubCategory>>();
            ((IQueryable<SubCategory>)subcategoriesSet).Provider.Returns(subcateogories.Provider);
            ((IQueryable<SubCategory>)subcategoriesSet).Expression.Returns(subcateogories.Expression);
            ((IQueryable<SubCategory>)subcategoriesSet).ElementType.Returns(subcateogories.ElementType);
            ((IQueryable<SubCategory>)subcategoriesSet).GetEnumerator().Returns(subcateogories.GetEnumerator());

            var booksSet = Substitute.For<DbSet<Book>, IQueryable<Book>>();
            ((IQueryable<Book>)booksSet).Provider.Returns(books.Provider);
            ((IQueryable<Book>)booksSet).Expression.Returns(books.Expression);
            ((IQueryable<Book>)booksSet).ElementType.Returns(books.ElementType);
            ((IQueryable<Book>)booksSet).GetEnumerator().Returns(books.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.SubCategories.Returns(subcategoriesSet);
            _dbContext.Books.Returns(booksSet);
        }

        [Test]
        public void Should_RetrunFalse_When_BooksHasCurrentSubcategory()
        {
            // Arrange.
            var sut = new CategoryService(_dbContext);
            var expectedResult = false;
            var subcategoryId = 1;

            // Act.
            var result = sut.IsSubcategoryRemovable(subcategoryId);

            // Assert.
            Assert.AreEqual(result, expectedResult);
        }

        [Test]
        public void Should_RetrunTrue_When_NoBooksHasCurrentSubcategory()
        {
            // Arrange.
            var sut = new CategoryService(_dbContext);
            var expectedResult = true;
            var subcategoryId = 2;

            // Act.
            var result = sut.IsSubcategoryRemovable(subcategoryId);

            // Assert.
            Assert.AreEqual(result, expectedResult);
        }
    }
}
