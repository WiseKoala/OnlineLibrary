using System;
using System.Runtime.Serialization;

namespace OnlineLibrary.Common.Exceptions
{
    [Serializable]
    public class BookNotAvailableException : Exception
    {
        public BookNotAvailableException()
        {
        }

        public BookNotAvailableException(string message)
            : base(message)
        {
        }

        public BookNotAvailableException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BookNotAvailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}