using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Concrete;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests
{
    [TestFixture]
    public class ChangeIsLostStatusTest
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            var bookCopies = new List<BookCopy>
                {
                    new BookCopy { Id = 1, Condition = BookCondition.Fine, IsLost = false },
                    new BookCopy { Id = 2, Condition = BookCondition.VeryGood, IsLost = true },
                };
            var bookCopyAsQueryable = bookCopies.AsQueryable();

            var isNotLostBookCopyId = 1;
            var isLostBookCopyId = 2;

            var bookCopiesSet = Substitute.For<DbSet<BookCopy>, IQueryable<BookCopy>>();
            ((IQueryable<BookCopy>)bookCopiesSet).Provider.Returns(bookCopyAsQueryable.Provider);
            ((IQueryable<BookCopy>)bookCopiesSet).Expression.Returns(bookCopyAsQueryable.Expression);
            ((IQueryable<BookCopy>)bookCopiesSet).ElementType.Returns(bookCopyAsQueryable.ElementType);
            ((IQueryable<BookCopy>)bookCopiesSet).GetEnumerator().Returns(bookCopyAsQueryable.GetEnumerator());

            bookCopiesSet.Find(isNotLostBookCopyId).Returns(x => bookCopies.Find(bc => bc.Id == isNotLostBookCopyId));
            bookCopiesSet.Find(isLostBookCopyId).Returns(x => bookCopies.Find(bc => bc.Id == isLostBookCopyId));

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.BookCopies.Returns(bookCopiesSet);
        }

        [Test]
        public void Should_ChangeIsLostStatusToTrue_IfIsLostStatusIsFalse()
        {
            // Arrange.
            var sut = new LibrarianService(_dbContext);
            var bookCopyId = 1;
            var isLost = true;
            var expectedResult = true;

            // Act.
            sut.ChangeIsLostStatus(bookCopyId, isLost);
            var actualResult = _dbContext.BookCopies.Find(bookCopyId).IsLost;

            // Assert.
            Assert.AreEqual(actualResult, expectedResult);
        }

        [Test]
        public void Should_ChangeIsLostStatusToFalse_IfIsLostStatusIsTrue()
        {
            // Arrange.
            var sut = new LibrarianService(_dbContext);
            var bookCopyId = 2;
            var isLost = false;
            var expectedResult = false;

            // Act.
            sut.ChangeIsLostStatus(bookCopyId, isLost);
            var actualResult = _dbContext.BookCopies.Find(bookCopyId).IsLost;

            // Assert.
            Assert.AreEqual(actualResult, expectedResult);
        }
    }
}
