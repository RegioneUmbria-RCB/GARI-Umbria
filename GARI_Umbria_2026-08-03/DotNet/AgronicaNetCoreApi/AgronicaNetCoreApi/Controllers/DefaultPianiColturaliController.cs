using System.Security.Claims;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.DefaultPianiColturali.BIZ.Services;
using AgronicaNetCoreApi.Resources;
using InData.DefaultPianiColturali;
using InData.SpecieVegetali;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AgronicaNetCoreApi.Controllers;

[ApiController]
[Authorize]
[Route("default-piani-colturali")]
public class DefaultPianiColturaliController : BaseController
{
    private readonly IDefaultPianiColturaliService _defaultPianiColturaliService;

    public DefaultPianiColturaliController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
    {
        _defaultPianiColturaliService = provider.GetRequiredService<IDefaultPianiColturaliService>();
    }

    [HttpGet("default-distinta-produzione")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiDefaultDistintaDiProduzione([FromQuery] string? piva, [FromQuery] int vegCod)
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

            var result = await _defaultPianiColturaliService.LeggiDefaultDistintaDiProduzioneAsync(piva ?? "", vegCod, objParametriServer);

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

    [HttpPost("default-distinta-produzione")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> SalvaDefaultDistintaDiProduzione([FromBody] DistintaProduzione_In body)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

            var result = await _defaultPianiColturaliService.SalvaDefaultDistintaDiProduzioneAsync(body, objParametriServer);

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

    [HttpGet("default-generale-specie")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiDefaultGeneraleSpecie([FromQuery] string? piva)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

            var result = await _defaultPianiColturaliService.LeggiDefaultGeneraleSpecieAsync(piva ?? "", objParametriServer);

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

    [HttpPost("default-generale-specie")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> ScriviDefaultSpecie([FromBody] SpecieVegetali_In body)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

            var result = await _defaultPianiColturaliService.ScriviSpecieVegetaleAsync(body, objParametriServer);

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

    [HttpDelete("default-generale-specie")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancellaDefaultSpecie([FromQuery] string? piva, [FromQuery] int vegCod)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

            var result = await _defaultPianiColturaliService.CancellaSpecieVegetaleAsync(piva ?? "", vegCod, objParametriServer);

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

    [HttpGet("default-generali")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeggiDefaultGenerali([FromQuery] string? piva)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

            var result = await _defaultPianiColturaliService.LeggiDefaultGeneraliAsync(piva ?? "", objParametriServer);

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

    [HttpPost("default-generali")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> ScriviDefaultGenerali([FromBody] DefaultGeneraliColtura_In body)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

            var result = await _defaultPianiColturaliService.ScriviDefaultGeneraliInizialiAsync(body, objParametriServer);
            result = result && await _defaultPianiColturaliService.ScriviDefaultSpecieInizialiAsync(body, objParametriServer);

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

    [HttpPut("default-generali")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> AggiornaDefaultGenerali([FromBody] DefaultGenerali_In body)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result = await _defaultPianiColturaliService.ScriviDefaultGeneraliAsync(body, objParametriServer);

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

    [HttpDelete("default-generali")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancellaDefaultGenerali([FromQuery] string? piva, [FromQuery] int vegCod,
        [FromQuery] int culCod)
    {
        var resp = new RispostaStandard();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
            if (!auth.status)
            {
                return Unauthorized();
            }

            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var result = await _defaultPianiColturaliService.CancellaDefaultGeneraleAsync(piva ?? "", vegCod, culCod, objParametriServer);

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