using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.Web.Models.LibrarianLoansViewModels
{
    public class LibrarianIndexViewModel
    {
        public byte PendingStatusValue { get; set; }
        public byte ApprovedStatusValue { get; set; }
        public byte InProgressStatusValue { get; set; }

        [Display(Name = "Book Condition")]
        public BookCondition BookCondition { get; set; }
    }
}