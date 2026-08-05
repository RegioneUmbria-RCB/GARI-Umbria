using System.Data;
using System.Security.Claims;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.CodificheCAC.BIZ.Services;
using AgronicaNetCore.CodificheCAC.DAL.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AgronicaNetCoreApi.Resources;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CodificaCACController : BaseController
    {
        private readonly ICodificheCACService _codificheCACService;
        private readonly ILogger<CodificaCACController> _logger;
        private readonly IStringLocalizer<Messages> _localizer;

        public CodificaCACController(
            IServiceProvider provider, IOptions<SecuritySettings> securitySettings, ILogger<CodificaCACController> logger, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _codificheCACService = provider.GetRequiredService<ICodificheCACService>();
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// Recupera tutti i record della tabella CAC_Codifica_Dati_SistemiEsterni
        /// </summary>
        /// <returns>Lista di codifiche CAC</returns>
        [HttpPost]
        [Route(nameof(GetCacCodifiche))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCacCodifiche()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable codifiche = await _codificheCACService.GetAllCodificheAsync(objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(codifiche);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                var respd = new RispostaStandard
                {
                    RispostaOK = false,
                    Errore = _localizer["ErrorGetCAC"] + ex.Message,
                };

                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        /// <summary>
        /// Crea una nuova codifica CAC.
        /// </summary>
        /// <param name="model">Modello della codifica CAC da creare.</param>
        /// <returns>Risultato dell'operazione.</returns>
        [HttpPost]
        [Route(nameof(WriteCacCodifiche))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteCacCodifiche([FromBody] CodificaCACModel model)
        {
            RispostaStandard resp = new();
            try
            {
         
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                if (model.TipoOperazioneDB == Enum_DBTypeOperation.Update)
                {
                    resp.RispostaOK = await _codificheCACService.UpdateCac(model, objParametriServer);
                   
                }
                else if (model.TipoOperazioneDB == Enum_DBTypeOperation.Write) 
                {
                    resp.RispostaOK = await _codificheCACService.WriteCac(model, objParametriServer);
                   
                }
                else if (model.TipoOperazioneDB == Enum_DBTypeOperation.Delete)
                {
                    resp.RispostaOK = await _codificheCACService.DeleteCac(model, objParametriServer);
                  
                }

                resp.RispostaStringa = _localizer["EditSuccess"];
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new RispostaStandard
                {
                    RispostaOK = false,
                    Errore = _localizer["WriteError"] + ex.Message
                });
            }
        }

    }
}