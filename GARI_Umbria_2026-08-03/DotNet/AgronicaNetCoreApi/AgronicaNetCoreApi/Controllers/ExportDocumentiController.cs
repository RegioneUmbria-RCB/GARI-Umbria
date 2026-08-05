using AgronicaCoreModelsSTD.Utility;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Documentale.BIZ.Services;
using AgronicaNetCoreApi.Resources;
using InData.DataExchange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OutData.DataExchange;
using System.Security.Claims;


namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ExportDocumentiController : BaseController
    {
        private readonly AgronicaNetCore.Documentale.BIZ.Services.IExportDocumentiService _ExportDocumentiService;

        public ExportDocumentiController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _ExportDocumentiService = provider.GetRequiredService<IExportDocumentiService>();
        }

        [HttpPost]
        [Route(nameof(LeggiDocPortaleSocio))]
        [ProducesResponseType(typeof(Api_Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiDocPortaleSocio([FromBody] InData.DataExchange.ExportDocumenti_In ExportDocumenti_In)
        {
            Api_Response resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();
                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                if (string.IsNullOrEmpty(ExportDocumenti_In.CUAA))
                    throw new Exception("Indicare un CUAA.");

                string result;
                if (ExportDocumenti_In.Id_Tipologia == CostantiPersonalizzate.TIPOLOGIA_ANALISI_PDC)
                {
                    result = await _ExportDocumentiService.GetDocumentiAnalisiPDCExportAsync(ExportDocumenti_In, objParametriTriple.ObjParametriServer, objParametriTriple.ObjParametriSuperServer) ?? throw new Exception("Nessun dato recuperato");
                }
                else
                {
                    result = await _ExportDocumentiService.GetDocumentiExportAsync(ExportDocumenti_In, objParametriTriple.ObjParametriServer, objParametriTriple.ObjParametriSuperServer) ?? throw new Exception("Nessun dato recuperato");
                }

                resp.dati = result;
                if (string.IsNullOrEmpty(result))
                    resp.message = "Nessun documento recuperato";
                else
                    resp.message = Api_Response_Message_Type.Ok;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.errore = ex.Message;
                resp.message = "";
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(SincroDocPortaleSocio))]
        [ProducesResponseType(typeof(Api_Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> SincroDocPortaleSocio([FromBody] SincroExportDocumenti_In SincroExportDocumenti_IN)
        {
            Api_Response resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();
                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);

                if (string.IsNullOrEmpty(SincroExportDocumenti_IN.CUAA))
                    throw new Exception("Indicare un CUAA.");

                string result = "";
                if (SincroExportDocumenti_IN.Id_Tipologia == CostantiPersonalizzate.TIPOLOGIA_ANALISI_PDC)
                {
                    result = await _ExportDocumentiService.SincroDocumentiAnalisiPDCExportAsync(SincroExportDocumenti_IN, objParametriTriple.ObjParametriServer, objParametriTriple.ObjParametriSuperServer) ?? throw new Exception("Nessun dato recuperato");
                }
                
                resp.dati = result;
                if (string.IsNullOrEmpty(result))
                    resp.message = "Nessun documento aggiornato";
                else
                    resp.message = Api_Response_Message_Type.Ok;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.errore = ex.Message;
                resp.message = "";
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

    }
}
