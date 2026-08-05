using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.DAL.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.PrecipitazioniM6
{
    /// <summary>
    /// Implementazione del data access per il recupero delle precipitazioni da Engine Meteo M6.
    /// <para>
    /// Costruisce l'URL di chiamata con le coordinate geografiche e il periodo richiesto,
    /// esegue una GET autenticata verso l'endpoint HYPERMETEO e somma i valori orari
    /// del sensore <c>PREC_HOURLY</c>.
    /// </para>
    /// Riferimento spec: DS05-BL RecuperoPrecipitazioniM6.
    /// </summary>
    public class PrecipitazioniM6DAL : BaseDALSostenibilitaH2O, IPrecipitazioniM6DAL
    {
        /// <summary>Chiave di configurazione per l'URL base di MeteoSuite Engine.</summary>
        private const string ChiaveUrlEngine = "urlEngine_MeteoSuite";

        /// <summary>Chiave di configurazione per l'API key di MeteoSuite Engine.</summary>
        private const string ChiaveApiKey = "apiKeyEngine_MeteoSuite";

        /// <summary>Provider HYPERMETEO utilizzato per la ricerca per coordinate.</summary>
        private const string HypermeteoProvider = "HYPERMETEO";

        /// <summary>Codice sensore per la precipitazione cumulata oraria.</summary>
        private const string PrecipitationSensorCode = "PREC_HOURLY";

        /// <summary>Timeout in secondi per la chiamata HTTP a M6.</summary>
        private const int TimeoutSeconds = 60;

        /// <inheritdoc cref="BaseDALSostenibilitaH2O"/>
        public PrecipitazioniM6DAL(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<decimal?> GetPioggiaTotaleMmAsync(
            decimal lat,
            decimal lon,
            string dataInizio,
            string dataFine,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(objParametriServer);
            ArgumentNullException.ThrowIfNull(objParametriSuperServer);
            if (string.IsNullOrWhiteSpace(dataInizio))
                throw new ArgumentException("Data inizio non può essere vuota.", nameof(dataInizio));
            if (string.IsNullOrWhiteSpace(dataFine))
                throw new ArgumentException("Data fine non può essere vuota.", nameof(dataFine));

            var securityDal = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
            var httpClient  = _serviceProvider.GetRequiredService<HttpClient>();

            var (urlEngine, apiKey) = await securityDal.RecuperaConfigurazioneEngineAsync(
                ChiaveUrlEngine, ChiaveApiKey, objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(urlEngine))
                throw new InvalidOperationException("URL Engine MeteoSuite non configurato.");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("API Key Engine MeteoSuite non configurata.");

            string latStr = lat.ToString(CultureInfo.InvariantCulture);
            string lonStr = lon.ToString(CultureInfo.InvariantCulture);

            string url = $"{urlEngine.TrimEnd('/')}/normalized/datapoints"
                       + $"?provider={HypermeteoProvider}&lon={lonStr}&lat={latStr}"
                       + $"&from={Uri.EscapeDataString(dataInizio)}&to={Uri.EscapeDataString(dataFine)}";

            using HttpRequestMessage httpRequest = new(HttpMethod.Get, url);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(TimeoutSeconds));
            using CancellationTokenSource linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            HttpResponseMessage response = await httpClient.SendAsync(
                httpRequest, HttpCompletionOption.ResponseContentRead, linkedCts.Token);

            // HTTP 404 → nessuna stazione nelle vicinanze → fallback gestito dal chiamante
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync(CancellationToken.None);
            return SommaPrecipitazioni(body);
        }

        /// <summary>
        /// Somma tutti i valori del sensore <see cref="PrecipitationSensorCode"/> nell'array JSON
        /// restituito dall'API M6 (formato: <c>[{pointInTime, sensors:{...}}]</c>).
        /// </summary>
        private static decimal SommaPrecipitazioni(string responseBody)
        {
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            decimal totale = 0m;

            foreach (JsonElement dataPoint in doc.RootElement.EnumerateArray())
            {
                if (dataPoint.TryGetProperty("sensors", out JsonElement sensors)
                    && sensors.TryGetProperty(PrecipitationSensorCode, out JsonElement precEl)
                    && precEl.TryGetDecimal(out decimal prec))
                {
                    totale += prec;
                }
            }

            return totale;
        }
    }
}
