using System.IO;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class HttpPostedFileServiceModel
    {
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
    }
}