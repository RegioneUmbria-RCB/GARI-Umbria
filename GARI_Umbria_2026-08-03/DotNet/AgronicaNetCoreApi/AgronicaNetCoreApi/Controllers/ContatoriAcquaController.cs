using AgronicaCoreDTOStd.InData.DomandaIrrigua;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.DomandaIrrigua.BIZ.Services;
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
    public class ContatoriAcquaController : BaseController
    {
        private readonly IContatoriAcquaService _contatoriAcquaService;

        public ContatoriAcquaController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _contatoriAcquaService = provider.GetRequiredService<IContatoriAcquaService>();
        }

        [HttpPost]
        [Route(nameof(GetLetturaContatori))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLetturaContatori([FromBody] LettureContatori_IN lettureContatori_IN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var result = await _contatoriAcquaService.GetLettureContatoriAsync(lettureContatori_IN, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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

        [HttpPost]
        [Route(nameof(WriteLetturaContatori))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteLetturaContatori([FromBody] ScriviLetturaContatore scriviLetturaContatore)
        {
            RispostaStandard resp = new();
            bool result;
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                if (scriviLetturaContatore.Flag_Cancellazione)
                    result = await _contatoriAcquaService.DeleteLetturaContatore(scriviLetturaContatore, objParametriServer);
                else
                    result = await _contatoriAcquaService.WriteUpdateLetturaContatore(scriviLetturaContatore, objParametriServer);

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

        [HttpPost]
        [Route(nameof(GetContatori))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContatori([FromBody] LeggiContatori_IN leggiContatori_IN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var result = await _contatoriAcquaService.GetContatoriAssociatiAppezzamenti(leggiContatori_IN, objParametriServer) ?? throw new Exception("Nessun dato recuperato");
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
