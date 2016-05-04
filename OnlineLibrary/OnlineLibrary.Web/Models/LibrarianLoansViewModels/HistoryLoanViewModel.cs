using System;

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
    }
}