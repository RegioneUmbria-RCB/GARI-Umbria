using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSDifesa.BIZ.Resources;
using AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Calcolo
{
    /// <summary>
    /// Business service implementing the DS05-BL "CalcoloRischioPerModelliDifesa" logic.
    /// </summary>
    public class CalcoloRischioPerModelliDifesaService : BaseDSSDifesaBIZService, ICalcoloRischioPerModelliDifesaService
    {
        private const string ChiaveUrlEngine = "urlEngine_DSSDifesa";
        private const string ChiaveApiKey = "apiKeyEngine_DSSDifesa";
        private const string ChiaveTenantName = "tenantNameEngine_DSSDifesa";

        private const int GlobalTimeoutSeconds = 60;
        private const int PollIntervalMilliseconds = 300;

        private static readonly TimeSpan ModelCacheDuration = TimeSpan.FromSeconds(10);

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;

        private readonly ConcurrentDictionary<string, CacheItem> _modelResultsCache = new();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _modelLocks = new();

        /// <summary>
        /// Initializes a new instance of the CalcoloRischioPerModelliDifesaService.
        /// </summary>
        public CalcoloRischioPerModelliDifesaService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
        }

        /// <inheritdoc />
        public async Task<CalcoloRischioPerModelliDifesaResponse> CalcolaRischioPerModelliDifesaAsync(
            CalcoloRischioPerModelliDifesaRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(objParametriServer);
            ArgumentNullException.ThrowIfNull(objParametriSuperServer);

            if (request.Modelli == null || request.Modelli.Count == 0)
                throw new ArgumentException("La lista Modelli non può essere vuota", nameof(request.Modelli));

            if (request.MeteoData == null || request.MeteoData.Count == 0)
                throw new ArgumentException("La lista MeteoData non può essere vuota", nameof(request.MeteoData));

            var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(
                ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(urlEngine))
                throw new InvalidOperationException($"Configuration missing: key '{ChiaveUrlEngine}' not found.");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException($"Configuration missing: key '{ChiaveApiKey}' not found.");

            if (string.IsNullOrWhiteSpace(tenantName))
                throw new InvalidOperationException($"Configuration missing: key '{ChiaveTenantName}' not found.");

            string apiUrl = urlEngine.TrimEnd('/') + "/dss-api/DEFENSE/" + tenantName.TrimStart('/').TrimEnd('/') + "/public/api/v1";

            using var globalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            globalCts.CancelAfter(TimeSpan.FromSeconds(GlobalTimeoutSeconds));

            var tasks = request.Modelli
                .Select(m => ProcessSingleModelAsync(m, request, apiUrl, apiKey, globalCts.Token))
                .ToList();

            var result = new CalcoloRischioPerModelliDifesaResponse
            {
                TimestampRaccolta = DateTimeOffset.UtcNow.ToString("O"),
                TimeoutGlobaleRaggiunto = false
            };

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);

                result.RisultatiModelli.AddRange(tasks.Select(t => t.Result));
            }
            catch (OperationCanceledException) when (globalCts.IsCancellationRequested)
            {
                result.TimeoutGlobaleRaggiunto = true;
                result.RisultatiModelli.AddRange(tasks.Where(t => t.IsCompletedSuccessfully).Select(t => t.Result));
            }

            return result;
        }

        private async Task<ModelloRisultatoOutput> ProcessSingleModelAsync(
            ModelloDifesaInput model,
            CalcoloRischioPerModelliDifesaRequest request,
            string apiUrl,
            string apiKey,
            CancellationToken cancellationToken)
        {
            //string modelCacheKey = GetModelCacheKey(model.Code, request.LivelloDettaglio);

            //if (TryGetCachedModelResult(modelCacheKey, out var cachedResult))
            //{
            //    return cachedResult;
            //}

            //var modelLock = _modelLocks.GetOrAdd(modelCacheKey, _ => new SemaphoreSlim(1, 1));

            //await modelLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                //if (TryGetCachedModelResult(modelCacheKey, out cachedResult))
                //{
                //    return cachedResult;
                //}

                var modelResult = new ModelloRisultatoOutput
                {
                    Code = model.Code,
                    Description = model.Description,
                    TimestampCalcolo = DateTimeOffset.UtcNow.ToString("O")
                };

                try
                {
                    var requestId = await SendExecutionRequestAsync(model, request, apiUrl, apiKey, cancellationToken).ConfigureAwait(false);

                    var statusResponse = await PollExecutionStatusAsync(model, request, requestId, apiUrl, apiKey, cancellationToken).ConfigureAwait(false);

                    if (statusResponse.Status.Equals("DONE", StringComparison.OrdinalIgnoreCase))
                    {
                        modelResult.ModelGroup = statusResponse.ModelGroup;
                        if (request.LivelloDettaglio <= 0)
                        {
                            modelResult.OutputSintetico = statusResponse.SynteticOutput;
                            modelResult.OutputAnalitico = null;
                        }
                        else
                        {
                            modelResult.OutputAnalitico = statusResponse.AnalyticOutput;
                            modelResult.OutputSintetico = null;
                        }

                        modelResult.Status = "COMPLETATO";
                        modelResult.MessaggioErrore = null;
                    }
                    else if (statusResponse.Status.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        modelResult.Status = "ERRORE";
                        modelResult.MessaggioErrore = statusResponse.Error?.Message ?? "Errore misterioso";
                        modelResult.OutputSintetico = null;
                        modelResult.OutputAnalitico = null;
                    }
                    else
                    {
                        modelResult.Status = "NON_DISPONIBILE";
                        modelResult.MessaggioErrore = $"Status finale inatteso: {statusResponse.Status}";
                        modelResult.OutputSintetico = null;
                        modelResult.OutputAnalitico = null;
                    }
                }
                catch (ModelExecutionTimeoutException)
                {
                    modelResult.Status = "NON_DISPONIBILE";
                    modelResult.MessaggioErrore = "Model execution timed out";
                    modelResult.OutputSintetico = null;
                    modelResult.OutputAnalitico = null;
                }
                catch (ModelExecutionErrorException mex)
                {
                    modelResult.Status = "ERRORE";
                    modelResult.MessaggioErrore = $"{mex.ErrorCode}: {mex.ErrorMessage}";
                    modelResult.OutputSintetico = null;
                    modelResult.OutputAnalitico = null;
                }
                catch (Exception ex) when (ex is ModelInvocationException || ex is HttpRequestException || ex is JsonException)
                {
                    modelResult.Status = "ERRORE";
                    modelResult.MessaggioErrore = ex.Message;
                    modelResult.OutputSintetico = null;
                    modelResult.OutputAnalitico = null;
                }
                catch (OperationCanceledException)
                {
                    // per-model timeout or cancellation
                    modelResult.Status = "NON_DISPONIBILE";
                    modelResult.MessaggioErrore = "Model execution timeout or global cancellation reached";
                    modelResult.OutputSintetico = null;
                    modelResult.OutputAnalitico = null;
                }

                //_modelResultsCache[modelCacheKey] = new CacheItem(modelResult, DateTimeOffset.UtcNow.Add(ModelCacheDuration));

                return modelResult;
            }
            finally
            {
                //modelLock.Release();
            }
        }

        //private bool TryGetCachedModelResult(string modelCacheKey, out ModelloRisultatoOutput result)
        //{
        //    result = default!;

        //    if (_modelResultsCache.TryGetValue(modelCacheKey, out var cacheItem))
        //    {
        //        if (DateTimeOffset.UtcNow < cacheItem.ExpiresAt)
        //        {
        //            result = cacheItem.Result;
        //            return true;
        //        }

        //        _modelResultsCache.TryRemove(modelCacheKey, out _);
        //    }

        //    return false;
        //}

        //private static string GetModelCacheKey(string modelCode, int livelloDettaglio)
        //{
        //    return $"{modelCode}:{livelloDettaglio}";
        //}

        private async Task<string> SendExecutionRequestAsync(
            ModelloDifesaInput model,
            CalcoloRischioPerModelliDifesaRequest request,
            string apiUrl,
            string apiKey,
            CancellationToken cancellationToken)
        {
            var requestUrl = apiUrl.TrimEnd('/') + $"/models/{Uri.EscapeDataString(model.Code)}/executionrequest";

            var payload = new
            {
                meteoData = request.MeteoData,
                verbosity = request.LivelloDettaglio <= 0 ? "SEMAPHORE" : "DETAILS"
            };

            using var message = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            message.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw new ModelExecutionTimeoutException(model.Code);
            }
            catch (Exception ex)
            {
                throw new ModelInvocationException($"Failed to invoke model {model.Code}", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ModelInvocationException($"Model invocation returned status code {response.StatusCode}.");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var engineResponse = JsonSerializer.Deserialize<EngineExecutionRequestResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (engineResponse == null || string.IsNullOrWhiteSpace(engineResponse.RequestId))
                throw new ModelInvocationException("Invalid execution request response from Engine.");

            return engineResponse.RequestId;
        }

        private async Task<EngineExecutionStatusResponse> PollExecutionStatusAsync(
            ModelloDifesaInput model,
            CalcoloRischioPerModelliDifesaRequest request,
            string requestId,
            string apiUrl,
            string apiKey,
            CancellationToken cancellationToken)
        {
            var statusUrl = apiUrl.TrimEnd('/') + $"/models/{Uri.EscapeDataString(model.Code)}/executionrequest/{Uri.EscapeDataString(requestId)}/status";

            while (true)
            {
                using var message = new HttpRequestMessage(HttpMethod.Get, statusUrl);
                message.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
                message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw new ModelExecutionTimeoutException(requestId);
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new ModelInvocationException($"Polling execution status for requestId {requestId} returned {response.StatusCode}.");
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var statusResponse = JsonSerializer.Deserialize<EngineExecutionStatusResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                if (statusResponse == null)
                    throw new ModelInvocationException("Invalid status response format from Engine.");

                if (statusResponse.Status.Equals("TODO", StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(PollIntervalMilliseconds, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                JObject jContent = JObject.Parse(content);
                var jResponseData = jContent["responseData"];
                //statusResponse.ResponseDataStr = JsonSerializer.Serialize(jResponseData?.Root);
                //statusResponse.ResponseDataStr = jResponseData?.First?.First?.ToString()?.Replace("\r\n", "");

                JProperty? statusResponseProp = jResponseData?.First as JProperty;
                if (statusResponseProp != null && statusResponseProp.First != null)
                {
                    statusResponse.ModelGroup = statusResponseProp.Name;
                    if (request.LivelloDettaglio <= 0)
                    {
                        statusResponse.SynteticOutput = new EngineExecutionResponseSynteticOutput();
                        statusResponse.SynteticOutput.Model = (string?)statusResponseProp.First["model"];
                        statusResponse.SynteticOutput.Status = (int?)statusResponseProp.First["status"];
                        statusResponse.SynteticOutput.Disease = (string?)statusResponseProp.First["disease"];
                        statusResponse.SynteticOutput.Message = (string?)statusResponseProp.First["message"];
                        statusResponse.SynteticOutput.CropType = (string?)statusResponseProp.First["cropType"];
                        statusResponse.SynteticOutput.StatusVerbose = (string?)statusResponseProp.First["statusVerbose"];
                        statusResponse.SynteticOutput.ResultProgress = (int?)statusResponseProp.First["resultProgress"];
                        statusResponse.SynteticOutput.WarningMessage = (string?)statusResponseProp.First["warningMessage"];

                        var statusResponseSuccess = statusResponseProp.First["resultSuccess"];
                        if (statusResponseSuccess != null)
                        {
                            statusResponse.SynteticOutput.ResultSuccess = new EngineExecutionResponseSynteticOutputSuccessData();
                            statusResponse.SynteticOutput.ResultSuccess.Bands = new EngineExecutionResponseSynteticOutputSuccessDataBand[3];
                            var bandsObj = statusResponseSuccess["bands"];
                            var bandObj = bandsObj?.First;
                            for (int i = 0; i < 3 && bandObj != null; i++)
                            {
                                EngineExecutionResponseSynteticOutputSuccessDataBand band = new EngineExecutionResponseSynteticOutputSuccessDataBand();
                                band.Max = ((int?)bandObj["max"]).GetValueOrDefault();
                                band.Min = ((int?)bandObj["min"]).GetValueOrDefault();
                                band.Name = bandObj["name"]?.ToString() ?? "";
                                band.Color = bandObj["color"]?.ToString() ?? "";
                                statusResponse.SynteticOutput.ResultSuccess.Bands[i] = band;
                                bandObj = bandObj.Next;
                            }
                            statusResponse.SynteticOutput.ResultSuccess.RiskIndex = ((int?)statusResponseSuccess["riskIndex"]).GetValueOrDefault();
                        }
                    }
                    else
                    {
                        statusResponse.AnalyticOutput = new EngineExecutionResponseAnalyticOutput();
                        statusResponse.AnalyticOutput.Modello_Tabella1 = statusResponseProp.First["modello_Tabella1"]?.ToString();
                    }
                }
                
                //((JProperty)jResponseData?.First)?.Name

                return statusResponse;
            }
        }

        private class CacheItem
        {
            public CacheItem(ModelloRisultatoOutput result, DateTimeOffset expiresAt)
            {
                Result = result;
                ExpiresAt = expiresAt;
            }

            public ModelloRisultatoOutput Result { get; }
            public DateTimeOffset ExpiresAt { get; }
        }
    }
}
