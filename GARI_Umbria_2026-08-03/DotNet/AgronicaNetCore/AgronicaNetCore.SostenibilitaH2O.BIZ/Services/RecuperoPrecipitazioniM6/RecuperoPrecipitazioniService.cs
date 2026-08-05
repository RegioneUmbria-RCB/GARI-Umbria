using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Agenda.DAL.DataLayer.Agenda;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Gis.DAL.DataLayer.Gis;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione;
using AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Exceptions;
using AgronicaNetCore.MeteoSuite.BIZ.Services.Engine.DatiMeteo.Acquisizione;
using AgronicaNetCore.MeteoSuite.DAL.DataLayer.DatiMeteo.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoPrecipitazioniM6
{
    /// <summary>
    /// Implementazione della business logic per il recupero delle precipitazioni (DS05-BL).
    /// <para>
    /// Per ogni esercizio del perimetro:
    /// <list type="number">
    /// <item><description>Recupera (Piva, Sa_Cod, Appezza) tramite <c>IAgenda.LeggiImpiantiAsync</c>.</description></item>
    /// <item><description>Estrae le coordinate GIS del centroide tramite <c>IGisClusterConfig</c>.</description></item>
    /// <item><description>Chiama M6 tramite <c>IPrecipitazioniM6DAL</c> per il periodo 1-gen/31-dic dell'anno.</description></item>
    /// <item><description>Applica fallback a zero se nessuna stazione meteo è disponibile.</description></item>
    /// </list>
    /// </para>
    /// Riferimento spec: DS05-BL RecuperoPrecipitazioniM6.
    /// </summary>
    public class RecuperoPrecipitazioniService
        : BaseServiceSostenibilitaH2OBiz, IRecuperoPrecipitazioniService
    {
        /// <summary>
        /// Regex per il parsing del WKT POINT: <c>POINT(longitudine latitudine)</c>.
        /// Gruppo 1 = longitudine, Gruppo 2 = latitudine.
        /// </summary>
        private static readonly Regex WktPointRegex = new(
            @"^POINT\((-?\d+(?:\.\d+)?)\s+(-?\d+(?:\.\d+)?)\)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(3));

        /// <inheritdoc cref="BaseServiceSostenibilitaH2OBiz"/>
        public RecuperoPrecipitazioniService(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <inheritdoc/>
        public async Task<PrecipitazioniResult> GetPrecipitazioniAsync(
            List<EsercizioH2O> esercizi,
            int anno,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(esercizi);
            if (esercizi.Count == 0)
                throw new ArgumentException("La lista degli esercizi non può essere vuota.", nameof(esercizi));
            if (anno <= 0)
                throw new ArgumentOutOfRangeException(nameof(anno), "L'anno di riferimento deve essere maggiore di zero.");

            var agenda       = _serviceProvider.GetRequiredService<IAgenda>();
            var gisCluster  = _serviceProvider.GetRequiredService<IGisClusterConfig>();
            var meteoService = _serviceProvider.GetRequiredService<IAcquisizioneDatiMeteoService>();

            var idEsercizi    = esercizi.Select(e => e.IdEsercizio).ToList();
            var lavCodRaccolta = new List<int>(Common.OperazioniRaccolta);

            string dataInizio = $"{anno}-01-01T00:00:00Z";
            string dataFine   = $"{anno}-12-31T23:59:59Z";

            LogInformation(
                "RecuperoPrecipitazioniM6 avviato — Anno: {Anno}, Esercizi: {Count}",
                objParametriServer, null, anno, idEsercizi.Count);

            // ── Step 1: recupera impianti per le operazioni di raccolta ─────────
            DataTable dtImpianti;
            try
            {
                dtImpianti = await agenda.LeggiImpiantiAsync(
                    Id_Agenda: new List<int>(),
                    objParametriServer: objParametriServer,
                    createTempEsercizi: false,
                    leggiFlagEsercizioChiuso: true,
                    leggiIndirizzoAppezzamento: false,
                    leggiGIS: false,
                    Id_Esercizi: idEsercizi,
                    Lav_cod: lavCodRaccolta);
            }
            catch (Exception ex) when (ex is not ArgumentException and not ArgumentOutOfRangeException)
            {
                throw new DatabaseQueryException(
                    $"Errore durante il recupero degliimpianti per le precipitazioni — Anno: {anno}.", ex);
            }

            var righeValide = dtImpianti.AsEnumerable().ToList();

            // ── Step 3: per ogni esercizio unico (progetto_cod) → GIS + M6 ──────
            // Raggruppa per progetto_cod e prende la prima riga per ottenere Piva/Sa_Cod/Appezza
            var gruppiEsercizio = righeValide
                .GroupBy(r => Convert.ToInt32(r["progetto_cod"]))
                .ToList();

            var pioggiaMmPerEsercizio = new Dictionary<int, decimal>();
            var pioggiaMmPerAzienda   = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (var gruppo in gruppiEsercizio)
            {
                int  progettoCod = gruppo.Key;
                var  primaRiga   = gruppo.First();
                var  piva        = primaRiga["Piva"]?.ToString() ?? string.Empty;
                int  saCod       = primaRiga["Sa_Cod"] != DBNull.Value ? Convert.ToInt32(primaRiga["Sa_Cod"]) : 0;
                int  appezza     = primaRiga["Appezza"] != DBNull.Value ? Convert.ToInt32(primaRiga["Appezza"]) : -1;

                decimal pioggiaMm = await GetPioggiaMmPerEsercizioAsync(
                    gisCluster, meteoService,
                    piva, saCod, appezza, progettoCod,
                    dataInizio, dataFine,
                    objParametriServer, objParametriSuperServer,
                    cancellationToken);

                pioggiaMmPerEsercizio[progettoCod] = pioggiaMm;

                // Aggregazione per azienda
                if (!string.IsNullOrEmpty(piva))
                {
                    pioggiaMmPerAzienda.TryGetValue(piva, out decimal corrente);
                    pioggiaMmPerAzienda[piva] = corrente + pioggiaMm;
                }
            }

            return new PrecipitazioniResult(pioggiaMmPerEsercizio, pioggiaMmPerAzienda);
        }

        // ────────────────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Recupera i mm di pioggia per un singolo esercizio:
        /// GIS → centroide WKT → lat/lon → MeteoSuite.
        /// Fallback a zero se GIS non ha il poligono o MeteoSuite non trova stazione.
        /// </summary>
        private async Task<decimal> GetPioggiaMmPerEsercizioAsync(
            IGisClusterConfig gisCluster,
            IAcquisizioneDatiMeteoService meteoService,
            string piva,
            int saCod,
            int appezza,
            int progettoCod,
            string dataInizio,
            string dataFine,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken)
        {
            // ── Recupero poligono GIS ─────────────────────────────────────────
            string? poligonoWkt;
            try
            {
                poligonoWkt = await gisCluster.GetPoligonoWktAsync(piva, saCod, appezza, objParametriServer);
            }
            catch (Exception ex)
            {
                LogWarning(
                    "Esercizio {ProgettoCod}: errore GIS recupero poligono. Pioggia impostata a zero.",
                    objParametriServer, ex, progettoCod);
                return 0m;
            }

            if (string.IsNullOrWhiteSpace(poligonoWkt))
            {
                LogWarning(
                    "Esercizio {ProgettoCod}: poligono GIS non disponibile. Pioggia impostata a zero.",
                    objParametriServer, null, progettoCod);
                return 0m;
            }

            // ── Recupero primo punto (centroide) ──────────────────────────────
            string? centroideWkt = await gisCluster.GetFirstPointCentroideWktAsync(poligonoWkt);

            if (string.IsNullOrWhiteSpace(centroideWkt))
            {
                LogWarning(
                    "Esercizio {ProgettoCod}: centroide GIS non disponibile. Pioggia impostata a zero.",
                    objParametriServer, null, progettoCod);
                return 0m;
            }

            // ── Parsing WKT "POINT(lon lat)" ──────────────────────────────────
            if (!TryParseWktPoint(centroideWkt, out decimal lat, out decimal lon))
            {
                LogWarning(
                    "Esercizio {ProgettoCod}: formato WKT non valido ({Wkt}). Pioggia impostata a zero.",
                    objParametriServer, null, progettoCod, centroideWkt);
                return 0m;
            }

            // ── Chiamata MeteoSuite ───────────────────────────────────────────
            var meteoRequest = new AcquisizioneDatiMeteoRequest
            {
                Coordinates = new GeographicalCoordinates { Latitude = lat, Longitude = lon },
                DataInizio = dataInizio,
                DataFine   = dataFine
            };

            AcquisizioneDatiMeteoPioggeResponse meteoResponse;
            try
            {
                meteoResponse = await meteoService.AcquisisciDatiMeteorologiciPioggeAsync(
                    meteoRequest, objParametriServer, objParametriSuperServer, cancellationToken);
            }
            catch (MeteoStationNotFoundException ex)
            {
                // nessuna stazione meteo nelle vicinanze → fallback non bloccante
                LogWarning(
                    "Esercizio {ProgettoCod}: nessuna stazione MeteoSuite disponibile (lat={Lat}, lon={Lon}). Pioggia impostata a zero.",
                    objParametriServer, ex, progettoCod, lat, lon);
                return 0m;
            }
            catch (Exception ex)
            {
                LogWarning(
                    "Esercizio {ProgettoCod}: errore chiamata MeteoSuite. Pioggia impostata a zero.",
                    objParametriServer, ex, progettoCod);
                return 0m;
            }

            var pioggiaTotale = meteoResponse.MeteoData.Sum(d => d.Prec ?? 0m);
            return Math.Round(pioggiaTotale, 2);
        }

        /// <summary>
        /// Tenta di fare il parsing di un WKT POINT nella forma <c>POINT(lon lat)</c>.
        /// </summary>
        private static bool TryParseWktPoint(string wkt, out decimal lat, out decimal lon)
        {
            lat = lon = 0m;
            var match = WktPointRegex.Match(wkt.Trim());
            if (!match.Success)
                return false;

            return decimal.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out lat)
                && decimal.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out lon);
        }
    }
}
