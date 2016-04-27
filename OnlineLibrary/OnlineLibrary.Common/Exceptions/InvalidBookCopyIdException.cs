using System;
using System.Runtime.Serialization;

namespace OnlineLibrary.Common.Exceptions
{
    [Serializable]
    public class InvalidBookCopyIdException : Exception
    {
        public InvalidBookCopyIdException()
        {
        }

        public InvalidBookCopyIdException(string message)
            : base(message)
        {
        }

        public InvalidBookCopyIdException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidBookCopyIdException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}