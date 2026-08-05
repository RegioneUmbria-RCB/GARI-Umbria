using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Widgets.BIZ.Services.WidgetStatistiche;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    public class WidgetDocumentaleController : BaseController
    {
        private readonly IWidgetDocumentaleService _widgetDocumentaleService;

        public WidgetDocumentaleController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _widgetDocumentaleService = provider.GetRequiredService<IWidgetDocumentaleService>();
        }

        [HttpPost]
        [Route("GetDocumentRecap")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentRecap([FromBody] DateTime timeStart)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var res = await _widgetDocumentaleService.GetDocumentRecapAsync(timeStart, objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti) ?? throw new Exception("Nessun dato recuperato");
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
}
