using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Concrete;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests
{
    [TestFixture]
    public class IsBookCopyRemovableTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            // Prepare test data.
            var bookCopies = new List<BookCopy>
                {
                    new BookCopy { Id = 1, Condition = BookCondition.Fine },
                    new BookCopy { Id = 2, Condition = BookCondition.VeryGood },
                    new BookCopy { Id = 3, Condition = BookCondition.Good },
                    new BookCopy { Id = 4, Condition = BookCondition.Poor },
                }
            .AsQueryable();

            var loans = new List<Loan>
                {
                    new Loan { BookCopyId = 1, Status = LoanStatus.Pending },
                    new Loan { BookCopyId = 3, Status = LoanStatus.Approved },
                    new Loan { BookCopyId = 4, Status = LoanStatus.InProgress }
                }
            .AsQueryable();

            // Prepare DbSets
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

            // Configure the DbContext.
            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.BookCopies.Returns(bookCopiesSet);
            _dbContext.Loans.Returns(loansSet);
        }

        [TestCase(2, true)]
        [TestCase(1, false)]
        public void Calculates(int bookCopyId, bool expectedResult)
        {
            // Arrange.
            var bookService = new BookService(_dbContext);

            // Act.
            bool actualResult = bookService.IsBookCopyRemovable(bookCopyId);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void ThrowsException()
        {
            // Arrange.
            var bookService = new BookService(_dbContext);
            int bookCopyId = 100; // Non-existing book copy.

            // Act.
            var testDelegate = new TestDelegate(() => bookService.IsBookCopyRemovable(bookCopyId));

            // Assert.
            Assert.Throws<KeyNotFoundException>(testDelegate);
        }
    }
}