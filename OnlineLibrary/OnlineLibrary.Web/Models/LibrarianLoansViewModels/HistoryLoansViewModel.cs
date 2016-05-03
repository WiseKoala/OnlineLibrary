using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class HistoryLoansViewModel
    {
        public IEnumerable<HistoryLoanViewModel> Rejected { get; set; }
        public IEnumerable<HistoryLoanViewModel> Completed { get; set; }
        public IEnumerable<HistoryLoanViewModel> LostBook { get; set; }
        public IEnumerable<HistoryLoanViewModel> Cancelled { get; set; }
    }
}