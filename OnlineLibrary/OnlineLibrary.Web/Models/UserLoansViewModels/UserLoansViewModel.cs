using System.Collections.Generic;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class UserLoansViewModel
    {

        public IEnumerable<CurrentUserLoanViewModel> PendingLoans { get; set; }
        public IEnumerable<CurrentUserLoanViewModel> CurrentLoans { get; set; }
        public IEnumerable<CurrentUserLoanViewModel> RejectedLoans { get; set; }
        public IEnumerable<CurrentUserLoanViewModel> ApprovedLoans { get; set; }
        public IEnumerable<CurrentUserLoanViewModel> ReturnedLoans { get; set; }
        public IEnumerable<CurrentUserLoanViewModel> LostBooks { get; set; }
        public IEnumerable<HistoryUserLoanViewModel> HistoryLoans { get; set; }

    }
}