using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.Provisioning;
using AgronicaCoreDTOStd.InData.Provisioning.Retail;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.provisioning;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using AgronicaDataProvider6.Models;
using InData.Provisioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace AgronicaCoreAPI.Controllers
{
    public class ProvisioningController : BaseController
    {
        public ProvisioningController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
        }

        [HttpPost]
        [Route("ClientValidation")]
        public ObjectResult PostClientValidation([FromBody] ClientValidationRequest ClientValidationRequest, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard Result = new RispostaStandard();
            ClientValidationResponse ResultApi = new ClientValidationResponse();

            try
            {
                var request = new CoreWS_ClientValidation(objP_super_server,
                                                            objP_server,
                                                            objP_utenti,
                                                            ClientValidationRequest.xAppName,
                                                            ClientValidationRequest.xAppVersion,
                                                            ClientValidationRequest.xPlatform,
                                                            ClientValidationRequest.xEnvironment);

                //Controllo prima il coreWs che non restituisce errore o risposta vuota
                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiClientValidation(request);
                if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, ResultApi);
                // dopo faccio un controllo dentro AgronicaWebService 
                if (string.IsNullOrEmpty(Result.RispostaStringa))
                {
                    Result = new CoreWSController(hc, agronicaWSBaseURL, bearerToken, Request).LeggiClientValidationWS2010(request);
                    if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, ResultApi);
                }

                Boolean forceUp = !String.IsNullOrEmpty(Result.RispostaStringa);

                ResultApi = new ClientValidationResponse { Found = forceUp, ForceUpgrade = forceUp, Message = Result.RispostaStringa, Query = ClientValidationRequest };
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ResultApi);
            }

            return StatusCode(StatusCodes.Status200OK, ResultApi);

        }

        [HttpPost]
        [Route("ApiValidation")]
        public ObjectResult PostApiValidation([FromBody] ApiValidationRequest ApiValidationRequest, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard Result = new RispostaStandard();
            ApiValidationResponse ResultApi = new ApiValidationResponse();
            ForcedDowngradeResponse ForcedDowngrade = new ForcedDowngradeResponse();
            string inputVersionAPI = "";

            try
            {
                // leggere valore inputVersionAPI dal file GiasVersioneCorrente.txt
                    string filePath = "GiasVersioneCorrente.txt";
                    inputVersionAPI = System.IO.File.ReadAllText(filePath).Trim();
     
                var request = new CoreWS_ApiValidation(objP_super_server,
                                                            objP_server,
                                                            objP_utenti,
                                                            ApiValidationRequest.xAppName,
                                                            ApiValidationRequest.xAppVersion,
                                                            ApiValidationRequest.xPlatform,
                                                            ApiValidationRequest.xEnvironment,
                                                            inputVersionAPI);

                // verifica inputVersionAPI isNumeric
                decimal numericValue;
                bool isNumber = decimal.TryParse(inputVersionAPI, out numericValue);

                if (isNumber)
                {

                    Result = new CoreWSController(hc, agronicaWSBaseURL, bearerToken, Request).LeggiApiValidationWS2010(request);
                    if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, ResultApi);

                    Boolean forceUp = !String.IsNullOrEmpty(Result.RispostaStringa);
                    ForcedDowngrade = JsonConvert.DeserializeObject<ForcedDowngradeResponse>(Result.RispostaStringa);

                    ResultApi = new ApiValidationResponse { Found = forceUp, ForceUpgrade = ForcedDowngrade.ForceUpgrade, ForceDowngrade = ForcedDowngrade.ForceDowngrade, Message = ForcedDowngrade.Message, Query = ApiValidationRequest };

                }
                else
                {
                    ResultApi = new ApiValidationResponse { Found = false, ForceUpgrade = false, ForceDowngrade = false, Message = "", Query = ApiValidationRequest };
                }

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ResultApi);
            }

            return StatusCode(StatusCodes.Status200OK, ResultApi);

        }

        /// <summary>
        /// Ritorna l'elenco degli utenti
        /// </summary>
        /// <returns>Elenco degli utenti in formato JSON</returns>
        /// <remarks>
        /// 
        ///     GET /ListaUtenti
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpGet]
        [Route("ListaUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ListaUtenti([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ListaUtenti(request);
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
        /// Ritorna l'elenco degli utenti con i dati di base
        /// </summary>
        /// <returns>Elenco degli utenti in formato strutturato</returns>
        /// <remarks>
        /// 
        ///     GET /ListaUtentiDatiBase
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpGet]
        [Route("ListaUtentiDatiBase")]
        [ProducesResponseType(typeof(RispostaStandard<ListaUtenti>), StatusCodes.Status200OK)]
        public ObjectResult ListaUtentiDatiBase([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ListaUtenti> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ListaUtentiDatiBase(request);
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
        /// Ritorna l'elenco dei gruppi utenti
        /// </summary>
        /// <returns>Elenco dei gruppi utenti in formato strutturato</returns>
        /// <remarks>
        /// 
        ///     GET /ListaGruppiUtente
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpGet]
        [Route("ListaGruppiUtente")]
        [ProducesResponseType(typeof(RispostaStandard<ListaGruppiUtente>), StatusCodes.Status200OK)]
        public ObjectResult ListaGruppiUtente([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ListaGruppiUtente> result = new();

            try
            {
                var request = getRequest("");

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ListaGruppiUtente(request);
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
        /// Ritorna i dati del server richiesto
        /// </summary>
        /// <param name="getDatiServerRequest">Il tipo di server di cui si richiedono i dati</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Elenco dei server corrispondenti ai dati di ricerca</returns>
        /// <remarks>
        /// 
        ///     POST /DatiServer
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("DatiServer")]
        [ProducesResponseType(typeof(RispostaStandard<DatiServer>), StatusCodes.Status200OK)]
        public ObjectResult DatiServer([FromBody] DatiServerRequest getDatiServerRequest, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard<DatiServer> Result = new RispostaStandard<DatiServer>();

            try
            {
                var request = getRequest(getDatiServerRequest);
                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).DatiServer(request);
                if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        /// <summary>
        /// Crea un nuovo token utente nelle tabelle super_server
        /// </summary>
        /// <param name="creaTokenIn">Contiene i riferimenti ai DB</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /CreaNuovoToken
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CreaNuovoToken")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CreaNuovoToken([FromBody] CreaToken_In creaTokenIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                var request = getRequest(creaTokenIn);
                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CreaNuovoToken(request);
                if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        /// <summary>
        /// Verifica l'esistenza di un token
        /// </summary>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaEsistenzaToken
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaEsistenzaToken")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult VerificaEsistenzaToken([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                var request = getRequest("");
                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaEsistenzaToken(request);
                if (!Result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        /// <summary>
        /// Verifica l'esistenza dell'utente retail e procede alla creazione se non esiste
        ///     oppure forza la creazione di un nuovo utente con Piva fittizia
        /// </summary>
        /// <param name="nuovoUtente">Contiene i riferimenti ai DB</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /NuovoUtenteRetail
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="401">Utente non autorizzato oppure token non attivo/esistente</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("NuovoUtenteRetail")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult NuovoUtenteRetail([FromBody] Nuovo_Utente_Retail_In nuovoUtente, [FromHeader] string Authorization)
        {
            //if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                string bearerValue = extractBearerToken(Authorization);
                bearerToken = bearerValue;

                JObject extendedInput = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(nuovoUtente));
                extendedInput.Add("Token", bearerValue);

                var request = getRequest(extendedInput);

                Result = new CoreWSController(hc, coreWSBaseURL.Equals("") ? RetailCoreWS_BaseURL : coreWSBaseURL, bearerToken, Request).NuovoUtenteRetail(request);

                if (!Result.RispostaOK)
                {
                    if (Result.Errore.Equals("INVALID_TOKEN") || Result.Errore.Equals("INVALID_USER")) 
                        return StatusCode(StatusCodes.Status401Unauthorized, null);

                    return StatusCode(StatusCodes.Status500InternalServerError, Result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        /// <summary>
        /// Riporta un utente dalla tabella tampone APP_UtentiRetail,
        /// crea una impresa, una transazione commerciale ed alla fine cancella logicamente la entry della tabella tampone
        /// </summary>
        /// <param name="riportaUtente">Oggetto contenente il codice attivazione dell'utente da riportare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /RiportaNuovoUtente
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("RiportaNuovoUtente")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult RiportaNuovoUtente([FromBody] Riporta_Utente_Retail_In riportaUtente, [FromHeader] string Authorization)
        {
            //if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                string bearerValue = extractBearerToken(Authorization);
                bearerToken = bearerValue;

                JObject extendedInput = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(riportaUtente));
                extendedInput.Add("Token", bearerValue);

                var request = getRequest(extendedInput);

                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RiportaNuovoUtente(request);

                if (!Result.RispostaOK)
                {
                    if (Result.Errore.Equals("INVALID_TOKEN") || Result.Errore.Equals("INVALID_USER"))
                        return StatusCode(StatusCodes.Status401Unauthorized, null);

                    return StatusCode(StatusCodes.Status500InternalServerError, Result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        /// <summary>
        /// Rinnova l'iscrizione di un utente retail
        /// </summary>
        /// <param name="rinnovaUtente">Oggetto contenente l'utente da rinnovare e la transazione commerciale associata</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /RinnovaUtente
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("RinnovaUtente")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult RinnovaUtente([FromBody] Rinnova_Utente_Retail_In rinnovaUtente, [FromHeader] string Authorization)
        {
            //if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                string bearerValue = extractBearerToken(Authorization);
                bearerToken = bearerValue;

                JObject extendedInput = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(rinnovaUtente));
                extendedInput.Add("Token", bearerValue);

                var request = getRequest(extendedInput);

                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RinnovaUtente(request);

                if (!Result.RispostaOK)
                {
                    if (Result.Errore.Equals("INVALID_TOKEN") || Result.Errore.Equals("INVALID_USER"))
                        return StatusCode(StatusCodes.Status401Unauthorized, null);

                    return StatusCode(StatusCodes.Status500InternalServerError, Result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        /// <summary>
        /// Cambia l'indirizzo email dell'utente
        /// </summary>
        /// <param name="nuovaEmail"> Nuova email da associare all'utente loggato</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /CambiaEmailUtente
        /// 
        /// </remarks>
        /// <response code="200">Operazione andata a buon fine</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CambiaEmailUtente")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CambiaEmailUtente([FromBody] NuovaEmail_In nuovaEmail, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();

            try
            {
                var request = getRequest(nuovaEmail);

                Result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CambiaEmailUtente(request);

                if (!Result.RispostaOK)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, Result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, Result);
            }
            catch (Exception ex)
            {
                Result.RispostaOK = false;
                Result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);

                return StatusCode(StatusCodes.Status500InternalServerError, Result);
            }

            return StatusCode(StatusCodes.Status200OK, Result);
        }

        private string extractBearerToken(string Authorization)
        {
            string bearerToken = "";
            if (!string.IsNullOrWhiteSpace(Authorization))
            {
                if (Authorization.StartsWith("Bearer "))
                {
                    bearerToken = Authorization.Substring("Bearer ".Length).Trim();
                }
                else //non dovrebbe mai entrare nel else
                {
                    bearerToken = Authorization;
                }
            }

            return bearerToken;
        }
    }
}
