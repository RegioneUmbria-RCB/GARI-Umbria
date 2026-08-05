
namespace AgronicaNetCore.Utility.BIZ.Services.AgroZip
{
    public interface IAgroZipService
    {
        string ZipBase64(string inputString);

        string UnZipBase64(string inputString);
    }
}
