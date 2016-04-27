using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.DataAccess.Entities
{
    public class Loan
    {
        public int Id { get; set; }

        [ForeignKey(nameof(BookCopy))]
        public int? BookCopyId { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public LoanStatus Status { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpectedReturnDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BookPickUpLimitDate { get; set; }

        public User User { get; set; }
        public BookCopy BookCopy { get; set; }
        public Book Book { get; set; }
    }
}