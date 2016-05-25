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
    public class IsBookAvailableTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            var bookCopies = new List<BookCopy>
                {
                    new BookCopy { Id = 1, Condition = BookCondition.Fine },
                    new BookCopy { Id = 2, Condition = BookCondition.VeryGood },
                }
           .AsQueryable();

            var loans = new List<Loan>
                {
                    new Loan { BookCopyId = 1, Status = LoanStatus.Pending },
                }
            .AsQueryable();

            var bookCopiesSet = Substitute.For<DbSet<BookCopy>, IQueryable<BookCopy>>();
            ((IQueryable<BookCopy>)bookCopiesSet).Provider.Returns(bookCopies.Provider);
            ((IQueryable<BookCopy>)bookCopiesSet).Expression.Returns(bookCopies.Expression);
            ((IQueryable<BookCopy>)bookCopiesSet).ElementType.Returns(bookCopies.ElementType);
            ((IQueryable<BookCopy>)bookCopiesSet).GetEnumerator().Returns(bookCopies.GetEnumerator());

            var loansSet = Substitute.For<DbSet<Loan>, IQueryable<Loan>>();
            ((IQueryable<Loan>)loansSet).Provider.Returns(loans.Provider);
            ((IQueryable<Loan>)loansSet).Expression.Returns(loans.Expression);
            ((IQueryable<Loan>)loansSet).ElementType.Returns(loans.ElementType);
            ((IQueryable<Loan>)loansSet).GetEnumerator().Returns(loans.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.BookCopies.Returns(bookCopiesSet);
            _dbContext.Loans.Returns(loansSet);
        }

        [Test]
        public void IsBookAvailable_Should_RetrunFalse_When_BookCopyIsLoaned()
        {
            // Arrange.           
            var expectedResult = false;
            var sut = new BookService(_dbContext);          
            // Book with status pending
            var bookCopyId = 1;
            
            // Act.
            var returnedResult = sut.IsBookCopyRemovable(bookCopyId);

            // Assert.
            Assert.AreEqual(expectedResult, returnedResult);
        }

        [Test]
        public void IsBookAvailable_Should_RetrunTrue_When_BookCopyIsNotLoaned()
        {
            // Arrange.
            var expectedResult = true;
            var sut = new BookService(_dbContext);
            // Book wich is not loaned 
            var bookCopyId = 2;

            // Act.
            var returnedResult = sut.IsBookCopyRemovable(bookCopyId);

            // Assert.
            Assert.AreEqual(expectedResult, returnedResult);
        }

        [Test]
        public void IsBookAvailable_Should_ThrowKeyNotFoundException_When_BookCopyDoesntExist()
        {
            // Arrange.
            var sut = new BookService(_dbContext);
            // Book that doesn't exist
            var bookCopyId = 3;

            // Act.
            var testDelegate = new TestDelegate(() => sut.IsBookCopyRemovable(bookCopyId));

            // Assert.
            Assert.Throws<KeyNotFoundException>(testDelegate);
        }
    }
}
