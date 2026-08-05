using System.Data;
using System.Security.Cryptography;
using System.Text;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace AgronicaNetCore.Base.Utility
{
    public static class Security
    {
        private const string ENCRYPTION_KEY = "13422245-16FA-4712-8BEF-26A32DC979BB";

        /// <summary>
        /// This method reads an encrypted field from the database (table configurazione_siti) and eventually decrypts it
        /// This function makes the assumption that the field storing the information whether or not the field is encrypted has the same name as the field but ends with '_isEncrypted'
        /// </summary>
        /// <param name="securityLayerDAL">DAL for reading in Configurazione_Siti</param>
        /// <param name="config"></param>
        /// <param name="objParametriServer"></param>
        /// <param name="encryptedFieldKey">Key of the encrypted field</param>
        /// <returns></returns>
        public static async Task<string> ReadEncryptedFieldFromDb(
            ISecurityLayerDAL securityLayerDAL,
            IConfiguration config,
            AgronicaCoreParametriServer objParametriServer,
            string encryptedFieldKey
        )
        {
            var flagIsEncryptedKey = $"{encryptedFieldKey}_isEncrypted";
            var keys = new List<string>() { encryptedFieldKey, flagIsEncryptedKey, "cr" };

            var dt = await securityLayerDAL.LeggiConfigurazioneSitiAsync(keys, objParametriServer);

            var keysAndValues = dt.AsEnumerable()
                .Select(r => new
                {
                    Key = r.Field<string>("Chiave"),
                    Value = r.Field<string>("Valore"),
                })
                .ToList();

            var encryptedFieldKeyAndValue = keysAndValues.FirstOrDefault(x =>
                x.Key == encryptedFieldKey
            );
            var flagIsEncryptedKeyAndValue = keysAndValues.FirstOrDefault(x =>
                x.Key == flagIsEncryptedKey
            );
            var crKeyAndValue = keysAndValues.FirstOrDefault(x => x.Key == "cr");

            var password = "";

            if (
                flagIsEncryptedKeyAndValue is not null
                && !string.IsNullOrEmpty(flagIsEncryptedKeyAndValue.Value)
                && encryptedFieldKeyAndValue is not null
                && !string.IsNullOrEmpty(encryptedFieldKeyAndValue.Value)
            )
            {
                if (flagIsEncryptedKeyAndValue.Value == "0")
                {
                    password = encryptedFieldKeyAndValue.Value;
                    if (crKeyAndValue is not null && !string.IsNullOrEmpty(crKeyAndValue.Value))
                    {
                        var cr2 = config.GetValue<string>("cr2");
                        var encryptedField = EncryptString(password, crKeyAndValue.Value, cr2);

                        var keysToUpdate = new List<string>()
                        {
                            encryptedFieldKey,
                            flagIsEncryptedKey,
                        };
                        var valuesToUpdate = new List<string>() { encryptedField, "1" };
                        await securityLayerDAL.AggiornaValoriAsync(
                            keysToUpdate,
                            valuesToUpdate,
                            objParametriServer
                        );
                    }
                }
                else if (flagIsEncryptedKeyAndValue.Value == "1")
                {
                    if (crKeyAndValue is not null && !string.IsNullOrEmpty(crKeyAndValue.Value))
                    {
                        var cr2 = config.GetValue<string>("cr2");
                        password = DecryptString(
                            encryptedFieldKeyAndValue.Value,
                            crKeyAndValue.Value,
                            cr2
                        );
                    }
                }
                else
                    throw new NotImplementedException(
                        "Valore della chiave _isEncrypted non gestita"
                    );
            }

            return password;
        }

        // VB: ogni token hex -> Chr(CInt(token)) = (char)codepoint
        private static string Agronica_Url_Decode(string sInput, string sSeparator = "G")
        {
            var sb = new System.Text.StringBuilder();
            foreach (var token in sInput.Split(sSeparator))
                sb.Append((char)Convert.ToInt32(token, 16));
            return sb.ToString();
        }

        // VB: AscW(c) = (int)c  — codepoint Unicode del carattere
        // VB: Hex(n)  = n.ToString("X")
        // VB: separatore finale rimosso con Left(str, Len(str)-1)
        private static string Agronica_Url_Encode(string sInput, string sSeparator = "G")
        {
            var parts = new List<string>(sInput.Length);
            foreach (var c in sInput)
                parts.Add(((int)c).ToString("X"));   // AscW equivalente
            return string.Join(sSeparator, parts);
        }

        private static bool UsaEncodingSemplice(IOptions<SecuritySettings> options)
        {
            if (options is null || options.Value is null)
                return false;

            return options.Value.UsaEncodingSemplice;
        }

        public static string Stringa_Codifica_LANCompatibile(
            string testo,
            string chiave,
            IOptions<SecuritySettings> options,
            bool esciSeVuoto = false
        )
        {
            if (esciSeVuoto && testo == "")
                return "";

            if (UsaEncodingSemplice(options))
            {
                testo = testo.Replace("\\", "@");
                return System.Web.HttpUtility.HtmlEncode(testo);
            }

            var cp1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252)!;

            // VB: Asc() = primo byte CP-1252 del carattere
            byte[] testoBytes  = cp1252.GetBytes(testo);
            byte[] chiaveBytes = cp1252.GetBytes(chiave);

            // VB: Chr(A1 + A2) = char il cui byte CP-1252 è (A1+A2) troncato
            var sbEncrypted = new System.Text.StringBuilder(testoBytes.Length);
            for (int i = 0; i < testoBytes.Length; i++)
            {
                byte sum = (byte)(testoBytes[i] + chiaveBytes[i % chiaveBytes.Length]);
                sbEncrypted.Append(cp1252.GetString(new[] { sum }));
            }

            // Poi passa la stringa a Agronica_Url_Encode che usa AscW (codepoint Unicode)
            return Agronica_Url_Encode(sbEncrypted.ToString());
        }

        public static string Stringa_Decodifica_LANCompatibile(
            string sText,
            string key,
            IOptions<SecuritySettings> options,
            bool Esci_Se_Vuoto = false
        )
        {
            if (Esci_Se_Vuoto && sText == "")
                return "";

            if (UsaEncodingSemplice(options))
            {
                var testoDecodificato = System.Web.HttpUtility.HtmlDecode(sText);
                return testoDecodificato.Replace("@", "\\");
            }

            var cp1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252)!;

            // Url-decode: ogni token hex -> char (AscW inverso)
            var decoded = Agronica_Url_Decode(sText);

            // La stringa decoded è stata prodotta da Chr(A1+A2), i cui byte CP-1252 sono i valori cifrati
            // VB: Asc(char) = byte CP-1252 → A1 = encByte - A2
            byte[] encBytes   = cp1252.GetBytes(decoded);
            byte[] chiaveBytes = cp1252.GetBytes(key);

            var sbResult = new System.Text.StringBuilder(encBytes.Length);
            for (int i = 0; i < encBytes.Length; i++)
            {
                byte diff = (byte)(encBytes[i] - chiaveBytes[i % chiaveBytes.Length]);
                sbResult.Append(cp1252.GetString(new[] { diff }));
            }

            return sbResult.ToString();
        }

        public static string DecryptString(string? encryptedText, string? encryptKey)
        {
            if (!string.IsNullOrEmpty(encryptedText) && !string.IsNullOrEmpty(encryptKey))
            {
                try
                {
                    return DecryptString(encryptedText, ENCRYPTION_KEY, encryptKey);
                }
                catch (Exception) { }
            }
            return encryptedText;
        }

        public static string DecryptString(string encryptedText, string cr, string cr2)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentNullException("encryptedText");

            if (string.IsNullOrEmpty(cr))
                throw new ArgumentNullException("cr");

            if (string.IsNullOrEmpty(cr2))
                throw new ArgumentNullException("cr2");

            byte[] key;
            byte[] iv;

            var keyStr = cr + cr2;
            key = CalculateSHA256(keyStr);
            var iv_32 = CalculateSHA256(cr);
            iv = iv_32.Take(16).ToArray();

            string decrypted = DecryptStringWithAes(encryptedText, key, iv);
            return decrypted;
        }

        private static string DecryptStringWithAes(string cipherText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");

            if (Information.IsNothing(key) || key.Length <= 0)
                throw new ArgumentNullException("key");

            if (Information.IsNothing(iv) || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            var cipherByteArray = Convert.FromBase64String(cipherText);
            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Create a decryptor
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create a memory stream with the encrypted data
                using (MemoryStream msDecrypt = new MemoryStream(cipherByteArray))
                {
                    using (
                        CryptoStream csDecrypt = new CryptoStream(
                            msDecrypt,
                            decryptor,
                            CryptoStreamMode.Read
                        )
                    )
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted data
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public static string EncryptString(string plainText, string cr2)
        {
            return EncryptString(plainText, ENCRYPTION_KEY, cr2);
        }

        public static string EncryptString(string plainText, string cr, string cr2)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");

            if (string.IsNullOrEmpty(cr))
                throw new ArgumentNullException("cr");

            if (string.IsNullOrEmpty(cr2))
                throw new ArgumentNullException("cr2");

            byte[] key;
            byte[] iv;

            var keyStr = cr + cr2;
            key = CalculateSHA256(keyStr);
            var iv_32 = CalculateSHA256(cr);
            iv = iv_32.Take(16).ToArray();

            string encrypted = EncryptStringWithAes(plainText, key, iv);
            return encrypted;
        }

        private static string EncryptStringWithAes(string plainText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");

            if (Information.IsNothing(key) || key.Length <= 0)
                throw new ArgumentNullException("key");

            if (Information.IsNothing(iv) || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Create an encryptor
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create a memory stream to hold the encrypted data
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (
                        CryptoStream csEncrypt = new CryptoStream(
                            msEncrypt,
                            encryptor,
                            CryptoStreamMode.Write
                        )
                    )
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write the data to be encrypted into the stream
                            swEncrypt.Write(plainText);
                        }
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static byte[] CalculateSHA256(string str)
        {
            SHA256 sha256 = SHA256.Create();
            System.Text.UTF8Encoding objUtf8 = new System.Text.UTF8Encoding();
            byte[] hashValue = sha256.ComputeHash(objUtf8.GetBytes(str ?? ""));

            return hashValue;
        }

        public static string? ToBase64(string value)
        {
            string? result = null;
            if (!string.IsNullOrEmpty(value))
                result = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
            return result;
        }
    }
}
