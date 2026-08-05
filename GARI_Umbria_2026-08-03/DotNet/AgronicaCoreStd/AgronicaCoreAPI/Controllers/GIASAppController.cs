using AgronicaCoreDTOStd.InData.GiasApp;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AgronicaCoreModelsSTD.GiasAPP;
using System;
using System.Net.Http;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class GIASAppController : BaseController
    {
        public GIASAppController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {

        }

        [HttpPost]
        [Route("PushNotification")]
        public ObjectResult PushNotification([FromBody] Notifica_Utente_In notificaUtenteRequest, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                var request = getRequest(notificaUtenteRequest);
                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PushNotification(request);
                if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        [HttpPost]
        [Route("PopNotification")]
        public ObjectResult PopNotification([FromBody] Notifica_Utente_In notificaUtenteRequest, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                var request = getRequest(notificaUtenteRequest);
                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PopNotification(request);
                if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        [HttpGet]
        [Route("ServiziNotificheSottoscrivibili")]
        [ProducesResponseType(typeof(RispostaStandard<ServiziSottoscrivibili_Out>), StatusCodes.Status200OK)]
        public ObjectResult ServiziNotificheSottoscrivibili([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard<ServiziSottoscrivibili_Out> Result = new RispostaStandard<ServiziSottoscrivibili_Out>();
            
            try
            {
                var request = getRequest("");
                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ServiziNotificheSottoscrivibili(request);
                if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }
    }
}
