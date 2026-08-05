using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.importazioni;
using AgronicaCoreDTOStd.InData.NewAgri;
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
using System.Net;
using System.Net.Http;

namespace AgronicaCoreAPI.Controllers
{
    public class NewAgriController : BaseController
    {
        public NewAgriController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("N_Distribuito")]
        public ObjectResult N_Distribuito(RequestNDistribuito inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).N_Distribuito(request);
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
        [Route("Import/OperazioniQdC")]
        public ObjectResult PostOperazioniQdC(ImportDemetra importNewAgri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importNewAgri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportAttivitaNewAgri(request);
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
        [Route("Import/Aggiorna_Impianti_N_Pua")]
        public ObjectResult Aggiorna_Impianti_N_Pua(RequestNDistribuito importNewAgri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importNewAgri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Aggiorna_Impianti_N_Pua(request);
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
        [Route("Import/Utente")]
        public ObjectResult ImportUtenteNewAgri(RequestUtente importNewAgri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importNewAgri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportUtenteNewAgri(request);
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
        [Route("Import/VisibilitaUtente")]
        public ObjectResult ImportVisibilitaUtenteNewAgri(RequestImportVisibilita importNewAgri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            Api_Response result = new Api_Response();

            try
            {
                var request = getRequest((object)importNewAgri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportVisibilitaUtenteNewAgri(request);
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
