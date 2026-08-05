using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.CalcoloSostenibilitaCO2;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ColtureAziende;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ConsumoAziendale;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.FiliereAziende;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RecuperoPayloadM4Token;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RiepilogoRaccolti;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.TipiCarburante;
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
    /// Espone gli endpoint REST per il modulo Sostenibilità CO₂ (FMP M1).
    /// Percorso base: <c>v1/sostenibilita</c>.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("v1/sostenibilita")]
    public class SostenibilitaCo2Controller : BaseController
    {
        private readonly IFiliereAziendeDropdownService _filiereService;
        private readonly IColtureAziendeDropdownService _coltureService;
        private readonly IRiepilogoRaccoltiService _riepilogoService;
        private readonly IValidazioneConsumiAziendaliService _validazioneConsumiService;
        private readonly ITipiCarburanteDAL _tipiCarburanteDAL;
        private readonly ICalcoloSostenibilitaCO2Service _calcoloSostenibilitaCO2Service;
        private readonly IRecuperoPayloadM4TokenService _recuperoPayloadM4TokenService;

        /// <summary>
        /// Inizializza il controller e risolve i servizi dalla DI.
        /// </summary>
        public SostenibilitaCo2Controller(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _filiereService                  = provider.GetRequiredService<IFiliereAziendeDropdownService>();
            _coltureService                  = provider.GetRequiredService<IColtureAziendeDropdownService>();
            _riepilogoService                = provider.GetRequiredService<IRiepilogoRaccoltiService>();
            _validazioneConsumiService       = provider.GetRequiredService<IValidazioneConsumiAziendaliService>();
            _tipiCarburanteDAL               = provider.GetRequiredService<ITipiCarburanteDAL>();
            _calcoloSostenibilitaCO2Service  = provider.GetRequiredService<ICalcoloSostenibilitaCO2Service>();
            _recuperoPayloadM4TokenService   = provider.GetRequiredService<IRecuperoPayloadM4TokenService>();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // GET v1/sostenibilita/tipi-carburante
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Restituisce l'elenco dei tipi di carburante disponibili per popolare
        /// il dropdown nella pagina di immissione consumi (FS2.02).
        /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Input <c>carburanti[].tipo_carburante</c>.
        /// </summary>
        /// <returns>
        /// Lista di <see cref="TipoCarburanteDropdownItem"/> (<c>valore</c> + <c>etichetta</c>)
        /// serializzata in <see cref="RispostaStandard.RispostaStringa"/>.
        /// </returns>
        [HttpGet("tipi-carburante")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTipiCarburante()
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(
                    HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var entities = await _tipiCarburanteDAL.GetAllAsync(objParametriServer);

                var result = entities
                    .Select(e => new TipoCarburanteDropdownItem
                    {
                        Valore    = e.Car_Cod,
                        Etichetta = e.Car_Des
                    })
                    .ToList();

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
        // DS-15  GET v1/sostenibilita/filiere-aziende
        // ─────────────────────────────────────────────────────────────────────────

        [HttpGet("filiere-aziende")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetFiliereAziende()
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _filiereService.GetFiliereAsync(objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti);

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

        /// <summary>
        /// Restituisce le specie colturali distinte presenti negli impianti delle
        /// aziende figlie delle filiere selezionate.
        /// </summary>
        /// <param name="pivaFiliera">
        /// Uno o più P.IVA di filiera (query-string multi-value);
        /// risultato di <see cref="GetFiliereAziende"/>.
        /// </param>
        /// <returns>
        /// <see cref="ColtureAziendeDropdownResult"/> serializzato in
        /// <see cref="RispostaStandard.RispostaStringa"/>.
        /// </returns>
        [HttpGet("colture-aziende")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetColtureAziende([FromQuery] List<string> pivaFiliera)
        {
            if (pivaFiliera == null || pivaFiliera.Count == 0)
                return BadRequest("Specificare almeno una P.IVA di filiera.");

            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var result = await _coltureService.GetColtureAsync(pivaFiliera, objParametriServer);

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

        /// <summary>
        /// Calcola la tabella di riepilogo dei raccolti per tutte le aziende figlie
        /// della filiera selezionata, filtrata per coltura e anno.
        /// Sono incluse solo le operazioni con tutti gli esercizi chiusi.
        /// </summary>
        /// <param name="request">
        /// Corpo del messaggio: <c>pivaFiliera</c>, <c>vegCod</c>, <c>anno</c>.
        /// </param>
        /// <returns>
        /// <see cref="RiepilogoRaccoltiResult"/> serializzato in
        /// <see cref="RispostaStandard.RispostaStringa"/>.
        /// </returns>
        [HttpPost("riepilogo-raccolti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostRiepilogoRaccolti([FromBody] RiepilogoRaccoltiRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PivaFiliera) || request.Anno <= 0)
                return BadRequest("PivaFiliera e Anno sono obbligatori.");

            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var result = await _riepilogoService.GetRiepilogoAsync(request, objParametriServer);

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
        // DS02-BL  POST v1/sostenibilita/validazione-sostenibilita
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Valida server-side i dati di consumo aziendale (carburanti ed energia) immessi
        /// dall'utente, garantendo coerenza con il perimetro selezionato, completezza dei
        /// campi obbligatori, correttezza dei range di valori e formati data.
        /// </summary>
        /// <param name="request">
        /// Corpo del messaggio: <c>perimetroAziende</c>,
        /// <c>carburanti[]</c> e <c>energia[]</c>.
        /// </param>
        /// <returns>
        /// <see cref="ValidazioneConsumiAziendaliResult"/> serializzato in
        /// <see cref="RispostaStandard.RispostaStringa"/>.
        /// <c>RispostaOK</c> è <c>true</c> anche quando la validazione fallisce:
        /// leggere <c>ValidazioneEsito</c> nell'oggetto serializzato.
        /// </returns>
        [HttpPost("validazione-sostenibilita")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostValidazioneSostenibilita([FromBody] ValidazioneConsumiAziendaliRequest request)
        {
            if (request == null)
                return BadRequest("Il corpo della richiesta è obbligatorio.");

            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _validazioneConsumiService.ValidaAsync(request, objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti);

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
        // DS03-BL  POST v1/sostenibilita/calcolo-sostenibilita-co2
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// DS03-BL — Assembla il payload M4 per la filiera azienda (Sostenibilità CO2).
        /// </summary>
        /// <param name="request">Dati di input per l'assemblaggio del payload M4.</param>
        /// <returns>
        /// 200 OK con il payload M4 assemblato;
        /// 404 Not Found se i dati richiesti non esistono;
        /// 422 Unprocessable Entity se la struttura del payload non è valida.
        /// </returns>
        [HttpPost("calcolo-sostenibilita-co2")]
        [ProducesResponseType(typeof(RispostaStandard<PayloadCo2Root>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalcoloSostenibilitaCo2FilieraAzienda([FromBody] AssemblyPayloadCo2Input request)
        {
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                //var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                var risposta = await _calcoloSostenibilitaCO2Service.CalcoloSostenibilitaCO2Async(request, objParametriTriple.ObjParametriUtenti, objParametriTriple.ObjParametriServer, objParametriTriple.ObjParametriSuperServer);

                return Ok(new RispostaStandard<RispostaEngineCo2> { RispostaOK = true, RispostaStringa = risposta });
            }
            catch (ValidazioneFinalePayloadCo2Exception ex)
            {
                return UnprocessableEntity(new RispostaStandard<ValidazioneFinalePayloadCo2Output> { RispostaOK = false, RispostaStringa = ex.ValidazioneOutput });
            }
            catch (DataNotFoundException ex)
            {
                return NotFound(new RispostaStandard<RispostaEngineCo2> { RispostaOK = false, Errore = ex.Message });
            }
            catch (PayloadStructureException ex)
            {
                return UnprocessableEntity(new RispostaStandard<RispostaEngineCo2> { RispostaOK = false, Errore = ex.Message });
            }
            catch (AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions.SerializationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new RispostaStandard<RispostaEngineCo2> { RispostaOK = false, Errore = ex.Message });
            }
            catch (EngineCo2Exception ex)
            {
                var httpCode = ex.Codice switch
                {
                    CodiceErroreEngineCo2.ValidationError    => StatusCodes.Status422UnprocessableEntity,
                    CodiceErroreEngineCo2.AuthenticationError => StatusCodes.Status401Unauthorized,
                    CodiceErroreEngineCo2.RateLimitExceeded  => StatusCodes.Status429TooManyRequests,
                    _                                        => StatusCodes.Status503ServiceUnavailable
                };
                return StatusCode(httpCode, new RispostaStandard<RispostaEngineCo2> { RispostaOK = false, Errore = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new RispostaStandard<RispostaEngineCo2> { RispostaOK = false, Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex) });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // DS09-BL  GET v1/sostenibilita/token-generabili/anni
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// DS09-BL §Regola 1 — Restituisce gli anni distinti per cui esiste almeno
        /// un'invocazione M4 in modalità "Aziendale" visibile all'utente per la filiera
        /// indicata, ordinati per anno decrescente.
        /// Riferimento spec: DS09-BL RecuperoPayloadM4DaLookupToken.
        /// </summary>
        /// <param name="filiera">P.IVA della filiera selezionata (obbligatorio).</param>
        /// <returns>
        /// Array di interi serializzato in <see cref="RispostaStandard.RispostaStringa"/>.
        /// Lista vuota se nessun dato disponibile; <c>401</c> se nessuna azienda è visibile.
        /// </returns>
        [HttpGet("token-generabili/anni")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAnniTokenGenerabili([FromQuery] string filiera)
        {
            if (string.IsNullOrWhiteSpace(filiera))
                return BadRequest("Il parametro 'filiera' è obbligatorio.");

            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(
                    HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var anni = await _recuperoPayloadM4TokenService.GetAnniDisponibiliAsync(
                    filiera,
                    objParametriDouble.ObjParametriServer,
                    objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(anni, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (UnauthorizedAccessException ex)
            {
                resp.RispostaOK = false;
                resp.Errore     = ex.Message;
                return Unauthorized(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore     = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // DS09-BL  GET v1/sostenibilita/token-generabili
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// DS09-BL §Regola 2/3 — Restituisce la tabella "Token Generabili" per la filiera
        /// e l'anno indicati: una riga per azienda, corrispondente all'invocazione M4 più
        /// recente, con indicatori estratti da <c>json_risposta</c>.
        /// Riferimento spec: DS09-BL RecuperoPayloadM4DaLookupToken.
        /// </summary>
        /// <param name="filiera">P.IVA della filiera selezionata (obbligatorio).</param>
        /// <param name="anno">Anno campagna (obbligatorio).</param>
        /// <returns>
        /// Lista di <see cref="TokenGenerabileItem"/> serializzata in
        /// <see cref="RispostaStandard.RispostaStringa"/>.
        /// Lista vuota se nessun dato; <c>401</c> se nessuna azienda è visibile.
        /// </returns>
        [HttpGet("token-generabili")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTokenGenerabili(
            [FromQuery] string filiera,
            [FromQuery] int anno)
        {
            if (string.IsNullOrWhiteSpace(filiera))
                return BadRequest("Il parametro 'filiera' è obbligatorio.");

            if (anno <= 0)
                return BadRequest("Il parametro 'anno' deve essere un anno valido.");

            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(
                    HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);

                if (!auth.status)
                    return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var result = await _recuperoPayloadM4TokenService.GetTokenGenerabiliAsync(
                    filiera,
                    anno,
                    objParametriDouble.ObjParametriServer,
                    objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (UnauthorizedAccessException ex)
            {
                resp.RispostaOK = false;
                resp.Errore     = ex.Message;
                return Unauthorized(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore     = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }
    }
}
