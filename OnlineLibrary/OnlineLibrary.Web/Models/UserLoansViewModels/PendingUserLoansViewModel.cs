using OnlineLibrary.DataAccess.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.Web.Models.UserLoansViewModels
{
    public class PendingUserLoansViewModel
    {
        public string BookTitle { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}