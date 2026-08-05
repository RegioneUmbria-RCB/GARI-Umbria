using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.CalcoloBilancioIdrico;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
    /// <summary>
    /// Espone gli endpoint REST per il modulo Sostenibilità H2O (FMP M1).
    /// Percorso base: <c>v1/sostenibilita-h2o</c>.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("v1/sostenibilita-h2o")]
    public class SostenibilitaH2OController : BaseController
    {
        private readonly ICalcoloSostenibilitaH2OService _calcoloH2OService;

        /// <summary>
        /// Inizializza il controller e risolve i servizi dalla DI.
        /// </summary>
        public SostenibilitaH2OController(
            IServiceProvider provider,
            IOptions<SecuritySettings> securitySettings,
            IStringLocalizer<Messages> localizer)
            : base(provider, securitySettings, localizer)
        {
            _calcoloH2OService = provider.GetRequiredService<ICalcoloSostenibilitaH2OService>();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // DS-02.2-BL  POST v1/sostenibilita-h2o/calcolo-sostenibilita-h2o
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Avvia il calcolo del bilancio idrico per il perimetro aziendale e gli esercizi specificati.
        /// Riferimento spec: DS-02.2-BL Chiamata WebApi Calcolo Bilancio Idrico.
        /// </summary>
        [HttpPost("calcolo-sostenibilita-h2o")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalcoloSostenibilitaH2O([FromBody] CalcoloBilancioIdricoRequest request)
        {
            if (request?.Perimetro is null || request.Esercizi.Count == 0)
                return BadRequest("Perimetro e almeno un esercizio sono obbligatori.");

            foreach (var esercizio in request.Esercizi.Where(e => string.IsNullOrWhiteSpace(e.Nazione)))
                esercizio.Nazione = "ITA";

            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(
                    HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                var risultato = await _calcoloH2OService.CalcoloSostenibilitaH2OAsync(
                    request,
                    objParametriTriple.ObjParametriServer,
                    objParametriTriple.ObjParametriSuperServer);

                resp.RispostaStringa = risultato;
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
    }
}
