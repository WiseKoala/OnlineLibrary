using System;
using OnlineLibrary.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class HistoryUserLoanViewModel
    {
        public string BookTitle { get; set; }
        public string LibrarianName { get; set; }
        public BookCondition? InitialBookCondition { get; set; }
        public BookCondition? FinalBookCondition { get; set; }
        public HistoryStatus Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpectedReturnDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ActualReturnDate { get; set; }
    }
}