using OnlineLibrary.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLibrary.DataAccess.Entities;

namespace OnlineLibrary.Services.Abstract
{
    public interface IBookService : IService
    {
        int GetNumberOfAvailableCopies(int bookId);
        DateTime? GetEarliestAvailableDate(int bookId);
        string GetConditionDescription(BookCondition cond);
        BookCopy DeleteBookCopy(int id);
        bool IsBookCopyRemovable(int id);
        Book DeleteBook(int id);
    }
}
