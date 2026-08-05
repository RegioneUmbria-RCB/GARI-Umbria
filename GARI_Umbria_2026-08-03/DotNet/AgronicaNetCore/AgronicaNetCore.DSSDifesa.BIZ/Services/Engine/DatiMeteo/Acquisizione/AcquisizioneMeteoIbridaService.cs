using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSDifesa.BIZ.Resources;
using AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Exceptions;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Implementation of the hybrid meteorological data acquisition service.
    /// Handles both station-based and coordinate-based meteorological data retrieval
    /// with validation, normalization, and error handling.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "Acquisire i dati meteorologici orari (temperatura, umidità, pioggia, bagnatura fogliare) 
    /// per centralina meteo o coordinate geografiche, a seconda del metodo chiamante, 
    /// standardizzati, con normalizzazione, validazione e tracciabilità della fonte."
    /// </summary>
    public class AcquisizioneMeteoIbridaService : BaseDSSDifesaBIZService, IAcquisizioneMeteoIbridaService
    {
        private const string ChiaveUrlEngine = "urlEngine_MeteoSuite";
        private const string ChiaveApiKey = "apiKeyEngine_MeteoSuite";
        private const string ChiaveTenantName = "tenantNameEngine_MeteoSuite";
        private const int TimeoutSeconds = 60;
        private const string HypermeteoProvider = "HYPERMETEO";

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;
        private readonly IMeteoDataValidationService _validationService;

        // Sensor code constants from DS02-BL_
        private const string TemperatureSensorCode = "TC2M_HOURLY";
        private const string PrecipitationSensorCode = "PREC_HOURLY";
        private const string HumiditySensorCode = "RH2M_HOURLY";
        private const string LeafWetnessSensorCode = "LEAFWET_HOURLY";

        /// <summary>
        /// Initializes a new instance of the AcquisizioneDatiMeteorologiciIbridaService.
        /// </summary>
        /// <param name="provider">The service provider for dependency resolution</param>
        /// <param name="localizer">The string localizer for messages</param>
        public AcquisizioneMeteoIbridaService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
            _validationService = provider.GetRequiredService<IMeteoDataValidationService>();
        }

        public async Task<AcquisizioneMeteoIbridaResponse> AcquisisciDatiMeteorologiciAsync(
            AcquisizioneMeteoIbridaRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            var response = new AcquisizioneMeteoIbridaResponse();

            try
            {
                // Validate request has a valid data source
                if (!request.HasValidDataSource())
                {
                    throw new MeteoMissingInputDataException(
                        "Neither station code nor valid coordinates provided. Unable to retrieve meteorological data.");
                }

                var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

                if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
                if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
                if (string.IsNullOrWhiteSpace(tenantName)) throw new ArgumentNullException(nameof(tenantName));

                string fullUrl = urlEngine.TrimEnd('/') + "/meteo-suite/" + tenantName.TrimStart('/').TrimEnd('/') + "/public/v1";
                if (!string.IsNullOrWhiteSpace(request.StationCod))
                {
                    fullUrl += $"/normalized/datapoints/devices/{request.StationCod}";
                    fullUrl += $"?from={Uri.EscapeDataString(request.DataInizio)}&to={Uri.EscapeDataString(request.DataFine)}";
                }
                else if (request.Coordinates is not null)
                {
                    // Format coordinates with invariant culture to ensure proper decimal formatting
                    string latStr = request.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture);
                    string lonStr = request.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture);
                    fullUrl += $"/normalized/datapoints?provider={HypermeteoProvider}&lon={lonStr}&lat={latStr}";
                    fullUrl += $"&from={Uri.EscapeDataString(request.DataInizio)}&to={Uri.EscapeDataString(request.DataFine)}";
                }
                else
                {
                    throw new ArgumentNullException(nameof(request.Coordinates));
                }

                var sensorReadings = await EseguiChiamataHttpAsync(
                    request,
                    fullUrl,
                    apiKey,
                    TimeoutSeconds,
                    cancellationToken);

                // Aggregate sensor readings into meteorological data points
                var meteoData = AggregateMeteoData(sensorReadings);
                response.MeteoData = new List<MeteoDataH>();

                // Validate the aggregated data
                List<MeteoDataValidationResult> validationResults = new List<MeteoDataValidationResult>();
                foreach (var meteoDataEntry in meteoData)
                {
                    var meteoDataValidationResult = _validationService.ValidateSingleDataPoint(meteoDataEntry);
                    validationResults.Add(meteoDataValidationResult);

                    if (meteoDataValidationResult.IsValid)
                        response.MeteoData.Add(meteoDataEntry);
                }

                // Determine overall status
                response.Status = _validationService.DetermineOverallStatus(response.MeteoData, validationResults);

                // Log successful acquisition
                LogMeteoAcquisition(request, response.Status, null);

                return response;
            }
            catch (MeteoMissingInputDataException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", ex.Message);
                throw;
            }
            catch (MeteoTimeoutException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Timeout: {ex.Message}");
                throw;
            }
            catch (MeteoUnauthorizedException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Unauthorized: {ex.Message}");
                throw;
            }
            catch (MeteoStationNotFoundException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Station Not Found: {ex.Message}");
                throw;
            }
            catch (MeteoInsuccessResponseException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"API Error: {ex.Message}");
                throw;
            }
            catch (MeteoInvalidResponseException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Invalid Response: {ex.Message}");
                throw;
            }
            catch (MeteoEmptyResultDataException ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Invalid Response: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                response.Status = "ERRORE";
                LogMeteoAcquisition(request, "ERRORE", $"Unexpected error: {ex.Message}");
                throw;
            }
        }

        private async Task<List<MeteoSensorsDataPoint>> EseguiChiamataHttpAsync(
            AcquisizioneMeteoIbridaRequest parametri,
            string requestUrl,
            string apiKey,
            int timeoutSecondi = 30,
            CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(timeoutSecondi));
            using CancellationTokenSource linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                throw new MeteoTimeoutException(timeoutSecondi);
            }

            string responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new MeteoUnauthorizedException("API key is invalid or expired. HTTP 401 Unauthorized received from Meteo Suite API");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new MeteoStationNotFoundException("The requested meteorological station or coordinates were not found. HTTP 404 Not Found received from Meteo Suite API");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new MeteoInsuccessResponseException(response.StatusCode, responseBody);
            }

            // HTTP 200 — parse JSON response
            List<MeteoSensorsDataPoint> sensorsData = ParseSensorsDataPoint(responseBody);

            if (sensorsData.Count == 0)
            {
                throw new MeteoEmptyResultDataException();
            }

            return sensorsData;
        }

        private static List<MeteoSensorsDataPoint> ParseSensorsDataPoint(string responseContent)
        {
            try
            {
                // Parse the response according to spec:
                // Response is an array of objects with "pointInTime" and "sensors" properties
                var dataPoints = JsonConvert.DeserializeObject<List<dynamic>>(responseContent);

                if (dataPoints == null)
                {
                    throw new MeteoInvalidResponseException("API returned null or empty response");
                }

                var sensorReadings = new List<MeteoSensorsDataPoint>();

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
                        var sensorDict = new Dictionary<string, decimal>();
                        if (sensorsObj is Newtonsoft.Json.Linq.JObject jObj)
                        {
                            foreach (var property in jObj.Properties())
                            {
                                if (decimal.TryParse(property.Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
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

                        sensorReadings.Add(new MeteoSensorsDataPoint
                        {
                            PointInTime = parsedTime,
                            Sensors = sensorDict
                        });
                    }
                    catch (Exception ex) when (!(ex is MeteoInvalidResponseException))
                    {
                        throw new MeteoInvalidResponseException("Failed to parse data point from API response", ex);
                    }
                }

                return sensorReadings;
            }
            catch (JsonException ex)
            {
                throw new MeteoInvalidResponseException("API returned malformed JSON", ex);
            }
        }

        /// <summary>
        /// Aggregates raw sensor readings into hourly meteorological data points.
        /// Maps sensor codes to their corresponding meteorological parameters.
        /// </summary>
        private List<MeteoDataH> AggregateMeteoData(List<MeteoSensorsDataPoint> sensorReadings)
        {
            // Group readings by point in time (hour)
            var groupedReadings = sensorReadings
                .GroupBy(r => r.PointInTime)
                .OrderBy(g => g.Key)
                .ToList();

            var meteoDataList = new List<MeteoDataH>();

            foreach (var hourGroup in groupedReadings)
            {
                // There should be only one reading per point in time according to spec
                // "non eventuali pointInTime duplicati"
                var reading = hourGroup.First();

                var meteoData = new MeteoDataH
                {
                    DataOra = reading.PointInTime,
                    Temp = ExtractSensorValue(reading.Sensors, TemperatureSensorCode),
                    Prec = ExtractSensorValue(reading.Sensors, PrecipitationSensorCode),
                    RelHum = ExtractSensorValue(reading.Sensors, HumiditySensorCode),
                    Lw = ExtractSensorValue(reading.Sensors, LeafWetnessSensorCode)
                };

                meteoDataList.Add(meteoData);
            }

            return meteoDataList;
        }

        /// <summary>
        /// Extracts a sensor value from the sensor dictionary, returning null if not present.
        /// </summary>
        private decimal? ExtractSensorValue(Dictionary<string, decimal> sensors, string sensorCode)
        {
            if (sensors != null && sensors.TryGetValue(sensorCode, out var value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Logs meteorological data acquisition activity for audit trail.
        /// Referenced in DS02-BL_: "Log acquisizione (scrittura): timestamp, source, validation_status per audit"
        /// </summary>
        private void LogMeteoAcquisition(
            AcquisizioneMeteoIbridaRequest request,
            string status,
            string? errorMessage)
        {
            string source = !string.IsNullOrWhiteSpace(request.StationCod)
                ? $"STATION:{request.StationCod}"
                : $"COORDINATES:{request.Coordinates?.Latitude},{request.Coordinates?.Longitude}";

            if (string.IsNullOrEmpty(errorMessage))
            {
                LogInformation(
                    $"Meteorological data acquisition successful. Source: {source}, Status: {status}, Period: {request.DataInizio} to {request.DataFine}");
            }
            else
            {
                LogWarning(
                    $"Meteorological data acquisition failed. Source: {source}, Status: {status}, Error: {errorMessage}");
            }
        }
    }
}
