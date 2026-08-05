using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Services.CalcoloRischiMeteo;
using AgronicaNetCore.RischiMeteo.BIZ.Services.PerimetroRaccolti;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
    /// <summary>
    /// Espone gli endpoint REST per il modulo Rischi Meteoclimatici (FMP M1).
    /// Percorso base: <c>v1/rischi</c>.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("v1/rischi")]
    public class RischiMeteoController : BaseController
    {
        private readonly IPerimetroRaccoltiRischiService _perimetroService;
        private readonly ICalcoloRischiMeteoService _calcoloRischiMeteoService;

        /// <summary>
        /// Inizializza il controller e risolve i servizi dalla DI.
        /// </summary>
        public RischiMeteoController(
            IServiceProvider provider,
            IOptions<SecuritySettings> securitySettings,
            IStringLocalizer<Messages> localizer)
            : base(provider, securitySettings, localizer)
        {
            _perimetroService = provider.GetRequiredService<IPerimetroRaccoltiRischiService>();
            _calcoloRischiMeteoService = provider.GetRequiredService<ICalcoloRischiMeteoService>();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // DS01-BL  POST v1/rischi/riepilogo-raccolti
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Restituisce la tabella di riepilogo del perimetro di calcolo per una filiera:
        /// tutte le combinazioni Azienda + Appezzamento + Impianto + Esercizio
        /// visibili all'utente (Cono di Visibilità) con esercizio in stato Aperto.
        /// Riferimento spec: DS01-BL CaricamentoPerimetroFiltrato.
        /// </summary>
        /// <param name="request">Corpo del messaggio: <c>piva_filiera</c>.</param>
        /// <returns>
        /// <see cref="PerimetroRaccoltiResult"/> serializzato in
        /// <see cref="RispostaStandard.RispostaStringa"/>.
        /// </returns>
        [HttpPost("riepilogo-raccolti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostRiepilogoRaccolti([FromBody] PerimetroRaccoltiRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PivaFiliera))
                return BadRequest("PivaFiliera è obbligatoria.");

            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(
                    HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var result = await _perimetroService.GetPerimetroAsync(request, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // DS02-BL  POST v1/rischi/calcolo-rischi
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Avvia il calcolo dei rischi meteoclimatici per gli Esercizi colturali selezionati
        /// all'interno di una Filiera. Per ogni Esercizio: costruisce il payload M2 da dati GIAS,
        /// invoca l'Engine M2 e persiste la risposta in <c>Lookup_Rischio_Meteo</c>.
        /// I fallimenti per singolo Esercizio sono isolati: gli altri proseguono.
        /// Riferimento spec: DS02-BL CostruttoPayloadM2.
        /// </summary>
        [HttpPost("calcolo-rischi")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CalcoloRischiMeteoOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalcolaRischi([FromBody] CalcoloRischiMeteoInput? input)
        {
            if (input is null)
                return BadRequest(new { error = "BAD_REQUEST", message = "Body della richiesta obbligatorio." });

            if (string.IsNullOrWhiteSpace(input.PivaFiliera))
                return BadRequest(new { error = "BAD_REQUEST", message = "Parametro 'pivaFiliera' obbligatorio." });

            if (input.Perimetro.Count == 0)
                return BadRequest(new { error = "BAD_REQUEST", message = "Specificare almeno un Esercizio in 'eserciziSelezionati'." });

            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
            if (!auth.status)
                return Unauthorized(new { error = "UNAUTHORIZED", message = "Token di autorizzazione assente o non valido." });

            try
            {
                //var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                var result = await _calcoloRischiMeteoService.CalcolaRischiMeteoAsync(input, objParametriTriple.ObjParametriServer, objParametriTriple.ObjParametriSuperServer);
                return Ok(new RispostaStandard<CalcoloRischiMeteoOutput> { RispostaOK = true, RispostaStringa = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new RispostaStandard<CalcoloRischiMeteoOutput> { RispostaOK = false, Errore = "Token di autorizzazione assente o non valido." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error     = "INTERNAL_ERROR",
                    message   = "Errore nel calcolo dei rischi meteoclimatici.",
                    timestamp = DateTime.UtcNow.ToString("O"),
                    detail    = ex.Message
                });
            }
        }
    }
}
