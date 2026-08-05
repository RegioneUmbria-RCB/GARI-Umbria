using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using AgronicaNetCore.ProfilazioneMacchine.BIZ.Services;
using InData.ProfilazioneMacchina;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;

namespace AgronicaNetCoreApi.Controllers;

[ApiController]
[Authorize]
[Route("profilazione-macchine")]
public class ProfilazioneMacchineController : BaseController
{
    private readonly IProfilazioneMacchineService _profilazioneMacchineService;

    public ProfilazioneMacchineController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
    {
        _profilazioneMacchineService = provider.GetRequiredService<IProfilazioneMacchineService>();
    }

    [HttpGet("leggi")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> Leggi([FromQuery] int idProfiloDati)
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
            var result = await _profilazioneMacchineService.LeggiAsync(idProfiloDati, objParametriServer);

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

    [HttpPut("caratteristiche")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCaratteristicheMacchina([FromBody] CaratteristicheMacchina_In body)
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
            var result = await _profilazioneMacchineService.UpdateCaratteristicheMacchinaAsync(body, objParametriServer);

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