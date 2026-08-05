using AgronicaNetCore.Base.Base;
using System.IO.Compression;
using System.Text;

namespace AgronicaNetCore.Utility.BIZ.Services.AgroZip
{
    public class AgroZipService : BaseService, IAgroZipService
    {
        public AgroZipService(IServiceProvider provider) : base(provider)
        {
        }

        public string ZipBase64(string inputString)
        {
            var ZippedString = "";

            try
            {
                if(inputString != "")
                {
                    var outputBytes = StrToByteArray(inputString);

                    if (outputBytes.Length > 0)
                    {
                        var output = Zip(outputBytes);

                        if (output.Length > 0)
                        {
                            ZippedString = Convert.ToBase64String(output);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogError(ex.Message, null, ex);
                throw;
            }


            return ZippedString;
        }


        public string UnZipBase64(string inputString)
        {
            var UnZippedString = "";

            try
            {
                if(inputString != "")
                {
                    var byteArray = Convert.FromBase64String(inputString);

                    if (byteArray.Length > 0)
                    {
                        var output = Unzip(byteArray);

                        if (output.Length > 0)
                        {
                            var enc = new UnicodeEncoding();
                            UnZippedString = enc.GetString(output);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogError(ex.Message, null, ex);
                throw;
            }


            return UnZippedString;
        }

        public byte[] Unzip(byte[] inputBytes)
        {
            byte[] outputBytes;

            try
            {
                using (var inputStream = new MemoryStream(inputBytes))
                using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var outputStream = new MemoryStream())
                {
                    gzip.CopyTo(outputStream);
                    outputBytes = outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, null, ex);
                throw;
            }

            return outputBytes;
        }

        public byte[] Zip(byte[] inputBytes)
        {
            byte[] outputBytes;

            try
            {

                using (var outputStream = new MemoryStream())
                {
                    using (var gzip = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        gzip.Write(inputBytes, 0, inputBytes.Length);
                    }

                    outputBytes = outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, null, ex);
                throw;
            }

            return outputBytes;
        }

        private byte[] StrToByteArray(string inputString)
        {
            var encoding = new UnicodeEncoding();

            return encoding.GetBytes(inputString);
        }

    }
}
