using System.Collections.Generic;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class UserLoansViewModel
    {
        public IEnumerable<CurrentUserLoanViewModel> PendingLoans { get; set; }
        public IEnumerable<CurrentUserLoanViewModel> InProgressLoans { get; set; }
        public IEnumerable<CurrentUserLoanViewModel> ApprovedLoans { get; set; }

        public IEnumerable<HistoryUserLoanViewModel> RejectedLoans { get; set; }
        public IEnumerable<HistoryUserLoanViewModel> CompletedLoans { get; set; }
        public IEnumerable<HistoryUserLoanViewModel> LostBookLoans { get; set; }
        public IEnumerable<HistoryUserLoanViewModel> CancelledLoans { get; set; }
    }
}