using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
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
    public class AgronicaCoreDPINGController : BaseController
    {
        public AgronicaCoreDPINGController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route("CaricaComboDisciplinareModello")]
        public ObjectResult CaricaComboDisciplinareModello([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari impostaLeggiDisciplinari, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>> result = new();

            try
            {
                var request = getRequest(impostaLeggiDisciplinari);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDisciplinari(request);
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
        [Route("CaricaComboDisciplinareEnte")]
        public ObjectResult CaricaComboDisciplinareEnte([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Disciplinare>> result = new RispostaStandard<List<Disciplinare>>();

            try
            {
                //CoreWSDisciplinari request = new CoreWSDisciplinari(objP_super_server, objP_server, objP_utenti, specie, flag);
                var request = getRequest(new LeggiDisciplinari() { specie = new Specie() { codice = 0 }, data = new DateTime(), privato = true });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDisciplinariEnte(request);
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
        [Route("CaricaComboIAFModello")]
        public ObjectResult CaricaComboIAFModello([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiIAF impostaLeggiIAF, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.ImpegniAggiuntiviFacoltativi>> result = new();

            try
            {
                var request = getRequest(impostaLeggiIAF);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboIAFModello(request);
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
        [Route("CalcoloNPKModello")]
        public ObjectResult CalcoloNPKModello([FromBody] AgronicaCoreDTOStd.InData.Metaschema.FiltroCalcoloNPK impostaLeggiIAF, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.metaschema.ApportoMacroelementi> result = new();

            try
            {
                var request = getRequest(impostaLeggiIAF);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CalcoloNPKModello(request);
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
        [Route("PCFinalitaRerWSModello")]
        public ObjectResult PCFinalitaRerWSModello([FromBody] AgronicaCoreDTOStd.InData.Metaschema.FiltroPC_Finalita_Rer impostaFiltroPC_Finalita_Rer, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.FinalitaPianoConcimazione>> result = new();

            try
            {
                var request = getRequest(impostaFiltroPC_Finalita_Rer);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PCFinalitaRerWSModello(request);
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
        [Route("Leggi_Disciplinari_Testata_conRegolamentoConcimazione")]
        public ObjectResult Leggi_Disciplinari_Testata_conRegolamentoConcimazione([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari leggiDisciplinari, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>> ();

            try
            {
                var request = getRequest((object)leggiDisciplinari);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Disciplinari_Testata_conRegolamentoConcimazione(request);
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
        [Route("Leggi_Disciplinari_Testata_DirettivaNitrati")]
        public ObjectResult Leggi_Disciplinari_Testata_DirettivaNitrati([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari leggiDisciplinari, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>>();

            try
            {
                var request = getRequest((object)leggiDisciplinari);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Disciplinari_Testata_DirettivaNitrati(request);
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
