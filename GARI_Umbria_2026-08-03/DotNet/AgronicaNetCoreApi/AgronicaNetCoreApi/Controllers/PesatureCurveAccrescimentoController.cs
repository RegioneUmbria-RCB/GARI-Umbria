using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesatureCurveAccrescimento;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesatureExport;
using OutData.Zoo;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
    /// <summary>
    /// Controller REST per le metriche della curva di accrescimento bovini.
    /// Implementa DS06-API: GET /api/v1/animals/weighing-curves.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/v1/animals/weighing-curves")]
    public class PesatureCurveAccrescimentoController : BaseController
    {
        private readonly IPesatureCurveAccrescimentoService _pesatureCurveService;
        private readonly IPesatureExportService _exportService;

        public PesatureCurveAccrescimentoController(
            IServiceProvider provider,
            IOptions<SecuritySettings> securitySettings,
            IStringLocalizer<Messages> localizer)
            : base(provider, securitySettings, localizer)
        {
            _pesatureCurveService = provider.GetRequiredService<IPesatureCurveAccrescimentoService>();
            _exportService        = provider.GetRequiredService<IPesatureExportService>();
        }

        /// <summary>
        /// Recupera le metriche della curva di accrescimento bovini con filtri, calcoli e paginazione.
        /// Vedere DS06-API: Endpoint Query Metriche Curva Accrescimento.
        /// </summary>
        /// <param name="stalla_key">
        /// Chiavi composite stalla "{piva}_{sa_cod}_{sta_num}", multi-select separato da virgola.
        /// Il piva viene validato server-side contro il JWT.
        /// </param>
        /// <param name="razza_key">
        /// Chiavi composite razza "{GEN_COD}_{SPE_COD}_{RAZ_COD}", multi-select separato da virgola.
        /// </param>
        /// <param name="date_from">Data inizio range (ISO 8601). Default: ultimi 90 giorni.</param>
        /// <param name="date_to">Data fine range (ISO 8601). Default: oggi.</param>
        /// <param name="kg_per_day">Fattore accrescimento giornaliero kg/giorno (0.1-2.0). Default: 0.8.</param>
        /// <param name="alert_threshold_pct">Soglia scostamento percentuale per alert flag. Default: 10.0.</param>
        /// <param name="page">Numero pagina (>= 1). Default: 1.</param>
        /// <param name="limit">Record per pagina (max 10000). Default: 100.</param>
        /// <param name="sort_by">Colonna ordinamento: lid | data_pesata | scostamento_pct. Default: lid.</param>
        /// <param name="sort_order">Direzione: ASC | DESC. Default: ASC.</param>
        [HttpGet]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurveAccrescimento(
            [FromQuery] string   piva,
            [FromQuery] string?  stalla_key            = null,
            [FromQuery] string?  razza_key             = null,
            [FromQuery] string?  date_from             = null,
            [FromQuery] string?  date_to               = null,
            [FromQuery] double   kg_per_day            = 0.8,
            [FromQuery] double   alert_threshold_pct   = 10.0,
            [FromQuery] int      page                  = 1,
            [FromQuery] int      limit                 = 100,
            [FromQuery] string   sort_by               = "lid",
            [FromQuery] string   sort_order            = "ASC",
            [FromQuery] string   view_mode             = "detailed",
            [FromQuery] string?  aggregation_level     = null)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(
                    HttpContext.User.Identity as ClaimsIdentity, Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var objParametriUtenti = ExtractObjParametriUtentiAndSetCulture(auth);

                // ── Validazione input (Fail Fast) ──────────────────────────────────────────

                var (validationErrors, isForbidden) = ValidateQueryParams(
                    stalla_key, razza_key, date_from, date_to,
                    kg_per_day, alert_threshold_pct,
                    page, limit, sort_by, sort_order,
                    piva,
                    out DateTime parsedDateFrom,
                    out DateTime parsedDateTo);

                // ── Validazione view_mode e aggregation_level ──────────────────────────

                var validViewModes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "detailed", "aggregated" };
                if (!validViewModes.Contains(view_mode))
                    validationErrors.Add(new { field = "view_mode", value = view_mode, error = "Valore non valido. Atteso: detailed, aggregated." });
                else if (view_mode.Equals("aggregated", StringComparison.OrdinalIgnoreCase))
                {
                    var validAggLevels = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "animal", "razza_key", "stalla_key" };
                    if (string.IsNullOrWhiteSpace(aggregation_level))
                        validationErrors.Add(new { field = "aggregation_level", value = (object?)null, error = "Obbligatorio quando view_mode=aggregated." });
                    else if (!validAggLevels.Contains(aggregation_level))
                        validationErrors.Add(new { field = "aggregation_level", value = aggregation_level, error = "Valore non valido. Atteso: animal, razza_key, stalla_key." });

                    // In aggregated mode the widget shows all weighings: override the 90-day
                    // default only when the caller did not supply an explicit date_from.
                    if (string.IsNullOrWhiteSpace(date_from))
                        parsedDateFrom = new DateTime(2000, 1, 1);
                }

                if (isForbidden)
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { error = "FORBIDDEN", message = "Accesso non autorizzato alla stalla richiesta" });

                if (validationErrors.Count > 0)
                    return BadRequest(new { error = "INVALID_PARAMETERS", invalid_fields = validationErrors });

                // ── Costruzione DTO di query ───────────────────────────────────────────────

                var queryParams = new PesatureCurveAccrescimentoQueryParams
                {
                    Piva              = piva,
                    StallaPKeys       = stalla_key,
                    RazzaKeys         = razza_key,
                    DateFrom          = parsedDateFrom,
                    DateTo            = parsedDateTo,
                    KgPerDay          = kg_per_day,
                    AlertThresholdPct = alert_threshold_pct,
                    Page              = page,
                    Limit             = limit,
                    SortBy            = sort_by,
                    SortOrder         = sort_order,
                    ViewMode          = view_mode,
                    AggregationLevel  = aggregation_level ?? "animal"
                };

                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };

                // ── Invocazione servizio ───────────────────────────────────────────────────

                if (view_mode.Equals("aggregated", StringComparison.OrdinalIgnoreCase))
                {
                    OutData.Zoo.PesatureCurveAccrescimentoAggregatedResult aggregatedResult =
                        await _pesatureCurveService.LeggiPesatureCurveAccrescimentoAggregatedAsync(
                            queryParams, objParametriServer, objParametriUtenti);

                    resp.RispostaStringa = JsonConvert.SerializeObject(aggregatedResult, serializerSettings);
                }
                else
                {
                    OutData.Zoo.PesatureCurveAccrescimentoResult result =
                        await _pesatureCurveService.LeggiPesatureCurveAccrescimentoAsync(
                            queryParams, objParametriServer, objParametriUtenti);

                    resp.RispostaStringa = JsonConvert.SerializeObject(result, serializerSettings);
                }

                resp.RispostaOK = true;
                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        /// <summary>
        /// Esporta i dati delle pesature / curva di accrescimento in formato CSV o XLS.
        /// Supporta modalità detailed (pagina dedicata) e aggregated (widget dashboard).
        /// Vedere DS07-API: GET /api/v1/animals/weighing-curves/export.
        /// </summary>
        /// <param name="stalla_key">Filtro stalla — stessa semantica di DS06.</param>
        /// <param name="razza_key">Filtro razza — stessa semantica di DS06.</param>
        /// <param name="date_from">Data inizio range (ISO 8601).</param>
        /// <param name="date_to">Data fine range (ISO 8601).</param>
        /// <param name="kg_per_day">Fattore accrescimento giornaliero (0.1-2.0). Default: 0.8.</param>
        /// <param name="alert_threshold_pct">Soglia scostamento percentuale. Default: 10.0.</param>
        /// <param name="sort_by">Colonna ordinamento. Default: lid.</param>
        /// <param name="sort_order">Direzione: ASC | DESC. Default: ASC.</param>
        /// <param name="format">Formato file: CSV | XLS. Default: CSV.</param>
        /// <param name="view_mode">Modalità export: detailed | aggregated. Default: detailed.</param>
        /// <param name="aggregation_level">Livello aggregazione (obbligatorio se aggregated): animal | razza_key | stalla_key.</param>
        /// <param name="include_metadata">Includi footer metadata. Default: true.</param>
        [HttpGet("export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413RequestEntityTooLarge)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExport(
            [FromQuery] string   piva,
            [FromQuery] string?  stalla_key          = null,
            [FromQuery] string?  razza_key           = null,
            [FromQuery] string?  date_from           = null,
            [FromQuery] string?  date_to             = null,
            [FromQuery] double   kg_per_day          = 0.8,
            [FromQuery] double   alert_threshold_pct = 10.0,
            [FromQuery] string   sort_by             = "lid",
            [FromQuery] string   sort_order          = "ASC",
            [FromQuery] string   format              = "CSV",
            [FromQuery] string   view_mode           = "detailed",
            [FromQuery] string?  aggregation_level   = null,
            [FromQuery] bool     include_metadata     = true)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(
                    HttpContext.User.Identity as ClaimsIdentity, Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var objParametriUtenti = ExtractObjParametriUtentiAndSetCulture(auth);

                // ── Validazione input (Fail Fast) ──────────────────────────────────────────────────
                var (validationErrors, isForbidden) = ValidateExportParams(
                    stalla_key, razza_key, date_from, date_to,
                    kg_per_day, alert_threshold_pct,
                    sort_by, sort_order,
                    format, view_mode, aggregation_level,
                    piva,
                    out DateTime parsedDateFrom,
                    out DateTime parsedDateTo);

                if (isForbidden)
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new { error = "FORBIDDEN", message = "Accesso negato alla stalla richiesta" });

                if (validationErrors.Count > 0)
                    return BadRequest(new
                    {
                        error          = "INVALID_PARAMETERS",
                        message        = "Parametri non validi",
                        invalid_fields = validationErrors
                    });

                // ── Costruzione DTO export ─────────────────────────────────────────────────────────
                var exportParams = new PesatureExportQueryParams
                {
                    Piva              = piva,
                    StallaPKeys       = stalla_key,
                    RazzaKeys         = razza_key,
                    DateFrom          = parsedDateFrom,
                    DateTo            = parsedDateTo,
                    KgPerDay          = kg_per_day,
                    AlertThresholdPct = alert_threshold_pct,
                    SortBy            = sort_by,
                    SortOrder         = sort_order,
                    Format            = format,
                    ViewMode          = view_mode,
                    AggregationLevel  = aggregation_level,
                    IncludeMetadata   = include_metadata
                };

                // ── Invocazione servizio ───────────────────────────────────────────────────────────
                PesatureExportFileResult exportResult =
                    await _exportService.ExportAsync(exportParams, objParametriServer, objParametriUtenti);

                if (exportResult.RecordCount == 0)
                    return NotFound(new { error = "NO_DATA", message = "Nessun dato da esportare per i filtri specificati" });

                return File(exportResult.Content, exportResult.ContentType, exportResult.FileName);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("50 MB"))
            {
                // Vedere DS07-API, sezione "Risposte / 413".
                return StatusCode(StatusCodes.Status413RequestEntityTooLarge, new
                {
                    error          = "PAYLOAD_TOO_LARGE",
                    message        = "Il file generato supera i 50MB. Applicare filtri più restrittivi.",
                    recommendation = "Ridurre il range temporale o applicare filtri per stalla/razza"
                });
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        // ── Helpers di validazione ─────────────────────────────────────────────────────────────────
        private static (List<object> Errors, bool IsForbidden) ValidateExportParams(
            string? stallaPKeys,
            string? razzaKeys,
            string? dateFromStr,
            string? dateToStr,
            double kgPerDay,
            double alertThresholdPct,
            string sortBy,
            string sortOrder,
            string format,
            string viewMode,
            string? aggregationLevel,
            string pivaJwt,
            out DateTime parsedDateFrom,
            out DateTime parsedDateTo)
        {
            // Riusa la validazione comune di DS06
            var (errors, isForbidden) = ValidateQueryParams(
                stallaPKeys, razzaKeys, dateFromStr, dateToStr,
                kgPerDay, alertThresholdPct,
                page: 1, limit: 100,
                sortBy, sortOrder,
                pivaJwt,
                out parsedDateFrom,
                out parsedDateTo);

            // Parametri specifici export
            var validFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CSV", "XLS" };
            if (!validFormats.Contains(format))
                errors.Add(new { field = "format", value = format, error = "Formato non supportato. Atteso: CSV, XLS." });

            var validViewModes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "detailed", "aggregated" };
            if (!validViewModes.Contains(viewMode))
                errors.Add(new { field = "view_mode", value = viewMode, error = "Valore non valido. Atteso: detailed, aggregated." });

            if (viewMode.Equals("aggregated", StringComparison.OrdinalIgnoreCase))
            {
                var validAggLevels = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "animal", "razza_key", "stalla_key" };
                if (string.IsNullOrWhiteSpace(aggregationLevel))
                    errors.Add(new { field = "aggregation_level", value = (string?)null, error = "Obbligatorio quando view_mode=aggregated." });
                else if (!validAggLevels.Contains(aggregationLevel))
                    errors.Add(new { field = "aggregation_level", value = aggregationLevel, error = "Valore non valido. Atteso: animal, razza_key, stalla_key." });
            }

            return (errors, isForbidden);
        }

        // ── Helpers di validazione ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Valida i query parameters dell'endpoint DS06.
        /// Restituisce la lista degli errori di validazione e un flag che indica se la richiesta
        /// deve essere rifiutata con 403 (piva nel stalla_key non corrispondente al JWT).
        /// Vedere DS06-API, sezione "Sicurezza / Input Validation".
        /// </summary>
        private static (List<object> Errors, bool IsForbidden) ValidateQueryParams(
            string? stallaPKeys,
            string? razzaKeys,
            string? dateFromStr,
            string? dateToStr,
            double kgPerDay,
            double alertThresholdPct,
            int page,
            int limit,
            string sortBy,
            string sortOrder,
            string pivaJwt,
            out DateTime parsedDateFrom,
            out DateTime parsedDateTo)
        {
            var errors = new List<object>();
            bool isForbidden = false;

            // Default date range: ultimi 90 giorni
            parsedDateFrom = DateTime.Today.AddDays(-90);
            parsedDateTo   = DateTime.Today;

            if (!string.IsNullOrWhiteSpace(dateFromStr))
            {
                if (!DateTime.TryParse(dateFromStr, out parsedDateFrom))
                    errors.Add(new { field = "date_from", value = dateFromStr, error = "Formato data non valido. Atteso ISO 8601." });
            }

            if (!string.IsNullOrWhiteSpace(dateToStr))
            {
                if (!DateTime.TryParse(dateToStr, out parsedDateTo))
                    errors.Add(new { field = "date_to", value = dateToStr, error = "Formato data non valido. Atteso ISO 8601." });
            }

            if (parsedDateFrom > parsedDateTo)
                errors.Add(new { field = "date_from", value = dateFromStr, error = "date_from non può essere posteriore a date_to." });

            if (kgPerDay < 0.1 || kgPerDay > 2.0)
                errors.Add(new { field = "kg_per_day", value = kgPerDay.ToString(), error = "Valore fuori range: atteso 0.1-2.0." });

            if (alertThresholdPct < 0)
                errors.Add(new { field = "alert_threshold_pct", value = alertThresholdPct.ToString(), error = "Valore non può essere negativo." });

            if (page < 1)
                errors.Add(new { field = "page", value = page.ToString(), error = "Numero pagina deve essere >= 1." });

            if (limit < 1 || limit > 10000)
                errors.Add(new { field = "limit", value = limit.ToString(), error = "Valore fuori range: atteso 1-10000." });

            var validSortBy = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { "lid", "data_pesata", "scostamento_pct" };
            if (!validSortBy.Contains(sortBy))
                errors.Add(new { field = "sort_by", value = sortBy, error = "Valore non valido. Atteso: lid, data_pesata, scostamento_pct." });

            var validSortOrder = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ASC", "DESC" };
            if (!validSortOrder.Contains(sortOrder))
                errors.Add(new { field = "sort_order", value = sortOrder, error = "Valore non valido. Atteso: ASC, DESC." });

            // Validazione sicurezza stalla_key: il piva nel composite key deve corrispondere al JWT
            // Vedere DS06-API, sezione "Sicurezza / stalla_key".
            if (!string.IsNullOrWhiteSpace(stallaPKeys))
            {
                foreach (var key in stallaPKeys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var segments = key.Split('_');
                    if (segments.Length < 3)
                    {
                        errors.Add(new { field = "stalla_key", value = key, error = "Formato chiave non valido. Atteso: {piva}_{sa_cod}_{sta_num}." });
                        continue;
                    }

                    if (!int.TryParse(segments[^1], out _) || !int.TryParse(segments[^2], out _))
                    {
                        errors.Add(new { field = "stalla_key", value = key, error = "sa_cod e sta_num devono essere interi." });
                        continue;
                    }

                    string pivaSegment = string.Join("_", segments[..^2]);
                    if (!pivaSegment.Equals(pivaJwt, StringComparison.OrdinalIgnoreCase))
                    {
                        // Accesso negato: il piva nel filtro non corrisponde al JWT.
                        // Vedere DS06-API: "403 senza rivelare l'esistenza della stalla".
                        isForbidden = true;
                    }
                }
            }

            // Validazione formato razza_key: {GEN_COD}_{SPE_COD}_{RAZ_COD} — tutti interi
            if (!string.IsNullOrWhiteSpace(razzaKeys))
            {
                foreach (var key in razzaKeys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var parts = key.Split('_');
                    if (parts.Length != 3 || !int.TryParse(parts[0], out _) || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _))
                        errors.Add(new { field = "razza_key", value = key, error = "Formato chiave non valido. Atteso: {GEN_COD}_{SPE_COD}_{RAZ_COD} con valori interi." });
                }
            }

            return (errors, isForbidden);
        }
    }
}
