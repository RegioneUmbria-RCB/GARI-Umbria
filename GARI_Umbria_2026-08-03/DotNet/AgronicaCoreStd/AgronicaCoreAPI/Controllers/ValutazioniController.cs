using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreUtilityStd;
using AgronicaCoreModelsSTD.valutazioni;
using AgronicaCoreDTOStd.InData.Valutazioni;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class ValutazioniController : BaseController
    {

        public ValutazioniController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("LeggiValutazioneTestata")]
        [ProducesResponseType(typeof(RispostaStandard<List<Valutazione_Testata>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazioneTestata([FromBody] LeggiTestata InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Valutazione_Testata>> response = new RispostaStandard<List<Valutazione_Testata>>();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiTestata> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiTestata>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniTestata(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("LeggiValutazioneTestataGriglia")]
        //[ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazioneTestataGriglia([FromBody] LeggiTestata InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard response = new RispostaStandard();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiTestata> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiTestata>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniTestataGriglia(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpPost]
        [Route("LeggiValutazioneDettaglioEcoGriglia")]
        //[ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazioneDettaglioEcoGriglia([FromBody] LeggiDettaglio InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard response = new RispostaStandard();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiDettaglio> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiDettaglio>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniDettaglioEcoGriglia(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpPost]
        [Route("LeggiValutazioneDettaglioPatGriglia")]
        //[ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazioneDettaglioPatGriglia([FromBody] LeggiDettaglio InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard response = new RispostaStandard();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiDettaglio> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiDettaglio>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniDettaglioPatGriglia(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }




        [HttpPost]
        [Route("LeggiValutazioneDettaglioSpecificoGriglia")]
        //[ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazioneDettaglioSpecificoGriglia([FromBody] LeggiDettaglio InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard response = new RispostaStandard();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiDettaglio> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiDettaglio>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniDettaglioSpecificoGriglia(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }





        [HttpPost]
        [Route("ScriviTestata")]
        public ObjectResult ScriviValutazioneTestata([FromBody] Valutazione_Testata InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Testata> response = new RispostaStandard<Valutazione_Testata>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Valutazione_Testata> requestData = new CoreWS_Generic<Valutazione_Testata>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Valutazione_Testata>> request = new CoreWSRequest<CoreWS_Generic<Valutazione_Testata>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviValutazioneTestata(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpPost]
        [Route("ScriviDettaglio")]
        public ObjectResult ScriviValutazioneDettaglio([FromBody] Valutazione_Dettaglio InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Dettaglio> response = new RispostaStandard<Valutazione_Dettaglio>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Valutazione_Dettaglio> requestData = new CoreWS_Generic<Valutazione_Dettaglio>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Valutazione_Dettaglio>> request = new CoreWSRequest<CoreWS_Generic<Valutazione_Dettaglio>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviValutazioneDettaglio(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpPost]
        [Route("ScriviDettaglioSpecifico")]
        public ObjectResult ScriviValutazioneDettaglioSpecifico([FromBody] Valutazione_Dettaglio_Specifico InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Dettaglio_Specifico> response = new RispostaStandard<Valutazione_Dettaglio_Specifico>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Valutazione_Dettaglio_Specifico> requestData = new CoreWS_Generic<Valutazione_Dettaglio_Specifico>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Valutazione_Dettaglio_Specifico>> request = new CoreWSRequest<CoreWS_Generic<Valutazione_Dettaglio_Specifico>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviValutazioneDettaglioSpecifico(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }





        [HttpPost]
        [Route("ScriviDettaglioGriglia")]
        public ObjectResult ScriviValutazioneDettaglioGriglia([FromBody] LeggiGriglia InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Scrivi_Griglia> response = new RispostaStandard<Valutazione_Scrivi_Griglia>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiGriglia> requestData = new CoreWS_Generic<LeggiGriglia>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiGriglia>> request = new CoreWSRequest<CoreWS_Generic<LeggiGriglia>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviValutazioneDettaglioGriglia(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpPost]
        [Route("ScriviDettaglioSpecificoGriglia")]
        public ObjectResult ScriviValutazioneDettaglioSpecificoGriglia([FromBody] LeggiGriglia InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Scrivi_Griglia> response = new RispostaStandard<Valutazione_Scrivi_Griglia>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiGriglia> requestData = new CoreWS_Generic<LeggiGriglia>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiGriglia>> request = new CoreWSRequest<CoreWS_Generic<LeggiGriglia>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviValutazioneDettaglioSpecificoGriglia(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }





        [HttpPost]
        [Route("AggiornaDettaglio")]
        public ObjectResult AggiornaValutazioneDettaglio([FromBody] LeggiTestata InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Testata> response = new RispostaStandard<Valutazione_Testata>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiTestata> requestData = new CoreWS_Generic<LeggiTestata>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiTestata>> request = new CoreWSRequest<CoreWS_Generic<LeggiTestata>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaValutazioneDettaglio(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }


        [HttpPost]
        [Route("AggiornaDettaglioSpecifico")]
        public ObjectResult AggiornaValutazioneDettaglioSpecifico([FromBody] LeggiTestata InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Testata> response = new RispostaStandard<Valutazione_Testata>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiTestata> requestData = new CoreWS_Generic<LeggiTestata>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiTestata>> request = new CoreWSRequest<CoreWS_Generic<LeggiTestata>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaValutazioneDettaglioSpecifico(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("AggiornaDettaglioSingolo")]
        public ObjectResult AggiornaValutazioneDettaglioSingolo([FromBody] LeggiDettaglio InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Testata> response = new RispostaStandard<Valutazione_Testata>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiDettaglio> requestData = new CoreWS_Generic<LeggiDettaglio>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiDettaglio>> request = new CoreWSRequest<CoreWS_Generic<LeggiDettaglio>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaValutazioneDettaglioSingolo(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }





        [HttpPost]
        [Route("AggiornaDettaglioArete")]
        public ObjectResult AggiornaValutazioneDettaglioArete([FromBody] LeggiDettaglioSpecificoArete InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Testata> response = new RispostaStandard<Valutazione_Testata>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiDettaglioSpecificoArete> requestData = new CoreWS_Generic<LeggiDettaglioSpecificoArete>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiDettaglioSpecificoArete>> request = new CoreWSRequest<CoreWS_Generic<LeggiDettaglioSpecificoArete>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaValutazioneDettaglioArete(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }




        [HttpGet]
        [Route("TipoAnno")]
        [ProducesResponseType(typeof(RispostaStandard<List<ValutazioneTipoAnno>>), StatusCodes.Status200OK)]
        public ObjectResult GetTipoAnno([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<ValutazioneTipoAnno>> response = new RispostaStandard<List<ValutazioneTipoAnno>>();

            try
            {
                var request = getRequest("");

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoTipoAnnoValutazione(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("LeggiValutazionePianoConti")]
        [ProducesResponseType(typeof(RispostaStandard<List<Valutazione_Piano_Conti>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazionePianoConti([FromBody] LeggiPianoConti InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Valutazione_Piano_Conti>> response = new RispostaStandard<List<Valutazione_Piano_Conti>>();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiPianoConti> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiPianoConti>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniPianoConti(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("LeggiValutazionePianoContiGriglia")]
        //[ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazionePianoContiGriglia([FromBody] LeggiPianoConti InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard response = new RispostaStandard();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiPianoConti> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiPianoConti>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniPianoContiGriglia(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("ScriviPianoConti")]
        public ObjectResult ScriviValutazionePianoConti([FromBody] Valutazione_Piano_Conti InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Valutazione_Piano_Conti> response = new RispostaStandard<Valutazione_Piano_Conti>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Valutazione_Piano_Conti> requestData = new CoreWS_Generic<Valutazione_Piano_Conti>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Valutazione_Piano_Conti>> request = new CoreWSRequest<CoreWS_Generic<Valutazione_Piano_Conti>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviValutazionePianoConti(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpPost]
        [Route("LeggiValutazionePianoContixContiTree")]
        [ProducesResponseType(typeof(RispostaStandard<List<TreeValutazionePianoContixConti>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazionePianoContixConti([FromBody] LeggiPianoContixConti InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<TreeValutazionePianoContixConti>> response = new RispostaStandard<List<TreeValutazionePianoContixConti>>();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiPianoContixConti> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiPianoContixConti>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniPianoContixConti(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("ScriviPianoContixContiTree")]
        public ObjectResult ScriviValutazionePianoContixConti_Tree([FromBody] TreeValutazionePianoContixConti InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<TreeValutazionePianoContixConti> response = new RispostaStandard<TreeValutazionePianoContixConti>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<TreeValutazionePianoContixConti> requestData = new CoreWS_Generic<TreeValutazionePianoContixConti>(objP, InData);
                CoreWSRequest<CoreWS_Generic<TreeValutazionePianoContixConti>> request = new CoreWSRequest<CoreWS_Generic<TreeValutazionePianoContixConti>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviValutazionePianoContixConti(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }





        [HttpPost]
        [Route("LeggiValutazioneConto")]
        [ProducesResponseType(typeof(RispostaStandard<List<Valutazione_Conto>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiValutazioneConto([FromBody] LeggiConto InData, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Valutazione_Conto>> response = new RispostaStandard<List<Valutazione_Conto>>();

            try
            {

                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiConto> requestData = new(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiConto>> request = new(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaValutazioniConto(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("ReportExcel")]
        public ObjectResult ReportExcelValutazione([FromBody] LeggiTestata InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ValutazioneExcel> response = new RispostaStandard<ValutazioneExcel>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiTestata> requestData = new CoreWS_Generic<LeggiTestata>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiTestata>> request = new CoreWSRequest<CoreWS_Generic<LeggiTestata>>(requestData);

                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ReportExcelValutazione(request);
                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

    }
}

