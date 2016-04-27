using System;
using OnlineLibrary.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.DataAccess.Entities
{
    public class History
    {
        public int Id { get; set; }

        public string ISBN { get; set; }

        [ForeignKey(nameof(BookCopy))]
        public int BookCopyId { get; set; }

        public LoanStatus Status { get; set; }

        public BookCondition InitialBookCondition { get; set; }

        public BookCondition FinalBookCondition { get; set; }

        [ForeignKey(nameof(User))]
        public string UserName { get; set; }

        [ForeignKey(nameof(Librarian))]
        public string LibrarianUserName { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpectedReturnDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActualReturnDate { get; set; }

        public virtual User User { get; set; }
        public virtual User Librarian { get; set; }
        public virtual BookCopy BookCopy { get; set; }
    }
}