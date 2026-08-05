using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using AgronicaCoreDTOStd.InData.ReadRequisitiStabilimento;
using AgronicaCoreDTOStd.InData.SaveRequisitiStabilimento;
using AgronicaCoreDTOStd.InData.ReadDettaglioAziendale;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class RequisitiStabilimentoController : BaseController
    {
        public RequisitiStabilimentoController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }
        
        [HttpPost]
        [Route(nameof(readRequisitiStabilimento))]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult readRequisitiStabilimento([FromBody] ReadRequisitiStabilimento readParams, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(readParams);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readRequisitiStabilimento(request);
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
        [Route(nameof(saveRequisitiStabilimento))]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult saveRequisitiStabilimento([FromBody] SaveRequisitiStabilimento saveParams, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(saveParams);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).saveRequisitiStabilimento(request);
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
        [Route(nameof(readPianoColturaleRequisitiStabilimento))]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult readPianoColturaleRequisitiStabilimento([FromBody] ReadRequisitiStabilimento leggi_ParticelleCampo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggi_ParticelleCampo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readPianoColturaleRequisitiStabilimento(request);
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
        [Route(nameof(readContracts))]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult readContracts([FromBody] ReadRequisitiStabilimento leggi_ParticelleCampo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggi_ParticelleCampo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readContracts(request);
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
        [Route(nameof(readSurfaceForProducts))]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult readSurfaceForProducts([FromBody] ReadRequisitiStabilimento leggi_ParticelleCampo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggi_ParticelleCampo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readSurfaceForProducts(request);
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
        [Route(nameof(readDettaglioAziendale))]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult readDettaglioAziendale([FromBody] ReadDettaglioAziendale readDettaglioAziendale, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(readDettaglioAziendale);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readDettaglioAziendale(request);
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
