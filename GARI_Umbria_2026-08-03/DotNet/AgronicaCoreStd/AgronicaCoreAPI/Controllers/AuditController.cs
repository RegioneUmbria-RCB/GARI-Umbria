using AgronicaCoreAPI.adapters;
using AgronicaCoreAPI.models.entities;
using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.Audit;
using AgronicaCoreModelsSTD.Audit;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;

namespace AgronicaCoreAPI.Controllers
{
    public class AuditController : BaseController
    {
        public AuditController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        /// <summary>
        /// Restituisce la checklist EUDR per il fornitore specificato.
        /// </summary>
        /// <param name="cod_fornitore">Codice identificativo del fornitore</param>
        /// <param name="Authorization">Token di autenticazione (header)</param>
        /// <returns>
        /// Oggetto RispostaStandard contenente la checklist EUDR.<br/>
        /// Status 200: OK<br/>
        /// Status 401: Non autorizzato<br/>
        /// Status 500: Errore interno
        /// </returns>
        [HttpGet]
        [Route("ChecklistEUDR")]
        [ProducesResponseType(typeof(RispostaStandard<ChecklistEUDR>), StatusCodes.Status200OK)]
        public ObjectResult GetChecklistEUDR([FromQuery] string cod_fornitore, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (string.IsNullOrEmpty(cod_fornitore)) return StatusCode(StatusCodes.Status400BadRequest, "Parametri ingresso non validi");

            var result = new RispostaStandard<ChecklistEUDR>();

            try
            {
                var request = getRequest(new LeggiChecklistEUDR(cod_fornitore));
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetChecklistEUDR(request);
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
        /// Scrive una checklist EUDR per il fornitore specificato.
        /// </summary>
        /// <param name="checklist">Oggetto ScriviChecklistEUDR con i dati della checklist</param>
        /// <param name="Authorization">Token di autenticazione (header)</param>
        /// <returns>
        /// Oggetto RispostaStandard contenente la checklist EUDR aggiornata.<br/>
        /// Status 200: OK<br/>
        /// Status 401: Non autorizzato<br/>
        /// Status 500: Errore interno
        /// </returns>
        [HttpPost]
        [Route("ChecklistEUDR")]
        [ProducesResponseType(typeof(RispostaStandard<ChecklistEUDR>), StatusCodes.Status200OK)]
        public ObjectResult PostChecklistEUDR([FromBody] ScriviChecklistEUDR checklist, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (checklist == null || string.IsNullOrEmpty(checklist.cod_fornitore))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Parametri ingresso non validi");
            }

            var result = new RispostaStandard<ChecklistEUDR>();

            try
            {
                var request = getRequest(checklist);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PostChecklistEUDR(request);
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
        /// Restituisce i dati necessari alla gestione dell'impostazione per checklist documentale.
        /// </summary>
        /// <param name="Authorization">Token di autenticazione (header)</param>
        /// <returns>
        /// Oggetto RispostaStandard contenente la checklist EUDR aggiornata.<br/>
        /// Status 200: OK<br/>
        /// Status 401: Non autorizzato<br/>
        /// Status 500: Errore interno
        /// </returns>
        [HttpPost]
        [Route("GetCheckListManagementData")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult GetCheckListManagementData([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var readChecklistData = () =>
            {
                var request = getRequest("");
                return new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetCheckListManagementData(request);
            };
            return CallController(readChecklistData);
        }

        /// <summary>
        /// Salva il nuovo valore dell'impostazione per la gestione di checklist documentale.
        /// </summary>
        /// <param name="newSettingValue">Il nuovo valore dell'impostazione</param>
        /// <param name="Authorization">Token di autenticazione (header)</param>
        /// <returns>
        /// Oggetto RispostaStandard contenente la checklist EUDR aggiornata.<br/>
        /// Status 200: OK<br/>
        /// Status 401: Non autorizzato<br/>
        /// Status 500: Errore interno
        /// </returns>
        [HttpPost]
        [Route("SaveChecklistManagement")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SaveChecklistManagement([FromBody] string newSettingValue, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var saveChecklistData = () =>
            {
                var request = getRequest(newSettingValue);
                return new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SaveChecklistManagement(request);
            };
            return CallController(saveChecklistData);
        }

        /// <summary>
        /// Restituisce i dati necessari alla gestione dell'impostazione per workflow documentale.
        /// </summary>
        /// <param name="Authorization">Token di autenticazione (header)</param>
        /// <returns>
        /// Oggetto RispostaStandard contenente la checklist EUDR aggiornata.<br/>
        /// Status 200: OK<br/>
        /// Status 401: Non autorizzato<br/>
        /// Status 500: Errore interno
        /// </returns>
        [HttpPost]
        [Route("GetWorkflowManagementData")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult GetWorkflowManagementData([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var readWorkflowData = () =>
            {
                var request = getRequest("");
                return new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetWorkflowManagementData(request);
            };
            return CallController(readWorkflowData);
        }

        /// <summary>
        /// Salva il nuovo valore dell'impostazione per la gestione di checklist documentale.
        /// </summary>
        /// <param name="newSettingValue">Il nuovo valore dell'impostazione</param>
        /// <param name="Authorization">Token di autenticazione (header)</param>
        /// <returns>
        /// Oggetto RispostaStandard contenente la checklist EUDR aggiornata.<br/>
        /// Status 200: OK<br/>
        /// Status 401: Non autorizzato<br/>
        /// Status 500: Errore interno
        /// </returns>
        [HttpPost]
        [Route("SaveWorkflowManagement")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SaveWorkflowManagement([FromBody] string newSettingValue, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var saveWorkflowData = () =>
            {
                var request = getRequest(newSettingValue);
                return new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SaveWorkflowManagement(request);
            };
            return CallController(saveWorkflowData);

        }
    }
}
