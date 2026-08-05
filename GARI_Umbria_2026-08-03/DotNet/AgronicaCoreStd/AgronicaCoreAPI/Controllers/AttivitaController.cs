using AgronicaCoreAPI.adapters;
using AgronicaCoreAPI.Resources;
using AgronicaCoreModelloSTD;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using AgronicaDataProvider6.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using static AgronicaCoreDataProviderSTD.CostantiPersonalizzate;
using static AgronicaCoreModelsSTD.attivita.Attivita;
using Attivita = AgronicaCoreModelsSTD.attivita.Attivita;

namespace AgronicaCoreAPI.Controllers
{
    public class AttivitaController : BaseController
    {
        public AttivitaController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpGet]
        [Route("{piva}")]
        public ObjectResult Get(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            
            RispostaStandard<List<Attivita>> result = new RispostaStandard<List<Attivita>>();
            
            try
            {
                var request = new CoreWS_RicettePerScarico(objP_super_server, objP_server, objP_utenti, piva, "", "");
                RispostaStandard risposta = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiRicette(request);

                if (!risposta.RispostaOK && string.IsNullOrEmpty(risposta.RispostaStringa)) {
                    result.RispostaOK = false;
                    result.Errore = risposta.Errore;
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

                AttivitaAdapter adapter = new AttivitaAdapter();
                List<Attivita> attivita = adapter.leggiAttivita(risposta.RispostaStringa);

                result.RispostaOK = true;
                result.RispostaStringa = attivita;

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
        public ObjectResult Post([FromBody] Attivita attivita, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {

                AttivitaAdapter adapter = new AttivitaAdapter();
                RicettePerScarico dati = adapter.initDati();
                dati.cancellato = attivita.cancellato;
                dati.guid = attivita.guid;
                dati.riferimentoPianificata = attivita.riferimentoPianificata;
                dati.isPianificata = attivita.stato == Stati.Da_Eseguire;

                if (!attivita.cancellato) {

                    Job job = attivita.job;
                    
                    // attività principale
                    adapter.scriviAttivita(dati, attivita, job, user);

                    // scrive dati comuni
                    adapter.scriviDatiComuni(dati, username);
                }

                var request = new CoreWS_RicettaScrivi_APP(objP_super_server, objP_server, objP_utenti, dati);
                result = new CoreWSController(hc,coreWSBaseURL, bearerToken, Request).ScriviRicette(request);

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
        [Route("Visite")]
        public ObjectResult Visite([FromBody] Attivita attivita, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                VisiteAdapter adapter = new VisiteAdapter();
                VisitePerScarico dati = adapter.initDati();
                dati.cancellato = attivita.cancellato;
                dati.guid = attivita.guid;

                if (!attivita.cancellato) {

                    Job job = attivita.job;
                    Job job2 = null;

                    if (job.getTipo() == TipiJob.JOB_COMPOSITE) {
                        var jobComposito = (JobComposito)attivita.job;
                        job = jobComposito.lavorazione;
                        job2 = jobComposito.attivitaCDG;
                    }
                    
                    if (job.getCodice() == LAVCOD_VISITA && job2 != null) {
                        adapter.scriviAttivita(dati, attivita, job2.getCodice(), user);
                        adapter.scriviDatiComuni(dati, username);
                    }
                }

                var request = new CoreWS_VisiteScrivi_APP(objP_super_server, objP_server, objP_utenti, dati);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviVisite(request);

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
