using System.Net.Http.Headers;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Exceptions;
using AgronicaNetCore.Base.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AgronicaNetCore.Base.Utility
{
    public class ChiamaCoreWS
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<SecuritySettings> _securitySettings;

        public ChiamaCoreWS(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<SecuritySettings> securitySettings
        )
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _securitySettings = securitySettings;
        }

        public async Task<string> ChiamaCoreWSAsync(
            string uri,
            object input,
            AgronicaCoreParametriTriple objParams,
            string? bearerToken = null,
            bool wrapInCoreWsGeneric = true
        )
        {
            var hc = _httpClientFactory.CreateClient("coreWs");

            if (!string.IsNullOrWhiteSpace(bearerToken))
                hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    bearerToken
                );

            ForwardAuthCookie(hc);

            var objP = new CoreWS_GenericObjP()
            {
                objP_super_server = UtilityAgronica.ConvertObjParametriToString(
                    objParams.ObjParametriSuperServer,
                    _securitySettings
                ),
                objP_server = UtilityAgronica.ConvertObjParametriToString(
                    objParams.ObjParametriServer,
                    _securitySettings
                ),
                objP_utenti = UtilityAgronica.ConvertObjParametriToString(
                    objParams.ObjParametriUtenti,
                    _securitySettings
                ),
                user_Agent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
                host = _httpContextAccessor.HttpContext?.Request.Headers["Host"].ToString(),
            };

            string? payload;
            if (wrapInCoreWsGeneric)
            {
                CoreWS_Generic<object> request = new CoreWS_Generic<object>(objP, input);
                var coreWsWrapped = new CoreWSRequest<CoreWS_Generic<object>>(request);
                payload = JsonConvert.SerializeObject(coreWsWrapped);
            }
            else
            {
                payload = JsonConvert.SerializeObject(input);
            }

            try
            {
                using var content = new StringContent(
                    payload,
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                using var response = await hc.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new CoreWsUnauthorizedException("CoreWS responded with status 401");
                    }

                    throw new WebServiceException(
                        $"HTTP {(int)response.StatusCode} da {uri}",
                        (int)response.StatusCode
                    );
                }

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

        private void ForwardAuthCookie(HttpClient hc)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request is null || !request.Headers.TryGetValue("cookie", out var headerValues))
                return;

            var authCookie = headerValues.FirstOrDefault(s => s!.Contains("auth_cookie="));
            if (authCookie is null)
                return;

            var authCookieContent = authCookie
                .Split(';')
                .First(s => s.Contains("auth_cookie"))
                .Split('=')[1];

            string authCookieValue;
            if (authCookieContent.Contains("chunks"))
            {
                var chunksNr = Math.Min(Convert.ToInt32(authCookieContent.Split('-')[1]), 3); // limit to avoid malicious attacks
                authCookieValue = string.Empty;
                for (int i = 1; i <= chunksNr; i++)
                {
                    var chunk = headerValues.FirstOrDefault(s => s!.Contains($"auth_cookieC{i}"));
                    if (chunk is not null)
                        authCookieValue += chunk
                            .Split(';')
                            .First(s => s.Contains($"auth_cookieC{i}"))
                            .Split('=')[1];
                }
            }
            else
            {
                authCookieValue = authCookieContent;
            }

            if (hc.DefaultRequestHeaders.Contains("auth_cookie"))
                hc.DefaultRequestHeaders.Remove("auth_cookie");

            hc.DefaultRequestHeaders.Add("auth_cookie", authCookieValue);
        }
    }
}
