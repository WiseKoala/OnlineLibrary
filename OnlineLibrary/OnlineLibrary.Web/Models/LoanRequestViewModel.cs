using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models
{
    public class LoanRequestViewModel
    {
        public int LoanRequestId { get; set; }
        public string UserName { get; set; }
        public string BookTitle { get; set; }
    }
}