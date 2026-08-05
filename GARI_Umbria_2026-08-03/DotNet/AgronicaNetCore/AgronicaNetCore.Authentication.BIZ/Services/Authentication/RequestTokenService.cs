using AgronicaCoreDTOStd.Identity;
using Newtonsoft.Json;
using OutData.Authentication;
using System.Net.Http.Headers;
using System.Text;

namespace AgronicaNetCore.Authentication.BIZ.Services.Authentication
{
    public class RequestTokenService: IRequestTokenService
    {
        private readonly HttpClient _httpClient;

        public RequestTokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenResponse> GetTokenFromUserPwdAsync(TokenUserPwdRequest request, CancellationToken cancellationToken = default)
        { 

            try
            {

                //WriteLog(LogEventLevel.Information, $"autenticazione ad Antares [{url}]");

                var auth = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "username", request.Username ),
                    new KeyValuePair<string, string>("password", request.Password ),
                    new KeyValuePair<string, string>("client_id", request.ClientId ),
                    new KeyValuePair<string, string>( "grant_type", request.GrantType ),

                };


                // Aggiungi eventuali campi extra
                if (request.ExtraFormFields != null)
                {
                    foreach (var field in request.ExtraFormFields)
                    {
                        auth.Add(new KeyValuePair<string, string>("scope", request.Scope));
                    }
                }

                if (!string.IsNullOrEmpty(request.Scope))
                    auth.Add(new KeyValuePair<string, string>("scope", request.Scope));

                var content = new FormUrlEncodedContent(auth);

                //TODO Da cancellare se funziona quella sotto
                //string authSerialized = JsonConvert.SerializeObject(auth);
                //HttpContent body = new StringContent(authSerialized, Encoding.UTF8, content_type);


                // Imposta gli header extra se forniti
                if (request.ExtraHeaders != null)
                {
                    foreach (var header in request.ExtraHeaders)
                    {
                        _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                // Se è necessario usare la Basic Auth per il client (client_id e client_secret)
                if (request.UseBasicAuthForClient && !string.IsNullOrEmpty(request.ClientSecret))
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{request.ClientId}:{request.ClientSecret}");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }

                // Fai la richiesta HTTP
                var response = await _httpClient.PostAsync(request.TokenEndpoint, content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error getting token: {errorMessage}");
                }

                // Deserializza la risposta in TokenResponse usando Newtonsoft.Json
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                return tokenResponse!;

                //HttpResponseMessage response = await client.PostAsync(url, body);
                //if (response.IsSuccessStatusCode)
                //{
                //    _objResponse = await response.Content.ReadAsStringAsync();
                //   // WriteLog(LogEventLevel.Fatal, $"Token Antares: {AccessToken}");
                //}
                //else
                //    throw new Exception("Login su Antares non riuscito, verificare parametri d'autenticazione");

                //// Verifica token accesso AGEA
                //if (string.IsNullOrEmpty(_objResponse))
                //    throw new Exception("Login su Antares non riuscito, verificare parametri d'autenticazione");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
