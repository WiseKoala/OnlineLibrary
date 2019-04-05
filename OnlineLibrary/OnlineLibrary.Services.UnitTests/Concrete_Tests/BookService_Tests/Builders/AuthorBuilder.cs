using System.Diagnostics;
using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.Services.UnitTests.Concrete_Tests.BookService_Tests.Builders
{
    public class AuthorBuilder
    {
        private string _firstName = "FooFirstname";
        private string _middleName = "FooMiddleName";
        private string _lastName = "FooLastName";

        public Author Build()
        {
            return new Author
            {
                FirstName = _firstName,
                MiddleName = _middleName,
                LastName = _lastName
            };
        }

        /*
         * To avoid too much logic here, we assume
         * that the given fullName will always consist of 3 parts
        */       
        public AuthorBuilder WithFullName(string fullName)
        {
            var nameParts = fullName.Split(' ');
            Debug.Assert(nameParts.Length == 3);

            _firstName = nameParts[0];
            _middleName = nameParts[1];
            _lastName = nameParts[2];

            return this;
        }
    }
}
