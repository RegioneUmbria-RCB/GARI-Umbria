using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.baseClass;
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

    public class FreshAndFoodController : BaseController
    {
        public FreshAndFoodController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route(nameof(LeggiValoriParametriQualitativi_Modello))]
        public ObjectResult LeggiValoriParametriQualitativi_Modello([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.FiltroValoriParametriQualitativi filtroValoriParametriQualitativi, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<BaseCodeDescr>> result = new RispostaStandard<List<BaseCodeDescr>>();

            try
            {
                var request = getRequest(filtroValoriParametriQualitativi);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiValoriParametriQualitativi_Modello(request);
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
