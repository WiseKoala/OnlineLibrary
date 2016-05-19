using OnlineLibrary.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.BooksManagement.CreateEditBookViewModels
{
    public class BookCopyViewModel
    {
        public int Id { get; set; }
        public BookCondition BookCondition { get; set; }
        public bool IsToBeDeleted { get; set; }
    }
}