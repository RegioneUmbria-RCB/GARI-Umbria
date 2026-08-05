using AgronicaCoreDTOStd.InData.Menu;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.Menu;
using AgronicaCoreModelsSTD.profilazione;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{

    public class MenuController : BaseController
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKeyPrefix = "MenuController_";
        private readonly MemoryCacheEntryOptions _defaultCacheEntryOptions;

        public MenuController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
            _cache = cache;

            _defaultCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(8))
                .SetSlidingExpiration(TimeSpan.FromHours(1));
        }
        #region "GET"

        [HttpGet]
        [Route("ottieniIconaDaTesto")]
        [ProducesResponseType(typeof(RispostaStandard<BreadcrumbsInfo>), StatusCodes.Status200OK)]
        public ObjectResult ottieniIconaDaTesto(string testo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            if (testo is null) return StatusCode(StatusCodes.Status400BadRequest, null);

            RispostaStandard<BreadcrumbsInfo> result = new RispostaStandard<BreadcrumbsInfo>();
            string cacheKeyFunction = "ottieniIconaDaTesto_";
            string key = _cacheKeyPrefix + cacheKeyFunction + testo.Trim();

            try
            {
                var request = getRequest(testo);


                //if (_cache.Get(key) is not RispostaStandard<string> cachedResponse)
                //{
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ottieniIconaDaTesto(request);
                //    if (result.RispostaOK)
                //        _cache.Set(key, result, _defaultCacheEntryOptions);
                //}
                //else
                //{
                //    result = cachedResponse;
                //}

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
        [Route("AlberoMenu")]
        [ProducesResponseType(typeof(RispostaStandard<AlberoMenu>), StatusCodes.Status200OK)]
        public ObjectResult GetAlberoMenu([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AlberoMenu> response = new RispostaStandard<AlberoMenu>();
            string cacheKeyFunction = "GetAlberoMenu";
            string key = _cacheKeyPrefix + cacheKeyFunction + objP_utenti;

            try
            {
                var request = getRequest((object)new { });


                //if ((Request.Headers.ContainsKey("cache-control") && (Request.Headers["cache-control"] == "no-cache")) ||
                //_cache.Get(key) is not RispostaStandard<AlberoMenu> cachedResponse)
                //{
                response = new CoreWSController(hc, coreWSBaseURL, bearerToken,  Request).LeggiAlberoMenu(request);
                //if (response.RispostaOK)
                //    _cache.Set(key, response, _defaultCacheEntryOptions);
                //}
                //else
                //{
                //    response = cachedResponse;
                //}

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
        [Route("CheckList")]
        [ProducesResponseType(typeof(RispostaStandard<AlberoMenu>), StatusCodes.Status200OK)]
        public ObjectResult GetChecklist([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AlberoMenu> response = new RispostaStandard<AlberoMenu>();
            string cacheKeyFunction = "GetChecklist";
            string key = _cacheKeyPrefix + cacheKeyFunction + objP_utenti;

            try
            {
                var request = getRequest((object)new { });
                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAlberoMenu_APP(request);
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
        [Route("ottieniInformazioniAssistenza")]
        [ProducesResponseType(typeof(RispostaStandard<InformazioniAssistenza>), StatusCodes.Status200OK)]
        public ObjectResult ottieniInformazioniAssistenza([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var cacheKeyFunction = "ottieniInformazioniAssistenza_";
            string key = _cacheKeyPrefix + cacheKeyFunction + objP_utenti;
            RispostaStandard<InformazioniAssistenza> response = new RispostaStandard<InformazioniAssistenza>();

            try
            {
                var request = getRequest((object)new { });


                //if ((Request.Headers.ContainsKey("cache-control") && (Request.Headers["cache-control"] == "no-cache")) ||
                //    _cache.Get(key) is not RispostaStandard<InformazioniAssistenza> cachedResponse)
                //{
                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ottieniInformazioniAssistenza(request);
                //    if (response.RispostaOK)
                //        _cache.Set(key, response, _defaultCacheEntryOptions);
                //}
                //else
                //{
                //    response = cachedResponse;
                //}

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
        [Route("informazioniUtente")]
        [ProducesResponseType(typeof(RispostaStandard<Utente>), StatusCodes.Status200OK)]
        public ObjectResult InformazioniUtente([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var cacheKeyFunction = "informazioniUtente_";
            string key = _cacheKeyPrefix + cacheKeyFunction + objP_utenti;
            RispostaStandard<Utente> response = new RispostaStandard<Utente>();

            try
            {
                var request = getRequest((object)new { });


                //if ((Request.Headers.ContainsKey("cache-control") && (Request.Headers["cache-control"] == "no-cache")) ||
                //    _cache.Get(key) is not RispostaStandard<Utente> cachedResponse)
                //{
                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ottieniInformazioneUtente(request);
                //    if (response.RispostaOK)
                //        _cache.Set(key, response, _defaultCacheEntryOptions);
                //}
                //else
                //{
                //    response = cachedResponse;
                //}

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
        [Route("ottieniUltimeAziendeSelezionate")]
        [ProducesResponseType(typeof(RispostaStandard<List<Impresa>>), StatusCodes.Status200OK)]
        public ObjectResult OttieniUltimeAziendeSelezionate([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            RispostaStandard<List<Impresa>> result = new RispostaStandard<List<Impresa>>();

            try
            {
                var request = getRequest((object)new { });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ottieniUltimeAziendeSelezionate(request);
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
        [Route("ottieniUltimeAziendeSelezionate2")]
        [ProducesResponseType(typeof(RispostaStandard<List<Impresa>>), StatusCodes.Status200OK)]
        public ObjectResult OttieniUltimeAziendeSelezionate2([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            RispostaStandard<List<Impresa>> result = new RispostaStandard<List<Impresa>>();

            try
            {
                var request = getRequest((object)new { });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ottieniUltimeAziendeSelezionate2(request);
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
        [Route("InformazioniBreadcrumbs")]
        [ProducesResponseType(typeof(RispostaStandard<BreadcrumbsInfo>), StatusCodes.Status200OK)]
        public ObjectResult ottieniBreadcrumbs(int Id_Sezione, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            if (Id_Sezione == -1) return StatusCode(StatusCodes.Status400BadRequest, null);

            RispostaStandard<BreadcrumbsInfo> result = new RispostaStandard<BreadcrumbsInfo>();

            try
            {
                var request = getRequest(Id_Sezione);

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).OttieniBreadcrumbs(request);
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
        [Route("ricercaAzienda")]
        [ProducesResponseType(typeof(RispostaStandard<List<Impresa>>), StatusCodes.Status200OK)]
        public ObjectResult RicercaAzienda(string stringaRicerca, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            RispostaStandard<List<Impresa>> result = new RispostaStandard<List<Impresa>>();

            try
            {
                var request = getRequest(stringaRicerca);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ricercaAzienda(request);
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
        [Route("VersioneHeader")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult GetVersioneHeader([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard response = new RispostaStandard();
            try
            {
                var request = getRequest((object)new { });
                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiVersioneHeader(request);
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

        #endregion

        #region "POST"
        [HttpPost]
        [Route("aggiornaAttivitaNavigazioneAziende")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult AggiornaAttivitaNavigazioneAziende([FromBody] attivitaNavigazioneAziende_in attivitaNavigazioneAziende, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            RispostaStandard<string> result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(attivitaNavigazioneAziende);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).aggiornaAttivitaMenuUtente(request);
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
        [Route("aggiornaPreferiti")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult AggiornaPreferiti([FromBody] preferiti_in preferiti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            RispostaStandard<string> result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(preferiti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaPreferiti(request);
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
    #endregion
}
