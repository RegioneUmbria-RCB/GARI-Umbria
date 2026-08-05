using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Operazione.BIZ.Services.ActivityImport;
using InData.Agenda;
using InData.ActivityImport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    public class ImportAttivitaController : BaseController
    {
        private readonly IActivityImportService _activityImportService;

        public ImportAttivitaController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _activityImportService = provider.GetRequiredService<IActivityImportService>();
        }

        [HttpPost]
        [Route("orogel/import/operazioniQdC")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportActivityOrogel([FromBody] ActivityImportJsonObject importJObject)
        {
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                OutData.ActivityImport.ActivityImportResult result = await _activityImportService.ImportExternalActivity(
                    importJObject, enum_Esportazioni_Sistema_Cod.OnPlantImport_Conferimenti,
                    objParametriServer);

                return Ok(result);
            }
            catch (Exception ex)
            {
                OutData.ActivityImport.ActivityImportResult result = new();
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
        }
    }
}
