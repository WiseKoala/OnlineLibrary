using System.IO;

namespace OnlineLibrary.Services.Models.BookServiceModels
{
    public class HttpPostedFileServiceModel
    {
        public int ContentLength { get; }
        public string ContentType { get; }
        public string FileName { get; }
        public Stream InputStream { get; }
    }
}