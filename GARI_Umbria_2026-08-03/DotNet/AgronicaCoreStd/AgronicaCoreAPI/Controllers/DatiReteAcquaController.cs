using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.IoT;
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
    public class DatiReteAcquaController : BaseController
    {

        public DatiReteAcquaController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
        }

        [HttpPost]
        [Route("LeggiTipologiaDispositivi")]
        public ObjectResult LeggiTipologiaDispositivi([FromBody] TipologiaDispositivi req, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiTipologiaDispositivi(request);
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
        [Route("LeggiDispositiviXSorgente")]
        public ObjectResult LeggiDispositiviXSorgente([FromBody] DispositiviXSorgente req, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDispositiviXSorgente(request);
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
        [Route("LeggiAnagraficaDispositivi")]
        public ObjectResult LeggiAnagraficaDispositivi([FromBody] LeggiAnagraficaDispositivi req, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAnagraficaDispositivi(request);
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
        [Route("DatiIOTElabora")]
        public ObjectResult DatiIOTElabora([FromBody] RichiediDatiIOT req, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<RisultatoIOT> result = new();

            try
            {
                var request = getRequest(req);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).DatiIOTElabora(request);
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
