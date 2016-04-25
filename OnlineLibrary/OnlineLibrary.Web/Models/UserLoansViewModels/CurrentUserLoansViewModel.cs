using OnlineLibrary.DataAccess.Entities;
using OnlineLibrary.DataAccess.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class CurrentUserLoansViewModel
    {
        public string Title { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public LoanStatus Status { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}