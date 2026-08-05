using AgronicaCoreDTOStd.InData.ConfrontoCatasto;
using AgronicaNetCore.Anagrafe.BIZ.Services.ConfrontoCatasto;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreUtilityStd;
using Newtonsoft.Json;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    public class ConfrontoCatastoController : BaseController
    {
        private readonly IConfrontoCatastoService _confrontoCatastoService;

        public ConfrontoCatastoController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _confrontoCatastoService = provider.GetRequiredService<IConfrontoCatastoService>();
        }


        [HttpPost]
        [Route("LeggiConfrontoCatasto")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfrontoCatasto([FromBody] ParametriTipoConfronto parametri)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var cmpCatasto = await _confrontoCatastoService.GetConfrontoCatastoAsync(parametri, objParametriServer);

                if (cmpCatasto == null) throw new Exception("Nessun dato recuperato");

                resp.RispostaStringa = JsonConvert.SerializeObject(cmpCatasto, Formatting.Indented);
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
