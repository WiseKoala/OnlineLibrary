using System.Collections.Generic;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class UserLoansViewModel
    {
        public IEnumerable<PendingUserLoansViewModel> PendingLoans { get; set; }
        public IEnumerable<CurrentUserLoansViewModel> CurrentLoans { get; set; }
        public IEnumerable<CurrentUserLoansViewModel> RejectedLoans { get; set; }
        public IEnumerable<CurrentUserLoansViewModel> ApprovedLoans { get; set; }
        public IEnumerable<CurrentUserLoansViewModel> ReturnedLoans { get; set; }
        public IEnumerable<CurrentUserLoansViewModel> LostBooks { get; set; }
    }
}