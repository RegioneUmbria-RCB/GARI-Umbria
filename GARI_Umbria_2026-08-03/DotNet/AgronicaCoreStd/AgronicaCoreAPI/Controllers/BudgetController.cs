using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreDTOStd.InData.Budget;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using InData;
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

    public class BudgetController : BaseController
    {
        public BudgetController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpPost]
        [Route(nameof(Leggi_ParticelleCampo))]
        public ObjectResult Leggi_ParticelleCampo([FromBody] BudgetAnagrafica<Parametri_ObjParametriAgenda_NG_GestioneRichieste> leggi_ParticelleCampo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggi_ParticelleCampo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_ParticelleCampo_NG(request);
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
        [Route(nameof(ScriviCampiAnagrafica))]
        public ObjectResult ScriviCampiAnagrafica([FromBody] AgronicaCoreDTOStd.InData.Budget.ScriviCampiAnagrafica scriviCampiAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(scriviCampiAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviCampiAnagrafica_NG(request);
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
        [Route(nameof(Leggi_Esercizi_Anagrafica))]
        public ObjectResult Leggi_Esercizi_Anagrafica([FromBody] BudgetAnagrafica<Parametri_ObjParametriAgenda_NG> leggi_Esercizi_Anagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggi_Esercizi_Anagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Esercizi_Anagrafica_Bdg(request);
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
        [Route(nameof(Leggi_Appezzamento_Anagrafica))]
        public ObjectResult Leggi_Appezzamento_Anagrafica([FromBody] BudgetAnagrafica<LeggiAppezzamento> leggi_Appezzamento_Anagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> result = new RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento>();

            try
            {
                var request = getRequest((object)leggi_Appezzamento_Anagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Appezzamento_Anagrafica(request);
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
        [Route(nameof(Scrivi_Appezzamento_Anagrafica))]
        public ObjectResult Scrivi_Appezzamento_Anagrafica([FromBody] BudgetAnagrafica<Appezzamento> scrivi_Appezzamento_Anagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> result = new RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento>();

            try
            {
                var request = getRequest((object)scrivi_Appezzamento_Anagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Scrivi_Appezzamento_Anagrafica(request);
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
        [Route(nameof(Leggi_Max_DataModifica))]
        public ObjectResult Leggi_Max_DataModifica([FromBody] BudgetAnagrafica<string> leggi_Max_DataModifica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreDTOStd.InData.Data> result = new RispostaStandard<AgronicaCoreDTOStd.InData.Data>();

            try
            {
                var request = getRequest((object)leggi_Max_DataModifica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Max_DataModifica(request);
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
        [Route("Leggi_Budget_Attivo")]
        public ObjectResult Leggi_Budget_Attivo([FromBody] Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreDTOStd.InData.Budget.Budget_Testata> result = new RispostaStandard<AgronicaCoreDTOStd.InData.Budget.Budget_Testata>();         
            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Budget_Attivo(request);
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
        [Route("LeggiElencoBudgetTestata")]
        public ObjectResult LeggiElencoBudgetTestata([FromBody] object InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreDTOStd.InData.Budget.Budget_Testata>> result = new RispostaStandard<List<AgronicaCoreDTOStd.InData.Budget.Budget_Testata>>();         
            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<object> requestData = new CoreWS_Generic<object>(objP, InData);
                CoreWSRequest<CoreWS_Generic<object>> request = new CoreWSRequest<CoreWS_Generic<object>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoBudgetTestata(request);
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
        [Route(nameof(LeggiCampi))]
        public ObjectResult LeggiCampi([FromBody] BudgetAnagrafica<AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi> leggiCampi, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>>();
         
            try
            {
                var request = getRequest(leggiCampi);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCampiBudget(request);
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
        [Route(nameof(Leggi_Campi_Anagrafica))]
        public ObjectResult Leggi_Campi_Anagrafica([FromBody] BudgetAnagrafica<Parametri_ObjParametriAgenda_NG> leggiCampiAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest((object)leggiCampiAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Campi_Anagrafica(request);
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
        [Route(nameof(Leggi_Campo_Anagrafica))]
        public ObjectResult Leggi_Campo_Anagrafica([FromBody] BudgetAnagrafica<Parametri_ObjParametriAgenda_NG> leggi_Campo_Anagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Campo> result = new RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Campo>();

            try
            {
                var request = getRequest(leggi_Campo_Anagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Campo_Anagrafica(request);
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
        [Route(nameof(Leggi_AppezzamentiCampo))]
        public ObjectResult Leggi_AppezzamentiCampo([FromBody] BudgetAnagrafica<Parametri_ObjParametriAgenda_NG> leggi_AppezzamentiCampo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggi_AppezzamentiCampo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_AppezzamentiCampo_NG(request);
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
        [Route(nameof(LeggiInvestimentoCatastale))]
        public ObjectResult LeggiInvestimentoCatastale([FromBody] BudgetAnagrafica<AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale> impostaLeggiInvestimentoCatastale, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<String> result = new();

            try
            {
                var request = getRequest(impostaLeggiInvestimentoCatastale);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiInvestimentoCatastaleBdg(request);
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
        [Route(nameof(Leggi_Investimento_Catastale_Campo))]
        public ObjectResult Leggi_Investimento_Catastale_Campo([FromBody] BudgetAnagrafica<AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo> leggi_Investimento_Catastale_Campo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(leggi_Investimento_Catastale_Campo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiInvestimentoCatastaleCampoBdg(request);
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
        [Route("Ribalta_BudgetReale")]
        public ObjectResult Ribalta_BudgetReale([FromBody] List<BudgetAnagrafica<Appezzamento>> InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Ribalta_BudgetReale(request);
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
        [Route("CaricaDatiCatastali")]
        public ObjectResult CaricaDatiCatastali([FromBody] BudgetAnagrafica<CaricaDatiCatastali> caricaDatiCatastaliBdg, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(caricaDatiCatastaliBdg);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaDatiCatastali_NG(request);
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
