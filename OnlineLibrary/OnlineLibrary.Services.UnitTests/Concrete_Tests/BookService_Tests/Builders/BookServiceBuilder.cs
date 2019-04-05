using System;
using OnlineLibrary.DataAccess.Abstract;
using OnlineLibrary.Services.Concrete;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests.Builders
{
    public class BookServiceBuilder
    {
        private ILibraryDbContext _libDb;

        public BookServiceBuilder()
        {
            if (_libDb == null)
                _libDb = new LibraryDbBuilder().Build();
        }

        public BookService Build()
        {
            return new BookService(_libDb);
        }

        public BookServiceBuilder WithLib(ILibraryDbContext lib)
        {
            _libDb = lib;
            return this;
        }

        public BookServiceBuilder WithLib(Func<LibraryDbBuilder, LibraryDbBuilder> libBlueprint)
        {
            var lib = libBlueprint
                .Invoke(new LibraryDbBuilder())
                .Build();

            return WithLib(lib);
        }
    }
}
