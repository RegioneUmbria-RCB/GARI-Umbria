using AgronicaCoreAPI.adapters;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static AgronicaCoreAPI.models.StatisticheEntity;
using System.Collections.Generic;
using System.Net.Http;
using System;
using InData.Statistiche;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class StatisticheController : BaseController
    {
        public StatisticheController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("RilieviProduzione")]
        public ObjectResult RilieviProduzione([FromBody] LeggiRilieviProduzione request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<RilievoProduzione>> result = new RispostaStandard<List<RilievoProduzione>>();

            try
            {
                var adapter = new StatisticheAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                var r = ws.LeggiRilieviProduzione(getRequest(request));
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaStringa = adapter.leggiRilieviProduzione(r.RispostaStringa);
                result.RispostaOK = true;
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
        [Route("StimeProduzione")]
        public ObjectResult StimeProduzione([FromBody] LeggiStimeProduzione request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<StimeProduzione>> result = new RispostaStandard<List<StimeProduzione>>();

            try
            {
                var adapter = new StatisticheAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                var r = ws.LeggiStimeProduzione(getRequest(request));
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaStringa = adapter.leggiStimeProduzione(r.RispostaStringa);
                result.RispostaOK = true;
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
