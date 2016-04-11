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
                new Author() { FirstName = "Jamie", MiddleName = "", LastName = "Chan"},        // Id = 1
                new Author() { FirstName = "R.", MiddleName = "B.", LastName = "Whitaker" },    // Id = 2
                new Author() { FirstName = "Brian", MiddleName = "", LastName = "Goetz" },      // Id = 3 <---
                new Author() { FirstName = "Tim", MiddleName = "", LastName = "Peierls" },      // Id = 4    |
                new Author() { FirstName = "Joshua", MiddleName = "", LastName = "Bloch" },     // Id = 5    | Same book
                new Author() { FirstName = "Joseph", MiddleName = "", LastName = "Bowbeer" },   // Id = 6    |
                new Author() { FirstName = "David", MiddleName = "", LastName = "Holmes" },     // Id = 7    |
                new Author() { FirstName = "Doug", MiddleName = "", LastName = "Lea" },         // Id = 8 <---
                new Author() { FirstName = "Harper", MiddleName = "", LastName = "Lee" },       // Id = 9
                new Author() { FirstName = "Bernard", MiddleName = "", LastName = "Cornwell" }, // Id = 10
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
                         Description = "",
                         PublishDate = new DateTime(2015,10,27),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 1) }
                       },
            new Book() {                                                       // Category: Computers & Technology; Subcategory: C# Programming;
                         Title = "The C# Player's Guide (2nd Edition)",
                         ISBN = "0985580127",
                         Authors = new List<Author>() { authors.First(a => a.Id == 2) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 3), bookCopies.First(b => b.Id == 4), bookCopies.First(b => b.Id == 5) },
                         Description = "",
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
                         PublishDate = new DateTime(2006,5,19),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 2) }
                       },
            new Book() {
                         Title = "To Kill a Mockingbird",        // Category: Literature & Fiction; Subcategory: Classics;
                         ISBN = "778587687",
                         Authors = new List<Author>() { authors.First(a => a.Id == 9) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 7), bookCopies.First(b => b.Id == 8) },
                         PublishDate = new DateTime(1988,10,11),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 3) }
                       },
            new Book() {                                       // Category: Literature & Fiction; Subcategory: Historical;
                         Title = "Warriors of the Storm: A Novel (Saxon Tales)",
                         ISBN = "32830723",
                         Authors = new List<Author>() { authors.First(a => a.Id == 10) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 9), bookCopies.First(b => b.Id == 10), bookCopies.First(b => b.Id == 11) },
                         PublishDate = new DateTime(2016,1,19),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 4) }
                       },
            new Book() {                                       // Category: History; Subcategory: Ancient Civilizations;
                         Title = "Heart of Europe: A History of the Holy Roman Empire",
                         ISBN = "189737695",
                         Authors = new List<Author>() { authors.First(a => a.Id == 11) },
                         BookCopies = new List<BookCopy>() { bookCopies.First(b => b.Id == 12) },
                         PublishDate = new DateTime(2016,4,4),
                         SubCategories = new List<SubCategory> { subCategories.First(sc => sc.Id == 5) }
                       }
            };
            books.ForEach(b => context.Books.Add(b));

            context.SaveChanges();
        }
    }
}