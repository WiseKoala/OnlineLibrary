using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLibrary.Services.Abstract
{
    public interface IBookService : IService
    {
        int GetAmountOfAvailableCopies(int bookId);
        DateTime? GetEarliestAvailableDate(int bookId);
    }
}
