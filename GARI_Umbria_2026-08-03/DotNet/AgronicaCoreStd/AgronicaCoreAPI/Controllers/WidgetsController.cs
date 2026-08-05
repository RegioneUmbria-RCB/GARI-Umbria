using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using AgronicaCoreDTOStd.InData.Widgets;
using AgronicaCoreModelsSTD.Widgets;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.exceptions;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.Audit;

namespace AgronicaCoreAPI.Controllers
{
    public class WidgetsController : BaseController
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKeyPrefix = "WidgetController_";
        private readonly MemoryCacheEntryOptions _defaultCacheEntryOptions;

        public WidgetsController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        { 
            _cache = cache;

            _defaultCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(8))
                .SetSlidingExpiration(TimeSpan.FromHours(1)); ;
        }

        [HttpPost]
        [Route("Configurazione/Reset")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult ConfigurazioneReset(string userName, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(userName);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).UserWidgetsReset(request);
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
        [Route("Configurazione/Aggiorna")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult ConfigurazioneAggiorna([FromBody] Widget_Configuration configurazione, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(configurazione);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaConfigurazioneWidget(request);
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
        [Route("Configurazione/ElencoWidgets")]
        [ProducesResponseType(typeof(RispostaStandard<Widget_Complete_Configuration>), StatusCodes.Status200OK)]
        public ObjectResult ElencoWidgetsXConfigurazione(string userName, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<Widget_Complete_Configuration>();

            try
            {
                var widgetIn = new Widgets_In
                {
                    Abilitato = true,
                    PresetIniziale = true,
                    UserName = userName
                };
                var request = getRequest(widgetIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ElencoWidgetsXConfigurazione(request);
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
        [Route("ElencoWidgets")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget>>), StatusCodes.Status200OK)]
        public ObjectResult ElencoWidgets([FromBody] Widgets_In parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            var result = new RispostaStandard<List<Widget>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ElencoWidgets(request);
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
        [Route("AggiornaWidgetUtente")]
        [ProducesResponseType(typeof(RispostaStandard<List<string>>), StatusCodes.Status200OK)]
        public ObjectResult AggiornaWidgetUtente([FromBody] Aggiorna_Widgets_In parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            var result = new RispostaStandard<List<string>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaWidgetsUtente(request);
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
        [Route("ScriviAggiornaWidgetUtente")]
        [ProducesResponseType(typeof(RispostaStandard<List<string>>), StatusCodes.Status200OK)]
        public ObjectResult ScriviAggiornaWidgetUtente([FromBody] Aggiorna_Widgets_In parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);


            var result = new RispostaStandard<List<string>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviAggiornaWidgetsUtente(request);
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
        [Route("LeggiUltimiMovimentiMagazzino")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_MovimentoMagazzino>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiUltimiMovimentiMagazzino([FromBody] Widget_MovimentiMagazziono_In parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_MovimentoMagazzino>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Widget_LeggiUltimiMovimentiMagazzino(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (CoreAPIException ex)
            {
                result.RispostaOK = false;
                result.Errore = JsonConvert.SerializeObject(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("LeggiUltimiRilievi")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_Operazione>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiUltimiRilievi([FromBody] Widget_Operazioni_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_Operazione>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUltimiRilievi(request);
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
        [Route("LeggiUltimeVisite")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_Operazione>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiUltimeVisite([FromBody] Widget_Operazioni_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_Operazione>>();

            try
            {
                var request = getRequest((parametri));
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUltimeVisite(request);
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
        [Route("LeggiUltimeAttivita")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_Operazione>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiUltimeAttivita([FromBody] Widget_Operazioni_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_Operazione>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUltimeAttivita(request);
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
        [Route("LeggiColture")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_Coltura>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiColture([FromBody] Widget_Culture_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_Coltura>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiColture(request);
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
        [Route("ConfigurazioneAgroMeteo")]
        [ProducesResponseType(typeof(RispostaStandard<Widget_AgroMeteo>), StatusCodes.Status200OK)]
        public ObjectResult ConfigurazioneAgroMeteo(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<Widget_AgroMeteo>();
            
            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ConfigurazioneAgroMeteo(request);
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
        [Route("WeatherLatLng")]
        [ProducesResponseType(typeof(RispostaStandard<Widget_MeteoImpresaLatLng>), StatusCodes.Status200OK)]
        public ObjectResult WeatherLatLng(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<Widget_MeteoImpresaLatLng>();
            
            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).WeatherLatLng(request);
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
        [Route("LeggiProduzioneColture")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_ProduzioneColtura>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiProduzioneColture([FromBody] Widget_Culture_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_ProduzioneColtura>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProduzioneColture(request);
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
        [Route("LinkGestioneCompleta")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]

        public ObjectResult LinkGestioneCompleta([FromQuery] Widget_LinkGestione_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Widget_LinkGestioneCompleta(request);
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
        [Route("ModelliPrevisionali_ElaboraIndicatori")]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult ModelliPrevisionali_ElaboraIndicatori([FromBody] Widget_Modelli_Previsionali_Indicatori_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<object>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModelliPrevisionali_ElaboraIndicatori(request);
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
        [Route("ElaboraMonitoraggioSuolo")]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult ElaboraMonitoraggioSuolo(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<object>();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ElaboraMonitoraggioSuolo(request);
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
        [Route("DatiMeteo_ElaboraRiepilogo")]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult DatiMeteo_ElaboraRiepilogo(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<object>();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).DatiMeteo_ElaboraRiepilogo(request);
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
        [Route("LeggiUltimiAcquisti")]
        [ProducesResponseType(typeof(RispostaStandard<List<WidgetAcquisto>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiUltimiAcquisti([FromBody] Widget_Acquisti_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<WidgetAcquisto>>();

            try
            {
                var request = getRequest((parametri));
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUltimiAcquisti(request);
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
        [Route("Leggi_GHGColture")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_GHGColture>>), StatusCodes.Status200OK)]
        public ObjectResult Leggi_GHGColture([FromBody] Widget_GHGColture_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_GHGColture>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_GHGColture(request);
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
        [Route("Leggi_StimeProduzioneColture")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_StimeProduzioneColture>>), StatusCodes.Status200OK)]
        public ObjectResult Leggi_StimeProduzioneColture([FromBody] Widget_StimeProduzioneColture_IN parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<Widget_StimeProduzioneColture>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_StimeProduzioneColture(request);
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
        [Route("readWidgetIndiciProduttivita")]
        [ProducesResponseType(typeof(RispostaStandard<WidgetIndiciProduttivitaGlobal>), StatusCodes.Status200OK)]
        public ObjectResult readWidgetIndiciProduttivita([FromBody] WidgetRequestIndiciProduttivita parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<WidgetIndiciProduttivitaGlobal>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readWidgetIndiciProduttivita(request);
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
        [Route("readAvailableYearsIndiciProduttivita")]
        [ProducesResponseType(typeof(RispostaStandard<List<int>>), StatusCodes.Status200OK)]
        public ObjectResult readAvailableYearsIndiciProduttivita([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<int>>();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readAvailableYearsIndiciProduttivita(request);
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
        [Route("readWidgetKPI")]
        [ProducesResponseType(typeof(RispostaStandard<List<WidgetKPI>>), StatusCodes.Status200OK)]
        public ObjectResult readWidgetKPI([FromBody] WidgetRequestKpi data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            var result = new RispostaStandard<List<WidgetKPI>>();

            try
            {
                var request = getRequest(data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readWidgetKPI(request);
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
        [Route("readStatistichePrevisioniAI")]
        [ProducesResponseType(typeof(RispostaStandard<List<Widget_PrevisioniAI>>), StatusCodes.Status200OK)]
        public ObjectResult readStatistichePrevisioniAI([FromBody] Widget_PrevisioniAI_IN data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            
            var result = new RispostaStandard<List<Widget_PrevisioniAI>>();

            try
            {
                var request = getRequest(data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readStatistichePrevisioniAI(request);
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
        [Route("completamentoCheckList")]
        [ProducesResponseType(typeof(RispostaStandard<List<AuditCompletamentoModel>>), StatusCodes.Status200OK)]
        public ObjectResult completamentoCheckList([FromBody] string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<AuditCompletamentoModel>>();

            try
            {
                var request = getRequest(data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetCompletamentoCheckList(request);
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
