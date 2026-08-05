using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using AgronicaNetCore.Note.BIZ.Services;
using AgronicaNetCore.ProfilazioneImprese.BIZ.Services;
using InData.Note;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;

namespace AgronicaNetCoreApi.Controllers;

[ApiController]
[Authorize]
[Route("note")]
public class NoteController : BaseController
{
    private readonly INoteService _noteService;
    private readonly IProfilazioneImpreseService _profilazioneImpreseService;

    public NoteController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
    {
        _noteService = provider.GetRequiredService<INoteService>();
        _profilazioneImpreseService = provider.GetRequiredService<IProfilazioneImpreseService>();
    }

    [HttpGet("leggi-note-intervento-utilizzo")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiNoteInterventoUtilizzo()
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
            var result = await _noteService.LeggiNoteInterventoUtilizzoAsync(0, objParametriServer);

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

    [HttpGet("leggi-profilazione-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiProfilazioneNote([FromQuery] int notaUtilizzoCod)
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
                await _noteService.LeggiProfilazioneNoteAsync(notaUtilizzoCod, 0, true, "", objParametriServer);

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

    [HttpPost("salva-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> SalvaNote([FromBody] SalvaNote_In body)
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
            var result = await _profilazioneImpreseService.SalvaNoteAsync(body, objParametriServer);

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

    [HttpGet("leggi-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiNote([FromQuery] int vegCod, [FromQuery] int lavCod, [FromQuery] string? piva)
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
            var result = await _noteService.LeggiNoteAsync(piva ?? "", lavCod, vegCod, objParametriServer);

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

    [HttpGet("leggi-gruppo-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiGruppoNote()
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
            var result = await _noteService.LeggiGruppoNoteAsync(objParametriServer);

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
    public async Task<IActionResult> CancellaNota([FromQuery] int notaCod)
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
            var result = await _noteService.CancellaNotaAsync(notaCod, objParametriServer);

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

    [HttpPost("gruppo-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> SalvaGruppoNote([FromBody] SalvaGruppoNote_In body)
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
            var result = await _noteService.SalvaGruppoNoteAsync(body, objParametriServer);

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

    [HttpPut("gruppo-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> AggiornaGruppoNote([FromBody] SalvaGruppoNote_In body)
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
            var result = await _noteService.AggiornaGruppoNoteAsync(body, objParametriServer);

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

    [HttpDelete("gruppo-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancellaGruppoNote([FromQuery] int gruppoNoteCod)
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
            var result = await _noteService.CancellaGruppoNoteAsync(gruppoNoteCod, objParametriServer);

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

    [HttpGet("note-utilizzo")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiNoteXUtilizzo([FromQuery] int notaUtilizzoCod, [FromQuery] int notaGruppoCod)
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
            var result = await _noteService.LeggiNoteXUtilizzoAsync(notaUtilizzoCod, notaGruppoCod, objParametriServer);

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

    [HttpPost("note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> AggiungiNota([FromQuery] string notaDes, int notaGruppoCod)
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
            var result = await _noteService.AggiungiNotaAsync(notaDes, notaGruppoCod, objParametriServer);

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

    [HttpGet("note-utilizzo-gruppi")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiNoteInterventoUtilizzoGruppi([FromQuery] int notaGruppoCod,
        [FromQuery] int notaUtilizzoCod)
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
                await _noteService.LeggiNoteInterventoUtilizzoGruppiAsync(notaGruppoCod, notaUtilizzoCod, objParametriServer);

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

    [HttpPost("salva-gruppo-note")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> SalvaGruppoNoteCompleto([FromBody] SalvaGruppoNoteCompleto_In body)
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
            var result = await _noteService.SalvaGruppoNoteCompletoAsync(body, objParametriServer);

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