using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MeteoSuite.BIZ.Resources;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Exceptions;
using AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http.Headers;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public class MeteoSuiteQueryDeviceService : BaseMeteoSuiteBIZService, IMeteoSuiteQueryDeviceService
    {
        private const string ChiaveUrlEngine = "urlEngine_MeteoSuite";
        private const string ChiaveApiKey = "apiKeyEngine_MeteoSuite";
        private const string ChiaveTenantName = "tenantNameEngine_MeteoSuite";

        private const int TimeoutSeconds = 60;
        private const string HypermeteoProvider = "HYPERMETEO";

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;
        private readonly IValidazioneDatiMeteoService _validationService;

        public MeteoSuiteQueryDeviceService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
            _validationService = provider.GetRequiredService<IValidazioneDatiMeteoService>();
        }

        public async Task<List<MeteoSuiteQueryWeatherDataPointDto>> LeggiDatiMeteoPerPosizioneAsync(
            string provider, decimal lon, decimal lat,
            string dataInizio, string dataFine,
            string[] sensori,
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            DateTime _DataInizio, _DataFine;

            if (string.IsNullOrWhiteSpace(provider)) throw new ArgumentNullException(nameof(provider));
            if (lon == 0) throw new ArgumentNullException(nameof(lon));
            if (lat == 0) throw new ArgumentNullException(nameof(lat));
            if (!DateTime.TryParse(dataInizio, out _DataInizio)) throw new ArgumentNullException(nameof(dataInizio));
            if (!DateTime.TryParse(dataFine, out _DataFine)) throw new ArgumentNullException(nameof(dataFine));
            if (_DataInizio >= _DataFine) throw new ArgumentNullException(nameof(dataFine));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                string strLon = lon.ToString().Replace(",", ".");
                string strLat = lat.ToString().Replace(",", ".");
                string endpoint = $"datapoints?provider={provider}&lon={strLon}&lat={strLat}&from={Uri.EscapeDataString(dataInizio)}&to={Uri.EscapeDataString(dataFine)}";
                if (sensori != null && sensori.Length > 0)
                {
                    endpoint += "&sensors" + string.Join(',', sensori);
                }

                var responseBody = await ChiamataAsynch(endpoint, objParametriServer, objParametriSuperServer);

                List<MeteoSuiteQueryWeatherDataPointDto> result = ParseSensorsData(responseBody);

                return result;
            }
            catch (Exception ex)
            {
                LogMeteoStationsAcquisitionByPosition(provider, lon, lat, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<MeteoSuiteQueryWeatherDataPointDto>> LeggiDatiMeteoPerCodiceAsync(
            string stationCode, 
            string dataInizio, string dataFine,
            string? Piva,
            string[] sensori,
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            DateTime _DataInizio, _DataFine;

            if (string.IsNullOrWhiteSpace(stationCode)) throw new ArgumentNullException(nameof(stationCode));
            if (!DateTime.TryParse(dataInizio, out _DataInizio)) throw new ArgumentNullException(nameof(dataInizio));
            if (!DateTime.TryParse(dataFine, out _DataFine)) throw new ArgumentNullException(nameof(dataFine));
            if (_DataInizio >= _DataFine) throw new ArgumentNullException(nameof(dataFine));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            string endpoint = $"datapoints/devices/{stationCode}?from={Uri.EscapeDataString(dataInizio)}&to={Uri.EscapeDataString(dataFine)}";
            if (!string.IsNullOrWhiteSpace(Piva))
            {
                endpoint += $"&fmisUserId={Piva}";
            }
            if (sensori != null && sensori.Length > 0)
            {
                endpoint += "&sensors=" + string.Join(',', sensori);
            }

            var responseBody = await ChiamataAsynch(endpoint, objParametriServer, objParametriSuperServer);

            List<MeteoSuiteQueryWeatherDataPointDto> result = ParseSensorsData(responseBody);

            return result;

            throw new NotImplementedException();
        }

        public async Task<MeteoSuiteQueryWeatherStationDto?> LeggiStazionePerPosizioneAsync(
            string provider,
            decimal lon, decimal lat,
            string? Piva,
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(provider)) throw new ArgumentNullException(nameof(provider));
            if (lon == 0) throw new ArgumentNullException(nameof(lon));
            if (lat == 0) throw new ArgumentNullException(nameof(lat));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                string strLon = lon.ToString().Replace(",", ".");
                string strLat = lat.ToString().Replace(",", ".");
                string endpoint = $"devices?provider={provider}&lon={strLon}&lat={strLat}";
                if (!string.IsNullOrWhiteSpace(Piva))
                {
                    endpoint += $"&fmisUserId={Piva}";
                }

                var responseBody = await ChiamataAsynch(endpoint, objParametriServer, objParametriSuperServer);

                var result = JsonConvert.DeserializeObject<MeteoSuiteQueryWeatherStationResponseDto>(responseBody);

                return result?.device;
            }
            catch (Exception ex)
            {
                LogMeteoStationsAcquisitionByPosition(provider, lon, lat, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public async Task<MeteoSuiteQueryWeatherStationDto?> LeggiStazionePerCodiceAsync(
            string stationCode, 
            string? Piva,
            AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(stationCode)) throw new ArgumentNullException(nameof(stationCode));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                string endpoint = $"devices/{stationCode}";
                if (!string.IsNullOrWhiteSpace(Piva))
                {
                    endpoint += $"?fmisUserId={Piva}";
                }

                var responseBody = await ChiamataAsynch(endpoint, objParametriServer, objParametriSuperServer);

                var result = JsonConvert.DeserializeObject<MeteoSuiteQueryWeatherStationDto>(responseBody);

                return result;
            }
            catch (Exception ex)
            {
                LogMeteoStationsAcquisitionByCode(stationCode, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        private async Task<string> ChiamataAsynch(string endpoint, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer, CancellationToken cancellationToken = default)
        {
            var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(tenantName)) throw new ArgumentNullException(nameof(tenantName));

            string fullUrl = urlEngine.TrimEnd('/') + "/meteo-suite/" + tenantName.TrimStart('/').TrimEnd('/') + "/public/v1/normalized/" + endpoint;
            
            using HttpRequestMessage request = new(HttpMethod.Get, fullUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new MeteoDevicesInsuccessResponseException(httpResponse.StatusCode, responseBody);
            }

            return responseBody;
        }

        private List<MeteoSuiteQueryWeatherDataPointDto> ParseSensorsData(string requestBody)
        {
            try
            {
                // Parse the response according to spec:
                // Response is an array of objects with "pointInTime" and "sensors" properties
                var dataPoints = JsonConvert.DeserializeObject<List<dynamic>>(requestBody);

                if (dataPoints == null)
                {
                    throw new MeteoInvalidResponseException("API returned null or empty response");
                }

                var readings = new List<MeteoSuiteQueryWeatherDataPointDto>();

                foreach (var dataPoint in dataPoints)
                {
                    try
                    {
                        var pointInTime = dataPoint["pointInTime"];
                        if (pointInTime == null)
                        {
                            throw new MeteoInvalidResponseException("Missing 'pointInTime' field in API response");
                        }

                        var sensorsObj = dataPoint["sensors"];
                        if (sensorsObj == null)
                        {
                            throw new MeteoInvalidResponseException("Missing 'sensors' field in API response");
                        }

                        // Convert sensors from dynamic object to dictionary
                        var sensorDict = new Dictionary<string, float>();
                        if (sensorsObj is JObject jObj)
                        {
                            foreach (var property in jObj.Properties())
                            {
                                if (float.TryParse(property.Value.ToString().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                                {
                                    sensorDict[property.Name] = value;
                                }
                            }
                        }

                        // Parse point in time
                        var pointInTimeStr = pointInTime.ToString("O");
                        if (!DateTime.TryParse(pointInTimeStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime parsedTime))
                        {
                            throw new MeteoInvalidResponseException($"Invalid date format in pointInTime: {pointInTime}");
                        }

                        readings.Add(new MeteoSuiteQueryWeatherDataPointDto
                        {
                            PointInTime = parsedTime,
                            Sensors = sensorDict.Select(s => new MeteoSuiteQueryWeatherSensorDataPointDto() { Name = s.Key, Value = s.Value }).ToList()
                        });
                    }
                    catch (Exception ex) when (!(ex is MeteoInvalidResponseException))
                    {
                        throw new MeteoInvalidResponseException("Failed to parse data point from API response", ex);
                    }
                }

                return readings;
            }
            catch (JsonException ex)
            {
                throw new MeteoInvalidResponseException("API returned malformed JSON", ex);
            }
        }


        private void LogMeteoStationsAcquisitionByPosition(
            string provider, decimal lon, decimal lat,
            string status,
            string? errorMessage)
        {
            string data_source = $"provider{provider}, longitude {lon.ToString()}, latitude: {lat.ToString()}";
            LogMeteoStationsAcquisition(data_source, status, errorMessage);
        }

        private void LogMeteoStationsAcquisitionByCode(
            string stationCode,
            string status,
            string? errorMessage)
        {
            string data_source = $"stationCode {stationCode}";
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
    }
}
