using OnlineLibrary.DataAccess.Enums;
using System;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class CurrentUserLoansViewModel
    {
        public string Title { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public LoanStatus Status { get; set; }
    }
}