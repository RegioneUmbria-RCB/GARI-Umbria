using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using InData.Anagrafica;
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
    public class MagazziniController : BaseController
    {

        public MagazziniController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpGet]
        // [Route("{piva}")]
        public ObjectResult GetMagazzini(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Fabbricato>> result = new RispostaStandard<List<Fabbricato>>();

            try
            {
                var request = getRequest(new LeggiMagazzini(piva, true));
                // CoreWSLeggiMagazzini request = new CoreWSLeggiMagazzini(objP_super_server, objP_server, objP_utenti, piva, true);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMagazziniModello(request);
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
        public ObjectResult Post([FromBody] List<Fabbricato> magazzini, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(magazzini);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviMagazziniModello(request);
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
