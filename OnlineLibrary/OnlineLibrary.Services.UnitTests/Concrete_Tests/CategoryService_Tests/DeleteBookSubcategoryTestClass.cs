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

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.CategoryService_Tests
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
            var subcategoryIdRemovable = 2;
            var subcategoryIdNotRemovable = 1;
            // Mock Remove method.
            subcategoriesSet.When(x => x.Remove(subcateogories.FirstOrDefault(c => c.Id == subcategoryIdRemovable)))
                            .Do(x => subcateogories.Remove(subcateogories.FirstOrDefault(c => c.Id == subcategoryIdRemovable)));

            subcategoriesSet.Find(subcategoryIdRemovable).Returns(x => subcateogories.Find(sc => sc.Id == subcategoryIdRemovable));
            subcategoriesSet.Find(subcategoryIdNotRemovable).Returns(x => subcateogories.Find(sc => sc.Id == subcategoryIdNotRemovable));


            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.SubCategories.Returns(subcategoriesSet);
            _dbContext.Books.Returns(booksSet);
        }

        [Test]
        public void Should_DeleteSubcategory_When_IsSubcategoryRemovable_ReturnTrue()
        {
            // Arrange.
            var sut = Substitute.For<CategoryService>(_dbContext);
            var subcategoryId = 2;
            // Act.
            // Current number of category minus one to be removed.
            var expectedResult = _dbContext.SubCategories.Count() - 1;
            sut.DeleteBookSubcategory(subcategoryId);
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

        [Test]
        public void Should_ThrowBookSubcateogryNotFoundException_When_IsSubcategoryRemovable_ReturnFalse()
        {
            // Arrange.
            var sut = Substitute.For<CategoryService>(_dbContext);
            int subcategoryId = 10;
            // Act.
            var testDelegate = new TestDelegate(() => sut.DeleteBookSubcategory(subcategoryId));

            // Assert.
            Assert.Throws<BookSubcategoryNotFoundException>(testDelegate);
        }

    }
}
