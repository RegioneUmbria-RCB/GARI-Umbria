using AgronicaNetCore.Base.Exceptions;
using Newtonsoft.Json;
using System.Net.Http;

namespace AgronicaNetCore.Base.Utility
{
    public class ChiamaWebService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChiamaWebService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> ChiamaWebServiceAsync(
            string uri,
            object input,
            string? bearerToken = null
        )
        {
            var payload = JsonConvert.SerializeObject(input);

            var hc = _httpClientFactory.CreateClient();

            if (!string.IsNullOrWhiteSpace(bearerToken))
                hc.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

            try
            {
                using var content = new StringContent(
                    payload,
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                using var response = await hc.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                    throw new WebServiceException(
                        $"HTTP {(int)response.StatusCode} da {uri}",
                        (int)response.StatusCode
                    );

                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException ex)
            {
                throw new WebServiceTimeoutException($"Timeout su {uri}", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new WebServiceException($"Errore chiamata WS: {uri}", ex);
            }
        }
    }
}
