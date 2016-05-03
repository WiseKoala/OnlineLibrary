using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class HistoryLoansViewModel
    {
        public List<HistoryLoanViewModel> Rejected { get; set; }
        public List<HistoryLoanViewModel> Completed { get; set; }
        public List<HistoryLoanViewModel> LostBook { get; set; }
        public List<HistoryLoanViewModel> Cancelled { get; set; }
    }
}