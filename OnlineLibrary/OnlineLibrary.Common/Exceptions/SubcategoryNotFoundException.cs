using System;
using System.Runtime.Serialization;

namespace OnlineLibrary.Common.Exceptions
{
    [Serializable]
    public class SubcategoryNotFoundException : Exception
    {
        public SubcategoryNotFoundException()
        {
        }

        public SubcategoryNotFoundException(string message)
            : base(message)
        {
        }

        public SubcategoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected SubcategoryNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
