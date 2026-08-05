using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Operazione.BIZ.Services.ReportImpiegoFitosanitari;
using AgronicaNetCoreApi.Resources;
using InData.QuadernoDiCampagna;
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
    [Route("[controller]")]
    public class QuadernoDiCampagnaController : BaseController
    {
        private readonly IReportImpiegoProdottiFitosanitariService _reportImpiegoProdottiFitosanitariService;
        public QuadernoDiCampagnaController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _reportImpiegoProdottiFitosanitariService = provider.GetRequiredService<IReportImpiegoProdottiFitosanitariService>();
        }

        [HttpPost]
        [Route(nameof(GetReportImpiegoProdottiFitosanitari))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReportImpiegoProdottiFitosanitari([FromBody]ReportImpiegoProdottiFitosanitari_IN reportImpiegoProdottiFitosanitariIN)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var result = await _reportImpiegoProdottiFitosanitariService.GetReportImpiegoFitosanitariAsync(reportImpiegoProdottiFitosanitariIN,
                    objParametriServer);

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
}
