using NSubstitute;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests.Builders
{
    public class LibraryDbBuilder
    {
        private readonly List<Author> _authors = new List<Author>();
        private readonly List<Book> _books = new List<Book>();
        private readonly List<Category> _categories = new List<Category>();
        private readonly List<SubCategory> _subCategories = new List<SubCategory>();

        public ILibraryDbContext Build()
        {
            var authorsDbSet = SubstituteToDbSet(_authors);
            var booksDbSet = SubstituteToDbSet(_books);
            var categoriesDbSet = SubstituteToDbSet(_categories);

            var subcategoriesDbSet = SubstituteToDbSet(_subCategories);
            subcategoriesDbSet.Include("Foo").ReturnsForAnyArgs(x => subcategoriesDbSet);

            var library = Substitute.For<ILibraryDbContext>();
            library.Authors.Returns(authorsDbSet);
            library.Books.Returns(booksDbSet);
            library.Categories.Returns(categoriesDbSet);
            library.SubCategories.Returns(subcategoriesDbSet);

            return library;
        }
        
        public LibraryDbBuilder WithAuthor(Author author)
        {
            _authors.Add(author);
            return this;
        }

        public LibraryDbBuilder WithBook(Book book)
        {
            _books.Add(book);
            return this;
        }

        public LibraryDbBuilder WithCategory(Category category)
        {
            _categories.Add(category);
            return this;
        }

        public LibraryDbBuilder WithSubCategory(SubCategory subCategory)
        {
            _subCategories.Add(subCategory);
            return this;
        }

        public LibraryDbBuilder WithBookTitledAs(string bookName)
        {
            var book = new BookBuilder()
                .WithTitle(bookName)
                .Build();

            return WithBook(book);
        }

        public LibraryDbBuilder WithBookWrittenBy(string authorFullName)
        {
            var author = new AuthorBuilder()
                .WithFullName(authorFullName)
                .Build();

            var book = new BookBuilder()
                .WithAuthor(author)
                .Build();

            author.Books.Add(book);
            return this
                .WithBook(book)
                .WithAuthor(author);
        }

        public LibraryDbBuilder WithBookDescribedAs(string bookDescription)
        {
            var book = new BookBuilder()
                .WithDescription(bookDescription)
                .Build();

            return WithBook(book);
        }

        public LibraryDbBuilder WithBookPublishedOn(DateTime bookPublicationDate)
        {
            var book = new BookBuilder()
                .WithPublicationDate(bookPublicationDate)
                .Build();

            return WithBook(book);
        }

        public LibraryDbBuilder WithBookInCategory(int categoryId)
        {
            var category = new Category { Id = categoryId };
            var subcategory = new SubCategory { CategoryId = category.Id };

            var book = new BookBuilder()
                .WithSubcategory(subcategory)
                .Build();

            subcategory.Books.Add(book);
            return this
                .WithBook(book)
                .WithSubCategory(subcategory)
                .WithCategory(category);
        }

        public LibraryDbBuilder WithBookInSubcategory(int subcategoryId)
        {
            var subcategory = new SubCategory { Id = subcategoryId };

            var book = new BookBuilder()
                .WithSubcategory(subcategory)
                .Build();

            subcategory.Books.Add(book);
            return this
                .WithBook(book)
                .WithSubCategory(subcategory);
        }

        private IQueryable<T> SubstituteToDbSet<T>(IEnumerable<T> entities) where T : class
        {
            var queryableEntities = entities.AsQueryable();

            IQueryable<T> dbSet = Substitute.For<DbSet<T>, IQueryable<T>>();
            dbSet.Provider.Returns(queryableEntities.Provider);
            dbSet.Expression.Returns(queryableEntities.Expression);
            dbSet.ElementType.Returns(queryableEntities.ElementType);
            dbSet.GetEnumerator().Returns(ci => queryableEntities.GetEnumerator());

            return dbSet;
        }
    }
}
