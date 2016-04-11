using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using OnlineLibrary.DataAccess.Enums;
using System.Linq;

namespace OnlineLibrary.DataAccess
{
    internal class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var roles = new List<Role>
            {
                new Role() { Id = Guid.NewGuid().ToString(), Name = "User" },
                new Role() { Id = Guid.NewGuid().ToString(), Name = "System administrator" },
                new Role() { Id = Guid.NewGuid().ToString(), Name = "Librarian" }
            };
            var roleManager = new RoleManager<Role>(new RoleStore<Role>(context));
            roles.ForEach(r => roleManager.Create(r));

            // Authors
            var authors = new List<Author>
            {
                new Author() { Id = 1, FirstName = "Jamie", MiddleName = "", LastName = "Chan"},        // Id = 1
                new Author() { Id = 2, FirstName = "R.", MiddleName = "B.", LastName = "Whitaker" },    // Id = 2
                new Author() { Id = 3, FirstName = "Brian", MiddleName = "", LastName = "Goetz" },      // Id = 3 <---
                new Author() { Id = 4, FirstName = "Tim", MiddleName = "", LastName = "Peierls" },      // Id = 4    |
                new Author() { Id = 5, FirstName = "Joshua", MiddleName = "", LastName = "Bloch" },     // Id = 5    | Same book
                new Author() { Id = 6, FirstName = "Joseph", MiddleName = "", LastName = "Bowbeer" },   // Id = 6    |
                new Author() { Id = 7, FirstName = "David", MiddleName = "", LastName = "Holmes" },     // Id = 7    |
                new Author() { Id = 8, FirstName = "Doug", MiddleName = "", LastName = "Lea" },         // Id = 8 <---
                new Author() { Id = 9, FirstName = "Harper", MiddleName = "", LastName = "Lee" },       // Id = 9
                new Author() { Id = 10, FirstName = "Bernard", MiddleName = "", LastName = "Cornwell" }, // Id = 10
                new Author() { Id = 11, FirstName = "Peter", MiddleName = "H.", LastName = "Wilson" }
            };

            // Categories
            var categories = new List<Category>
            {
                new Category() { Id = 1, Name = "Computers & Technology" },
                new Category() { Id = 2, Name = "Literature & Fiction" },
                new Category() { Id = 3, Name = "History" }
            };

            // SubCategories
            var subCategories = new List<SubCategory>
            {
                new SubCategory { Id = 1, Name = "C# Programming", Category = categories.First(c => c.Id == 1) },
                new SubCategory { Id = 2, Name = "Java Programming;", Category = categories.First(c => c.Id == 1) },
                new SubCategory { Id = 3, Name = "Classics", Category = categories.First(c => c.Id == 2) },
                new SubCategory { Id = 4, Name = "Historical", Category = categories.First(c => c.Id == 2) },
                new SubCategory { Id = 5, Name = "Ancient Civilizations", Category = categories.First(c => c.Id == 3) }
            };

            // Book items
            var bookCopies = new List<BookCopy>
            {
                new BookCopy() { Id = 1, Condition = BookCondition.New },           // Id = 1   <-- same book  
                new BookCopy() { Id = 2, Condition = BookCondition.Poor },          // Id = 2   <--
                                 
                new BookCopy() { Id = 3, Condition = BookCondition.New },           // Id = 3   <--
                new BookCopy() { Id = 4, Condition = BookCondition.Poor },          // Id = 4     | same book
                new BookCopy() { Id = 5, Condition = BookCondition.Good },          // Id = 5   <--
                                 
                new BookCopy() { Id = 6, Condition = BookCondition.Good },          // Id = 6
                                
                new BookCopy() { Id = 7, Condition = BookCondition.Poor },           // Id = 7    <-- same book
                new BookCopy() { Id = 8, Condition = BookCondition.Fair },           // Id = 8    <--
                                
                new BookCopy() { Id = 9, Condition = BookCondition.New },            // Id = 9    <--
                new BookCopy() { Id = 10, Condition = BookCondition.New },           // Id = 10     | same book
                new BookCopy() { Id = 11, Condition = BookCondition.New },           // Id = 11   <--

                new BookCopy() { Id = 12, Condition = BookCondition.New }            // Id = 12
            };

