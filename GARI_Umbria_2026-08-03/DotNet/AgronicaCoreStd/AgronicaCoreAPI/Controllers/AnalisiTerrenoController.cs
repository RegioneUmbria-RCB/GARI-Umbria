
using AgronicaCoreDTOStd.InData.Analisi;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using InData.Analisi;
using AgronicaCoreWebServiceSTD;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class AnalisiTerrenoController : BaseController
    {
        public AnalisiTerrenoController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }


        [HttpPost]
        [Route("LeggiListaAnalisiTerreno")]
        public ObjectResult GetListaAnalisiTerreno([FromBody] LeggiListaAnalisiTerreno InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiListaAnalisiTerreno(request);
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
        [Route("LeggiAnalisiTerreno")]
        public ObjectResult LeggiAnalisiTerreno([FromBody] LeggiAnalisiTerreno InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.analisi.AnalisiTerreno> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAnalisiTerreno(request);
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
        [Route("ScriviAnalisiTerreno")]
        public ObjectResult ScriviAnalisiTerreno([FromBody] ScriviAnalisiTerreno InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviAnalisiTerreno(request);
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
        [Route("ModificaAnalisiTerrenoInLine")]
        public ObjectResult ModificaAnalisiTerrenoInLine([FromBody] ScriviAnalisiTerreno InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModificaAnalisiTerrenoInLine(request);
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
        [Route("CancellaAnalisiTerreno")]
        public ObjectResult CancellaAnalisiTerreno([FromBody] ScriviAnalisiTerreno InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CancellaAnalisiTerreno(request);
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
        [Route("CancellaListaAnalisiTerreno")]
        public ObjectResult CancellaListaAnalisiTerreno([FromBody] ScriviListaAnalisiTerreno InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CancellaListaAnalisiTerreno(request);
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
        [Route("LeggiLaboratori")]
        public ObjectResult LeggiLaboratori([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiLaboratori(request);
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
        [Route("LeggiSchemiAnalisi")]
        public ObjectResult getSchemiAnalisi([FromBody] LeggiSchemiAnalisi InData, [FromHeader] string Authorization) 
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSchemiAnalisi(request);
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
        [Route("LeggiParametriAnalisi")]
        public ObjectResult getParametriAnalisi([FromBody] LeggiParametriAnalisiDettagli InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiParametriAnalisi(request);
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
