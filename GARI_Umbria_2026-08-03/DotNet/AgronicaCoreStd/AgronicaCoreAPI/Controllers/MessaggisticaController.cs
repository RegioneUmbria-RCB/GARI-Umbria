using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.Gis;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using CoreApi.BusinessLayer.Services;
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
    public class MessaggisticaController : BaseController
    {
        private readonly IGisDataReadParam gisDataReadParamValidator;

        public MessaggisticaController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IGisDataReadParam gisDataReadParamValidator, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
            this.gisDataReadParamValidator = gisDataReadParamValidator;
        }

        /// <summary>
        /// Metodo che accoda un messaggio relativo ad una esecuzione differita da mostrare all'utente
        /// </summary>
        /// <param name="MessaggioEsecuzioneIn"> Messaggio da accodare </param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /AccodaMessaggioEsecuzione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("AccodaMessaggioEsecuzione")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult AccodaMessaggioEsecuzione([FromBody] MessaggioEsecuzione MessaggioEsecuzioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(MessaggioEsecuzioneIn);

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AccodaMessaggioEsecuzione(request);
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

        /// <summary>
        /// Metodo che legge i messaggi di esecuzione ancora da mostrare all'utente
        /// </summary>
        /// <returns>Lista dei messaggi da mostrare</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiMessaggiEsecuzioneNonLetti
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Lista dei messaggi da mostrare</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiMessaggiEsecuzioneNonLetti")]
        [ProducesResponseType(typeof(RispostaStandard<List<MessaggioEsecuzione>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiMessaggiEsecuzioneNonLetti([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<MessaggioEsecuzione>> result = new();

            try
            {
                var request = getRequest("");

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMessaggiEsecuzioneNonLetti(request);
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

        /// <summary>
        /// Metodo che legge un messaggio di esecuzione
        /// </summary>
        /// <param name="ID_Messaggio"> L'ID del messaggio da leggere</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Il messaggio relativo all'ID_Messaggi in input</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiMessaggioEsecuzione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Lista dei messaggi da mostrare</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiMessaggioEsecuzione")]
        [ProducesResponseType(typeof(RispostaStandard<MessaggioEsecuzione>), StatusCodes.Status200OK)]
        public ObjectResult LeggiMessaggioEsecuzione(Int32 ID_Messaggio, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<MessaggioEsecuzione> result = new();

            try
            {
                var request = getRequest(ID_Messaggio);

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMessaggioEsecuzione(request);
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

        /// <summary>
        /// Metodo che setta il flag letto e / o annullato del messaggio esecuzione
        /// </summary>
        /// <param name="ModificaMessaggioEsecuzioneIn"> Messaggio da modificare </param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /ModificaMessaggioEsecuzione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ModificaMessaggioEsecuzione")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ModificaMessaggioEsecuzione(MessaggioEsecuzione ModificaMessaggioEsecuzioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(ModificaMessaggioEsecuzioneIn);

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModificaMessaggioEsecuzione(request);
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
