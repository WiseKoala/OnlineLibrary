using System.Collections.Generic;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class UserLoansViewModel
    {
        public List<PendingUserLoansViewModel> PendingLoans { get; set; }
        public List<CurrentUserLoansViewModel> CurrentLoans { get; set; }
    }
}