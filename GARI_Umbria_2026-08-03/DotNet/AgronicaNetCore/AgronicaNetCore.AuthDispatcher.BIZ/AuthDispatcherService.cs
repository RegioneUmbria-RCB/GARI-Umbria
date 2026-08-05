using System.Net;
using System.Text;
using System;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using System.Xml.Linq;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.AuthDispatcher.BIZ.Resources;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AgronicaNetCore.AuthDispatcher.BIZ
{
    public class AuthDispatcherService : BaseServiceAuthDispatcherBIZ
    {
        private Dictionary<string, KeyValuePair<TokenResponse, DateTime>> _tokenCache;
        private object _cacheLock;
        private readonly HttpClient _httpClient;
        private readonly ILoggingService _loggingService;
        private readonly ISecurityLayerDAL _securityLayerDal;

        public AuthDispatcherService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _httpClient = provider.GetRequiredService<HttpClient>();
            _loggingService = provider.GetRequiredService<ILoggingService>();
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
        }

        private async Task<TokenResponse> GenerateNewBearerToken(AgronicaCoreParametriServer serverParams, AgronicaCoreParametriSuperServer superServerParams)
        {
            try
            {
                string key = serverParams.StringaConnessione;

                lock (_cacheLock)
                {
                    if (_tokenCache.ContainsKey(key))
                    {
                        var value = _tokenCache[key];
                        TokenResponse cachedResp = value.Key;
                        DateTime creationTime = value.Value;
                        if (DateTime.Now < creationTime.AddSeconds(cachedResp.expiresIn))
                            return cachedResp;
                        else
                            _tokenCache.Remove(key);
                    }
                }

                var satBaseUrl = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("urlEngine_SmartTractor", serverParams, superServerParams);
                //var satApiKey = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("apiKeyEngine_SmartTractor", serverParams, superServerParams);

                string endpoint = "https://" + satBaseUrl.TrimEnd('/') + "/sso2/api/v1/apikey/exchange-token";

                var requestBody = new JObject();
                requestBody["apikey"] = "developer"; // serverParams.UsernameOperazione;
                requestBody["apikey"] = "D3v3l0per";

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                //request.Headers.Add("Authorization", "JWT " + satApiKey);
                request.Content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.SendAsync(request).GetAwaiter().GetResult();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("SAT token endpoint returned: " + response.StatusCode.ToString());

                string responseBody = response.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                if (string.IsNullOrEmpty(tokenResponse.accessToken) || tokenResponse.expiresIn <= 0 || string.IsNullOrEmpty(tokenResponse.tokenType))
                    throw new Exception("Invalid token response from SAT");

                lock (_cacheLock)
                {
                    if (!_tokenCache.ContainsKey(key))
                        _tokenCache[key] = new KeyValuePair<TokenResponse, DateTime>(tokenResponse, DateTime.Now);
                }

                return tokenResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to authenticate with SAT service", ex);
            }
        }
    }

    public class TokenResponse
    {
        public string accessToken { get; set;}
        public int expiresIn { get; set;}
        public string tokenType { get; set;}
    }
}
