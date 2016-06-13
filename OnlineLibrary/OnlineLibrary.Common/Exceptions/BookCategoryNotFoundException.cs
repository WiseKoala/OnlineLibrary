using System;
using System.Runtime.Serialization;

namespace OnlineLibrary.Common.Exceptions
{

    [Serializable]
    public class BookCategoryNotFoundException : Exception
    {
        public BookCategoryNotFoundException()
        {
        }

        public BookCategoryNotFoundException(string message)
            : base(message)
        {
        }

        public BookCategoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BookCategoryNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
