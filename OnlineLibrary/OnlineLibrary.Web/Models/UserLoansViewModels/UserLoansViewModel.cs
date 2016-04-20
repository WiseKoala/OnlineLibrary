using System.Collections.Generic;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class UserLoansViewModel
    {
        public IEnumerable<PendingUserLoansViewModel> PendingLoans { get; set; }
        public IEnumerable<CurrentUserLoansViewModel> CurrentLoans { get; set; }
    }
}