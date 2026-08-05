using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AgronicaNetCore.FiltroRicerca.BIZ.Services;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class FiltroRicercaController : BaseController
    {
        private readonly IFiltroRicercaService _filtroRicercaService;

        public FiltroRicercaController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _filtroRicercaService = provider.GetRequiredService<IFiltroRicercaService>();
        }

        [HttpPost]
        [Route(nameof(GetResult))]
        [ProducesResponseType(typeof(RispostaStandard<CriteriRicerca_OUT>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetResult([FromBody] AgronicaCoreDTOStd.InData.FiltroRicerca.CriteriRicerca_IN criteriRicerca_IN)
        {
            RispostaStandard<CriteriRicerca_OUT> resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();
                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                var result = await _filtroRicercaService.GetResultAsync(criteriRicerca_IN, objParametriTriple.ObjParametriServer, objParametriTriple.ObjParametriUtenti, objParametriTriple.ObjParametriSuperServer) ?? throw new Exception("Nessun dato recuperato");

                var res = new CriteriRicerca_OUT { kendoColumns = result.kendoColumns.ToArray(), result = result.result, overflowSelectTopRows = result.overflowSelectTopRows };

                resp.RispostaStringa = res;
                resp.RispostaOK = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }

        }

        [HttpGet]
        [Route(nameof(GetPivaReale))]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPivaReale(string piva)
        {
            RispostaStandard<string> resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();
                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                string result = await _filtroRicercaService.GetPivaRealeAsync(piva, objParametriTriple.ObjParametriServer) ?? throw new Exception("Nessun dato recuperato");

                resp.RispostaStringa = result;
                resp.RispostaOK = true;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

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
