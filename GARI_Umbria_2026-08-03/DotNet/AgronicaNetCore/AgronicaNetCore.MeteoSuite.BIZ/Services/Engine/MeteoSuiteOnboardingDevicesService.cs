using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MeteoSuite.BIZ.Resources;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione;
using AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni.Exceptions;
using InData.Engine.MeteoSuite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public class MeteoSuiteOnboardingDevicesService : BaseMeteoSuiteBIZService, IMeteoSuiteOnboardingDevicesService
    {
        private const string ChiaveUrlEngine = "urlEngine_MeteoSuite";
        private const string ChiaveApiKey = "apiKeyEngine_MeteoSuite";
        private const string ChiaveTenantName = "tenantNameEngine_MeteoSuite";

        private const int TimeoutSeconds = 60;

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;
        private readonly IValidazioneDatiMeteoService _validationService;

        public MeteoSuiteOnboardingDevicesService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
            _validationService = provider.GetRequiredService<IValidazioneDatiMeteoService>();
        }

        public async Task<MeteoSuiteOnboardingDevicesDto?> LeggiStazioniAutorizzateAsynch(
            string piva, 
            bool? reale,
            MeteoStationVisibility visibility,
            int offset, int limit, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(piva)) throw new ArgumentNullException(nameof(piva));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                int visibilityValue = (int)visibility;
                string endpoint = $"devices?fmis_user_id={piva}";
                if (reale.HasValue)
                {
                    endpoint += $"&flIsReal={reale.Value.ToString()}";
                }
                endpoint += $"&visibility={visibilityValue.ToString()}&offset={offset.ToString()}&limit={limit.ToString()}";

                string responseBody = await EseguiChiamataGetAsynch(endpoint, objParametriServer, objParametriSuperServer, cancellationToken);

                var response = JsonConvert.DeserializeObject<MeteoSuiteOnboardingDevicesDto>(responseBody);

                return response;
            }
            catch (Exception ex)
            {
                LogMeteoStationsAcquisitionByPiva(piva, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async Task<MeteoSuiteOnboardingDeviceDto?> LeggiStazionePerIdAsynch(
            string piva, 
            int IdStazione, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(piva)) throw new ArgumentNullException(nameof(piva));
            if (IdStazione <= 0) throw new ArgumentNullException(nameof(IdStazione));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                string endpoint = $"devices/{IdStazione.ToString()}?fmis_user_id={piva}";
                string responseBody = await EseguiChiamataGetAsynch(endpoint, objParametriServer, objParametriSuperServer, cancellationToken);

                var response = JsonConvert.DeserializeObject<MeteoSuiteOnboardingDeviceDto>(responseBody);

                return response;
            }
            catch (Exception ex)
            {
                LogMeteoStationsAcquisitionById(IdStazione, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async Task<MeteoSuiteOnboardingDeviceDto?> LeggiStazionePerProviderCodeAsynch(
            string piva, 
            string ProviderCode,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(piva)) throw new ArgumentNullException(nameof(piva));
            if (string.IsNullOrEmpty(ProviderCode)) throw new ArgumentNullException(nameof(ProviderCode));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                string endpoint = $"devices/provider-code/{ProviderCode}?fmis_user_id={piva}";
                string responseBody = await EseguiChiamataGetAsynch(endpoint, objParametriServer, objParametriSuperServer, cancellationToken);

                var response = JsonConvert.DeserializeObject<MeteoSuiteOnboardingDeviceDto>(responseBody);

                return response;
            }
            catch (Exception ex)
            {
                LogMeteoStationsAcquisitionByCode(ProviderCode, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }


        public async Task<MeteoSuiteOnboardingDeviceDto?> AggiornaOCreaStazioneAsynch(
            AggiornaStazioneMeteoRequest request, 
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

                if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
                if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
                if (string.IsNullOrWhiteSpace(tenantName)) throw new ArgumentNullException(nameof(tenantName));

                if (request.IdStazione <= 0)
                {
                    var sensorsi = new List<object>();
                    foreach (var sensore in request.Sensori)
                    {
                        sensorsi.Add(new { 
                            id = sensore.Id,
                            description = sensore.Descrizione
                        });
                    }
                    var payload = new
                    {
                        description = request.Descrizione,
                        fmisUserId = request.PIVA,
                        latitude = request.Latitude,
                        longitude= request.Longitude,
                        sensors = sensorsi
                    };

                    string endpoint = "devices/virtual";
                    string content = JsonConvert.SerializeObject(payload);
                    string responseBody = await EseguiChiamataPostAsynch(endpoint, content, objParametriServer, objParametriSuperServer, cancellationToken);

                    var response = JsonConvert.DeserializeObject<MeteoSuiteOnboardingDeviceDto>(responseBody);

                    if (response != null)
                    {
                        var payload2 = new
                        {
                            owner_fmis_user_id = request.PIVA,
                            authorized_fmis_user_ids = new string[] { request.PIVASuperUser }
                        };

                        endpoint = $"devices/{response.id}/grants";
                        content = JsonConvert.SerializeObject(payload2);
                        await EseguiChiamataPostAsynch(endpoint, content, objParametriServer, objParametriSuperServer, cancellationToken);
                    }

                    return response;
                }
                else
                {
                    var sensorsi = new List<object>();
                    foreach (var sensore in request.Sensori)
                    {
                        sensorsi.Add(new
                        {
                            id = sensore.Id,
                            description = sensore.Descrizione
                        });
                    }
                    var payload = new
                    {
                        fmisUserId = request.PIVA,
                        description = request.Descrizione,
                        latitude = request.Latitude,
                        longitude = request.Longitude,
                        sensors = sensorsi
                    };

                    string endpoint = $"devices/{HttpUtility.UrlEncode(request.IdStazione.ToString())}";
                    string content = JsonConvert.SerializeObject(payload);
                    string responseBody = await EseguiChiamataPutAsynch(endpoint, content, objParametriServer, objParametriSuperServer, cancellationToken);

                    var response = JsonConvert.DeserializeObject<MeteoSuiteOnboardingDeviceDto>(responseBody);

                    return response;
                }
            }
            catch (Exception ex)
            {
                LogMeteoStationsUpdate(request, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EliminaStazioneAsynch(
            string piva, 
            int IdStazione,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (IdStazione <= 0) throw new ArgumentNullException(nameof(IdStazione));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                string endpoint = $"devices/{IdStazione.ToString()}?fmis_user_id={piva}";
                string responseBody = await EseguiChiamataDeleteAsynch(endpoint, objParametriServer, objParametriSuperServer, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                LogMeteoStationsDelete(IdStazione, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        private async Task<string> EseguiChiamataGetAsynch(
            string endpoint,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            return await EseguiChiamataAsynch(HttpMethod.Get, endpoint, null, objParametriServer, objParametriSuperServer, cancellationToken);
        }

        private async Task<string> EseguiChiamataPostAsynch(
            string endpoint,
            string content,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            return await EseguiChiamataAsynch(HttpMethod.Post, endpoint, content, objParametriServer, objParametriSuperServer, cancellationToken);
        }

        private async Task<string> EseguiChiamataPutAsynch(
            string endpoint,
            string content,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            return await EseguiChiamataAsynch(HttpMethod.Put, endpoint, content, objParametriServer, objParametriSuperServer, cancellationToken);
        }

        private async Task<string> EseguiChiamataDeleteAsynch(
            string endpoint,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            return await EseguiChiamataAsynch(HttpMethod.Delete, endpoint, null, objParametriServer, objParametriSuperServer, cancellationToken);
        }

        private async Task<string> EseguiChiamataAsynch(HttpMethod method,
            string endpoint,
            string? content,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(tenantName)) throw new ArgumentNullException(nameof(tenantName));

            string fullUrl = urlEngine.TrimEnd('/') + "/onboarding/" + tenantName.TrimStart('/').TrimEnd('/') + "/public/v1/" + endpoint;

            using HttpRequestMessage request = new(method, fullUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (content != null)
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(TimeoutSeconds));
            using CancellationTokenSource linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                throw new MeteoDevicesTimeoutException(TimeoutSeconds);
            }

            string responseBody = await httpResponse.Content.ReadAsStringAsync(CancellationToken.None);

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new MeteoDeviceNotFoundException("");
            }
            else if (!httpResponse.IsSuccessStatusCode)
            {
                throw new MeteoDevicesInsuccessResponseException(httpResponse.StatusCode, responseBody);
            }

            return responseBody;
        }


        private void LogMeteoStationsAcquisitionByPiva(
            string piva,
            string status,
            string? errorMessage)
        {
            string data_source = $"piva: {piva}";
            LogMeteoStationsAcquisition(data_source, status, errorMessage);
        }

        private void LogMeteoStationsAcquisitionById(
            int stationId,
            string status,
            string? errorMessage)
        {
            string data_source = $"station id: {stationId.ToString()}";
            LogMeteoStationsAcquisition(data_source, status, errorMessage);
        }

        private void LogMeteoStationsAcquisitionByCode(
            string code,
            string status,
            string? errorMessage)
        {
            string data_source = $"station provider code: {code}";
            LogMeteoStationsAcquisition(data_source, status, errorMessage);
        }

        private void LogMeteoStationsAcquisition(
            string data_source,
            string status,
            string? errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                LogInformation(
                    $"User meteo stations acquisition successful. {data_source}");
            }
            else
            {
                LogWarning(
                    $"User meteo stations acquisition failed. {data_source}");
            }
        }

        private void LogMeteoStationsUpdate(
            AggiornaStazioneMeteoRequest request,
            string status,
            string? errorMessage)
        {
            string source = $"Station ID: {request.IdStazione.ToString()}";
            source += !string.IsNullOrEmpty(request.Descrizione) ? $", Station: {request.Descrizione}" : "";
            source += request.Longitude != null && request.Latitude != null ? $"Longitude: {request.Longitude}:, Latitude: {request.Latitude}" : "";

            if (string.IsNullOrEmpty(errorMessage))
            {
                LogInformation(
                    "User meteo stations update successful. " + source);
            }
            else
            {
                LogWarning(
                    "User meteo stations update failed. " + source);
            }
        }

        private void LogMeteoStationsDelete(
            int stationId,
            string status,
            string? errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                LogInformation(
                    "User meteo stations delete successful. station id: " + stationId.ToString());
            }
            else
            {
                LogWarning(
                    "User meteo stations delete failed. station id: " + stationId.ToString());
            }
        }
    }
}
