using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.Notifiche;
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
    public class NotificationController : BaseController
    {
        public NotificationController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
        }

        [HttpPost]
        [Route("NotificaCUAA")]
        public ObjectResult PostNotificaElencoCUAA([FromBody] NotificaCUAA req, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            foreach(var cuaa in req.elencoCUAA)
            {
                if (cuaa.Priorita == null)
                    cuaa.Priorita = 0;
            }

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RegistraNotificheCUAA(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result.Errore);
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
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errore);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }
    }
}
