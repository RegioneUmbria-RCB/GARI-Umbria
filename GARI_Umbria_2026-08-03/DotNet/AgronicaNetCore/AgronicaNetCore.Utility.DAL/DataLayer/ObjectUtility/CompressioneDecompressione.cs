using AgronicaNetCore.Utility.DAL.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.DAL.DataLayer.ObjectUtility
{
    public class CompressioneDecompressione : BaseDALUtility, ICompressioneDecompressione
    {
        public CompressioneDecompressione(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, localizer, securityBypass)
        {
        }

        /// <summary>
        /// Restituisce una stringa codificata in Base64 partendo da una stringa tradizionale.
        /// </summary>
        /// <param name="zipMode">Modalità compressione: 0 = Nessuna, 1 = GZIP, 2 = Deflate</param>
        /// <param name="inputString">Stringa in input tradizionale</param>
        /// <param name="encoding">(Opzionale) Tipo di encoding della stringa (Unicode, UTF8, ...). Default = Unicode</param>
        /// <returns>Stringa in Base64 eventualmente compressa</returns>
        public string CompressioneBase64(byte zipMode, string inputString, Encoding encoding = null)
        {
            // Marco Grilli, 17/06/2014: Converto la stringa in un array di byte
            byte[] byteArray;

            encoding ??= Encoding.Unicode;

            if (encoding.Equals(Encoding.UTF8))
            {
                byteArray = StrToByteArrayUTF8(inputString);
            }
            else if (encoding.Equals(Encoding.Unicode))
            {
                byteArray = StrToByteArray(inputString);
            }
            else if (encoding.Equals(Encoding.UTF32))
            {
                byteArray = StrToByteArrayUTF32(inputString);
            }
            else if (encoding.Equals(Encoding.ASCII))
            {
                byteArray = StrToByteArrayASCII(inputString);
            }
            else
            {
                byteArray = StrToByteArray(inputString);
            }

            // Marco Grilli, 17/06/2014: Comprimo come al solito
            byte[] byteOut = Compressione(zipMode, byteArray);

            // Marco Grilli, 17/06/2014: Ritorno l'array convertito in stringa Base64
            return ByteArrayToStrBASE64(byteOut);

        }


        // C# to convert a string to a byte array using Unicode encoding (equivalente a UTF-16 LE)
        private static byte[] StrToByteArray(string str)
        {
            var encoding = new UnicodeEncoding(); // same as Encoding.Unicode
            return encoding.GetBytes(str);
        }

        // C# to convert a string to a byte array using ASCII encoding
        private static byte[] StrToByteArrayASCII(string str)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        // C# to convert a string to a byte array using UTF-8 encoding
        private static byte[] StrToByteArrayUTF8(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        // C# to convert a string to a byte array using UTF-32 encoding
        private static byte[] StrToByteArrayUTF32(string str)
        {
            var encoding = new UTF32Encoding();
            return encoding.GetBytes(str);
        }


        /// <summary>
        /// Comprime un array di byte secondo la modalità specificata.
        /// </summary>
        /// <param name="zipMode">Modalità di compressione: 0 = Nessuna, 1 = GZIP, 2 = Deflate</param>
        /// <param name="inputBytes">Array di byte in input</param>
        /// <returns>Array di byte compresso (o originale se zipMode = 0)</returns>
        private static byte[] Compressione(byte zipMode, byte[] inputBytes)
        {
            using MemoryStream memStream = new MemoryStream();

            switch (zipMode)
            {
                case 0: // Nessuna compressione
                    return inputBytes;

                case 1: // GZIP
                    using (GZipStream gzip = new GZipStream(memStream, CompressionMode.Compress, leaveOpen: true))
                    {
                        gzip.Write(inputBytes, 0, inputBytes.Length);
                    }
                    break;

                case 2: // Deflate
                    using (DeflateStream deflate = new DeflateStream(memStream, CompressionMode.Compress, leaveOpen: true))
                    {
                        deflate.Write(inputBytes, 0, inputBytes.Length);
                    }
                    break;

                default: // Caso non definito
                    return inputBytes;
            }

            // Ritorna i byte compressi dal MemoryStream
            return memStream.ToArray();
        }

        /// <summary>
        /// Converte un array di byte in una stringa codificata in Base64.
        /// </summary>
        /// <param name="byteArray">Array di byte da convertire</param>
        /// <returns>Stringa Base64</returns>
        private static string ByteArrayToStrBASE64(byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        /// Converte una stringa codificata in Base64 in un array di byte.
        /// </summary>
        /// <param name="str">Stringa Base64 da decodificare</param>
        /// <returns>Array di byte</returns>
        private static byte[] StrBASE64ToByteArray(string str)
        {
            return Convert.FromBase64String(str);
        }

    }
}
