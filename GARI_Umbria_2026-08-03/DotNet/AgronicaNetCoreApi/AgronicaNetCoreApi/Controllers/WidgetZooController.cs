using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCoreApi.Resources;
using AgronicaNetCore.Widgets.BIZ.Services.WidgetZoo;
using AgronicaCoreDTOStd.InData.Widgets;

namespace AgronicaNetCoreApi.Controllers
{
    [Authorize]
    [ApiController]
    public class WidgetZooController : BaseController
    {
        private readonly IWidgetZoo _widgetZoo;

        public WidgetZooController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _widgetZoo = provider.GetRequiredService<IWidgetZoo>();
        }

        [HttpPost]
        [Route(nameof(GetInvalidAnimals))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInvalidAnimals(Widget_Zoo_IN data)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objPs = ExtractObjParametriDoubleAndSetCulture(auth);

                var res = await _widgetZoo.GetInvalidAnimals(data.Piva, data.Sa_Cod, data.Sta_Num, data.timeStart, objPs.ObjParametriServer, objPs.ObjParametriUtenti);
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

        [HttpPost]
        [Route(nameof(GetTreatmentsToDo))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTreatmentsToDo(Widget_Zoo_IN data)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objPs = ExtractObjParametriDoubleAndSetCulture(auth);

                var res = await _widgetZoo.GetTreatmentsToDo(data.Piva, data.Sa_Cod, data.Sta_Num, data.timeStart, objPs.ObjParametriServer, objPs.ObjParametriUtenti);
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

        [HttpPost]
        [Route(nameof(GetTreatmentsToSend))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTreatmentsToSend(Widget_Zoo_IN data)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objPs = ExtractObjParametriDoubleAndSetCulture(auth);

                var res = await _widgetZoo.GetTreatmentsToSend(data.Piva, data.Sa_Cod, data.Sta_Num, data.timeStart, objPs.ObjParametriServer, objPs.ObjParametriUtenti);
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

        [HttpPost]
        [Route(nameof(GetExpiringDrugs))]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExpiringDrugs(Widget_Zoo_IN data)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objPs = ExtractObjParametriDoubleAndSetCulture(auth);

                var res = await _widgetZoo.GetExpiringDrugs(data.Piva, data.timeStart, objPs.ObjParametriServer, objPs.ObjParametriUtenti);
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
