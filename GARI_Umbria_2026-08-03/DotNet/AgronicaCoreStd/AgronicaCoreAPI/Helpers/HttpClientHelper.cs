using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Helpers
{
    public class HttpClientHelper
    {
        private string _tokenType;
        private string _authorization;

        public HttpClientHelper()
        {
            _tokenType = null;
            _authorization = null;
        }

        public HttpClientHelper(string tokenType, string authorization)
        {
            _tokenType = tokenType;
            _authorization = authorization;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, string body)
        {
            HttpResponseMessage result = null!;

            using (HttpClient _httpClient = new HttpClient())
            {
                //_httpClient.Timeout = new TimeSpan(timeout);

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    if (!string.IsNullOrEmpty(_authorization))
                        request.Headers.Authorization = new AuthenticationHeaderValue(_tokenType, _authorization);
                    request.Content = new StringContent(body!, null, "application/json");
                    result = await _httpClient!.SendAsync(request);
                }
            }

            return result;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            HttpResponseMessage result = null!;

            using (HttpClient _httpClient = new HttpClient())
            {
                //_httpClient.Timeout = new TimeSpan(timeout);

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    if (!string.IsNullOrEmpty(_authorization))
                        request.Headers.Authorization = new AuthenticationHeaderValue(_tokenType, _authorization);
                    result = await _httpClient!.SendAsync(request);
                }
            }

            return result;
        }
    }
}
