using AgronicaCoreDTOStd.InData.Zoo;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Magazzino.BIZ.Services;
using AgronicaNetCore.MetaSchema.BIZ.Services.Lista_Razze_Animali;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.OperazioniZoo;
using AgronicaNetCore.OperazioniZoo.BIZ.Services.Trattamenti;
using AgronicaNetCore.Zoo.BIZ.Services.Prescrizioni;
using AgronicaNetCoreApi.Resources;
using InData.OperazioniZoo;
using InData.Zoo;
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
    public class OperazioniZooController : BaseController
    {
        private readonly IOperazioniZooService _operazioniZooService;
        private readonly ITrattamentoZooService _trattamentoZooService;
        private readonly IPrescrizionIService _prescrizionIService;
        private readonly IMagazzinoService _magazzinoServiceBIZ;
        private readonly IListaRazzeAnimaliService _listaRazzeAnimaliService;

    public OperazioniZooController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, securitySettings, localizer)
        {
            _operazioniZooService = provider.GetRequiredService<IOperazioniZooService>();
            _trattamentoZooService = provider.GetRequiredService<ITrattamentoZooService>();
            _prescrizionIService = provider.GetRequiredService<IPrescrizionIService>();
            _magazzinoServiceBIZ = provider.GetRequiredService<IMagazzinoService>();
            _listaRazzeAnimaliService = provider.GetRequiredService<IListaRazzeAnimaliService>();
        }

        [HttpGet]
        [Route(nameof(GetCentriAziendali))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCentriAziendali([FromQuery] string piva)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var objParametriUtenti = ExtractObjParametriUtentiAndSetCulture(auth);
                DataTable centri = await _operazioniZooService.LeggiCentriAziendaliZooAsync(piva, objParametriServer, objParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(centri, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpGet]
        [Route(nameof(GetStalle))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStalle([FromQuery] string piva, int centro)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var objParametriUtenti = ExtractObjParametriUtentiAndSetCulture(auth);
                DataTable stalle = await _operazioniZooService.LeggiStalleZooAsync(piva, centro, objParametriServer, objParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(stalle, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpGet]
        [Route(nameof(GetRaggruppamenti))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRaggruppamenti([FromQuery] string piva, int centro, int stanum)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                //Verificare se centro è valorizzato

                DataTable ragguppamenti = await _operazioniZooService.LeggiStalleRaggruppamentiZooAsync(piva, centro, stanum, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(ragguppamenti, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpGet]
        [Route(nameof(GetRazzeZoo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRazzeZoo()
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                DataTable razze = await _listaRazzeAnimaliService.LeggiRazzeAsync(objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(razze, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpGet]
        [Route(nameof(GetOperazioni))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperazioni()
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {

                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var operazioni = await _operazioniZooService.LeggiOperazioniZooAsync(objParametriDouble.ObjParametriServer,
                                                                                     objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(operazioni, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpGet]
        [Route(nameof(GetOperazioniPreferite))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperazioniPreferite()
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {

                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var operazioniPreferite = await _operazioniZooService.LeggiOperazioniZooPreferiteAsync(objParametriDouble.ObjParametriServer,
                                                                                                       objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(operazioniPreferite, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetOperazioniAgenda))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperazioniAgenda([FromBody] OperazioniAgendaInput parameterInput)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {

                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();


                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var operazioni = await _operazioniZooService.LeggiOperazioniAgendaZooAsync(parameterInput.Piva, parameterInput.Sa_Cod, parameterInput.Sta_Num, parameterInput.ValiditaInizio, parameterInput.ValiditaFine, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(operazioni, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetOperazioniAgendaNew))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOperazioniAgendaNew([FromBody] OperazioniAgendaInput parameterInput)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {

                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();


                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var operazioni = await _operazioniZooService.LeggiOperazioniAgendaZooNewAsync(parameterInput.Piva, parameterInput.Sa_Cod, parameterInput.Sta_Num, parameterInput.ValiditaInizio, parameterInput.ValiditaFine, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(operazioni, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        
        [HttpPost]
        [Route(nameof(GetGiacenzeZoo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetGiacenzeZoo(LeggiGiacenzeZooDto parameterInput)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var giacenze = await _operazioniZooService.LeggiGiacenzeZooAsync(
                    parameterInput,
                    objParametriDouble.ObjParametriServer,
                    objParametriDouble.ObjParametriUtenti
                );

                giacenze.DefaultView.Sort = "chiave asc";
                giacenze = giacenze.DefaultView.ToTable();

                resp.RispostaStringa = JsonConvert.SerializeObject(giacenze, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetTrattamentiZoo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetTrattamentiZoo(GetTrattamentiZooDto parameterInput)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {

                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var dt = await _operazioniZooService.LeggiTrattamentiZooAsync(
                    parameterInput,
                    objParametriDouble.ObjParametriUtenti,
                    objParametriDouble.ObjParametriServer
                );

                resp.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetSomministrazioneProdotti))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSomministrazioneProdotti(GetSomministrazioneProdottiDto parameterInput)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {

                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var prodotti = await _trattamentoZooService.LeggiSomministrazioneProdottiAsync(parameterInput.sommNumero,
                    objParametriServer
                );

                resp.RispostaStringa = JsonConvert.SerializeObject(prodotti, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetSenzaTrattamentiZoo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSenzaTrattamentiZoo(GetSenzaTrattamentiZooDto parameterInput)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var giacenze = await _operazioniZooService.LeggiCapiSenzaTrattamentiAsync(
                    parameterInput,
                    objParametriDouble.ObjParametriUtenti,
                    objParametriDouble.ObjParametriServer
                );

                resp.RispostaStringa = JsonConvert.SerializeObject(giacenze, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetStazionamentoZoo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetStazionamentoZoo(GetStazionamentoZooDto parameterInput)
        {
            RispostaStandard resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);

                var giacenze = await _operazioniZooService.LeggiStazionamentoZooAsync(
                    parameterInput,
                    objParametriDouble.ObjParametriUtenti,
                    objParametriDouble.ObjParametriServer
                );

                resp.RispostaStringa = JsonConvert.SerializeObject(giacenze, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetGiacenzeZooFirstSomm))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetGiacenzeZooFirstSomm(LeggiGiacenzeZooFirstSommDto dto)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var giacenze = await _operazioniZooService.LeggiGiacenzeZooFirstSommAsync(
                    dto,
                    objParametriServer,
                    UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings)
                );

                resp.RispostaStringa = JsonConvert.SerializeObject(giacenze, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(ScriviFirstSomministrazioniDaProtocollo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> ScriviFirstSomministrazioniDaProtocollo(int Id_Protocollo, List<Attivita> Somministrazioni)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var r = await _trattamentoZooService.CreaTrattamentoDaProtocollo(Id_Protocollo, Somministrazioni, objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(r, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(GetAttivitaTrattamentoZooFromAgenda))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetAttivitaTrattamentoZooFromAgenda(string Piva, int Id_Agenda)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                AgronicaCoreParametriServer objParametriServer = ExtractObjParametriServerAndSetCulture(auth); ;
                Attivita? attivita = await _trattamentoZooService.GetAttivitaFromAgenda(Piva, Id_Agenda, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(attivita, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route("LeggiGiacenzaFarmaci")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> LeggiGiacenzaFarmaci([FromBody] InData.Zoo.LeggiGiacenzaFarmaci leggiFarmaci)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriTriple = ExtractObjParametriTripleAndSetCulture(auth);
                List<DettaglioRegistroSomministrazioni> giacenzeList = await _magazzinoServiceBIZ.Leggi_Giacenze_Farmaci(leggiFarmaci,
                    AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna,
                    AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire,
                    true,
                    leggiFarmaci.codiceAIC.ToList(),
                    objParametriTriple.ObjParametriSuperServer,
                    objParametriTriple.ObjParametriServer,
                    objParametriTriple.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(giacenzeList, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(ConfermaSomministrazioneFutura))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> ConfermaSomministrazioneFutura(Attivita Somministrazione)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var r = await _trattamentoZooService.ScriviModificaSomministrazione(Somministrazione, objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(r, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(ModificaSomministrazione))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> ModificaSomministrazione(Attivita Somministrazione)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var r = await _trattamentoZooService.ScriviModificaSomministrazione(Somministrazione, objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(r, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(EliminaSomministrazione))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> EliminaSomministrazione(DeleteSomministrazione deleteSomministrazione)
        {
            RispostaStandard<bool> resp = new();

            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                AgronicaCoreParametriServer objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var r = await _trattamentoZooService.EliminaSomministrazione(deleteSomministrazione, objParametriServer);

                resp.RispostaStringa = r;
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(ScriviSomministrazioniDaPrescrizione))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> ScriviSomministrazioniDaPrescrizione(int Id_Prescrizione, List<Attivita> Somministrazioni)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objP = ExtractObjParametriDoubleAndSetCulture(auth);
                var r = await _trattamentoZooService.CreaTrattamentoDaPrescrizione(Id_Prescrizione, Somministrazioni, objP.ObjParametriServer, objP.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(r, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        [HttpPost]
        [Route(nameof(ProgramFirstSomministrazioneDaProtocollo))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> ProgramFirstSomministrazioneDaProtocollo(int Id_Protocollo, List<Attivita> Somministrazioni)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
                var r = await _trattamentoZooService.ProgrammaTrattamentoDaProtocollo(Id_Protocollo, Somministrazioni, objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti);

                resp.RispostaStringa = JsonConvert.SerializeObject(r, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (GiasException ex)
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
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
