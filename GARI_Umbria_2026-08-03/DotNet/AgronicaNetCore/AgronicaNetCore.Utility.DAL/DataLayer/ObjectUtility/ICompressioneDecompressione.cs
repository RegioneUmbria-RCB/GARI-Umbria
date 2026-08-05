using System.Text;

namespace AgronicaNetCore.Utility.DAL.DataLayer.ObjectUtility
{
    public interface ICompressioneDecompressione
    {
       string CompressioneBase64(byte zipMode, string inputString, Encoding encoding = null);
    }
}