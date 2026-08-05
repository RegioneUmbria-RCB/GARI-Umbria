using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using AgronicaNetCore.ProfilazioneImprese.BIZ.Services;
using InData.ProfilazioneImprese;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;

namespace AgronicaNetCoreApi.Controllers;

[ApiController]
[Authorize]
[Route("profilazione-imprese")]
public class ProfilazioneImpreseController : BaseController
{
    private readonly IProfilazioneImpreseService _profilazioneImpreseService;

    public ProfilazioneImpreseController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
    {
        _profilazioneImpreseService = provider.GetRequiredService<IProfilazioneImpreseService>();
    }

    [HttpGet("leggi")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> Leggi([FromQuery] string? piva, [FromQuery] int idProfiloDati,
        [FromQuery] string codiceChiave, [FromQuery] string idGruppo, [FromQuery] int lavCod, [FromQuery] int vegCod,
        [FromQuery] bool leggiSoloNoteConLavorazioneNullSeLavCodZero)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result = await _profilazioneImpreseService.LeggiAsync(piva ?? "", idProfiloDati, codiceChiave, idGruppo, lavCod,
                vegCod, leggiSoloNoteConLavorazioneNullSeLavCodZero, objParametriServer);

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

    [HttpPost("salva-profilazione")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> SalvaProfilazione([FromBody] SalvaProfilazione_In body)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result = await _profilazioneImpreseService.SalvaProfilazioneAsync(body, objParametriServer);

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

    [HttpGet("leggi-profilazione-macchine-contatti")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiProfilazioneMacchineXContatti([FromQuery] string? piva, [FromQuery] int vegCod)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result =
                await _profilazioneImpreseService.LeggiProfilazioneMacchineXContattiAsync(piva ?? "", vegCod, objParametriServer);

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

    [HttpDelete]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> EliminaProfilazione([FromQuery] string idGruppo,
        [FromQuery] int notaUtilizzoCod, [FromQuery] int lavCod, [FromQuery] int vegCod, [FromQuery] string? piva)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result =
                await _profilazioneImpreseService.CancellaAsync(piva ?? "", notaUtilizzoCod, idGruppo, lavCod, vegCod, objParametriServer);

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

    [HttpPut("propaga-su-tutte-le-lavorazioni")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> PropagaSuTutteLeLavorazioni([FromQuery] int lavCod, [FromQuery] int vegCod,
        [FromQuery] string? piva)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result =
                await _profilazioneImpreseService.PropagaSuTutteLeOperazioniAsync(piva ?? "", lavCod, vegCod, objParametriServer);

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
    

    [HttpPut("aggiorna-ore-minuti")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> AggiornaOreMinuti([FromBody] AggiornaOreMinuti_In body)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result =
                await _profilazioneImpreseService.AggiornaOreMinutiAsync(body, objParametriServer);

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
}