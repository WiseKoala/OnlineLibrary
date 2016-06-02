using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Common.Exceptions
{
    [Serializable]
    public class BookCategoryIsNotRemovableException : Exception
    {
        public BookCategoryIsNotRemovableException()
        {
        }

        public BookCategoryIsNotRemovableException(string message)
            : base(message)
        {
        }

        public BookCategoryIsNotRemovableException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BookCategoryIsNotRemovableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
