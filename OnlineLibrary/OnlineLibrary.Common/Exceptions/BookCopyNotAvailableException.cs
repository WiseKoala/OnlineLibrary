using System;
using System.Runtime.Serialization;

namespace OnlineLibrary.Common.Exceptions
{
    [Serializable]
    public class BookCopyNotAvailableException : Exception
    {
        public BookCopyNotAvailableException()
        {
        }

        public BookCopyNotAvailableException(string message)
            : base(message)
        {
        }

        public BookCopyNotAvailableException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BookCopyNotAvailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}