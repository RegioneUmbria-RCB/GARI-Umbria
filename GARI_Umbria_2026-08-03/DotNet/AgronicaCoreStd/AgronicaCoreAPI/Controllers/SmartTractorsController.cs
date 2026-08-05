using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.SmartTractors_HubIoT;
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
using System.Collections.Generic;
using System.Net.Http;

namespace AgronicaCoreAPI.Controllers
{
    public class SmartTractorsController : BaseController
    {
        public SmartTractorsController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {

        }

        [HttpPost]
        [Route("InviaRicetta")]
        public ObjectResult InviaRicetta([FromBody] List<RicettaOperazione2WorkOrderKey> req, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).InviaRicetta(request);
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

        [HttpPost]
        [Route("LeggiParametriConnessioni")]
        public ObjectResult LeggiParametriConnessioni([FromBody] List<ParametriConnessioni_In> req, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ParametriConnessioni_Out> result = new();

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiParametriConnessioni(request);
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

        [HttpPost]
        [Route("ScriviParametriConnessioni")]
        public ObjectResult ScriviParametriConnessioni([FromBody] SalvaParametriConnessioni_In req, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaParametriConnessioni(request);
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
