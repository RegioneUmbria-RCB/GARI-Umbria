using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaCoreDTOStd.OutData.Pratiche;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiProfiliPratiche;
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
    /// Controller per la gestione del profilo utente con filtri pratiche.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("v1/profilo-utente")]
    public class ProfiloUtentePraticheController : BaseController
    {
        private readonly IUtentiProfiliPraticheService _utentiProfiliPraticheService;

        public ProfiloUtentePraticheController(
            IServiceProvider provider,
            IOptions<SecuritySettings> securitySettings,
            IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _utentiProfiliPraticheService = provider.GetRequiredService<IUtentiProfiliPraticheService>();
        }

        /// <summary>
        /// Legge i filtri pratiche configurati per l'utente specificato.
        /// DS02-BL – GET /v1/profilo-utente/pratiche/{idUtente}
        /// </summary>
        [HttpGet("pratiche/{idUtente}")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfiloUtentePratiche([FromRoute] string idUtente)
        {
            RispostaStandard resp = new();

            if (string.IsNullOrWhiteSpace(idUtente))
            {
                resp.RispostaOK = false;
                resp.Errore = "Il parametro idUtente è obbligatorio.";
                return BadRequest(resp);
            }

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _utentiProfiliPraticheService.LeggiAsync(idUtente, objParametriDouble);

                resp.RispostaOK = true;
                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                return Ok(resp);
            }
            catch (KeyNotFoundException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return NotFound(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        /// <summary>
        /// Salva (crea o aggiorna) i filtri pratiche del profilo utente specificato.
        /// DS02-BL – PUT /v1/profilo-utente/pratiche
        /// </summary>
        [HttpPut("pratiche")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutProfiloUtentePratiche([FromBody] SalvaProfiloUtenteFiltriPratiche_IN inData)
        {
            RispostaStandard resp = new();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _utentiProfiliPraticheService.SalvaAsync(inData, objParametriDouble);

                resp.RispostaOK = true;
                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return BadRequest(resp);
            }
            catch (KeyNotFoundException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return NotFound(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        /// <summary>
        /// Preview della visibilità risultante da una configurazione filtri pratiche, senza persistere.
        /// DS06-API – POST /v1/profilo-utente/pratiche/{idUtente}/preview.
        /// </summary>
        [HttpPost("pratiche/{idUtente}/preview")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPreviewProfiloUtentePratiche([FromRoute] string idUtente, [FromBody] SalvaProfiloUtenteFiltriPratiche_IN inData)
        {
            RispostaStandard resp = new();

            if (string.IsNullOrWhiteSpace(idUtente))
            {
                resp.RispostaOK = false;
                resp.Errore = "Il parametro idUtente è obbligatorio.";
                return BadRequest(resp);
            }

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _utentiProfiliPraticheService.PreviewAsync(idUtente, inData, objParametriDouble);

                resp.RispostaOK = true;
                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return BadRequest(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        /// <summary>
        /// Restituisce la configurazione pratiche corrente dell'utente specificato.
        /// GET /v1/profilo-utente/ConfigurazionePraticheUtente?username={username}
        /// </summary>
        [HttpGet("ConfigurazionePraticheUtente")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConfigurazioneAsync([FromQuery] string username)
        {
            RispostaStandard resp = new();

            if (string.IsNullOrWhiteSpace(username))
            {
                resp.RispostaOK = false;
                resp.Errore = "Il parametro 'username' è obbligatorio.";
                return BadRequest(resp);
            }

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _utentiProfiliPraticheService.LeggiConfigurazioneAsync(username, objParametriDouble);

                resp.RispostaOK = true;
                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                return Ok(resp);
            }
            catch (KeyNotFoundException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return NotFound(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        /// <summary>
        /// Aggiorna la configurazione pratiche dell'utente specificato.
        /// POST /v1/profilo-utente/ConfigurazionePraticheUtente
        /// </summary>
        [HttpPost("ConfigurazionePraticheUtente")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostConfigurazioneAsync([FromBody] ConfigurazionePraticheUtente inData)
        {
            RispostaStandard resp = new();

            if (string.IsNullOrWhiteSpace(inData?.Username))
            {
                resp.RispostaOK = false;
                resp.Errore = "Il parametro 'Username' è obbligatorio.";
                return BadRequest(resp);
            }

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _utentiProfiliPraticheService.SalvaConfigurazioneAsync(inData, objParametriDouble);

                resp.RispostaOK = true;
                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return BadRequest(resp);
            }
            catch (KeyNotFoundException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return NotFound(resp);
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
