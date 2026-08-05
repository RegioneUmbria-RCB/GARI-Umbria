using AgronicaCoreAPI.adapters;
using AgronicaCoreAPI.models.entities;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using static AgronicaCoreDataProviderSTD.TipiEnumerativi;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;

namespace AgronicaCoreAPI.Controllers
{
    public class AnagraficaController : BaseController
    {
        public AnagraficaController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config,httpClientFactory, securitySettings, localizer) { }

        [HttpGet]
        [Route("Imprese")]
        public ObjectResult GetImprese([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImprese(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("CentriAziendali")]
        public ObjectResult GetCentriAziendali(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Centri_Aziendali request = new CoreWS_Centri_Aziendali(objP_super_server, objP_server, objP_utenti, piva, data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCentriAziendali(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("Impianti")]
        public ObjectResult GetImpianti(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Impianti request = new CoreWS_Impianti(objP_super_server, objP_server, objP_utenti, piva, data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpianti(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("Contatti")]
        public ObjectResult GetContatti(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Contatti request = new CoreWS_Contatti(objP_super_server, objP_server, objP_utenti, piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiContatti(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("Macchine")]
        public ObjectResult GetMacchine(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Parco_Macchine request = new CoreWS_Parco_Macchine(objP_super_server, objP_server, objP_utenti, piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMacchine(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("Magazzini")]
        public ObjectResult GetMagazzini(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Magazzini request = new CoreWS_Magazzini(objP_super_server, objP_server, objP_utenti, piva, data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMagazzini(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("Prodotti")]
        public ObjectResult GetProdotti(string piva, string data, int categoria, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                bool azienda = !string.IsNullOrEmpty(piva);
                string metaschema = (azienda && categoria == (int)enum_CategorieMagazzino.SEMENTI) ? "N" : "S";
                CoreWS_Prodotti request = new CoreWS_Prodotti(objP_super_server, objP_server, objP_utenti, azienda ? piva : "", data, categoria, metaschema, azienda, false);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProdotti(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("ProdottiGias")]
        public ObjectResult GetProdottiGias(string piva, int categoria, string filtro, string specie, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Prodotti_GIAS request = new CoreWS_Prodotti_GIAS(objP_super_server, objP_server, objP_utenti, piva, categoria, filtro, specie);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProdottiGias(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("RilevamentiMagazzini")]
        public ObjectResult GetRilevamentiMagazzini(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<RilevamentoDiMagazzinoEntity>>();

            try
            {
                var adapter = new ModelloAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                var r = ws.LeggiProdottiGiacenze(new CoreWS_Prodotti_Giacenze(objP_super_server, objP_server, objP_utenti, piva, data));

                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaOK = true;
                result.RispostaStringa = adapter.leggiRilevamentiMagazzini(r.RispostaStringa);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("ProdottiOnline")]
        public ObjectResult GetProdottiOnline(string piva, int categoria, string filtro, string specie, string codici, string stato,int lav_cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<ProdottoEntity>>();

            try
            {
                var adapter = new ModelloAdapter();
                var request = new CoreWS_ProdottiSenzaGiacenza(objP_super_server, objP_server, objP_utenti, piva, categoria, filtro, specie, codici, stato,lav_cod);
                var r = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProdottiSenzaGiacenza(request);                
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaOK = true;
                result.RispostaStringa = adapter.leggiProdottiOnline(r.RispostaStringa, "");

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("ParcoMacchine")]
        public ObjectResult GetParcoMacchine(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<ParcoMacchineEntity>>();

            try
            {
                var adapter = new ModelloAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                var r = ws.LeggiMacchine(new CoreWS_Parco_Macchine(objP_super_server, objP_server, objP_utenti, piva));
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaOK = true;
                result.RispostaStringa = adapter.leggiParcoMacchine(r.RispostaStringa);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        /// <summary>verifica associazione macchina con BTM</summary>
        /// "0" = Macchina non associata a BTM e BTM non associato ad altre macchine
        /// "1" = Macchina già associata a quel BTM (codice, VIN e BTM coincidono)
        /// "2" = Macchina associata ad altro BTM (codice e VIN coincidono ma BTM diverso)
        /// "3" = BTM associato ad altra macchina (BTM presente su macchine con codice e VIN diverso)
        /// "9" = Macchina non trovata o VIN non corrispondentente
        [HttpGet]
        [Route("ParcoMacchineBTM")]
        public ObjectResult GetParcoMacchineBTM(string codice, string VIN, string btmserial, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard();

            try
            {
                var adapter = new ModelloAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                result = ws.VerificaAssociazioneBTM(new CoreWS_Parco_Macchine_BTM(objP_super_server, objP_server, objP_utenti, int.Parse(codice), VIN, btmserial));
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("Macchina")]
        public ObjectResult PostMacchina([FromBody] ParcoMacchine macchina, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = new CoreWS_Scrivi_Macchina(objP_super_server, objP_server, objP_utenti, macchina, enum_TipoOperazioneDB.Scrittura);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviMacchina(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("Manutenzioni")]
        public ObjectResult PostManutenzioni([FromBody] List<Manutenzione> manutenzioni, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(manutenzioni);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviManutenzioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("Insetti")]
        [ProducesResponseType(typeof(RispostaStandard<List<DettaglioTrattamento>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiInsettiUtiliQdC([FromBody] InData.Anagrafica.LeggiProdotti request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<DettaglioTrattamento>> result = new RispostaStandard<List<DettaglioTrattamento>>();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiInsettiUtiliQdC(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("LeggiGiacenzaFarmaci")]
        public ObjectResult LeggiGiacenzaFarmaci([FromBody] InData.Zoo.LeggiGiacenzaFarmaci leggiFarmaci, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            
            RispostaStandard<List<DettaglioRegistroSomministrazioni>> result = new RispostaStandard<List<DettaglioRegistroSomministrazioni>>();

            try
            {
                var request = getRequest(leggiFarmaci);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGiacenzaFarmaci(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("LeggiProdottiDaTrattareQdC")]
        [ProducesResponseType(typeof(RispostaStandard<List<MovimentoDiMagazzino>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiProdottiDaTrattareQdC([FromBody] InData.Anagrafica.LeggiProdotti request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<MovimentoDiMagazzino>> result = new RispostaStandard<List<MovimentoDiMagazzino>>();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProdottiDaTrattareQdC(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("LeggiSpecieVegetaliQdC")]
        [ProducesResponseType(typeof(RispostaStandard<List<UtilizzoTerreno>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiSpecieVegetaliQdC([FromBody] InData.Agenda.LeggiSpecieQdC request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<UtilizzoTerreno>> result = new RispostaStandard<List<UtilizzoTerreno>>();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSpecieVegetaliQdC(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("LeggiConversioni_UdmAlt")]
        [ProducesResponseType(typeof(RispostaStandard<List<UnitaDiMisura_Alternativa>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiConversioni_UdmAlt([FromBody] int udmCodFrom, DateTime validitaInizio, DateTime validitaFine, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<UnitaDiMisura_Alternativa>> result = new RispostaStandard<List<UnitaDiMisura_Alternativa>>();

            try
            {
                var request = new CoreWS_UnitaMisuraAlternativeLeggi(objP_super_server, objP_server, objP_utenti, udmCodFrom, validitaInizio, validitaFine);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiConversioniAlt(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("LeggiTassoConversione")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiTassoConversione([FromBody] int udmFrom, int udmAlt, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = new CoreWS_ConversioneAltUdmLeggi.LeggiTassoConversione(objP_super_server, objP_server, objP_utenti, udmFrom, udmAlt);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetTassoConversioneAlt(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("GetSuperificieEttari")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult GetSuperificieEttari([FromBody] double superficie, int udmAlt, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = new CoreWS_ConversioneAltUdmLeggi.GetSuperficieEttari(objP_super_server, objP_server, objP_utenti, superficie, udmAlt);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetSuperificieEttari(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }
    }
}
