using System;
using NUnit.Framework;
using OnlineLibrary.Services.Models;
using OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests.Builders;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests
{
    [TestFixture]
    public class BookServiceFindTests
    {
        [Test]
        public void GivenATitle_FindTheBookContainingThatTitle()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookTitledAs("Foo Kek Bar"))
                .Build();

            var searchModel = new BookSearchServiceModel { Title = "Kek" };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(1, actual.NumberOfBooks);
        }

        [Test]
        public void ForNonexistentTitle_FindNoBooks()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookTitledAs("Foo"))
                .Build();

            var searchModel = new BookSearchServiceModel { Title = "Bar" };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(0, actual.NumberOfBooks);
        }

        [Test]
        public void GivenAnAuthor_FindTheBookWithThatAuthor()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookWrittenBy("Foo Kek Bar"))
                .Build();

            var searchModel = new BookSearchServiceModel { Author = "Foo Kek Bar" };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(1, actual.NumberOfBooks);
        }

        [Test]
        public void ForNonexistentAuthor_FindNoBooks()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookWrittenBy("Foo Kek Bar"))
                .Build();

            var searchModel = new BookSearchServiceModel { Author = "Kekos" };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(0, actual.NumberOfBooks);
        }

        [Test]
        public void GivenADescription_FindTheBookContainingThatDescription()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookDescribedAs("Foo Bar"))
                .Build();

            var searchModel = new BookSearchServiceModel { Description = "Bar" };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(1, actual.NumberOfBooks);
        }

        [Test]
        public void GivenAPublicationDate_FindTheBookInTheSameDay()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookPublishedOn(new DateTime(2000, 1, 1)))
                .Build();

            var searchModel = new BookSearchServiceModel { PublishDate = new DateTime(2000, 1, 1) };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(1, actual.NumberOfBooks);
        }

        [Test]
        public void GivenACategoryId_FindTheBookInTheSameCategory()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookInCategory(42))
                .Build();

            var searchModel = new BookSearchServiceModel { CategoryId = 42 };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(1, actual.NumberOfBooks);
        }

        [Test]
        public void GivenASubcategoryId_FindTheBookInTheSameSubcategory()
        {
            var sut = new BookServiceBuilder()
                .WithLib(lib => lib.WithBookInSubcategory(42))
                .Build();

            var searchModel = new BookSearchServiceModel { SubcategoryId = 42 };

            var actual = sut.Find(searchModel);
            Assert.AreEqual(1, actual.NumberOfBooks);
        }
    }
}
