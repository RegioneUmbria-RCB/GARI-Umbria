using AgronicaCoreAPI.Resources;
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
    public class FiltroRicercaController : BaseController
    {
        public FiltroRicercaController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("ProseguiSelezionati")]
        public ObjectResult ProseguiSelezionati([FromBody] AgronicaCoreDTOStd.InData.FiltroRicerca.ProseguiSelezionati request, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            string result = "";

            RispostaStandard<string> rispostaStandard = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ProseguiSelezionati(getRequest(request));
                rispostaStandard.RispostaStringa = result;
                rispostaStandard.RispostaOK = true;
                if (!rispostaStandard.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, rispostaStandard);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                rispostaStandard.RispostaOK = false;
                rispostaStandard.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, rispostaStandard);
            }
            catch (Exception ex)
            {
                rispostaStandard.RispostaOK = false;
                rispostaStandard.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, rispostaStandard);
            }

            return StatusCode(StatusCodes.Status200OK, rispostaStandard);
        }

    }
}
