using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.MetaSchema.BIZ.Services.Servizi;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
    /// <summary>
    /// Controller per il recupero del catalogo pratiche/servizi.
    /// DS10 – API: GET /v1/catalogo/pratiche
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("v1/catalogo")]
    public class CatalogoPraticheController : BaseController
    {
        private static readonly HashSet<string> _sortValues = new(StringComparer.OrdinalIgnoreCase) { "ASC", "DESC" };

        private readonly IServiziService _serviziService;

        public CatalogoPraticheController(
            IServiceProvider provider,
            IOptions<SecuritySettings> securitySettings,
            IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _serviziService = provider.GetRequiredService<IServiziService>();
        }

        /// <summary>
        /// Restituisce il catalogo completo delle pratiche/servizi disponibili nel sistema.
        /// Supporta filtro testo, ordinamento, esclusione scaduti e paginazione.
        /// DS10 – API: GET /v1/catalogo/pratiche (§ Specifiche Tecniche – GET Recupero Catalogo Pratiche).
        /// </summary>
        /// <param name="inData">Query params mappati su <see cref="LeggiCatalogoServizi_IN"/>: filter, sort, includeExpired, page, pageSize.</param>
        [HttpGet("pratiche")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCatalogoPratiche([FromQuery] LeggiCatalogoServizi_IN inData)
        {
            RispostaStandard resp = new();

            if (!_sortValues.Contains(inData.Sort))
            {
                resp.RispostaOK = false;
                resp.Errore = "Il parametro sort deve essere 'ASC' o 'DESC'.";
                return BadRequest(resp);
            }

            if (inData.Page < 1)
            {
                resp.RispostaOK = false;
                resp.Errore = "Il parametro page deve essere >= 1.";
                return BadRequest(resp);
            }

            if (inData.PageSize < 1 || inData.PageSize > 1000)
            {
                resp.RispostaOK = false;
                resp.Errore = "Il parametro pageSize deve essere compreso tra 1 e 1000.";
                return BadRequest(resp);
            }

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable dt = await _serviziService.LeggiCatalogoServiziAsync(inData, objParametriServer);

                int totalCount = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["TotalCount"]) : 0;
                int totalPages = (int)Math.Ceiling((double)totalCount / inData.PageSize);

                var catalogo = dt.AsEnumerable().Select(row => new
                {
                    servizio_cod = row["Servizio_Cod"],
                    servizio_descrizione = row["Servizio_Des"],
                    dataValiditaInizio = row["Validita_Inizio"],
                    dataValiditaFine = row["Validita_Fine"],
                    isValida = row["IsValida"],
                    isAttiva = row["IsAttiva"]
                });

                var result = new
                {
                    success = true,
                    catalogo,
                    total_count = totalCount,
                    page = inData.Page,
                    page_size = inData.PageSize,
                    total_pages = totalPages,
                    timestamp_recupero = DateTime.UtcNow
                };

                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                resp.RispostaOK = true;
                Response.Headers.CacheControl = "max-age=3600";
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }
    }
}

