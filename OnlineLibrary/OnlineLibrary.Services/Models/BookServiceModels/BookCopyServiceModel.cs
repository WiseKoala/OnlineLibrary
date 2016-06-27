using OnlineLibrary.DataAccess.Enums;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class BookCopyServiceModel
    {
        public int Id { get; set; }
        public BookCondition BookCondition { get; set; }
        public bool IsToBeDeleted { get; set; }
        public bool IsLost { get; set; }
    }
}