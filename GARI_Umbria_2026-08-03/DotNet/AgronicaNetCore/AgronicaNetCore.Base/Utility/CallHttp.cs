using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Base.Utility
{
    /// <summary>
    /// Utility HTTP stateless per il download di risorse binarie via GET.
    /// Usa un <see cref="HttpClient"/> condiviso per evitare socket exhaustion.
    /// </summary>
    public class CallHttp
    {
        // HttpClient è thread-safe e va condiviso per tutta la vita dell'applicazione
        private static readonly HttpClient _httpClient = new();

        private readonly ILoggingService _loggingService;

        public CallHttp(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Esegue una GET sull'URL indicato e restituisce il corpo come array di byte.
        /// Restituisce <c>null</c> se la risposta non è success (2xx).
        /// </summary>
        /// <param name="url">URL da scaricare.</param>
        /// <param name="timeoutInSeconds">Timeout della richiesta in secondi. -1 = nessun timeout esplicito.</param>
        /// <param name="objParametri">Parametri applicativi passati al logger.</param>
        public async Task<byte[]?> ReadByteArrayFromUrlGetAsync(
            string url,
            int timeoutInSeconds = -1,
            AgronicaCoreParametri? objParametri = null
        )
        {
            using var cts =
                timeoutInSeconds > 0
                    ? new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds))
                    : new CancellationTokenSource();

            using var response = await _httpClient.GetAsync(url, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                string responseBody;
                try
                {
                    responseBody = await response.Content.ReadAsStringAsync(cts.Token);
                }
                catch (Exception ex)
                {
                    responseBody = $"[Impossibile leggere il body: {ex.GetType().Name} - {ex.Message}]";
                }
                _loggingService.LogError(
                   $"CallHttp.ReadByteArrayFromUrlGetAsync - Errore lettura bytes da URL: {RedactApiKeyFromUrl(url)} - StatusCode: {response.StatusCode} - ReasonPhrase: {response.ReasonPhrase} - Body: {responseBody}",
                   objParametri
               );
                return null;
            }

            return await response.Content.ReadAsByteArrayAsync(cts.Token);
        }

        private static string RedactApiKeyFromUrl(string url)
        {
            const string keyParam = "key=";
            var keyIndex = url.IndexOf(keyParam, StringComparison.OrdinalIgnoreCase);
            if (keyIndex < 0)
                return url;

            return url[..(keyIndex + keyParam.Length)] + "[APIKEY]";
        }
    }
}
