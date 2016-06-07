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
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Concrete;
using OnlineLibrary.Services.Models;

namespace OnlibeLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests
{
    [TestFixture]
    public class FindTestClass
    {
        private ILibraryDbContext _dbContext;

        [OneTimeSetUp]
        public void Init()
        {
            // Authors
            var authors = new List<Author>
            {
                new Author() { Id = 1, FirstName = "Jamie", LastName = "Chan",
                    Books = new List<Book>
                    {
                        new Book()
                        {
                             Title = "Learn C# in One Day and Learn It Well: C# for Beginners with Hands-on Project (Learn Coding Fast with Hands-On Project) (Volume 3)",
                             ISBN = "1518800270",
                             FrontCover = "~/Content/Images/Books/front-covers/1518800270.jpg",
                             Description = "If you are a beginning programmer wanting to learn C# .NET programming, this book is the perfect introduction. It is too basic for experienced programmers, but it is just perfect for someone just starting out with C# programming, since it is easy to follow and all the concepts are explained very well.",
                             PublishDate = new DateTime(2015,10,27),
                        }
                    }
                },
                new Author() { Id = 2, FirstName = "R.", MiddleName = "B.", LastName = "Whitaker",
                    Books = new List<Book>
                    {
                        new Book()
                        {                                                      
                            Title = "The C# Player's Guide (2nd Edition)",
                            ISBN = "0985580127",
                            FrontCover = "~/Content/Images/Books/front-covers/0985580127.jpg",
                            Description = "The C# Player's Guide (2nd Edition) is the ultimate guide for people starting out with C#, whether you are new to programming, or an experienced vet. This guide takes you from your journey's beginning, through the most challenging parts of programming in C#, and does so in a way that is casual, informative, and fun. This version of the book is updated for C# 6.0, .NET 4.6, and Visual Studio 2015.",
                            PublishDate = new DateTime(2015,9,22),
                        }
                    }
                },
                new Author() { Id = 3, FirstName = "Brian", LastName = "Goetz",
                    Books = new List<Book>
                    {
                        new Book()
                        {                                                       
                            Title = "Java Concurrency in Practice 1st Edition",
                            ISBN = "7678678676",
                            FrontCover = "~/Content/Images/Books/front-covers/7678678676.jpg",
                            Description = "This book will give you a good grounding in concurrent programming in Java.",
                            PublishDate = new DateTime(2006,5,19),
                       },
                    }
                },
                new Author() { Id = 4, FirstName = "Tim", LastName = "Peierls",
                    Books = new List<Book>
                    {
                        new Book()
                        {                                                       
                            Title = "Java Concurrency in Practice 1st Edition",
                            ISBN = "7678678676",
                            FrontCover = "~/Content/Images/Books/front-covers/7678678676.jpg",
                            Description = "This book will give you a good grounding in concurrent programming in Java.",
                            PublishDate = new DateTime(2006,5,19),
                       },
                    }
                },
                new Author() { Id = 5, FirstName = "Joshua", LastName = "Bloch" ,
                    Books = new List<Book>
                    {
                        new Book()
                        {                                                       
                            Title = "Java Concurrency in Practice 1st Edition",
                            ISBN = "7678678676",
                            FrontCover = "~/Content/Images/Books/front-covers/7678678676.jpg",
                            Description = "This book will give you a good grounding in concurrent programming in Java.",
                            PublishDate = new DateTime(2006,5,19),
                       },
                    }
                },                      
                new Author() { Id = 6, FirstName = "Joseph", LastName = "Bowbeer" ,
                    Books = new List<Book>
                    {
                        new Book()
                        {                                                       
                            Title = "Java Concurrency in Practice 1st Edition",
                            ISBN = "7678678676",
                            FrontCover = "~/Content/Images/Books/front-covers/7678678676.jpg",
                            Description = "This book will give you a good grounding in concurrent programming in Java.",
                            PublishDate = new DateTime(2006,5,19),
                       },
                    }
                },                    
                new Author() { Id = 7, FirstName = "David", LastName = "Holmes" ,
                    Books = new List<Book>
                    {
                        new Book()
                        {                                                       
                            Title = "Java Concurrency in Practice 1st Edition",
                            ISBN = "7678678676",
                            FrontCover = "~/Content/Images/Books/front-covers/7678678676.jpg",
                            Description = "This book will give you a good grounding in concurrent programming in Java.",
                            PublishDate = new DateTime(2006,5,19),
                       },
                    }
                },                       
                new Author() { Id = 8, FirstName = "Doug", LastName = "Lea" ,
                    Books = new List<Book>
                    {
                        new Book()
                        {                                                       
                            Title = "Java Concurrency in Practice 1st Edition",
                            ISBN = "7678678676",
                            FrontCover = "~/Content/Images/Books/front-covers/7678678676.jpg",
                            Description = "This book will give you a good grounding in concurrent programming in Java.",
                            PublishDate = new DateTime(2006,5,19),
                       },
                    }
                },                          
                new Author() { Id = 9, FirstName = "Harper", LastName = "Lee" },                        
                new Author() { Id = 10, FirstName = "Bernard", LastName = "Cornwell" },                 
                new Author() { Id = 11, FirstName = "Peter", MiddleName = "H.", LastName = "Wilson" }   
            }
            .AsQueryable();

            // Categories
            var categories = new List<Category>
            {
                new Category() { Id = 1, Name = "Computers & Technology" },
                new Category() { Id = 2, Name = "Literature & Fiction" },
                new Category() { Id = 3, Name = "History" }
            }
            .AsQueryable();

            // SubCategories
            var subCategories = new List<SubCategory>
            {
                new SubCategory { Id = 1, Name = "C# Programming", Category = categories.First(c => c.Id == 1) },
                new SubCategory { Id = 2, Name = "Java Programming", Category = categories.First(c => c.Id == 1) },
                new SubCategory { Id = 3, Name = "Classics", Category = categories.First(c => c.Id == 2) },
                new SubCategory { Id = 4, Name = "Historical", Category = categories.First(c => c.Id == 2) },
                new SubCategory { Id = 5, Name = "Ancient Civilizations", Category = categories.First(c => c.Id == 3) },
                new SubCategory { Id = 6, Name = "World", Category = categories.First(c => c.Id == 3) },
            }
            .AsQueryable();

            // Books
            var books = new List<Book>
            {
            new Book() {                                                       // Category: Computers & Technology; Subcategory: C# Programming;
                         Title = "Learn C# in One Day and Learn It Well: C# for Beginners with Hands-on Project (Learn Coding Fast with Hands-On Project) (Volume 3)",
                         ISBN = "1518800270",
                         FrontCover = "~/Content/Images/Books/front-covers/1518800270.jpg",
                         Authors = new List<Author>() { authors.First(a => a.Id == 1) },
                         Description = "If you are a beginning programmer wanting to learn C# .NET programming, this book is the perfect introduction. It is too basic for experienced programmers, but it is just perfect for someone just starting out with C# programming, since it is easy to follow and all the concepts are explained very well.",
                         PublishDate = new DateTime(2015,10,27),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 1) }
                       },
            new Book() {                                                       // Category: Computers & Technology; Subcategory: C# Programming;
                         Title = "The C# Player's Guide (2nd Edition)",
                         ISBN = "0985580127",
                         FrontCover = "~/Content/Images/Books/front-covers/0985580127.jpg",
                         Authors = new List<Author>() { authors.First(a => a.Id == 2) },
                         Description = "The C# Player's Guide (2nd Edition) is the ultimate guide for people starting out with C#, whether you are new to programming, or an experienced vet. This guide takes you from your journey's beginning, through the most challenging parts of programming in C#, and does so in a way that is casual, informative, and fun. This version of the book is updated for C# 6.0, .NET 4.6, and Visual Studio 2015.",
                         PublishDate = new DateTime(2015,9,22),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 1) }
                       },
            new Book() {                                                       // Category: Computers & Technology; Subcategory: Java Programming;
                         Title = "Java Concurrency in Practice 1st Edition",
                         ISBN = "7678678676",
                         FrontCover = "~/Content/Images/Books/front-covers/7678678676.jpg",
                         Authors = new List<Author>() {
                                                             authors.First(a => a.Id == 3),
                                                             authors.First(a => a.Id == 4),
                                                             authors.First(a => a.Id == 5),
                                                             authors.First(a => a.Id == 6),
                                                             authors.First(a => a.Id == 7),
                                                             authors.First(a => a.Id == 8)
                                                           },
                         Description = "This book will give you a good grounding in concurrent programming in Java.",
                         PublishDate = new DateTime(2006,5,19),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 2) }
                       },
            }
            .AsQueryable();

            var authorsSet = Substitute.For<DbSet<Author>, IQueryable<Author>>();
            ((IQueryable<Author>)authorsSet).Provider.Returns(authors.Provider);
            ((IQueryable<Author>)authorsSet).Expression.Returns(authors.Expression);
            ((IQueryable<Author>)authorsSet).ElementType.Returns(authors.ElementType);
            ((IQueryable<Author>)authorsSet).GetEnumerator().Returns(authors.GetEnumerator());

            var booksSet = Substitute.For<DbSet<Book>, IQueryable<Book>>();
            ((IQueryable<Book>)booksSet).Provider.Returns(books.Provider);
            ((IQueryable<Book>)booksSet).Expression.Returns(books.Expression);
            ((IQueryable<Book>)booksSet).ElementType.Returns(books.ElementType);
            ((IQueryable<Book>)booksSet).GetEnumerator().Returns(books.GetEnumerator());

            _dbContext = Substitute.For<ILibraryDbContext>();
            _dbContext.Authors.Returns(authorsSet);
            _dbContext.Books.Returns(booksSet);
        }

        [Test]
        public void Should_RetrunFalse_When_BookCopyIsLoaned()
        {
            // Arrange.           
            var expectedResult = 1;
            var sut = new BookService(_dbContext);
            var model = new BookSearchServiceModel
            {
                Title = "Learn C#"
            };

            // Act.
            var returnedResult = sut.Find(model);

            // Assert.
            Assert.AreEqual(expectedResult, returnedResult.Count());
        }

    }
}
