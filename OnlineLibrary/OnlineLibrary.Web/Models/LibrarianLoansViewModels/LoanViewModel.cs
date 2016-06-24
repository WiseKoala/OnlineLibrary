using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class LoanViewModel
    {
        public int LoanId { get; set; }
        public string UserName { get; set; }
        public string BookTitle { get; set; }
        public string ApprovingDate { get; set; }
        public string ExpectedReturnDate { get; set; }
        public bool IsApprovedLoanLate { get; set; }
        public bool IsExpectedReturnDateLate { get; set; }
    }
}