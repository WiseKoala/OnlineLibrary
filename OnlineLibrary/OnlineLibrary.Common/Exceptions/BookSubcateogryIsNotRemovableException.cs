using System;
using System.Runtime.Serialization;

namespace OnlineLibrary.Common.Exceptions
{
    [Serializable]
    public class BookSubcateogryIsNotRemovableException : Exception
    {
        public BookSubcateogryIsNotRemovableException()
        {
        }

        public BookSubcateogryIsNotRemovableException(string message)
            : base(message)
        {
        }

        public BookSubcateogryIsNotRemovableException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BookSubcateogryIsNotRemovableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
