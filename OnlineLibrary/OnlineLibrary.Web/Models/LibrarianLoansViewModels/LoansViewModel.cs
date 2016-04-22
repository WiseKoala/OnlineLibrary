using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class LoansViewModel
    {
        public IEnumerable<LoanViewModel> PendingLoans { get; set; }
        public IEnumerable<LoanViewModel> ApprovedLoans { get; set; }
        public IEnumerable<LoanViewModel> LoanedBooks { get; set; }
        public IEnumerable<LoanViewModel> RejectedLoans { get; set; }
        public IEnumerable<LoanViewModel> ReturnedBooks { get; set; }
        public IEnumerable<LoanViewModel> LostBooks { get; set; }
    }
}
