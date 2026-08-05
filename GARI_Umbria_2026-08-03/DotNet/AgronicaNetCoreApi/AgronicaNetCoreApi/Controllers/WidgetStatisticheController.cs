using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using AgronicaNetCore.Widgets.BIZ.Services.WidgetStatistiche;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    public class WidgetStatisticheController : BaseController
    {
        private readonly IWidgetStatisticheService _widgetStatisticheService;

        public WidgetStatisticheController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _widgetStatisticheService = provider.GetRequiredService<IWidgetStatisticheService>();
        }

        [HttpPost]
        [Route("GetCountries")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCountries([FromBody] int year)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var countries = await _widgetStatisticheService.GetCountriesAsync(year, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
                resp.RispostaStringa = JsonConvert.SerializeObject(countries, Formatting.Indented);
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
        [Route("GetGeneralStatistics")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGeneralStatistics([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetGeneralStatisticsAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetMappedFarmers")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMappedFarmers([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetMappedFarmersAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetMovedMappedFarmersPriorWeek")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovedMappedFarmersPriorWeek([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetMovedMappedFarmersPriorWeekAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetMovedMappedFarmersCampaignBegin")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovedMappedFarmersCampaignBegin([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetMovedMappedFarmersCampaignBeginAsync(input, objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetCropMap")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCropMap([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetCropMapAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetFarmersHarvestSowingData")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFarmersHarvestSowingData([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetFarmersHarvestSowingDataAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetPlotsHarvestSowingData")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlotsHarvestSowingData([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetPlotsHarvestSowingDataAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetTargetHA")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTargetHA([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetTargetHAAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
        [Route("GetFarmerxRegionxRange")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFarmerxRegionxRange([FromBody] AgronicaCoreDTOStd.InData.Widgets.Widget_Statistics_IN input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var res = await _widgetStatisticheService.GetFarmerxRegionxRangeAsync(input, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
