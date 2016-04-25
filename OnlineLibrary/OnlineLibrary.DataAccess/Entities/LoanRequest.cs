using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.DataAccess.Entities
{
    public class LoanRequest
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public User User { get; set; }
        public Book Book { get; set; }
    }
}