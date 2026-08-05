using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreUtilityStd;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreDTOStd.InData.Visite;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class VisiteController : BaseController
    {
        public VisiteController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("MenuVisite/LeggiVisite")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiVisite([FromBody] AgronicaCoreDTOStd.InData.Visite.LeggiVisite request, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            string result = "";

            RispostaStandard<string> rispostaStandard = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_ListaVisiteDettagli(getRequest(request));
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

        [HttpGet]
        [Route("LeggiVisiteOperazioni")]
        [ProducesResponseType(typeof(RispostaStandard<List<Lavorazione>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiVisiteOperazioni([FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Lavorazione>> response = new RispostaStandard<List<Lavorazione>>();

            try
            {
                var request = getRequest((object)new { });
                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaVisiteOperazioni(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [Route("LeggiVisiteAttivita")]
        [ProducesResponseType(typeof(RispostaStandard<List<AttivitaPersonalizzata>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiVisiteAttivita([FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AttivitaPersonalizzata>> response = new RispostaStandard<List<AttivitaPersonalizzata>>();

            try
            {
                var request = getRequest((object)new { });
                response = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getListaVisiteAttivita(request);

                if (!response.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }
            catch (Exception ex)
            {
                response.RispostaOK = false;
                response.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [Route("LeggiUtenteTecnicoOCapo")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiUtenteTecnicoOCapo([FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            string result = "";

            RispostaStandard<string> rispostaStandard = new();

            try
            {
                var request = getRequest((object)new { });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUtenteTecnicoOCapo(request);
                rispostaStandard.RispostaStringa = result;
                rispostaStandard.RispostaOK = true;
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


        [HttpGet]
        [Route("LeggiListaTecnici")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiListaTecnici([FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            string result = "";

            RispostaStandard<string> rispostaStandard = new();

            try
            {
                var request = getRequest((object)new { });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiListaTecnici(request);
                rispostaStandard.RispostaStringa = result;
                rispostaStandard.RispostaOK = true;
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

        [HttpPost]
        [Route("LeggiListaAziende")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiListaAziende([FromBody] AgronicaCoreDTOStd.InData.Visite.LeggiAziende request, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            string result = "";

            RispostaStandard<string> rispostaStandard = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiListaAziende(getRequest(request));
                rispostaStandard.RispostaStringa = result;
                rispostaStandard.RispostaOK = true;
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

        [HttpPost]
        [Route("LeggiListaAgenzie")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiListaAgenzie([FromBody] AgronicaCoreDTOStd.InData.Visite.LeggiAziende request, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            string result = "";

            RispostaStandard<string> rispostaStandard = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiListaAgenzie(getRequest(request));
                rispostaStandard.RispostaStringa = result;
                rispostaStandard.RispostaOK = true;
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

        [HttpGet]
        [Route("LeggiRisorseZootecniche")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiRisorseZootecniche([FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica>>();

            try
            {
                var request = getRequest((object)new { });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiRisorseZootecniche(request);
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
        [Route("EliminaRilieviVisite")]
        [ProducesResponseType(typeof(RispostaStandard<decimal>), StatusCodes.Status200OK)]
        public ObjectResult EliminaRilieviVisite([FromBody] Elimina_Rilievi_Visita InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).EliminaRilieviVisite(request);
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
