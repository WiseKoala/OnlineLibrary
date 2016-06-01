using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.Services.Concrete;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
{
    [TestFixture]
    class DeleteBookSubcategoryTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void init()
        {
            var subcateogories = new List<SubCategory>
            {
                new SubCategory { Id = 1 },
                new SubCategory { Id = 2 },
                new SubCategory { Id = 3 }
            };
            var subcateogoriesQueryable = subcateogories.AsQueryable();

            var subcategoryId = 1;
            var book = new Book
            {
                Id = 1,
                SubCategories = new List<SubCategory> { subcateogories.FirstOrDefault(sc => sc.Id == subcategoryId) }
            };

            var books = new List<Book>
            {
                book
            }
            .AsQueryable();

            var booksSet = Substitute.For<DbSet<Book>, IQueryable<Book>>();
            ((IQueryable<Book>)booksSet).Provider.Returns(books.Provider);
            ((IQueryable<Book>)booksSet).Expression.Returns(books.Expression);
            ((IQueryable<Book>)booksSet).ElementType.Returns(books.ElementType);
            ((IQueryable<Book>)booksSet).GetEnumerator().Returns(books.GetEnumerator());

            var subcategoriesSet = Substitute.For<DbSet<SubCategory>, IQueryable<SubCategory>>();
            ((IQueryable<SubCategory>)subcategoriesSet).Provider.Returns(subcateogoriesQueryable.Provider);
            ((IQueryable<SubCategory>)subcategoriesSet).Expression.Returns(subcateogoriesQueryable.Expression);
            ((IQueryable<SubCategory>)subcategoriesSet).ElementType.Returns(subcateogoriesQueryable.ElementType);
            ((IQueryable<SubCategory>)subcategoriesSet).GetEnumerator().Returns(subcateogoriesQueryable.GetEnumerator());

            // Category to remove.
            var subcategoryIdToRemove = 2;
            // Mock Remove method.
            subcategoriesSet.When(x => x.Remove(subcateogories.FirstOrDefault(c => c.Id == subcategoryIdToRemove)))
                            .Do(x => subcateogories.Remove(subcateogories.FirstOrDefault(c => c.Id == subcategoryIdToRemove)));

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.SubCategories.Returns(subcategoriesSet);
            _dbContext.Books.Returns(booksSet);
        }

        [Test]
        public void Should_DeleteSubcategory_When_IsSubcategoryRemovable_ReturnTrue()
        {
            // Arrange.
            var sut = Substitute.For<CategoryService>(_dbContext);

            // Act.
            // Current number of category minus one to be removed.
            var expectedResult = _dbContext.SubCategories.Count() - 1;
            sut.DeleteBookSubcategory(2);
            // Number of categories after remove method.
            var result = _dbContext.SubCategories.Count();

            // Assert.
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Should_ThrowBookSubcateogryIsNotRemovableException_When_IsSubcategoryRemovable_ReturnFalse()
        {
            // Arrange.
            var sut = Substitute.For<CategoryService>(_dbContext);
            int subcategoryId = 1;
            // Act.
            var testDelegate = new TestDelegate(() => sut.DeleteBookSubcategory(subcategoryId));

            // Assert.
            Assert.Throws<BookSubcateogryIsNotRemovableException>(testDelegate);
        }

    }
}
