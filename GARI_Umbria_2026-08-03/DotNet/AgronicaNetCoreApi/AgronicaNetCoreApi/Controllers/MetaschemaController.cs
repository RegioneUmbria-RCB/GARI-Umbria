using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.OutData.FiltroRicerca;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.MetaSchema.BIZ.Services.DivisioniAmministrative;
using AgronicaNetCore.MetaSchema.BIZ.Services.LettureStatiche;
using AgronicaNetCore.MetaSchema.BIZ.Services.Operazioni;
using AgronicaNetCore.MetaSchema.BIZ.Services.Servizi;
using AgronicaNetCore.MetaSchema.BIZ.Services.SpecieVegetali;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using AgronicaNetCore.MetaSchema.BIZ.Services.Contatti;
using AgronicaNetCore.MetaSchema.BIZ.Services.GruppiOperazione;
using AgronicaNetCore.MetaSchema.BIZ.Services.ParcoMacchine;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.PrincipiAttivi;
using Microsoft.Extensions.Localization;
using AgronicaNetCoreApi.Resources;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisura;

namespace AgronicaNetCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class MetaschemaController : BaseController
    {
        private readonly ISpecieVegetaliService _specieVegetaliService;
        private readonly IDivisioniAmministrativeService _divisioniAmministrativeService;
        private readonly ILettureStaticheService _lettureStaticheService;
        private readonly IServiziService _serviziService;
        private readonly IOperazioniService _operazioniService;
        private readonly IGruppiOperazioneService _gruppiOperazioniService;
        private readonly IParcoMacchineService _parcoMacchinaService;
        private readonly IContattiService _contattiService;
        private readonly IPrincipiAttivi _principiAttivi;
        private readonly IUnitaMisura _unitaMisuraService;

        public MetaschemaController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _specieVegetaliService = provider.GetRequiredService<ISpecieVegetaliService>();
            _divisioniAmministrativeService = provider.GetRequiredService<IDivisioniAmministrativeService>();
            _lettureStaticheService = provider.GetRequiredService<ILettureStaticheService>();
            _serviziService = provider.GetRequiredService<IServiziService>();
            _operazioniService = provider.GetRequiredService<IOperazioniService>();
            _gruppiOperazioniService = provider.GetRequiredService<IGruppiOperazioneService>();
            _parcoMacchinaService = provider.GetRequiredService<IParcoMacchineService>();
            _contattiService = provider.GetRequiredService<IContattiService>();
            _principiAttivi = provider.GetRequiredService<IPrincipiAttivi>();
            _unitaMisuraService = provider.GetRequiredService<IUnitaMisura>();
        }

        [HttpPost]
        [Route(nameof(GetSpecie_FiltroUtente))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpecie_FiltroUtente([FromBody] LeggiSpecieVegetali_IN leggiSpecie_IN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var dtConVisibilita_OUT = await _specieVegetaliService.SpecieVegetali_GestioneFiltroUtente_LeggiAsync(
                    leggiSpecie_IN, objParametriDouble.ObjParametriUtenti, objParametriDouble.ObjParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(dtConVisibilita_OUT, Formatting.Indented);
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
        [Route(nameof(GetCultivar))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCultivar([FromBody] Cultivar_GestioneFiltroUtente_Leggi_IN leggiCultivar_IN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                DataTable cultivar = await _specieVegetaliService.Cultivar_GestioneFiltroUtente_LeggiAsync(leggiCultivar_IN, objParametriDouble.ObjParametriUtenti, objParametriDouble.ObjParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(cultivar, Formatting.Indented);
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
        [Route(nameof(GetGruppiVegetali_FiltroUtente))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGruppiVegetali_FiltroUtente(int gru_cod)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                DataTable gruppiVegetali = await _specieVegetaliService.GruppoVegetale_GestioneFiltroUtente_LeggiAsync(gru_cod, objParametriDouble.ObjParametriUtenti, objParametriDouble.ObjParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(gruppiVegetali, Formatting.Indented);
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
        [Route(nameof(GetGruppiVarietali))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGruppiVarietali([FromBody] LeggiGruppiVarietali_IN leggiGruppiVarietali_IN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable gruppiVarietali = await _specieVegetaliService.LeggiGruppiVarietaliAsync(leggiGruppiVarietali_IN,objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(gruppiVarietali, Formatting.Indented);
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
        [Route(nameof(LeggiOperazioniPerTipo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiOperazioniPerTipo([FromQuery] string tipoGruppoOperazione)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var operazioni = await _operazioniService.LeggiOperazioniPerTipoAsync(tipoGruppoOperazione,objParametriServer);

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

        [HttpGet]
        [Route(nameof(LeggiParcoMacchine))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiParcoMacchine([FromQuery] string? piva)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var parcoMacchine = await _parcoMacchinaService.ParcoMacchine_LeggiAsync(piva ?? "",
                    objParametriDouble.ObjParametriServer,
                    objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(parcoMacchine, Formatting.Indented);
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
        [Route(nameof(LeggiContatti))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiContatti([FromQuery] string? piva)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var contatti = await _contattiService.Contatti_LeggiAsync(
                    piva ?? "sadasdas", // This 'sadasdas' value comes from the old backend
                    objParametriDouble.ObjParametriServer,
                    objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(contatti, Formatting.Indented);
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
        [Route(nameof(GetStati))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStati([FromBody] string codice)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable stati = await _divisioniAmministrativeService.LeggiStatiAsync(codice,objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(stati, Formatting.Indented);
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
        [Route(nameof(GetRegioni))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegioni([FromBody] LeggiRegioni_IN leggiRegioniIN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable regioni = await _divisioniAmministrativeService.LeggiRegioniAsync(leggiRegioniIN,objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(regioni, Formatting.Indented);
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
        [Route(nameof(GetProvince))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvince([FromBody] LeggiProvince_IN leggiProvinceIN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable province = await _divisioniAmministrativeService.LeggiProvinceAsync(leggiProvinceIN,objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(province, Formatting.Indented);
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
        [Route(nameof(GetComuni))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComuni([FromBody] LeggiComuni_IN istatParametriIN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable comuni = await _divisioniAmministrativeService.LeggiComuniAsync(istatParametriIN,objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(comuni, Formatting.Indented);
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
        [Route(nameof(GetMetodiDiProduzione))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMetodiDiProduzione()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                ExtractObjParametriServerAndSetCulture(auth);

                DataTable metodiDiProduzione = await _lettureStaticheService.LeggiMetodiDiProduzioneAsync();

                resp.RispostaStringa = JsonConvert.SerializeObject(metodiDiProduzione, Formatting.Indented);
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
        [Route(nameof(GetServizi))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServizi([FromBody] LeggiServizi_IN leggiServiziIN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable servizi = await _serviziService.LeggiServiziEffettivamenteUsatiAsync(leggiServiziIN,objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(servizi, Formatting.Indented);
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
        [Route(nameof(GetServiziStati))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiziStati([FromBody] LeggiServiziStati_IN leggiServiziStatiIN)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable servizi_stati = await _serviziService.LeggiServizi_StatiAsync(leggiServiziStatiIN,objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(servizi_stati, Formatting.Indented);
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
        [Route(nameof(GetOperazioni_FiltroUtente))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperazioni_FiltroUtente([FromBody] LeggiOperazioni_IN inData)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                DtConVisibilita_OUT dtConVisibilita_OUT = new();

                dtConVisibilita_OUT = await _operazioniService.Operazioni_GestioneFiltroUtente_LeggiAsync(inData, objParametriDouble.ObjParametriServer,
                    objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(dtConVisibilita_OUT, Formatting.Indented);
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
        [Route(nameof(GetGruppiOperazione))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGruppiOperazione()
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var dataTable = await _gruppiOperazioniService.GruppiOperazione_LeggiAsync(objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
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

        [HttpGet("varieta")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiVarieta([FromQuery] int culCod, [FromQuery] int vegCod)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var result = await _specieVegetaliService.LeggiVarietaAsync(culCod, vegCod, objParametriDouble.ObjParametriUtenti, objParametriDouble.ObjParametriServer);

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

        [HttpGet("specie/aziendali")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiSpecieAziendali([FromQuery] string piva)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var result = await _specieVegetaliService.LeggiSpecieAziendaliAsync(piva, objParametriServer);

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

        [HttpGet("varieta/aziendali")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiVarietaFiltered([FromQuery] string? piva, [FromQuery] int culCod,
            [FromQuery] int vegCod)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var result = await _specieVegetaliService.LeggiVarietaFilteredAsync(piva ?? "", culCod, vegCod, objParametriServer);

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
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [Route(nameof(GetPrincipiAttivi))]
        public async Task<IActionResult> GetPrincipiAttivi()
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var result = await _principiAttivi.LeggiPrincipiAttiviAsync(objParametriServer);

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
        [ProducesResponseType(typeof(RispostaStandard<BaseCodeDescr[]>), StatusCodes.Status200OK)]
        [Route(nameof(GetUnitaMisuraProtocolli))]
        public async Task<IActionResult> GetUnitaMisuraProtocolli()
        {
            var resp = new RispostaStandard<BaseCodeDescr[]>();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                DataTable result = await _unitaMisuraService.LeggiAsyncxProtocolli(objParametriServer);

                BaseCodeDescr[] unitaMisuraList = result.AsEnumerable()
                    .Select(row => new BaseCodeDescr
                    {
                        codice = row.Field<int>("Udm_Cod"),
                        descrizione = row.Field<string>("Udm_Sim")
                    }).ToArray();

                resp.RispostaStringa = unitaMisuraList;
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