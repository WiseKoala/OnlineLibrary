using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class LibrarianIndexViewModel
    {
        public byte PendingStatusValue { get; set; }
        public byte ApprovedStatusValue { get; set; }
        public byte InProgressStatusValue { get; set; }
    }
}