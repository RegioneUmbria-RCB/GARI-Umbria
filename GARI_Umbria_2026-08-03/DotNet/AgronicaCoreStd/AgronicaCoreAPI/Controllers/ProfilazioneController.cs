using System;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.profilazione;
using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.InData.Profilazione;
using System.IO;
using AgronicaCoreModelsSTD.Utility;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class ProfilazioneController : BaseController
    {
        public ProfilazioneController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        #region Guida Impostazioni
        [HttpPost]
        [Route("AggiungiGuidaImpostazione")]
        public ObjectResult AggiungiGuidaImpostazione([FromBody] Impostazione[] guida, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(guida);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiungiGuidaImpostazione(request);
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
        [Route("LeggiImpostazioniSezioni")]
        public ObjectResult LeggiImpostazioniSezioni([FromBody] int livelloSezione, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(livelloSezione);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpostazioniSezioni(request);
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

        #endregion

        #region Impostazioni
        /// <summary>
        /// Carica le guide impostazioni per uno o più utenti.
        /// </summary>
        /// <param name="Utenti">Il codice fiscale o la piva degli utenti interessati</param>
        /// <param name="flagLetturaSuperUser">Flag per indicare la lettura delle sulle impostazioni superuser</param>
        /// <param name="Impostazioni">Il codice delle impostazioni interessate, 0 se si vogliono leggere tutte</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Una RispostaStandard contenente la lista di Impostazioni caricate</returns>
        [HttpPost]
        [Route("LeggiGuideImpostazioniUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiGuideImpostazioniUtenti([FromBody] Leggi_Impostazioni inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGuideImpostazioniUtenti(request);
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
        /// Carica i valori dei controlli impostazione.
        /// </summary>
        /// <param name="Utenti">Il codice fiscale o la piva degli utenti interessati</param>
        /// <param name="flagLetturaSuperUser">Flag per indicare la lettura delle sulle impostazioni superuser</param>
        /// <param name="Impostazioni">Il codice delle impostazioni interessate, 0 se si vogliono leggere tutte</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Una RispostaStandard contenente la lista di Impostazioni caricate</returns>
        [HttpPost]
        [Route("LeggiControlliImpostazioniUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiControlliImpostazioniUtenti([FromBody] Leggi_Impostazioni inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiControlliImpostazioniUtenti(request);
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
        /// </summary>
        /// <param name="inData">Lista di oggetti di tipo Utente, in cui vanno specificati
        /// <param name="Authorization">Token di autorizzazione</param>
        /// Username e lista di impostazioni da salvare.</param>
        /// <returns>Una RispostaStandard</returns>
        [HttpPost]
        [Route("SalvaImpostazioniUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SalvaImpostazioniUtenti([FromBody] AgronicaCoreModelsSTD.utente.Utente[] inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaImpostazioniUtenti(request);
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
        /// Copia i valori delle impostazioni utente di un utente a un altro.
        /// </summary>
        /// <param name="inData">Lista di oggetti di tipo Utente, in cui vanno specificati
        /// <param name="Authorization">Token di autorizzazione</param>
        /// Username e lista di impostazioni da salvare.</param>
        /// <returns>Una RispostaStandard</returns>
        [HttpPost]
        [Route("CopiaImpostazioniUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CopiaImpostazioniUtenti([FromBody] CopiaImpostazioniObj inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CopiaImpostazioniUtenti(request);
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
        /// Reimposta le impostazioni utente specificate ai valori stabiliti dal superuser o quelli definiti di default.
        /// </summary>
        /// <param name="inData">Lista di oggetti di tipo Utente, in cui vanno specificati
        /// Username e lista di impostazioni da reimpostare.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Una RispostaStandard</returns>
        [HttpPost]
        [Route("ResetImpostazioniUtentiDefault")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ResetImpostazioniUtentiDefault([FromBody] AgronicaCoreModelsSTD.utente.Utente[] inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ResetImpostazioniUtentiDefault(request);
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

        #endregion

        #region Visibilità
        /// <summary>
        /// </summary>
        /// <param name="inData">Lista di oggetti di tipo Utente, in cui vanno specificati
        /// <param name="Authorization">Token di autorizzazione</param>
        /// Username e lista con le pive delle aziende visibili.</param>
        /// <returns>Una RispostaStandard</returns>
        [HttpPost]
        [Route("CreaUtenteConVisibilita")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CreaUtenteConVisibilita([FromBody] LeggiScriviVisibilitaUtente inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CreaUtenteConVisibilita(request);
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
        /// </summary>
        /// <param name="inData">Utente di cui si desidera alterare la visibilità, 
        /// piva dell'azienda la quale visibilità si vuole rimuovere dall'utente specificato.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Una RispostaStandard</returns>
        [HttpPost]
        [Route("RimuoviVisibilitaPerUtente")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult RimuoviVisibilitaPerUtente([FromBody] RimuoviVisibilitaUtente inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RimuoviVisibilitaPerUtente(request);
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
        /// </summary>
        /// <param name="inData">Lista di oggetti di tipo Utente, in cui vanno specificati
        /// <param name="Authorization">Token di autorizzazione</param>
        /// Username e lista con le pive delle aziende visibili</param>
        /// <returns>Una RispostaStandard</returns>
        [HttpPost]
        [Route("ModificaVisibilitaUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ModificaVisibilitaUtenti([FromBody] LeggiScriviVisibilitaUtenti inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModificaVisibilitaUtente(request);
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
        /// Rimuove le aziende dalla visibilità degli utenti selezionati
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        [HttpPost]
        [Route("RimuoviVisibilitaUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult RimuoviVisibilitaUtenti([FromBody] LeggiScriviVisibilitaUtenti inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RimuoviVisibilitaUtente(request);
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
        /// Assegna agli utenti in base la visibilità dell'utente template.
        /// </summary>
        /// <param name="inData">Oggetto composto da <code>base</code>: lista di username  degli utenti a cui si vuole assegnare la visibilità;
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <code>template</code>: username dell'utente template da cui copiare la visibilità
        /// </param>
        [HttpPost]
        [Route("CopiaVisibilitaUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CopiaVisibilitaUtenti([FromBody] CopiaVisibilitaObj inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CopiaVisibilitaUtenti(request);
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
        /// Carica la visibilità per le imprese specificate. Se non viene
        /// passata alcuna impresa, viene caricata la visibilità di tutte le imprese.
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di oggetti contenenti l'impresa di riferimento e la
        /// lista di utenti che sono i grado di vederla</returns>
        [HttpPost]
        [Route("LeggiAziendeVisibilita")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiAziendeVisibilita([FromBody] LeggiScriviVisibilitaUtenti inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAziendeVisibilita(request);
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
        /// Carica la visibilità per gli utenti specificati.
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di oggetti contenenti l'utente di riferimento e la lista di imprese che può vedere</returns>
        [HttpPost]
        [Route("LeggiVisibilitaUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiVisibilitaUtenti([FromBody] LeggiScriviVisibilitaUtenti inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiVisibilitaUtenti(request);
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
        /// Carica la visibilità per gli utenti ei gruppi specificati.
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di oggetti contenenti l'utente di riferimento e la lista di imprese che può vedere</returns>
        [HttpPost]
        [Route("LeggiVisibilitaGruppi")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiVisibilitaGruppi([FromBody] LeggiScriviVisibilitaUtenti inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiVisibilitaGruppi(request);
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
        /// Esegue un check sugli utenti specificati indicando se hanno visibilità totale o meno.
        /// </summary>
        /// <param name="inData">Lista degli username degli utenti interessati</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di oggetti contenenti l'utente di riferimento e un valore booleano
        /// indicante la il possedimento o meno della visibilità totale</returns>
        [HttpPost]
        [Route("HaVisibilitaTotale")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult HaVisibilitaTotale([FromBody] List<string> inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).HaVisibilitaTotale(request);
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
        [Route("ControllaStessaVisibilita")]
        public ObjectResult ControllaStessaVisibilita([FromBody] List<string> listaPive, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(listaPive);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ControllaStessaVisibilita(request);
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
        [Route("ConfrontaVisibilitaUtenti")]
        public ObjectResult ConfrontaVisibilitaUtenti([FromBody] ConfrontaVisibilitaUtenti confrontaVisibilitaUtenti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(confrontaVisibilitaUtenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ConfrontaVisibilitaUtenti(request);
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
        [Route("CheckFathersInList")]
        public ObjectResult CheckFathersInList([FromBody] AgronicaCoreModelsSTD.anagrafiche.ImpresaDto[] impreseList, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(impreseList);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CheckFathersInList(request);
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

        #endregion

        #region Profili
        /// <summary>
        /// Associa un profilo (aka tipologia utente) a uno o più utenti,
        /// 
        /// </summary>
        [HttpPost]
        [Route(nameof(AssociaProfilo))]
        public ObjectResult AssociaProfilo([FromBody] AssociaProfiloObj inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AssociaProfilo(request);
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
        /// Aggiorna l'intervallo di tempo per cui sono validi i permessi di uno o più utenti.
        /// </summary>
        [HttpPost]
        [Route(nameof(ModificaValiditaPermessi))]
        public ObjectResult ModificaValiditaPermessi([FromBody] AssociaProfiloObj inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModificaValiditaPermessi(request);
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
        /// Aggiorna la finestra temporale di uno o più utenti.
        /// </summary>
        [HttpPost]
        [Route(nameof(ModificaFinestraTemporale))]
        public ObjectResult ModificaFinestraTemporale([FromBody] UtenteFinestraTemp[] inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModificaFinestraTemporale(request);
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
        /// Aggiorna l'intervallo di tempo per cui sono validi i permessi di uno o più utenti.
        /// </summary>
        [HttpPost]
        [Route(nameof(AggiornaPermessiUtentiTipologia))]
        public ObjectResult AggiornaPermessiUtentiTipologia([FromBody] TipologiaUtente inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaPermessiUtentiTipologia(request);
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
        /// Aggiorna le impostazioni degli utenti collegati al profilo.
        /// </summary>
        [HttpPost]
        [Route(nameof(AggiornaImpostazioniUtentiTipologia))]
        public ObjectResult AggiornaImpostazioniUtentiTipologia([FromBody] AssociaProfiloObj inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaImpostazioniUtentiTipologia(request);
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

        #endregion

        #region Utenti

        [HttpGet]
        [Route(nameof(CreateRandomCF))]
        public ObjectResult CreateRandomCF([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CreateRandomCF(request);
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
        /// Carica i dati inerenti a uno o più utenti.
        /// Versione con parametro di: <seealso cref="ProvisioningController.ListaUtenti"/>
        /// </summary>
        /// <param name="filtriUtenti">Stringa in formato JSON contenente i filtri da applicare alla lettura degli utenti.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(CaricaUtenti))]
        public ObjectResult CaricaUtenti([FromBody] string filtriUtenti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(filtriUtenti);
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

        [HttpPost]
        [Route(nameof(GetUsersCount))]
        public ObjectResult GetUsersCount([FromBody] string inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetUsersCount(request);
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
        /// Modifica la lingua di visualizzazione di un utente.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(ImpostaLingua))]
        public ObjectResult ImpostaLingua([FromBody] CambioLinguaObj inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImpostaLingua(request);
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
        [Route("AggiornaCliente_Permessi")]
        public ObjectResult AggiornaCliente_Permessi([FromBody] Cliente_PermessiScrivi scriviFunzionalitaAttiveUtente, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(scriviFunzionalitaAttiveUtente);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaCliente_Permessi(request);
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
        [Route("LeggiCliente_Permessi")]
        public ObjectResult LeggiCliente_Permessi([FromBody] string username, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Cliente_Permesso>> result = new RispostaStandard<List<Cliente_Permesso>>();

            try
            {
                var request = getRequest(username);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCliente_Permessi(request);
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
        [Route("ControllaEsistenzaTabellaCliente_Permessi")]
        public ObjectResult ControllaEsistenzaTabellaCliente_Permessi([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ControllaEsistenzaTabellaCliente_Permessi(request);
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

        #endregion

        #region Import utenti

        /// <param name="xlsFile">File excel con i dati degli utenti da importare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        [HttpPost]
        [Route(nameof(ImportaUtentiDaExcel))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ImportaUtentiDaExcel([FromForm] AgronicaCoreModelsSTD.Utility.FileWrapper xlsFile, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                if (xlsFile.file == null || xlsFile.file.Length == 0)
                    return BadRequest("File is missing.");

                var fileName = xlsFile.file.FileName;
                string base64File;
                using (var memoryStream = new MemoryStream())
                {
                    xlsFile.file.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    base64File = Convert.ToBase64String(fileBytes);
                }

                var request = getRequest(new FileWrapperAlt(fileName, base64File));
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImportaUtentiDaExcel(request);
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

        #endregion
    }
}