            // Boooks
            var books = new List<Book>
            {
            new Book() {                                                       // Category: Computers & Technology; Subcategory: C# Programming;
                         Title = "Learn C# in One Day and Learn It Well: C# for Beginners with Hands-on Project (Learn Coding Fast with Hands-On Project) (Volume 3)",
                         ISBN = "1518800270",
                         Authors = new List<Author>() { authors.First(a => a.Id == 1) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 1), bookCopies.First(b => b.Id == 2) },
                         Description = "If you are a beginning programmer wanting to learn C# .NET programming, this book is the perfect introduction. It is too basic for experienced programmers, but it is just perfect for someone just starting out with C# programming, since it is easy to follow and all the concepts are explained very well.",
                         PublishDate = new DateTime(2015,10,27),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 1) }
                       },
            new Book() {                                                       // Category: Computers & Technology; Subcategory: C# Programming;
                         Title = "The C# Player's Guide (2nd Edition)",
                         ISBN = "0985580127",
                         Authors = new List<Author>() { authors.First(a => a.Id == 2) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 3), bookCopies.First(b => b.Id == 4), bookCopies.First(b => b.Id == 5) },
                         Description = "The C# Player's Guide (2nd Edition) is the ultimate guide for people starting out with C#, whether you are new to programming, or an experienced vet. This guide takes you from your journey's beginning, through the most challenging parts of programming in C#, and does so in a way that is casual, informative, and fun. This version of the book is updated for C# 6.0, .NET 4.6, and Visual Studio 2015.",
                         PublishDate = new DateTime(2015,9,22),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 1) }
                       },
            new Book() {                                                       // Category: Computers & Technology; Subcategory: Java Programming;
                         Title = "Java Concurrency in Practice 1st Edition",
                         ISBN = "7678678676",
                         Authors = new List<Author>() {
                                                             authors.First(a => a.Id == 3),
                                                             authors.First(a => a.Id == 4),
                                                             authors.First(a => a.Id == 5),
                                                             authors.First(a => a.Id == 6),
                                                             authors.First(a => a.Id == 7),
                                                             authors.First(a => a.Id == 8)
                                                           },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 6) },
                         Description = "This book will give you a good grounding in concurrent programming in Java.",
                         PublishDate = new DateTime(2006,5,19),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 2) }
                       },
            new Book() {
                         Title = "To Kill a Mockingbird",        // Category: Literature & Fiction; Subcategory: Classics;
                         ISBN = "778587687",
                         Authors = new List<Author>() { authors.First(a => a.Id == 9) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 7), bookCopies.First(b => b.Id == 8) },
                         Description = "The unforgettable novel of a childhood in a sleepy Southern town and the crisis of conscience that rocked it, To Kill A Mockingbird became both an instant bestseller and a critical success when it was first published in 1960. It went on to win the Pulitzer Prize in 1961 and was later made into an Academy Award-winning film, also a classic. Compassionate, dramatic, and deeply moving, To Kill A Mockingbird takes readers to the roots of human behavior - to innocence and experience, kindness and cruelty, love and hatred, humor and pathos. Now with over 18 million copies in print and translated into forty languages, this regional story by a young Alabama woman claims universal appeal. Harper Lee always considered her book to be a simple love story. Today it is regarded as a masterpiece of American literature.",
                         PublishDate = new DateTime(1988,10,11),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 3) }
                       },
            new Book() {                                       // Category: Literature & Fiction; Subcategory: Historical;
                         Title = "Warriors of the Storm: A Novel (Saxon Tales)",
                         ISBN = "32830723",
                         Authors = new List<Author>() { authors.First(a => a.Id == 10) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 9), bookCopies.First(b => b.Id == 10), bookCopies.First(b => b.Id == 11) },
                         Description = "The ninth installment of Bernard Cornwell’s bestselling series chronicling the epic saga of the making of England, “like Game of Thrones, but real” (The Observer, London)—the basis for The Last Kingdom, the hit BBC America television series.",
                         PublishDate = new DateTime(2016,1,19),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 4) }
                       },
            new Book() {                                       // Category: History; Subcategory: Ancient Civilizations;
                         Title = "Heart of Europe: A History of the Holy Roman Empire",
                         ISBN = "189737695",
                         Authors = new List<Author>() { authors.First(a => a.Id == 11) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 12) },
                         Description = "The Holy Roman Empire lasted a thousand years, far longer than ancient Rome. Yet this formidable dominion never inspired the awe of its predecessor. Voltaire distilled the disdain of generations when he quipped it was neither holy, Roman, nor an empire. Yet as Peter Wilson shows, the Holy Roman Empire tells a millennial story of Europe better than the histories of individual nation-states. And its legacy can be seen today in debates over the nature of the European Union.",
                         PublishDate = new DateTime(2016,4,4),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 5) }
                       }
            };
            books.ForEach(b => context.Books.Add(b));

            context.SaveChanges();
        }
    }
}