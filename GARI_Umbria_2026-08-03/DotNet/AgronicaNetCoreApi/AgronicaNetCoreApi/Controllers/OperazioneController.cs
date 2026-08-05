using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.MetaSchema.BIZ.Services.Operazioni;
using AgronicaNetCore.Operazione.BIZ.Services.OperazioneCausale;
using AgronicaNetCoreApi.Resources;
using AgronicaNetCore.Operazione.BIZ.Services.Trattamenti;
using AgronicaNetCore.Operazione.DAL.DataLayer.Trattamenti;
using InData.Operazione;
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
    public class OperazioneController : BaseController
    {
        private readonly IOperazioneCausaleService _operazioneCausaleService;
        private readonly IOperazioniService _operazioniService;
        private readonly ITrattamentiService _trattamentiService;

        public OperazioneController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _operazioneCausaleService = provider.GetRequiredService<IOperazioneCausaleService>();
            _operazioniService = provider.GetRequiredService<IOperazioniService>();
            _trattamentiService = provider.GetRequiredService<ITrattamentiService>();
        }

        [HttpPost]
        [Route("GetOperazioneCausale")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperazioneCausale([FromBody] LeggiOperazione leggiOperazione)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var operazioniCausali = await _operazioneCausaleService.OperazioneCausale_LeggiAsync(leggiOperazione, objParametriServer);
                resp.RispostaStringa = JsonConvert.SerializeObject(operazioniCausali, Formatting.Indented);
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
        [Route("GetOperazioni")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperazioni([FromBody] LeggiOperazioni_IN leggiOperazioni)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var operazioni = await _operazioniService.Operazioni_GestioneFiltroUtente_LeggiAsync(leggiOperazioni, objParametriDouble.ObjParametriServer,
                                                                                                 objParametriDouble.ObjParametriUtenti);
                resp.RispostaStringa = JsonConvert.SerializeObject(operazioni, Formatting.Indented);
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
        [Route("SaveOperazioneCausale")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveOperazioneCausale([FromBody] OperazioneCausale_In body)
        {

            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var result = await _operazioneCausaleService.OperazioneCausale_ScriviModificaAsync(body, objParametriDouble.ObjParametriServer,
                                                                                                         objParametriDouble.ObjParametriUtenti);

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

        [HttpDelete]
        [Route("DeleteOperazioneCausale")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancellaOperazioneCausale([FromQuery] int id, int lavCod)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var result = await _operazioneCausaleService.OperazioneCausale_CancellaAsync(id, lavCod, objParametriServer);
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

        [HttpGet]
        [Route("LeggiTrattamentiPerAvversita")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiTrattamentiPerAvversita([FromQuery] string Piva, int Sa_Cod, int APPEZZA, int ID_REG, int Av_Cod, int Intervallo)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var result = await _trattamentiService.LeggiTrattamentiPerAvversitaAsync(Piva, Sa_Cod, APPEZZA, ID_REG, Av_Cod, Intervallo, objParametriServer);

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
