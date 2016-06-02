using System;
using System.Runtime.Serialization;

namespace OnlineLibrary.Common.Exceptions
{
    [Serializable]
    public class BookSubcategoryNotFoundException : Exception
    {
        public BookSubcategoryNotFoundException()
        {
        }

        public BookSubcategoryNotFoundException(string message)
            : base(message)
        {
        }

        public BookSubcategoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BookSubcategoryNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
