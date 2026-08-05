using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.importazioni;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.Utility;
using AgronicaCoreUtilityStd;
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
    public class DemetraController : BaseController
    {
        public DemetraController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("Import/OperazioniQdC")]
        public ObjectResult PostOperazioniQdC(ImportDemetra importDemetra, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importDemetra);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportAttivitaDemetra(request);
                if (result.message != Api_Response_Message_Type.Ok) return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.message = "";
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }

        [HttpPost]
        [Route("Import/AnalisiTerreno")]
        public ObjectResult PostAnalisiTerreno(ImportDemetra importDemetra, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importDemetra);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportAnalisiTerrenoDemetra(request);
                if (result.message != Api_Response_Message_Type.Ok) return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.message = "";
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }

        [HttpPost]
        [Route("Import/Fabbricati")]
        public ObjectResult PostFabbricati(ImportDemetra importDemetra, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importDemetra);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportFabbricatiDemetra(request);
                if (result.message != Api_Response_Message_Type.Ok) return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.message = "";
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }


            return StatusCode(StatusCodes.Status200OK, result);

        }

        [HttpPost]
        [Route("Import/LavoratoriQDC")]
        public ObjectResult PostLavoratoriQDC(ImportDemetra importDemetra, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importDemetra);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportContattiDemetra(request);
                if (result.message != Api_Response_Message_Type.Ok) return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.message = "";
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }

        [HttpPost]
        [Route("Import/Squadre")]
        public ObjectResult PostSquadre(ImportDemetra importDemetra, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importDemetra);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportSquadreDemetra(request);
                if (result.message != Api_Response_Message_Type.Ok) return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.message = "";
                result.errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }


            return StatusCode(StatusCodes.Status200OK, result);

        }

    }

}
