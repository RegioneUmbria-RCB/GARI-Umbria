using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.anagrafiche;
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

    public class CodificheController : BaseController
    {
        public CodificheController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }


        [HttpGet]
        [Route("LeggiDettaglioVarietaPersonalizzato")]
        public ObjectResult LeggiDettaglioVarietaPersonalizzato([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato>>();

            try
            {

                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDettaglioVarietaPersonalizzato(request);
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
        [HttpGet]
        [Route("LeggiCapitolatoPrivato")]
        public ObjectResult LeggiCapitolatoPrivato([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>();

            try
            {

                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCapitolatoPrivato(request);
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

        [HttpGet]
        [Route("LeggiResiduiDisponibili")]
        public ObjectResult LeggiResiduiDisponibili([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>();

            try
            {

                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiResiduiDisponibili(request);
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

        [HttpGet]
        [Route("LeggiCertProdDisponibili")]
        public ObjectResult LeggiCertProdDisponibili([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>();

            try
            {

                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCertProdDisponibili(request);
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
        [HttpGet]
        [Route("LeggiPianiSemina")]
        public ObjectResult LeggiPianiSemina([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>();

            try
            {

                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPianiSemina(request);
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
        [Route("LeggiCAC_Codifica_InfoAggiuntive")]
        public ObjectResult LeggiCAC_Codifica_InfoAggiuntive([FromBody] LeggiCAC_Codifica_InfoAggiuntive leggiCAC, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>();

            try
            {

                var request = getRequest(leggiCAC);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCAC_Codifica_InfoAggiuntive(request);
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
