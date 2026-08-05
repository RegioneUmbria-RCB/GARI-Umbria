using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using InData.Zoo;
using OutData.Zoo;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCoreApi.Resources;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Zoo.DAL.DataLayer.Terapie;
using AgronicaNetCore.Zoo.BIZ.Services.Terapie;

namespace AgronicaNetCoreApi.Controllers
{

    [Authorize]
    [ApiController]
    [Route("terapie")]
    public class TerapieController : BaseController
    {
        private readonly ITerapie _terapieDal;
        private readonly ITerapieService _terapieBiz;

        public TerapieController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _terapieDal = provider.GetRequiredService<ITerapie>();
            _terapieBiz = provider.GetRequiredService<ITerapieService>();
        }

        [HttpPost(nameof(ReadTerapieGrid))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReadTerapieGrid([FromBody] ReadTerapie body)
        {
            var resp = new RispostaStandard();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                if (string.IsNullOrEmpty(body.Piva)) throw new ArgumentNullException(nameof(body.Piva));

                var objP_Server = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
                var objP_Utenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

                var result = await _terapieDal.ReadTerapieGrid(body.Piva, body.Sa_Cod ?? 0, body.Sta_Num ?? 0, body.Data, objP_Server);

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

        [HttpPost(nameof(GetTerapia))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTerapia([FromQuery] int? Id_Terapia)
        {
            var resp = new RispostaStandard();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                if (Id_Terapia == 0) throw new ArgumentNullException(nameof(Id_Terapia));

                var objP_Server = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
                var objP_Utenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

                var result = await _terapieBiz.GetTerapia(Id_Terapia ?? 0, objP_Server);

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

        [HttpPost(nameof(CreateTerapia))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTerapia([FromBody] TerapiaZoo body)
        {
            var resp = new RispostaStandard();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                if (string.IsNullOrEmpty(body.Piva)) throw new ArgumentNullException(nameof(body.Piva));

                var objP_Server = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
                var objP_Utenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

                var result = await _terapieBiz.CreateTerapia(body, objP_Server);

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
        
        [HttpPost(nameof(UpdateTerapia))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTerapia([FromBody] TerapiaZoo body)
        {
            var resp = new RispostaStandard();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                if (string.IsNullOrEmpty(body.Piva)) throw new ArgumentNullException(nameof(body.Piva));

                var objP_Server = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
                var objP_Utenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

                var result = await _terapieBiz.UpdateTerapia(body, objP_Server);

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

        [HttpPost(nameof(DeleteTerapia))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTerapia([FromQuery] int Id_Terapia)
        {
            var resp = new RispostaStandard();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                if (Id_Terapia == 0) throw new ArgumentNullException(nameof(Id_Terapia));

                var objP_Server = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
                var objP_Utenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

                var res = await _terapieBiz.DeleteTerapia(Id_Terapia, objP_Server);

                resp.RispostaStringa = "";
                resp.RispostaOK = res;
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost(nameof(AddInterventoToTerapia))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddInterventoToTerapia([FromQuery] int Id_Terapia, [FromBody] InterventoZoo intervento)
        {
            var resp = new RispostaStandard();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                if (Id_Terapia == 0 || intervento == null || intervento.IsValid()) throw new ArgumentNullException(nameof(Id_Terapia));

                var objP_Server = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
                var objP_Utenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

                var res = await _terapieBiz.AddInterventoToTerapia(Id_Terapia, intervento, objP_Server);

                resp.RispostaStringa = "";
                resp.RispostaOK = res;
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost(nameof(UpdateInterventoInTerapia))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateInterventoInTerapia([FromQuery] int Id_Terapia, [FromBody] InterventoZoo intervento)
        {
            var resp = new RispostaStandard();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                if (Id_Terapia == 0 || intervento == null || intervento.IsValid()) throw new ArgumentNullException(nameof(Id_Terapia));

                var objP_Server = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
                var objP_Utenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

                var res = await _terapieBiz.UpdateInterventoInTerapia(Id_Terapia, intervento, objP_Server);

                resp.RispostaStringa = "";
                resp.RispostaOK = res;
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
