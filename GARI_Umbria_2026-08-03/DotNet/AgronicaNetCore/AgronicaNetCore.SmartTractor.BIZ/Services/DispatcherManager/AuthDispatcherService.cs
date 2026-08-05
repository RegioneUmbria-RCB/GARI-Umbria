using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgronicaNetCore.SmartTractor.BIZ.Resources;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.DispatcherManager
{
    public class AuthDispatcherService: BaseServiceSmartTractorBIZ, IAuthDispatcherService
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
            _cacheLock = new object();
            _tokenCache = new Dictionary<string, KeyValuePair<TokenResponse, DateTime>>();
        }

        public async Task<TokenResponse> GenerateNewBearerTokenAsync(AgronicaCoreParametriServer serverParams, AgronicaCoreParametriSuperServer superServerParams)
        {
            try
            {
                string key = serverParams.StringaConnessione;

                //lock (_cacheLock)
                //{
                //    if (_tokenCache.ContainsKey(key))
                //    {
                //        var value = _tokenCache[key];
                //        TokenResponse cachedResp = value.Key;
                //        DateTime creationTime = value.Value;
                //        if (DateTime.Now < creationTime.AddSeconds(cachedResp.expiresIn))
                //            return cachedResp;
                //        else
                //            _tokenCache.Remove(key);
                //    }
                //}

                var satBaseUrl = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("urlEngine_SmartTractor", serverParams, superServerParams);
                //var satApiKey = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync("apiKeyEngine_SmartTractor", serverParams, superServerParams);

                //string endpoint = "https://" + satBaseUrl.TrimEnd('/') + "/sso2/api/v1/apikey/exchange-token";
                string endpoint = "https://smart-tractor.engines.development.diagramgroup.it/sso2/master/public/v1/login";

                var requestBody = new JObject();
                requestBody["username"] = "developer"; // serverParams.UsernameOperazione;
                requestBody["password"] = "D3v3l0per";

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                //request.Headers.Add("Authorization", "JWT " + satApiKey);
                request.Content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.SendAsync(request).GetAwaiter().GetResult();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("SAT token endpoint returned: " + response.StatusCode.ToString());

                string responseBody = response.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                if (string.IsNullOrEmpty(tokenResponse.auth_token))
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
        public string user_id { get; set; }
        public bool isParentUser { get; set; }
        public string auth_token { get; set; }
        public string username { get; set; }
        public string tenant { get; set; }
        public string refresh_token { get; set; }
        public string description { get; set; }
        public string locale { get; set; }
        public string dateFormat { get; set; }
        public string[] warnings { get; set; }
        public string[] errors { get; set; }
    }
}

