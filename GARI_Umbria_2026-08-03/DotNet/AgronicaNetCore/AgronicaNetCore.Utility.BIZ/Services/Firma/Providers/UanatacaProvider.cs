using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Utility.BIZ.Resources;
using AgronicaNetCore.Utility.BIZ.Services.Firma.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Services.Firma.Providers
{
    public class UanatacaProvider : BaseService, IFirmaProvider
    {

        public string Name => "UANATACA";

        public UanatacaProvider(IServiceProvider provider) : base(provider)
        {
        }

        private bool IsTransient(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.RequestTimeout   // 408
                || statusCode == HttpStatusCode.TooManyRequests  // 429
                || (int)statusCode >= 500;                       // 5xx
        }

        private MultipartFormDataContent CreateForm(string jsonContent, bool timeStampLevelEnabled, ConfigFirmaDigitale config)
        {
            var form = new MultipartFormDataContent();

            // 🔥 STRINGA → STREAM (come file)
            var bytes = new UTF8Encoding(false).GetBytes(jsonContent);
            var stream = new MemoryStream(bytes);

            var fileContent = new StreamContent(stream);

            form.Add(fileContent, "file", "file.json");

            Add(form, "username", config.UserName);
            Add(form, "password", config.Password);
            Add(form, "pin", config.Pin);
            Add(form, "format", config.Format);

            if (timeStampLevelEnabled)
            {
                Add(form, "level", config.Level);
                Add(form, "tsa_url", config.TsaUrl);
                Add(form, "tsa_user", config.TsaUser);
                Add(form, "tsa_pass", config.TsaPass);
            }

            return form;
        }

        public async Task<byte[]?> SignAsync(
            string jsonContent,
            bool timeStampLevelEnabled,
            ConfigFirmaDigitale config)
        {

            // exponential backoff + jitter
            int maxRetries = 3;
            int baseDelayMs = 500;
            var random = new Random();

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {

                    var client = new HttpClient();

                    var form = CreateForm(jsonContent, timeStampLevelEnabled, config);

                    var response = await client.PostAsync(config.Url, form);

                    // Gestione errori robusta
                    if (response.IsSuccessStatusCode)
                    {
                        var bytes = await response.Content.ReadAsByteArrayAsync();

                        LogInformation($"[{Name}] Firma OK - size {bytes.Length} bytes");

                        return bytes;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!IsTransient(response.StatusCode))
                    {
                        var message = $"[{Name}] Errore NON retryabile {(int)response.StatusCode} - {responseContent}";
                        LogError(message);
                        throw new FirmaProviderException(
                                    Name,
                                    response.StatusCode,
                                    message);
                    }

                    LogWarning($"[{Name}] Retry {attempt}/{maxRetries} - HTTP {(int)response.StatusCode}");
                }
                catch (HttpRequestException ex)
                {
                    LogWarning($"[{Name}] Errore di rete - retry {attempt}/{maxRetries}: {ex.Message}");

                    if (attempt == maxRetries)
                        throw new FirmaProviderException(
                            Name,
                            0,
                            $"Errore di rete dopo {maxRetries} tentativi",
                            ex);
                }

                if (attempt < maxRetries)
                {
                    // Exponential backoff + jitter
                    var delay = TimeSpan.FromMilliseconds(
                        baseDelayMs * Math.Pow(2, attempt) +
                        random.Next(0, 300)
                    );

                    await Task.Delay(delay);
                }
            }

            LogError($"[{Name}] Tutti i retry falliti");

            throw new FirmaProviderException(
                Name,
                0,
                "Errore nella chiamata all'endpoint della Firma Digitale");

        }

        private void Add(MultipartFormDataContent form, string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                form.Add(new StringContent(value), key);
        }
    }
}
