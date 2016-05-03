using System;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class HistoryLoanViewModel
    {
        public string ISBN { get; set; }
        public int? BookCopyId { get; set; }
        public string UserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public HistoryStatus Status { get; set; }
    }
}