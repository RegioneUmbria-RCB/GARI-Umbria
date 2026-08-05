using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using AgronicaCoreDTOStd.InData.Widgets;
using AgronicaNetCore.Widgets.BIZ.Services.WidgetIndici;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Converters;
using AgronicaNetCoreApi.Resources;

namespace AgronicaNetCoreApi.Controllers;

[ApiController]
[Authorize]
public class WidgetIndiciController : BaseController
{
    private readonly IWidgetIndiciService _widgetIndiciService;

    public WidgetIndiciController(
        IServiceProvider provider,
        IOptions<SecuritySettings> securitySettings,
        IStringLocalizer<Messages> localizer)
        : base(provider, securitySettings, localizer)
    {
        _widgetIndiciService = provider.GetRequiredService<IWidgetIndiciService>();
    }

    [HttpPost("widget-kpi")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWidgetKpi([FromBody] WidgetRequestKpi payload)
    {
        RispostaStandard resp = new();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status) return Unauthorized();

            var objParamsServer = ExtractObjParametriServerAndSetCulture(auth);

            var res = await _widgetIndiciService.GetWidgetKpiAsync(payload.year, payload.piva, objParamsServer) ??
                      throw new Exception("Nessun dato recuperato");
            resp.RispostaStringa = JsonConvert.SerializeObject(res, Formatting.Indented, new StringEnumConverter());
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

    [HttpPost("widget-produttivita")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWidgetProduttivita([FromBody] WidgetRequestIndiciProduttivita payload)
    {
        RispostaStandard resp = new();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status) return Unauthorized();

            var objParamsServer = ExtractObjParametriServerAndSetCulture(auth);

            var res = await _widgetIndiciService.GetWidgetProduttivitaAsync(payload, objParamsServer) ??
                      throw new Exception("Nessun dato recuperato");
            resp.RispostaStringa = JsonConvert.SerializeObject(res, Formatting.Indented);
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

    [HttpPost("available-years-indici-produttivita")]
    [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableYearsIndiciProduttivita([FromBody] string piva)
    {
        RispostaStandard resp = new();
        try
        {
            var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                Request.Headers);
            if (!auth.status) return Unauthorized();

            var objParamsServer = ExtractObjParametriServerAndSetCulture(auth);

            var res = await _widgetIndiciService.GetAvailableYearsIndiciProduttivitaAsync(piva, objParamsServer) ??
                      throw new Exception("Nessun dato recuperato");
            resp.RispostaStringa = JsonConvert.SerializeObject(res, Formatting.Indented);
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