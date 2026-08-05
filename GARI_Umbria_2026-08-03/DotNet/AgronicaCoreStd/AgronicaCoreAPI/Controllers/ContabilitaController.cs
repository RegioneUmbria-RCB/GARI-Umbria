using AgronicaCoreAPI.adapters;
using AgronicaCoreAPI.models;
using AgronicaCoreAPI.models.entities;
using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using AgronicaDataProvider6.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AgronicaCoreAPI.Controllers
{
    public class ContabilitaController : BaseController
    {
        public ContabilitaController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpGet]
        [Route("Attivita")]
        public ObjectResult GetAttivita(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Attivita request = new CoreWS_Attivita(piva, objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAttivita(request);
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
        [Route("AttivitaOperazioni")]
        public ObjectResult GetAttivitaOperazioni(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_AttivitaXOperazioni request = new CoreWS_AttivitaXOperazioni(piva, objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAttivitaOperazioni(request);
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
        [Route("AttivitaCentriAziendali")]
        public ObjectResult GetAttivitaCentriAziendali(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_AttivitaXCentri_Aziendali request = new CoreWS_AttivitaXCentri_Aziendali(piva, objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAttivitaCentriAziendali(request);
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
        [Route("Progetti")]
        public ObjectResult GetProgetti(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Imputazione_Fasi request = new CoreWS_Imputazione_Fasi(piva, objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProgetti(request);
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
        [Route("ProdottiGiacenze")]
        public ObjectResult GetProdottiGiacenze(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Prodotti_Giacenze request = new CoreWS_Prodotti_Giacenze(objP_super_server, objP_server, objP_utenti, piva, data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProdottiGiacenze(request);
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
        [Route("MovimentiMagazzini")]
        public ObjectResult GetMovimentiMagazzino(string parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            MovimentiMagazziniRequest r = JsonConvert.DeserializeObject<MovimentiMagazziniRequest>(parametri);
            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Movimenti_Magazzini request = new CoreWS_Movimenti_Magazzini(objP_super_server, objP_server, objP_utenti, r.piva, r.sacod, r.fabbricato, r.categoria, r.prodotto, r.inizio, r.fine);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMovimentiMagazzini(request);
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
        [Route("MovimentiMagazzini")]
        public ObjectResult PostMovimentiMagazzino([FromBody] List<MovimentoDiMagazzino> movimenti, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(movimenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviMovimentiMagazzini(request);
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

    }
}
