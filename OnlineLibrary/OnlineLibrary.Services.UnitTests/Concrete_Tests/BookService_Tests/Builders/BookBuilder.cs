using OnlineLibrary.DataAccess.Entities;
using System;
using System.Collections.Generic;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests.Builders
{
    public class BookBuilder
    {
        private int _id = 0;
        private string _title = "Foo Title";
        private string _description = "Foo Description";
        private string _isbn = "00000000";
        private string _frontCover = "/foo/bar.png";
        private DateTime _publishDate = DateTime.MinValue;

        private readonly List<Author> _authors = new List<Author>();
        private readonly List<SubCategory> _subCategories = new List<SubCategory>();

        public Book Build()
        {
            return new Book
            {
                Id = _id,
                Title = _title,
                Description = _description,
                ISBN = _isbn,
                FrontCover = _frontCover,
                PublishDate = _publishDate,

                Authors = _authors,
                SubCategories = _subCategories,
            };
        }

        public BookBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public BookBuilder WithAuthor(Author author)
        {
            _authors.Add(author);
            return this;
        }

        public BookBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public BookBuilder WithPublicationDate(DateTime date)
        {
            _publishDate = date;
            return this;
        }

        public BookBuilder WithSubcategory(SubCategory subCategory)
        {
            _subCategories.Add(subCategory);
            return this;
        }
    }
}
