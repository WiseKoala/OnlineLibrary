using System;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class HistoryLoanViewModel
    {
        public string ISBN { get; set; }
        public int? BookCopyId { get; set; }
        public string UserName { get; set; }
        public string StartDate { get; set; }
        public string ExpectedReturnDate { get; set; }
        public string ActualReturnDate { get; set; }
        public HistoryStatus Status { get; set; }
    }
}