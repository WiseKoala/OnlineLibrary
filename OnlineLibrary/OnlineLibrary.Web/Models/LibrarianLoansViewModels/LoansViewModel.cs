using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class LoansViewModel
    {
        public IEnumerable<LoanRequestViewModel> LoansRequestViewModels { get; set; }
        public IEnumerable<ApprovedLoanViewModel> ApprovedLoansViewModels { get; set; }
    }
}
