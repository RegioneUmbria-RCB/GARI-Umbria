using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.meteo;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using AgronicaDataProvider6.Models;
using InData.Meteo;
using InData.WidgetManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AgronicaCoreAPI.Controllers
{
    public class DSSController : BaseController
    {
        public DSSController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {

        }
        [HttpPost]
        [Route("WidgetManager")]
        public ObjectResult PostWidgetManager([FromBody] WidgetManagerRequest widgetManagerRequest, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard Result = new RispostaStandard();
            widgetManagerResponse responseWidgetManager = new widgetManagerResponse();

            try
            {
                var request = new CoreWS_WidgetManager(objP_super_server,
                                                           objP_server,
                                                           objP_utenti,
                                                           widgetManagerRequest);

                //recupero il token ------- Start
                string token = "";
                APICallsBasic requestImpostazione = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                var resultImpostazione = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpostazioni(requestImpostazione);

                var LeggiImpostazione = new List<LeggiImpostazione>();
                LeggiImpostazione = JsonConvert.DeserializeObject<List<LeggiImpostazione>>(resultImpostazione.RispostaStringa);
                foreach (var item in LeggiImpostazione)
                {
                    if (item.Impostazione_Cod == 860)
                    {
                        if (item.Impostazione_Valore_1.ToString().Length > 0)
                        {
                            var ImpostazioneToken = new ImpostazioneToken();
                            ImpostazioneToken = JsonConvert.DeserializeObject<ImpostazioneToken>(item.Impostazione_Valore_1.ToString());
                            if (ImpostazioneToken.Token != "")
                            {
                                token = ImpostazioneToken.Token;
                            }
                        }
                        break;
                    }
                }

                if (string.IsNullOrEmpty(token))
                {
                    Result.RispostaOK = false;
                    return StatusCode(StatusCodes.Status400BadRequest, responseWidgetManager);
                }
                //recupero il token ------- End
         
                String urlServizio = coreWSBaseURL.Replace("AgronicaCoreWS", "AgronicaAgenda", StringComparison.CurrentCultureIgnoreCase) + "?token=" + token + "&username=" + user + "&rDir=D3GD0GC9GC6GBEGD9GD3GD5GD7GC2G161GA0";
       
                //creo widgetManager  -- Start
                string widgetManagerStr = "{ \"message\": \"OK\", \"statusCode\": 0, \"token\": null, \"dettaglioEsito\": { \"dettaglioRisposta\": [ { \"descrizione\": \"apri direttamente GIAS in una nuova scheda\", \"descrizioneAggiuntiva\": \"senza ripetere il login accedi all’applicazione completa\", \"idWidget\": \"SSOByPassGias\", \"nascosto\": false, \"tipoRender\": \"nuovaScheda\", \"titolo\": \"ACCEDI A GIAS\", \"urlImmagine\": \"\", \"urlServizio\": \"\" }, { \"descrizione\": \"riepilogo delle previsioni dei modelli di difesa\", \"descrizioneAggiuntiva\": \"\", \"idWidget\": \"DSSAgronica1\", \"nascosto\": false, \"tipoRender\": \"iframe\", \"titolo\": \"INDICATORI DSS DIFESA\", \"urlImmagine\": \"\", \"urlServizio\": \"\" } ], \"tipoRisposta\": null, \"titoloWidget\": \"widget di prova\" }, \"errori\": \"\" }";

                responseWidgetManager = JsonConvert.DeserializeObject<widgetManagerResponse>(widgetManagerStr);
                foreach (var item in responseWidgetManager.dettaglioEsito.dettaglioRisposta) 
                {
                    item.urlServizio = urlServizio;
                }

                //creo widgetManager  -- End            
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, responseWidgetManager);
            }
            catch (Exception ex)
            {
                responseWidgetManager.message = "NO";
                //responseWidgetManager.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, responseWidgetManager);
            }

               return StatusCode(StatusCodes.Status200OK, responseWidgetManager);
        }

        [HttpGet]
        [Route("Leggi_Turni_Salvati")]
        [ProducesResponseType(typeof(RispostaStandard<List<TurnoConsiglioIrrigazione>>), StatusCodes.Status200OK)]
        public ObjectResult Leggi_Turni_Salvati([FromQuery] int idDss, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<TurnoConsiglioIrrigazione>> result = new();

            try
            {
                var InData = getRequest(idDss);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Turni_Salvati(InData);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result.Errore);
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
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errore);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost]
        [Route("CercaPioggeIrrigazione")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CercaPioggeIrrigazione([FromBody] LeggiRilieviPiogge leggiRilieviPiogge, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();
            try
            {
                var InData = getRequest(leggiRilieviPiogge);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CercaPioggeIrrigazione(InData);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result.Errore);
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
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errore);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }
    }
}
