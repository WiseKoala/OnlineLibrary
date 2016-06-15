using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using OnlineLibrary.Services.Models;
using System;
using System.Collections.Generic;

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
        string FormatISBN(string ISBN);
        IEnumerable<Book> Find(BookSearchServiceModel model);
        bool IsValidISBN(string ISBN);
    }
}
