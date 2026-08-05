using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiImpostazioni;
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
    [Route("[controller]")]
    public class UtentiController : BaseController
    {
        private readonly IUtentiImpostazioniService _utentiService;

        public UtentiController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _utentiService = provider.GetRequiredService<IUtentiImpostazioniService>();
        }

        [HttpGet]
        [Route(nameof(GetAnnataAgraria))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAnnataAgraria()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var dataInizioEFine = await _utentiService.LeggiAnnataAgrariaAsync(objParametriDouble.ObjParametriUtenti, objParametriDouble.ObjParametriServer);
                DateTime inizioCampagna = dataInizioEFine.DataInizio;
                DateTime fineCampagna = dataInizioEFine.DataFine;
                
                var res = new { inizioCampagna, fineCampagna };

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
