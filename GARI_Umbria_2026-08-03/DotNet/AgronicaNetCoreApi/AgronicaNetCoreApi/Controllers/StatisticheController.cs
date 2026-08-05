using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Zone;
using AgronicaNetCore.Statistiche.BIZ.Services.Statistiche;
using AgronicaNetCoreApi.Resources;
using InData.Statistiche;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OutData.Varie;
using System.Data;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class StatisticheController : BaseController
    {
        private readonly IReportStatisticheService _reportStatisticheService;

        public StatisticheController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _reportStatisticheService = provider.GetRequiredService<IReportStatisticheService>();
        }

        [HttpPost]
        [Route(nameof(StatisticheUtilizzoGiornaliere))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> StatisticheUtilizzoGiornaliere([FromBody] ExportStatisticheUtilizzo request)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                string? result = await _reportStatisticheService.ImportReportElencoSinteticoAsync(
                    request.JobId,
                    request.JobAvvio,
                    request.FiltroPerDataCompetenzaOrDataRegistrazione,
                    request.IntervalloOperazioniDiCampagna,
                    request.IntervalloPratiche,
                    request.Username,
                    objParametriTriple.ObjParametriUtenti,
                    objParametriTriple.ObjParametriServer,
                    objParametriTriple.ObjParametriSuperServer
                );


                //bool? result = await _reportStatisticheService.ImportReportElencoSinteticoAsync(
                //    request.FiltroPerDataCompetenzaOrDataRegistrazione,
                //    request.IntervalloOperazioniDiCampagna,
                //    request.IntervalloPratiche,
                //    request.Username,
                //    objParametriTriple.ObjParametriUtenti,
                //    objParametriTriple.ObjParametriServer,
                //    objParametriTriple.ObjParametriSuperServer);

                //if (result.HasValue && !result.Value)
                //    throw new Exception("Il processo di popolamento delle statistiche di utilizzo ha generato un errore imprevisto");

                if (string.IsNullOrEmpty(result))
                    resp.RispostaOK = true;
                else
                    resp.Errore = result!;
                
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
        [Route(nameof(StatisticheUtilizzo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> StatisticheUtilizzo([FromBody] ExportStatisticheUtilizzo request)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                objAllegato? result = await _reportStatisticheService.ExportReportElencoSinteticoAsync(
                    request.FiltroPerDataCompetenzaOrDataRegistrazione,
                    request.IntervalloOperazioniDiCampagna,
                    request.IntervalloPratiche,
                    request.DettagliQdC,
                    request.Username,
                    request.Piva,
                    objParametriTriple.ObjParametriUtenti,
                    objParametriTriple.ObjParametriServer,
                    objParametriTriple.ObjParametriSuperServer);

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
