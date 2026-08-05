using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese;
using AgronicaNetCore.Anagrafe.BIZ.Services.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.BIZ.Services.Impianti;
using AgronicaNetCore.Anagrafe.BIZ.Services.Imprese;
using AgronicaNetCore.Anagrafe.BIZ.Services.Zone;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AnagrafeController : BaseController
    {
        private readonly IZoneService _zoneService;
        private readonly ICodiciAnagrafeService _codiciAnagrafeService;
        private readonly IImpresaService _impresaService;
        private readonly IImpiantiService _impiantiService;
        private readonly IAlberoGerarchiaImpreseFASTService _alberoGerarchiaImpreseService;

        public AnagrafeController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _zoneService = provider.GetRequiredService<IZoneService>();
            _codiciAnagrafeService = provider.GetRequiredService<ICodiciAnagrafeService>();
            _impresaService = provider.GetRequiredService<IImpresaService>();
            _alberoGerarchiaImpreseService = provider.GetRequiredService<IAlberoGerarchiaImpreseFASTService>();
            _impiantiService = provider.GetRequiredService<IImpiantiService>();
        }

        [HttpPost]
        [Route(nameof(GetZone))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetZone([FromBody] int Zona_Cod)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable zone = await _zoneService.LeggiZoneAsync(Zona_Cod, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(zone, Formatting.Indented);
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
        [Route(nameof(GetCodiciAnagrafeUsatixEntita))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCodiciAnagrafeUsatixEntita([FromBody] LeggiCodiciUsatixEntitaAnagrafe_IN inData)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                List<CodiceAnagrafeBase> codiciAnagrafe = await _codiciAnagrafeService.LeggiCodiciUsatixEntitaAsync(inData, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(codiciAnagrafe, Formatting.Indented);
                resp.RispostaOK = true;

                codiciAnagrafe.Clear();

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
        [Route(nameof(GetCodiciDestinazioniDUso))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCodiciDestinazioniDUso()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable destinazioniDUso = await _codiciAnagrafeService.LeggiCodiciUsatixEntitaDestinazioniDUsoAsync(objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(destinazioniDUso, Formatting.Indented);
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
        [Route(nameof(GetImpresePadre))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetImpresePadre()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable impresePadre = await _impresaService.LeggiPadriAsync(objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(impresePadre, Formatting.Indented);
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
        [Route(nameof(GetAlberoGerarchiaImprese))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAlberoGerarchiaImprese()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status)
                    return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var kendoHierarchicalDataSource = await _alberoGerarchiaImpreseService.GetNodesGearchiaObjAsync(objParametriDouble.ObjParametriServer,
                    objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(kendoHierarchicalDataSource, Formatting.Indented);
                resp.RispostaOK = true;

                kendoHierarchicalDataSource.Dispose();

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

        [HttpPost]
        [Route(nameof(LeggiImprese))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiImprese([FromBody] string piva)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var imprese = await _impresaService.LeggiImpreseAsync(piva, objParametriServer);
                resp.RispostaStringa = JsonConvert.SerializeObject(imprese, Formatting.Indented);
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
        [Route(nameof(GetContributiACA))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContributiACA()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var contributi = await _impiantiService.GetContributiACAAsync(objParametriServer);
                resp.RispostaStringa = JsonConvert.SerializeObject(contributi, Formatting.Indented);
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
