using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.AuthDispatcher;
using AgronicaCoreModelsSTD.AuthDispatcher;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace AgronicaCoreAPI.Controllers
{
    public class AuthDispatcherController : BaseController
    {
        public AuthDispatcherController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
        }

        /// <summary>
        /// Endpoint che richiede un signed URL per la chiamata a funzioni Google Earth Engine
        /// </summary>
        /// <param name="richiesta">Parametri della richiesta URL.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Signed URL</returns>
        /// <remarks>
        /// 
        ///     POST /RichiediSignedURL
        /// 
        /// </remarks>
        /// <response code="200">Signed URL</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("RichiediSignedURL")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoUrlFirmati>), StatusCodes.Status200OK)]
        public ObjectResult RichiediSignedURL([FromBody] RichiestaSignedUrl_In richiesta, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoUrlFirmati> result = new();

            try
            {
                var request = getRequest(richiesta);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RichiediSignedURL(request);
                if (!result.RispostaOK)
                {
                    switch (result.Errore.Substring(0,4))
                    {
                        case "400_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status400BadRequest, result);
                        case "401_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status401Unauthorized, result);
                        case "500_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                    }
                }

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

        /// <summary>
        /// Endpoint che richiede un URL Abaco per la chiamata a funzioni Google Earth Engine
        /// </summary>
        /// <param name="richiesta">Parametri della richiesta URL.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Signed URL</returns>
        /// <remarks>
        /// 
        ///     POST /RichiediAbacoURL
        /// 
        /// </remarks>
        /// <response code="200">Abaco URL</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("RichiediAbacoURL")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoUrlFirmati>), StatusCodes.Status200OK)]
        public ObjectResult RichiediAbacoURL([FromBody] RichiestaAbacoUrl_In richiesta, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoUrlFirmati> result = new();

            try
            {
                var request = getRequest(richiesta);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RichiediAbacoURL(request);
                if (!result.RispostaOK)
                {
                    switch (result.Errore.Substring(0, 4))
                    {
                        case "400_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status400BadRequest, result);
                        case "401_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status401Unauthorized, result);
                        case "500_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                    }
                }

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
        
        /// <summary>
        /// Endpoint che richiede un URL con bearer per la chiamata a funzioni SAT
        /// </summary>
        /// <param name="richiesta">Parametri della richiesta URL.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Signed URL</returns>
        /// <remarks>
        /// 
        ///     POST /RichiediSatUrl
        /// 
        /// </remarks>
        /// <response code="200">SatURL</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("RichiediSatUrl")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoUrlFirmati>), StatusCodes.Status200OK)]
        public ObjectResult RichiediSatUrl([FromBody] RichiestaSatUrl_In richiesta, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoUrlFirmati> result = new();

            try
            {
                var request = getRequest(richiesta);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RichiediSatUrl(request);
                if (!result.RispostaOK)
                {
                    switch (result.Errore.Substring(0, 4))
                    {
                        case "400_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status400BadRequest, result);
                        case "401_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status401Unauthorized, result);
                        case "500_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                    }
                }

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

        /// <summary>
        /// Endpoint che esegue l'autenticazione a Google Earth Engine
        /// </summary>
        /// <returns>Autenticazione Google Earth Engine</returns>
        /// <remarks>
        /// 
        ///     POST /AutenticaGEE
        /// 
        /// </remarks>
        /// <response code="200">Token autenticazione GEE</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("AutenticaGEE")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult AutenticaGEE([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AutenticaGEE(request);
                if (!result.RispostaOK)
                {
                    switch (result.Errore.Substring(0, 4))
                    {
                        case "400_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status400BadRequest, result);
                        case "401_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status401Unauthorized, result);
                        case "500_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                    }
                }

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

        /// <summary>
        /// Endpoint che esegue l'autenticazione a Google Cloud
        /// </summary>
        /// <returns>Autenticazione Google Cloud</returns>
        /// <remarks>
        /// 
        ///     POST /AutenticaGoogleCloud
        /// 
        /// </remarks>
        /// <response code="200">Token autenticazione Google Cloud</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("AutenticaGoogleCloud")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult AutenticaGoogleCloud([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AutenticaGoogleCloud(request);
                if (!result.RispostaOK)
                {
                    switch (result.Errore.Substring(0, 4))
                    {
                        case "400_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status400BadRequest, result);
                        case "401_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status401Unauthorized, result);
                        case "500_":
                            result.Errore = result.Errore.Substring(4);
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                        default:
                            return StatusCode(StatusCodes.Status500InternalServerError, result);
                    }
                }

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
