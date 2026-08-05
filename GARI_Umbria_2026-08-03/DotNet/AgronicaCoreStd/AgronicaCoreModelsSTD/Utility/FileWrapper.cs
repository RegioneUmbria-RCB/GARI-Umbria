using Microsoft.AspNetCore.Http;

namespace AgronicaCoreModelsSTD.Utility
{
    public interface IFileWrapper
    {
        IFormFile file { get; set; }
    }

    public class FileWrapper : IFileWrapper
    {
        public IFormFile file { get; set; }
    }
}
