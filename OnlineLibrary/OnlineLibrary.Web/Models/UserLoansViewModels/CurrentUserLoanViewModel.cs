using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class CurrentUserLoanViewModel
    {
        public string Title { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime? BookPickUpLimitDate { get; set; }
        public int BookId { get; set; }
    }
}