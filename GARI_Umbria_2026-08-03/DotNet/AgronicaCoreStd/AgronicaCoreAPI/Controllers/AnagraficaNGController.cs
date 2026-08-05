using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using InData;
using InData.Anagrafica;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using InData.DatiPrevisionaliColture;
using InData.Agenda;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class AnagraficaNGController : BaseController
    {
        public AnagraficaNGController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        #region Imprese
        [HttpPost]
        [Route("Imprese")]
        public ObjectResult GetImprese([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseNG(request);
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
        [Route(nameof(GetImpreseMaxDataModifica))]
        public ObjectResult GetImpreseMaxDataModifica([FromBody] String InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreDTOStd.InData.Data> result = new RispostaStandard<AgronicaCoreDTOStd.InData.Data>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<String> requestData = new CoreWS_Generic<String>(objP, InData);
                CoreWSRequest<CoreWS_Generic<String>> request = new CoreWSRequest<CoreWS_Generic<String>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseMaxDataModifica(request);
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
        [Route("LeggiImpresa")]
        [ProducesResponseType(typeof(RispostaStandard<Impresa>), StatusCodes.Status200OK)]
        public ObjectResult GetImpresa([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Impresa> result = new RispostaStandard<Impresa>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpresaNG(request);
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
        [Route("ScriviImpresa")]
        public ObjectResult ScriviImpresa([FromBody] Impresa InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            // validazione input partitaIvaReale - max 25 caratteri alfanumerici
            if (!string.IsNullOrEmpty(InData?.partitaIvaReale) &&
                !System.Text.RegularExpressions.Regex.IsMatch(InData.partitaIvaReale, @"^[a-zA-Z0-9]{1,25}$", RegexOptions.None, TimeSpan.FromSeconds(3)))
            {
                RispostaStandard<Impresa> validationResult = new RispostaStandard<Impresa>();
                validationResult.RispostaOK = false;
                validationResult.Errore = "partitaIvaReale non valida: deve contenere solo caratteri alfanumerici e avere lunghezza massima di 25 caratteri.";
                return StatusCode(StatusCodes.Status500InternalServerError, validationResult);
            }

            RispostaStandard<Impresa> result = new RispostaStandard<Impresa>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Impresa> requestData = new CoreWS_Generic<Impresa>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Impresa>> request = new CoreWSRequest<CoreWS_Generic<Impresa>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviImpresaNG(request);
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
        [Route("ControlloPresenzaPiva")]
        public ObjectResult ControlloPresenzaPiva([FromBody] string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ControlloPresenzaPiva(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("ImpresaBiologica")]
        public ObjectResult ImpresaBiologica([FromBody] AgronicaCoreModelsSTD.anagrafiche.Impresa impostaImpresa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new();

            try
            {
                var request = getRequest(impostaImpresa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImpresaBiologica(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpGet]
        [Route("LeggiImpreseCodici")]
        public ObjectResult LeggiImpreseCodici([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseCodici(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("LeggiImpreseFiltro")]
        public ObjectResult LeggiImpreseFiltro([FromBody] AgronicaCoreDTOStd.InData.LeggiFiltro impostaLeggiFiltro, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> result = new();

            try
            {
                var request = getRequest(impostaLeggiFiltro);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseFiltro(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("LeggiImpresePadri")]
        public ObjectResult LeggiImpresePadri([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.Leggi_Padri impostaLeggiImpresePadri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> result = new();

            try
            {
                var request = getRequest(impostaLeggiImpresePadri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpresePadri(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("LeggiImpresePadriBaseCodeDescr")]
        public ObjectResult LeggiImpresePadriBaseCodeDescr([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<BaseCodeDescrStr>> result = new();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Leggi_Padri> requestData = new CoreWS_Generic<Leggi_Padri>(objP, new Leggi_Padri());
                CoreWSRequest<CoreWS_Generic<Leggi_Padri>> request = new CoreWSRequest<CoreWS_Generic<Leggi_Padri>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpresePadriBaseCodeDescr(request);

                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("LeggiImpresaPadreSementieri")]
        public ObjectResult LeggiImpresaPadreSementieri([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.Leggi_Padri impostaLeggiImpresaPadreSementieri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Impresa> result = new();

            try
            {
                var request = getRequest(impostaLeggiImpresaPadreSementieri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpresaPadreSementieri(request);

                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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

        #region Gruppi Raccolta
        [HttpPost]
        [Route("LeggiGruppiRaccoltaValidi")]
        public ObjectResult LeggiGruppiRaccoltaValidi([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<GruppoRaccolta>> result = new RispostaStandard<List<GruppoRaccolta>>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGruppiRaccoltaValidi(request);
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
        [Route("LeggiGruppiRaccolta")]
        public ObjectResult LeggiGruppiRaccolta([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<GruppoRaccolta>> result = new RispostaStandard<List<GruppoRaccolta>>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGruppiRaccolta(request);
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
        [Route("LeggiGruppoRaccoltaImpresa")]
        public ObjectResult LeggiGruppoRaccoltaImpresa([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<GruppoRaccolta> result = new RispostaStandard<GruppoRaccolta>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGruppoRaccoltaImpresa(request);
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
        [Route("LeggiDisciplinari")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiDisciplinari([FromBody] LeggiDisciplinariCombo InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDisciplinariCombo(request);
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
        [Route("ScriviModificaCancella_GruppoRaccolta")]
        public ObjectResult ScriviModificaCancella_GruppoRaccolta([FromBody] ScriviGruppoRaccolta InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<GruppoRaccolta> result = new RispostaStandard<GruppoRaccolta>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<ScriviGruppoRaccolta> requestData = new CoreWS_Generic<ScriviGruppoRaccolta>(objP, InData);
                CoreWSRequest<CoreWS_Generic<ScriviGruppoRaccolta>> request = new CoreWSRequest<CoreWS_Generic<ScriviGruppoRaccolta>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviModificaCancella_GruppoRaccolta(request);
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

        #region Centri
        [HttpPost]
        [Route("Centri")]
        public ObjectResult GetCentri([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCentriNG(request);
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
        [Route("readCentreAddresses")]
        public ObjectResult readCentreAddresses([FromBody] LeggiIndirizziCentro InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<IndirizzoAssociato>> result = new RispostaStandard<List<IndirizzoAssociato>>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LeggiIndirizziCentro> requestData = new CoreWS_Generic<LeggiIndirizziCentro>(objP, InData);
                CoreWSRequest<CoreWS_Generic<LeggiIndirizziCentro>> request = new CoreWSRequest<CoreWS_Generic<LeggiIndirizziCentro>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readCentreAddresses(request);
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
        [Route("LeggiCentro")]
        public ObjectResult GetCentro([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CentroAziendale> result = new RispostaStandard<CentroAziendale>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCentroNG(request);
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
        [Route("ScriviCentro")]
        public ObjectResult ScriviCentro([FromBody] CentroAziendale InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CentroAziendale> result = new RispostaStandard<CentroAziendale>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<CentroAziendale> requestData = new CoreWS_Generic<CentroAziendale>(objP, InData);
                CoreWSRequest<CoreWS_Generic<CentroAziendale>> request = new CoreWSRequest<CoreWS_Generic<CentroAziendale>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviCentroNG(request);
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
        [Route("LeggiCentriAziendaliModello")]
        public ObjectResult LeggiCentriAziendaliModello([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali impostaLeggiCentriAziendali, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CentroAziendale>> result = new();

            try
            {
                var request = getRequest(impostaLeggiCentriAziendali);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCentriAziendaliModello(request);
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
        [Route("CaricaCentriImpresa")]
        public ObjectResult CaricaCentriImpresa([FromBody] AgronicaCoreDTOStd.InData.Metaschema.CaricaCentriImpresa impostaCaricaCentriImpresa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Object> result = new();

            try
            {
                var request = getRequest(impostaCaricaCentriImpresa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaCentriImpresa(request);
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

        #region Contatti
        [HttpPost]
        [Route("EliminaContatto")]
        public ObjectResult EliminaContatto([FromBody] Contatto InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).EliminaContatto(request);
                // if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("CaricaComboCmbOrganismoReferenteModello")]
        public ObjectResult CaricaComboCmbOrganismoReferenteModello([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa impostaFiltroImpresa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>> result = new();

            try
            {
                var request = getRequest(impostaFiltroImpresa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmbOrganismoReferenteModello(request);
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
        [Route("CaricaComboCmbRiferimentoTrasferimentoDati_Modello")]
        public ObjectResult CaricaComboCmbRiferimentoTrasferimentoDati_Modello([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa impostaFiltroImpresa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>> result = new();

            try
            {
                var request = getRequest(impostaFiltroImpresa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmbRiferimentoTrasferimentoDati_Modello(request);
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
        [Route("CaricaComboCmbMagazzinoConferimentoModello")]
        public ObjectResult CaricaComboCmbMagazzinoConferimentoModello([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa impostaFiltroImpresa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Fabbricato>> result = new();

            try
            {
                var request = getRequest(impostaFiltroImpresa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmbMagazzinoConferimentoModello(request);
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
        [Route("CaricaComboCmbTecnici")]
        public ObjectResult CaricaComboCmbTecnici([FromBody] AgronicaCoreDTOStd.InData.LeggiFiltro impostaPiva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaPiva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmbTecnici(request);
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
        [Route("CaricaComboCmbTecniciModello")]
        public ObjectResult CaricaComboCmbTecniciModello([FromBody] AgronicaCoreDTOStd.InData.LeggiFiltro impostaPiva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>> result = new();

            try
            {
                var request = getRequest(impostaPiva.Ricerca);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmbTecniciModello(request);
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

        [HttpGet]
        [Route("CaricaComboCmbOrganismiODC")]
        public ObjectResult CaricaComboCmbOrganismiODC([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmbOrganismiODC(request);
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
        [Route("CaricaComboCmbOrganismiODCModello")]
        public ObjectResult CaricaComboCmbOrganismiODCModello([FromBody] AgronicaCoreDTOStd.InData.LeggiFiltro impostaPiva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.RisorseUmane>> result = new();

            try
            {
                var request = getRequest(impostaPiva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmbOrganismiODCModello(request);
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
        [Route("LeggiContattiAnagrafica1")]
        public ObjectResult LeggiContattiAnagrafica1([FromBody] InData.Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiContattiAnagrafica1(request);
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
        [Route("LeggiContattiAnagrafica")]
        public ObjectResult LeggiContattiAnagrafica([FromBody] InData.Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Contatti_Anagrafica_NG(request);
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
        [Route("LeggiContattiMacchina")]
        public ObjectResult LeggiContattiMacchina([FromBody] LeggiContattiMacchina InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Contatto>> result = new RispostaStandard<List<Contatto>>();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiContattiMacchina(request);
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
        [Route("LeggiContattiStazioniMeteo")]
        public ObjectResult LeggiContattiStazioniMeteo([FromBody] string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Contatto>> result = new RispostaStandard<List<Contatto>>();

            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiContattiStazioniMeteo(request);
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
        [Route("Test_Campi_Archivio_Lettura")]
        public ObjectResult Test_Campi_Archivio_Lettura([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Campi_Archivio_Lettura(request);
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

        #region Macchine
        /// <summary>
        /// Prepara oggetto di configurazione albero
        /// </summary>
        /// <param name="impostaLeggiMacchinaAnagrafica">Parametri necessari per configurazione albero</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto configurazione albero</returns>
        /// <remarks>
        /// 
        ///     POST /ConfiguraAlbero
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiMacchinaAnagrafica")]
        [ProducesResponseType(typeof(RispostaStandard<ParcoMacchine>), StatusCodes.Status200OK)]
        public ObjectResult LeggiMacchinaAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaLeggiMacchinaAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ParcoMacchine> result = new();

            try
            {
                var request = getRequest(impostaLeggiMacchinaAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMacchinaAnagrafica(request);
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
        /// Prepara oggetto di configurazione albero
        /// </summary>
        /// <param name="impostaLeggiMacchineAnagrafica">Parametri necessari per configurazione albero</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto configurazione albero</returns>
        /// <remarks>
        /// 
        ///     POST /ConfiguraAlbero
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiMacchineAnagrafica")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiMacchineAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaLeggiMacchineAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaLeggiMacchineAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMacchineAnagrafica(request);
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
        /// Restituisce l'elenco delle macchine per irrigazione associate alla partita IVA specificata.
        /// </summary>
        /// <param name="leggiMacchineIrrigazione">
        /// Oggetto <c>Leggi_Macchine_Irrigazione</c> contenente la proprietà <c>piva</c> (partita IVA dell'azienda).
        /// </param>
        /// <param name="Authorization">
        /// Token di autenticazione passato nell'header della richiesta.
        /// </param>
        /// <returns>
        /// Oggetto <c>RispostaStandard</c> con l'elenco delle macchine per irrigazione.<br/>
        /// <b>Status 200</b>: Operazione riuscita.<br/>
        /// <b>Status 401</b>: Non autorizzato.<br/>
        /// <b>Status 500</b>: Errore interno.
        /// </returns>
        [HttpPost]
        [Route("LeggiMacchineIrrigazione")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiMacchineIrrigazione([FromBody] Leggi_Macchine_Irrigazione leggiMacchineIrrigazione, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var parametri = new Leggi_Macchine_Per_Tipo { piva = leggiMacchineIrrigazione.piva, classCode = "05" };
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Macchine_Per_Tipo_NG(request);
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
        /// Restituisce l'elenco delle macchine per irrigazione associate all'impianto.
        /// </summary>
        /// <param name="parametri">
        /// Oggetto <c>Leggi_Macchine_Irrigazione_Impianto</c> contenente le proprietà dell'impianto.
        /// </param>
        /// <param name="Authorization">
        /// Token di autenticazione passato nell'header della richiesta.
        /// </param>
        /// <returns>
        /// Oggetto <c>RispostaStandard</c> con l'elenco delle macchine per irrigazione.impianto<br/>
        /// <b>Status 200</b>: Operazione riuscita.<br/>
        /// <b>Status 401</b>: Non autorizzato.<br/>
        /// <b>Status 500</b>: Errore interno.
        /// </returns>
        [HttpPost]
        [Route("LeggiMacchineIrrigazioneImpianto")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiMacchineIrrigazioneImpianto([FromBody] Leggi_Macchine_Irrigazione_Impianto parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Macchine_Irrigazione_Impianto_NG(request);
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

        [HttpGet]
        [Route(nameof(LeggiImpiantoIrrigazione))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiImpiantoIrrigazione(int impCod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();
            try
            {
                var request = getRequest(impCod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpiantoIrrigazione(request);
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
        [Route(nameof(LeggiMacchinaIrrigazioneDefault))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiMacchinaIrrigazioneDefault([FromBody]Leggi_Macchine_Irrigazione_Impianto inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();
            try
            {
                var request = getRequest(inData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMacchinaIrrigazioneDefault(request);
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
        [Route(nameof(ReadMachinesByClassCode))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ReadMachinesByClassCode([FromBody] InData.Anagrafica.MachinesXTypeReadParams readParams, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(readParams);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ReadMachinesByClassCode(request);
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
        [Route("ScriviMacchinaAnagrafica")]
        public ObjectResult ScriviMacchinaAnagrafica([FromBody] Scrivi_Macchina_Anagrafica impostaScriviMacchina, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaScriviMacchina);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviMacchina(request);
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

        #region Movimenti Macchina

        [HttpPost]
        [Route("IsMacchinaMovimentata")]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult IsMacchinaMovimentata([FromBody] ParcoMacchine macchina, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(macchina);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).IsMacchinaMovimentata(request);
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
        [Route("IsEditAllowed")]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult IsEditAllowed([FromBody] ParcoMacchine macchina, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(macchina);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).IsEditAllowed(request);
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
        [Route("CentresOnWhichIsUsed")]
        [ProducesResponseType(typeof(RispostaStandard<List<int>>), StatusCodes.Status200OK)]
        public ObjectResult CentresOnWhichIsUsed([FromBody] ParcoMacchine macchina, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<int>> result = new();

            try
            {
                var request = getRequest(macchina);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CentresOnWhichIsUsed(request);
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
        [Route("CompaniesByWichIsUsed")]
        [ProducesResponseType(typeof(RispostaStandard<List<string>>), StatusCodes.Status200OK)]
        public ObjectResult CompaniesByWichIsUsed([FromBody] ParcoMacchine macchina, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<string>> result = new();

            try
            {
                var request = getRequest(macchina);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CompaniesByWichIsUsed(request);
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
        #endregion

        #region Catasto
        [HttpPost]
        [Route("LeggiCatastoAnagrafica")]
        public ObjectResult LeggiCatastoAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaLeggiCatastoAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaLeggiCatastoAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCatastoAnagrafica(request);
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
        [Route("LeggiParticellaAnagrafica")]
        public ObjectResult LeggiParticellaAnagrafica([FromBody] CatastoCentroAziendale impostaLeggiParticellaAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CatastoCentroAziendale> result = new();

            try
            {
                var request = getRequest(impostaLeggiParticellaAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCatastoAnagrafica(request);
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
        [Route("ScriviParticellaAnagrafica")]
        public ObjectResult ScriviParticellaAnagrafica([FromBody] ScriviCatasto impostaScriviParticellaAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaScriviParticellaAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviCatasto(request);
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
        [Route("CheckPossessi")]
        public ObjectResult CheckPossessi([FromBody] CatastoCentroAziendale impostaCheckPossessi, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaCheckPossessi);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CheckPossessi(request);
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
        [Route("LeggiInvestimentoCatastale")]
        public ObjectResult LeggiInvestimentoCatastale([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale impostaLeggiInvestimentoCatastale, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<String> result = new();

            try
            {
                var request = getRequest(impostaLeggiInvestimentoCatastale);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiInvestimentoCatastale(request);
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
        [Route("LeggiInvestimentoCatastaleCampo")]
        public ObjectResult LeggiInvestimentoCatastaleCampo([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo impostaLeggiInvestimentoCatastaleCampo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<String> result = new();

            try
            {
                var request = getRequest(impostaLeggiInvestimentoCatastaleCampo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiInvestimentoCatastaleCampo(request);
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

        #region Campi
        [HttpPost]
        [Route("LeggiCampi")]
        public ObjectResult LeggiCampi([FromBody] LeggiCampi impostaLeggiCampi, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>> result = new();

            try
            {
                var request = getRequest(impostaLeggiCampi);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCampi(request);
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
        [Route("LeggiCampiAnagrafica")]
        public ObjectResult LeggiCampiAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaLeggiCampiAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaLeggiCampiAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCampiAnagrafica(request);
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
        [Route("LeggiCampoAnagrafica")]
        public ObjectResult LeggiCampoAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaLeggiCampoAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Campo> result = new();

            try
            {
                var request = getRequest(impostaLeggiCampoAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCampoAnagrafica(request);
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
        [Route("LeggiAppezzamentiCampo")]
        public ObjectResult LeggiAppezzamentiCampo([FromBody] Parametri_ObjParametriAgenda_NG impostaLeggiAppezzamentiCampo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaLeggiAppezzamentiCampo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAppezzamentiCampo(request);
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
        [Route("ScriviCampiAnagrafica")]
        public ObjectResult ScriviCampiAnagrafica([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.ScriviCampiAnagrafica impostaScriviCampiAnagrafica, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaScriviCampiAnagrafica);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviCampiAnagrafica(request);
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
        [Route("ScriviCampiAnagraficaInLine")]
        public ObjectResult ScriviCampiAnagraficaInLine([FromBody] ScriviCampiAnagraficaInLine impostaScriviCampiAnagraficaInLine, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaScriviCampiAnagraficaInLine);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviCampiAnagraficaInLine(request);
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
        [Route("CaricaGridImpianti")]
        public ObjectResult CaricaGridImpianti([FromBody] LeggiImpianto impostaLeggiImpianto, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaLeggiImpianto);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaGridImpianti(request);
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
        [Route("LeggiParticellePerCentro")]
        public ObjectResult LeggiParticellePerCentro([FromBody] InData.Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiParticellePerCentro(request);
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
        [Route("LeggiCampi_perSpecieImpianti")]
        public ObjectResult LeggiCampi_perSpecieImpianti([FromBody] object LeggiCampi, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>> result = new();

            try
            {
                var request = getRequest(LeggiCampi);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCampi_perSpecieImpianti(request);
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
        [Route("GetCampi")]
        public ObjectResult GetCampi([FromBody] GetCampi GetCampi, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(GetCampi);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetCampi_NG(request);
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

        [HttpGet]
        [Route("Leggi_Campi_Codici")]
        public ObjectResult Leggi_Campi_Codici([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Campi_Codici(request);
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
        [Route("Test_Campi_Archivio_Scrittura")]
        public ObjectResult Test_Campi_Archivio_Scrittura([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Campi_Archivio_Scrittura(request);
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

        #region Appezzamenti/Impianti/Esercizi
        [HttpPost]
        [Route("LeggiSpecieVegetaliAttiveImpianti")]
        public ObjectResult LeggiSpecieVegetaliAttiveImpianti([FromBody] LeggiImpianto impostaLeggiImpianto, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno>> result = new();


            try
            {
                var request = getRequest(impostaLeggiImpianto);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSpecieVegetaliAttiveImpianti(request);
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
        [Route("LeggiAppezzamentiAnagrafica")]
        public ObjectResult LeggiAppezzamentiAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAppezzamentiAnagrafica(request);
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
        [Route("LeggiAppezzamentoGlobalAnagrafica")]
        public ObjectResult LeggiAppezzamentoGlobalAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAppezzamentoGlobalAnagrafica(request);
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
        [Route("LeggiAppezzamentoAnagrafica")]
        public ObjectResult LeggiAppezzamentoAnagrafica([FromBody] LeggiAppezzamento impostaLeggiAppezzamento, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> result = new();

            try
            {
                var request = getRequest(impostaLeggiAppezzamento);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAppezzamentoAnagrafica(request);
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
        [Route("LeggiAppezzamentoUtilizzoTerreno")]
        public ObjectResult LeggiAppezzamentoUtilizzoTerreno([FromBody] AgronicaCoreModelsSTD.anagrafiche.Impianto.PK impostaPK, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno> result = new();

            try
            {
                var request = getRequest(impostaPK);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAppezzamentoUtilizzoTerreno(request);
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
        public ObjectResult CaricaDatiCatastali([FromBody] CaricaDatiCatastali impostaCaricaDatiCatastali, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaCaricaDatiCatastali);
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

        [HttpPost]
        [Route("LeggiParticellePerAppezzamento")]
        public ObjectResult LeggiParticellePerAppezzamento([FromBody] Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiParticellePerAppezzamento(request);
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
        [Route("ControllaValiditaAppezzamenti")]
        public ObjectResult ControllaValiditaAppezzamenti([FromBody] Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ControllaValiditaAppezzamenti(request);
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
        /// Esegue le operazioni di scrittura, modifica e cancellazione di un appezzamento
        /// </summary>
        /// <param name="scriviAppezzamentoAnagrafica_In"> Oggetto che descrive l'appezzamento su cui effettuare l'operazione
        ///                                                e gli eventuali parametri dello sportello sementieri</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>L'oggetto Appezzamento risultante</returns>
        /// <remarks>
        /// 
        ///     POST /ScriviAppezzamentoAnagrafica
        /// 
        /// </remarks>
        /// <response code="200">Appezzamento risultante</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ScriviAppezzamentoAnagrafica")]
        [ProducesResponseType(typeof(RispostaStandard<Appezzamento>), StatusCodes.Status200OK)]

        public ObjectResult ScriviAppezzamentoAnagrafica([FromBody] Appezzamento scriviAppezzamentoAnagrafica_In, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> result = new();

            try
            {
                var request = getRequest(scriviAppezzamentoAnagrafica_In);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviAppezzamentoAnagrafica(request);
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
        /// Esegue le operazioni di scrittura, modifica e cancellazione di un appezzamento
        /// </summary>
        /// <param name="appezzamentoFiltroTemporale_In"> Oggetto che descrive l'appezzamento su cui effettuare l'operazione
        ///                                                e gli eventuali parametri dello sportello sementieri,
        ///                                                comprende inoltre il filtro temporale per limitare la rilettura
        ///                                                dopo la scrittura</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>L'oggetto Appezzamento risultante</returns>
        /// <remarks>
        /// 
        ///     POST /ScriviAppezzamentoAnagrafica
        /// 
        /// </remarks>
        /// <response code="200">Appezzamento risultante</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ScriviAppezzamentoAnagraficaFiltroTemporale")]
        [ProducesResponseType(typeof(RispostaStandard<Appezzamento>), StatusCodes.Status200OK)]
        public ObjectResult ScriviAppezzamentoAnagraficaFiltroTemporale([FromBody] AppezzamentoFiltroTemporale appezzamentoFiltroTemporale_In, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> result = new();

            try
            {
                var request = getRequest(appezzamentoFiltroTemporale_In);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviAppezzamentoAnagraficaFiltroTemporale(request);
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
        [Route("BloccaSbloccaAppezzamenti")]
        public ObjectResult BloccaSbloccaAppezzamenti([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.BloccaSbloccaAppezzamenti impostaBloccaSbloccaAppezzamenti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(impostaBloccaSbloccaAppezzamenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).BloccaSbloccaAppezzamenti(request);
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
        [Route("VerificaSuperficie")]
        public ObjectResult VerificaSuperficie([FromBody] AgronicaCoreModelsSTD.anagrafiche.Impianto impostaImpianto, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaImpianto);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaSuperficie(request);
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
        [Route("VerificaOperazioniAgenda")]
        public ObjectResult VerificaOperazioniAgenda([FromBody] AgronicaCoreModelsSTD.anagrafiche.Impianto impostaImpianto, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreDTOStd.InData.Agenda.PresenzaMovimentazioni> result = new();

            try
            {
                var request = getRequest(impostaImpianto);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaOperazioniAgenda(request);
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
        [Route("LeggiMaxDataModifica")]
        public ObjectResult LeggiMaxDataModifica([FromBody] AgronicaCoreDTOStd.InData.LeggiFiltro impostaImpianto, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreDTOStd.InData.Data> result = new();

            try
            {
                var request = getRequest(impostaImpianto);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMaxDataModifica(request);
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
        [Route("LeggiEserciziAnagrafica")]
        public ObjectResult LeggiEserciziAnagrafica([FromBody] Parametri_ObjParametriAgenda_NG impostaParametri_ObjParametriAgenda_NG, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaParametri_ObjParametriAgenda_NG);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Esercizi_Anagrafica(request);
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

        [HttpGet]
        [Route("LeggiDropDownAppezzamentiCodici")]
        public ObjectResult LeggiDropDownAppezzamentiCodici([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe>> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDropDownAppezzamentiCodici(request);
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

        [HttpGet]
        [Route("LeggiDropDownEserciziCodici")]
        public ObjectResult LeggiDropDownEserciziCodici([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe>> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDropDownEserciziCodici(request);
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
        [Route("CaricaImpiantiEsistenti_GIS")]
        public ObjectResult CaricaImpiantiEsistenti_GIS([FromBody] CaricaImpiantiEsistenti_GIS caricaImpiantiEsistenti_GIS, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            //coreWSBaseURL:coreWSBaseURL
            try
            {
                var request = getRequest(caricaImpiantiEsistenti_GIS);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaImpiantiEsistenti_GIS_NG(request);
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
        [Route("CaricaCombo_SpecieColtivate")]
        public ObjectResult CaricaCombo_SpecieColtivate([FromBody] CaricaCombo_SpecieColtivate caricaCombo_SpecieColtivate, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(caricaCombo_SpecieColtivate);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaCombo_SpecieColtivate_NG(request);
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
        [Route("GetImpresexAppezza_Movimentati")]
        public ObjectResult GetImpresexAppezza_Movimentati([FromBody] GetImpresexAppezza_Movimentati getImpresexAppezza_Movimentati, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(getImpresexAppezza_Movimentati);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetImpresexAppezza_Movimentati_NG(request);
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
        [Route("CopiaSpostaAppezzamenti")]
        public ObjectResult CopiaSpostaAppezzamenti([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.CopiaSpostaAppezzamenti InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CopiaSpostaAppezzamenti(request);
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
        [Route("Leggi_Impianti_Anagrafica")]
        public ObjectResult Leggi_Impianti_Anagrafica([FromBody] InData.Parametri_ObjParametriAgenda_NG InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, InData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Impianti_Anagrafica_NG(request);
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
        [Route(nameof(ReadAgriculturalPlotLight))]
        [ProducesResponseType(typeof(RispostaStandard<AppezzamentoJoinDescrizioni>), StatusCodes.Status200OK)]
        public ObjectResult ReadAgriculturalPlotLight([FromBody] InData.Parametri_ObjParametriAgenda_NG inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AppezzamentoJoinDescrizioni> result = new RispostaStandard<AppezzamentoJoinDescrizioni>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG> requestData = new CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>(objP, inData);
                CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request = new CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ReadAgriculturalPlotLight(request);
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

        #region AppezzamentiXParcoMacchine
        [HttpPost]
        [Route(nameof(ReadAppezzamentiXParcoMacchine))]
        [ProducesResponseType(typeof(RispostaStandard<LinkedMachine<Appezzamento.PK>[]>), StatusCodes.Status200OK)]
        public ObjectResult ReadAppezzamentiXParcoMacchine([FromBody] InData.Anagrafica.AppezzamentoXParcoMacchine inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LinkedMachine<Appezzamento.PK>[]> result = new RispostaStandard<LinkedMachine<Appezzamento.PK>[]>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<InData.Anagrafica.AppezzamentoXParcoMacchine> requestData = new CoreWS_Generic<InData.Anagrafica.AppezzamentoXParcoMacchine>(objP, inData);
                CoreWSRequest<CoreWS_Generic<InData.Anagrafica.AppezzamentoXParcoMacchine>> request = new CoreWSRequest<CoreWS_Generic<InData.Anagrafica.AppezzamentoXParcoMacchine>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ReadAppezzamentiXParcoMacchine(request);
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
        [Route(nameof(WriteAppezzamentiXParcoMacchine))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult WriteAppezzamentiXParcoMacchine([FromBody] Appezzamento[] inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Appezzamento[]> requestData = new CoreWS_Generic<Appezzamento[]>(objP, inData);
                CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request = new CoreWSRequest<CoreWS_Generic<Appezzamento[]>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).WriteAppezzamentiXParcoMacchine(request);
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
        [Route(nameof(EditAppezzamentiXParcoMacchine))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult EditAppezzamentiXParcoMacchine([FromBody] LinkedMachine<Appezzamento.PK> inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<LinkedMachine<Appezzamento.PK>> requestData = new CoreWS_Generic<LinkedMachine<Appezzamento.PK>>(objP, inData);
                CoreWSRequest<CoreWS_Generic<LinkedMachine<Appezzamento.PK>>> request = new CoreWSRequest<CoreWS_Generic<LinkedMachine<Appezzamento.PK>>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).EditAppezzamentiXParcoMacchine(request);
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
        [Route(nameof(DeleteAppezzamentiXParcoMacchine))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult DeleteAppezzamentiXParcoMacchine([FromBody] AppezzamentoXParcoMacchine inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<AppezzamentoXParcoMacchine> requestData = new CoreWS_Generic<AppezzamentoXParcoMacchine>(objP, inData);
                CoreWSRequest<CoreWS_Generic<AppezzamentoXParcoMacchine>> request = new CoreWSRequest<CoreWS_Generic<AppezzamentoXParcoMacchine>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).DeleteAppezzamentiXParcoMacchine(request);
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
        [Route(nameof(DeleteAppezzamentiXParcoMacchineRecords))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult DeleteAppezzamentiXParcoMacchineRecords([FromBody] List<Appezzamento> inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<List<Appezzamento>> requestData = new CoreWS_Generic<List<Appezzamento>>(objP, inData);
                CoreWSRequest<CoreWS_Generic<List<Appezzamento>>> request = new CoreWSRequest<CoreWS_Generic<List<Appezzamento>>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).DeleteAppezzamentiXParcoMacchineRecords(request);
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

        #region PlotWeaving
        [HttpPost]
        [Route(nameof(UpdatePlotsWeaving))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult UpdatePlotsWeaving([FromBody] Appezzamento[] inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Appezzamento[]> requestData = new CoreWS_Generic<Appezzamento[]>(objP, inData);
                CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request = new CoreWSRequest<CoreWS_Generic<Appezzamento[]>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).UpdatePlotsWeaving(request);
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
        
        #region PlotSlope
        [HttpPost]
        [Route(nameof(UpdatePlotsSlope))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult UpdatePlotsSlope([FromBody] Appezzamento[] inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Appezzamento[]> requestData = new CoreWS_Generic<Appezzamento[]>(objP, inData);
                CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request = new CoreWSRequest<CoreWS_Generic<Appezzamento[]>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).UpdatePlotsSlope(request);
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
        
        #region PlotConstrain
        [HttpPost]
        [Route(nameof(UpdatePlotsConstrain))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult UpdatePlotsConstrain([FromBody] Appezzamento[] inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Appezzamento[]> requestData = new CoreWS_Generic<Appezzamento[]>(objP, inData);
                CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request = new CoreWSRequest<CoreWS_Generic<Appezzamento[]>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).UpdatePlotsConstrain(request);
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

        #region ProjectsXContributes
        [HttpPost]
        [Route(nameof(WriteProjectsXContributesRecords))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult WriteProjectsXContributesRecords([FromBody] List<Esercizio> inData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new RispostaStandard<bool>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<List<Esercizio>> requestData = new CoreWS_Generic<List<Esercizio>>(objP, inData);
                CoreWSRequest<CoreWS_Generic<List<Esercizio>>> request = new CoreWSRequest<CoreWS_Generic<List<Esercizio>>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).WriteProjectsXContributesRecords(request);
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

        #endregion

        #region Fabbricati
        [HttpPost]
        [Route("LeggiMagazziniQdC")]
        public ObjectResult LeggiMagazziniQdC([FromBody] InData.Anagrafica.LeggiMagazzini_QdC impostaLeggiMagazzini_QdC, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Fabbricato>> result = new();

            try
            {
                var request = getRequest(impostaLeggiMagazzini_QdC);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMagazziniQdC(request);
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
        [Route("LeggiFabbricatiOmni")]
        public ObjectResult LeggiFabbricatiOmni([FromBody] AgronicaCoreDTOStd.InData.Anagrafica.LeggiFabbricatiOmni impostaLeggiFabbricatiOmni, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(impostaLeggiFabbricatiOmni);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiFabbricatiOmni(request);
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

        #region Prodotti
        [HttpPost]
        [Route("CreaProdotti")]
        public ObjectResult CreaProdotti([FromBody] CreaProdotti creaProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(creaProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CreaProdotti(request);
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
        [Route("LeggiFertilizzantiQdC")]
        public ObjectResult LeggiFertilizzantiQdC([FromBody] InData.Anagrafica.LeggiProdotti impostaLeggiProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione>> result = new();

            try
            {
                var request = getRequest(impostaLeggiProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiFertilizzantiQdC(request);
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
        [Route("LeggiFormulatiQdC")]
        public ObjectResult LeggiFormulatiQdC([FromBody] Object impostaLeggiProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Object> result = new();

            try
            {
                var request = getRequest(impostaLeggiProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiFormulatiQdC(request);
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
        [Route("LeggiProdottiQdC")]
        public ObjectResult LeggiProdottiQdC([FromBody] InData.Anagrafica.LeggiProdotti impostaLeggiProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto>> result = new();

            try
            {
                var request = getRequest(impostaLeggiProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProdottiQdC(request);
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
        [Route("LeggiSementiQdC")]
        public ObjectResult LeggiSementiQdC([FromBody] InData.Anagrafica.LeggiProdotti impostaLeggiProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina>> result = new();

            try
            {
                var request = getRequest(impostaLeggiProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSementiQdC(request);
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
        [Route("LeggiTrasformatiVegetaliQdC")]
        public ObjectResult LeggiTrasformatiVegetaliQdC([FromBody] InData.Anagrafica.LeggiProdotti impostaLeggiProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta>> result = new();
            try
            {
                var request = getRequest(impostaLeggiProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiTrasformatiVegetaliQdC(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
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
        [Route("LeggiTrasformatiVegetaliAnagrafica")]
        public ObjectResult LeggiTrasformatiVegetaliAnagrafica([FromBody] InData.Anagrafica.LeggiProdotti impostaLeggiProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.Prodotto>> result = new();
            try
            {
                var request = getRequest(impostaLeggiProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_TrasformatiVegetali_Anagrafica(request);
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

        #region DPI
        [HttpPost]
        [Route("LeggiVincoli")]
        public ObjectResult LeggiVincoli([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Vincolo>> result = new RispostaStandard<List<Vincolo>>();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiVincoli(request);
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
        [Route("LeggiVincoliOrdered")]
        public ObjectResult LeggiVincoliOrdered([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Vincolo>> result = new RispostaStandard<List<Vincolo>>();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiVincoliOrdered(request);
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

        #region GHG
        [HttpPost]
        [Route(nameof(LeggiImpreseParametri))]
        public ObjectResult LeggiImpreseParametri([FromBody] LeggiImpreseParametri leggiImpreseParametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<ImpresaParametri>> result = new RispostaStandard<List<ImpresaParametri>>();

            try
            {
                var request = getRequest(leggiImpreseParametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseParametri(request);
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
        [Route(nameof(ScriviImpreseParametri))]
        public ObjectResult ScriviImpreseParametri([FromBody] ScriviModificaImpreseParametri scriviImpreseParametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(scriviImpreseParametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviImpreseParametri(request);
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
        [Route(nameof(ModificaImpreseParametri))]
        public ObjectResult ModificaImpreseParametri([FromBody] ScriviModificaImpreseParametri modificaImpreseParametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(modificaImpreseParametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModificaImpreseParametri(request);
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
        [Route(nameof(CancellaImpreseParametri))]
        public ObjectResult CancellaImpreseParametri([FromBody] ScriviModificaImpreseParametri cancellaImpreseParametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(cancellaImpreseParametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CancellaImpreseParametri(request);
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





        [HttpPost]
        [Route("ContattoAssociaUtente")]
        public ObjectResult ContattoAssociaAzienda([FromBody] AssociaUtente utente, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(utente);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ContattoAssociaUtente(request);
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
        [Route("LeggiUtentiDaAssociare")]
        public ObjectResult LeggiUtentiDaAssociare([FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUtentiDaAssociare(request);
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
        [Route("LeggiUtenteAssociato")]
        public ObjectResult LeggiUtenteAssociato([FromBody] AssociaUtente utente, [FromHeader] string Authorization)
        {

            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(utente);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUtenteAssociato(request);
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


        [HttpGet]
        [Route("Carica_Cmb_Imprese")]
        public ObjectResult Carica_Cmb_Imprese([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Carica_Cmb_Imprese(request);
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
        [Route(nameof(Carica_Cmb_Imprese_Area))]
        public ObjectResult Carica_Cmb_Imprese_Area(string area, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(area);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Carica_Cmb_Imprese_Area_NG(request);
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
        [Route(nameof(Carica_Cmb_Imprese_UMA))]
        public ObjectResult Carica_Cmb_Imprese_UMA(int Tipo_Azienda, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(Tipo_Azienda);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Carica_Cmb_Imprese_UMA_NG(request);
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
        [Route(nameof(CaricaAzienda_GIS))]
        public ObjectResult CaricaAzienda_GIS(string testoRicerca, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();
            //coreWSBaseURL
            try
            {
                var request = getRequest(testoRicerca);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaAzienda_GIS_NG(request);
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
        [Route("Leggi_Certificazioni")]
        public ObjectResult Leggi_Certificazioni([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<BaseCodeDescr>> result = new RispostaStandard<List<BaseCodeDescr>>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Certificazioni(request);
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

        [HttpGet]
        [Route("LeggiImpreseConFiltroUtente")]
        public ObjectResult LeggiImpreseConFiltroUtente([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseConFiltroUtente(request);
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

        [HttpGet]
        [Route("LeggiImpreseConFiltroUtente_Modello")]
        public ObjectResult LeggiImpreseConFiltroUtente_Modello([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>>();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseConFiltroUtente_Modello(request);
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
        [Route("LeggiImpreseConFiltroUtenteHubAgea_Modello")]
        public ObjectResult LeggiImpreseConFiltroUtenteHubAgea_Modello([FromBody] FarmFilters InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>>();

            try
            {
                var objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                var requestData = new CoreWS_Generic<FarmFilters>(objP, InData);
                var request = new CoreWSRequest<CoreWS_Generic<FarmFilters>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseConFiltroUtenteHubAgea_Modello(request);
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
        [Route("ReportImpreseExcelFiltroAgea")]
        public ObjectResult ReportImpreseExcelFiltroAgea([FromBody] FarmFilters InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ImpresaAgeaExcel> result = new RispostaStandard<ImpresaAgeaExcel>();

            try
            {
                var objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                var requestData = new CoreWS_Generic<FarmFilters>(objP, InData);
                var request = new CoreWSRequest<CoreWS_Generic<FarmFilters>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ReportImpreseExcelFiltroAgea(request);
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
        [Route(nameof(LeggiImpreseConFiltroUtenteCodiceSocio))]
        public ObjectResult LeggiImpreseConFiltroUtenteCodiceSocio(int solo_aziende_attive, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(solo_aziende_attive);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseConFiltroUtenteCodiceSocio_NG(request);
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
        [Route("Test_Imprese_Archivio_Lettura")]
        public ObjectResult Test_Imprese_Archivio_Lettura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Imprese_Archivio_Lettura(request);
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
        [Route("Test_Imprese_Archivio_Scrittura")]
        public ObjectResult Test_Imprese_Archivio_Scrittura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Imprese_Archivio_Scrittura(request);
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

        // ToDo_RR: da finire
        [HttpPost]
        [Route(nameof(cancellaMacchina))]
        public ObjectResult cancellaMacchina([FromBody] CancellaMacchina cancellaMacchina, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ParcoMacchineDto> result = new RispostaStandard<ParcoMacchineDto>();

            try
            {
                var request = getRequest(cancellaMacchina);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).cancellaMacchina_NG(request);
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

        [HttpGet]
        [Route(nameof(getAlimentazioneMacchine))]
        public ObjectResult getAlimentazioneMacchine([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<rispostaValore_IntTesto>> result = new RispostaStandard<List<rispostaValore_IntTesto>>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getAlimentazioneMacchine(request);
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
        [Route(nameof(getCentriVisibilita))]
        public ObjectResult getCentriVisibilita(string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<rispostaValore_IntTesto>> result = new RispostaStandard<List<rispostaValore_IntTesto>>();

            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getCentriVisibilita_NG(request);
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
        [Route(nameof(getElencoDettaglio1))]
        public ObjectResult getElencoDettaglio1(string Tipo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>> result = new RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>>();

            try
            {
                var request = getRequest(Tipo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getElencoDettaglio1_NG(request);
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
        [Route(nameof(getElencoDettaglio2))]
        public ObjectResult getElencoDettaglio2([FromBody] getElencoDettaglio2 getElencoDettaglio2, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>> result = new RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>>();

            try
            {
                var request = getRequest(getElencoDettaglio2);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getElencoDettaglio2_NG(request);
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

        [HttpGet]
        [Route("getElencoMarche")]
        public ObjectResult getElencoMarche([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> result = new RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getElencoMarche_NG(request);
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

        [HttpGet]
        [Route("getElencoMarcheWS")]
        public ObjectResult getElencoMarcheWS([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getElencoMarcheWS_NG(request);
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

        [HttpGet]
        [Route("getElencoTipo")]
        public ObjectResult getElencoTipo([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>> result = new RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>>();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getElencoTipo_NG(request);
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

        [HttpGet]
        [Route(nameof(getFinalita))]
        public ObjectResult getFinalita([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<rispostaValore_IntTesto>> result = new RispostaStandard<List<rispostaValore_IntTesto>>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getFinalita(request);
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

        [HttpGet]
        [Route(nameof(getJSON_ParcoMacchine_js))]
        public ObjectResult getJSON_ParcoMacchine_js([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new RispostaStandard<string>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getJSON_ParcoMacchine_js(request);
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
        [Route(nameof(getMacchina))]
        public ObjectResult getMacchina([FromBody] getMacchina getMacchina_request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ParcoMacchineDto> result = new RispostaStandard<ParcoMacchineDto>();

            try
            {
                var request = getRequest(getMacchina_request);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getMacchina_NG(request);
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
        [Route(nameof(getNewMacchina))]
        public ObjectResult getNewMacchina(string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ParcoMacchineDto> result = new RispostaStandard<ParcoMacchineDto>();

            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getNewMacchina_NG(request);
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

        [HttpGet]
        [Route(nameof(getPotenzaUdm))]
        public ObjectResult getPotenzaUdm([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<rispostaValore_IntTesto>> result = new RispostaStandard<List<rispostaValore_IntTesto>>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getPotenzaUdm(request);
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

        [HttpGet]
        [Route("GetTipoMacchine")]
        public ObjectResult GetTipoMacchine([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetTipoMacchine(request);
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

        [HttpGet]
        [Route(nameof(getTitoloPossesso))]
        public ObjectResult getTitoloPossesso([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<rispostaValore_IntTesto>> result = new RispostaStandard<List<rispostaValore_IntTesto>>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getTitoloPossesso(request);
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

        [HttpGet]
        [Route(nameof(getTipoTarga))]
        public ObjectResult getTipoTarga([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<rispostaValore_IntTesto>> result = new RispostaStandard<List<rispostaValore_IntTesto>>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getTipoTarga(request);
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
        [Route(nameof(Leggi_Macchine_Per_Contatto))]
        public ObjectResult Leggi_Macchine_Per_Contatto([FromBody] Leggi_Macchine_Per_Contatto leggi_Macchine_Per_Contatto_request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggi_Macchine_Per_Contatto_request);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Macchine_Per_Contatto_NG(request);
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
        [Route(nameof(Leggi_Macchine_Per_Piva))]
        public ObjectResult Leggi_Macchine_Per_Piva([FromBody] string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Macchine_Per_Piva_NG(request);
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
        [Route(nameof(Leggi_Caratteristiche_Macchina_Anagrafica))]
        public ObjectResult Leggi_Caratteristiche_Macchina_Anagrafica([FromBody] string Class_Code, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<BaseCodeDescr>> result = new RispostaStandard<List<BaseCodeDescr>>();

            try
            {
                var request = getRequest(Class_Code);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Caratteristiche_Macchina_Anagrafica(request);
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
        [Route(nameof(scriviMacchina))]
        public ObjectResult scriviMacchina([FromBody] scriviMacchina scriviMacchina_request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(scriviMacchina_request);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).scriviMacchina_NG(request);
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
        [Route("Test_Macchine_Archivio_Lettura")]
        public ObjectResult Test_Macchine_Archivio_Lettura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Macchine_Archivio_Lettura(request);
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
        [Route(nameof(Test_Macchine_Archivio_Lettura2))]
        public ObjectResult Test_Macchine_Archivio_Lettura2([FromBody] scriviMacchina scriviMacchina_request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(scriviMacchina_request);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Macchine_Archivio_Lettura(request);
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
        [Route("Test_Macchine_Archivio_Scrittura")]
        public ObjectResult Test_Macchine_Archivio_Scrittura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Macchine_Archivio_Scrittura(request);
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
        [Route(nameof(LeggiParticelleCatastali_toKendoGrid))]
        public ObjectResult LeggiParticelleCatastali_toKendoGrid([FromBody] LeggiParticelleCatastali_toKendoGrid leggiParticelleCatastali_toKendoGrid, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Object> result = new RispostaStandard<Object>();

            try
            {
                var request = getRequest(leggiParticelleCatastali_toKendoGrid);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiParticelleCatastali_toKendoGrid(request);
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
        [Route("Test_Catasto_Archivio_Lettura")]
        public ObjectResult Test_Catasto_Archivio_Lettura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Catasto_Archivio_Lettura(request);
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
        [Route("Test_Catasto_Archivio_Scrittura")]
        public ObjectResult Test_Catasto_Archivio_Scrittura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Catasto_Archivio_Scrittura(request);
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
        [Route(nameof(Carica_Comuni))]
        public ObjectResult Carica_Comuni([FromBody] string provincia, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(provincia);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Carica_Comuni(request);
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
        [Route(nameof(CaricaCentroAziendale_GIS))]
        public ObjectResult CaricaCentroAziendale_GIS([FromBody] string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaCentroAziendale_GIS_NG(request);
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
        [Route(nameof(GetCentri))]
        public ObjectResult GetCentri([FromBody] string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();
            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetCentri_NG(request);
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
        [Route(nameof(Leggi_Centri_Aziendali_perSpecieImpianti))]
        public ObjectResult Leggi_Centri_Aziendali_perSpecieImpianti([FromBody] AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali leggiCentriAziendali, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CentroAziendale>> result = new RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CentroAziendale>>();

            try
            {
                var request = getRequest(leggiCentriAziendali);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Centri_Aziendali_perSpecieImpianti(request);
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
        [Route(nameof(LeggiCentriConFiltroUtente))]
        public ObjectResult LeggiCentriConFiltroUtente([FromBody] LeggiCentriConFiltroUtente leggiCentriConFiltroUtente, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiCentriConFiltroUtente);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiCentriConFiltroUtente_NG(request);
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
        [Route("Test_Centri_Archivio_Lettura")]
        public ObjectResult Test_Centri_Archivio_Lettura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Centri_Archivio_Lettura(request);
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
        [Route("Test_Centri_Archivio_Scrittura")]
        public ObjectResult Test_Centri_Archivio_Scrittura([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Boolean> result = new RispostaStandard<Boolean>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Test_Centri_Archivio_Scrittura(request);
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
        [Route("Prodotti_NG")]
        public ObjectResult ProdottiNG([FromBody] Prodotti prodotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(prodotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Prodotti_NG(request);
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
        [Route("Prodotti_x_CAC_NG")]
        public ObjectResult Prodotti_x_CAC_NG([FromBody] Prodotti_x_CAC prodotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(prodotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Prodotti_x_CAC_NG(request);
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
        [Route("LeggiElencoCompletoProdotti")]
        public ObjectResult LeggiElencoCompletoProdotti([FromBody] LeggiElencoCompletoProdotti leggiElencoCompletoProdotti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiElencoCompletoProdotti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoCompletoProdotti_NG(request);
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
        [Route("LeggiElencoCompletoProdottiMultiCategoria")]
        public ObjectResult LeggiElencoCompletoProdottiMultiCategoria([FromBody] LeggiElencoCompletoProdottiMultiCategoria leggiElencoCompletoProdottiMultiCategoria, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiElencoCompletoProdottiMultiCategoria);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoCompletoProdottiMultiCategoria_NG(request);
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
        [Route("LeggiElencoProdotti_APP")]
        public ObjectResult LeggiElencoProdotti_APP([FromBody] LeggiElencoProdotti_APP leggiElencoProdotti_APP, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiElencoProdotti_APP);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoProdotti_APP_NG(request);
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
        [Route("Leggi_Contributi")]
        public ObjectResult Leggi_Contributi([FromBody] AgronicaCoreModelsSTD.anagrafiche.Impianto impianto_request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<BaseCodeDescrStr>> result = new RispostaStandard<List<BaseCodeDescrStr>>();

            try
            {
                var request = getRequest(impianto_request);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Contributi(request);
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
        [Route("Leggi_Max_DataModifica")]
        public ObjectResult Leggi_Max_DataModifica([FromBody] AgronicaCoreDTOStd.InData.LeggiFiltro leggiFiltro, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AgronicaCoreDTOStd.InData.Data> result = new RispostaStandard<AgronicaCoreDTOStd.InData.Data>();

            try
            {
                var request = getRequest(leggiFiltro);
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
        [Route("leggiGenerazionePoligoniDefaultValue")]
        public ObjectResult leggiGenerazionePoligoniDefaultValue([FromBody] string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new RispostaStandard<string>();

            try
            {
                var request = getRequest(data);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).leggiGenerazionePoligoniDefaultValue(request);
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
        [Route("Carica_Stalle")]
        public ObjectResult Carica_Stalle([FromBody] Carica_Stalle carica_Stalle, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(carica_Stalle);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Carica_Stalle_NG(request);
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
        [Route("Leggi_Fabbricati_Anagrafica")]
        public ObjectResult Leggi_Fabbricati_Anagrafica([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Fabbricati_Anagrafica(request);
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
        [Route("Leggi_Fabbricati_Anagrafica_CodDescr")]
        public ObjectResult Leggi_Fabbricati_Anagrafica_CodDescr([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Fabbricati_Anagrafica_CodDescr(request);
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
        [Route("Leggi_Ultimo_Magazzino_Prodotto_Movimentato")]
        public ObjectResult Leggi_Ultimo_Magazzino_Prodotto_Movimentato([FromBody] LeggiUltimo_Magazzino_Prodotto_Movimentato InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Fabbricato> result = new();

            try
            {
                var request = getRequest(InData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiUltimo_Magazzino_Prodotto_Movimentato(request);
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
        [Route(nameof(Leggi_Magazzini_Organismoreferente))]
        public ObjectResult Leggi_Magazzini_Organismoreferente([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Magazzini_Organismoreferente_NG(request);
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
        [Route(nameof(LeggiFabbricati))]
        public ObjectResult LeggiFabbricati([FromBody] LeggiFabbricati leggiFabbricati, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiFabbricati);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiFabbricati_NG(request);
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
        [Route(nameof(CaricaComboCmb_MagazzinoConferimento))]
        public ObjectResult CaricaComboCmb_MagazzinoConferimento([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmb_MagazzinoConferimento_NG(request);
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
        [Route(nameof(CaricaComboCmb_OrganismoReferente))]
        public ObjectResult CaricaComboCmb_OrganismoReferente([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmb_OrganismoReferente_NG(request);
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
        [Route(nameof(CaricaComboCmb_Riferimento_Trasferimento_Dati))]
        public ObjectResult CaricaComboCmb_Riferimento_Trasferimento_Dati([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaComboCmb_Riferimento_Trasferimento_Dati_NG(request);
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
        [Route("Leggi_Contatti_Anagrafica")]
        public ObjectResult Leggi_Contatti_Anagrafica([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Contatti_Anagrafica_NG(request);
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
        [Route(nameof(Leggi_Contatti_Per_Piva))]
        public ObjectResult Leggi_Contatti_Per_Piva([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Contatti_Per_Piva_NG(request);
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
        [Route(nameof(LeggiFornitori_FF))]
        public ObjectResult LeggiFornitori_FF([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiFornitori_FF_NG(request);
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
        [Route(nameof(LeggiRapportoDocumenti))]
        public ObjectResult LeggiRapportoDocumenti([FromBody] LeggiRapportoDocumenti leggiRapportoDocumenti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiRapportoDocumenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiRapportoDocumenti_NG(request);
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
        [Route(nameof(LeggiRapportoSpecifico))]
        public ObjectResult LeggiRapportoSpecifico([FromBody] LeggiRapportoSpecifico leggiRapportoSpecifico, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiRapportoSpecifico);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiRapportoSpecifico_NG(request);
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
        [Route(nameof(Leggi_Analisi_Testata_Per_Piva))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult Leggi_Analisi_Testata_Per_Piva([FromBody] string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Analisi_Testata_Per_Piva_NG(request);
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
        [Route(nameof(LeggiAree))]
        public ObjectResult LeggiAree([FromBody] LeggiAree leggiAree, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiAree);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAree_NG(request);
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
        [Route(nameof(PadriGerarchia2))]
        public ObjectResult PadriGerarchia2([FromBody] PadriGerarchia2 padriGerarchia2, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(padriGerarchia2);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PadriGerarchia2_NG(request);
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

        [HttpGet]
        [Route("PadriGerarchia")]
        public ObjectResult PadriGerarchia([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {

                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PadriGerarchia(request);
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
        [Route(nameof(LeggiIndirizziContatto))]
        public ObjectResult LeggiIndirizziContatto([FromBody] LeggiIndirizziContatto leggiIndirizziContatto, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(leggiIndirizziContatto);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiIndirizziContatto_NG(request);
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
        [Route(nameof(LeggiIndirizzoAziendaSuperUser))]
        public ObjectResult LeggiIndirizzoAziendaSuperUser([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiIndirizzoAziendaSuperUser(request);
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
        [Route(nameof(GetImpresexParticelle))]
        public ObjectResult GetImpresexParticelle([FromBody] GetImpresexParticelle getImpresexParticelle, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(getImpresexParticelle);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetImpresexParticelle_NG(request);
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
        [Route(nameof(GetImpresexParticelle_Movimentate))]
        public ObjectResult GetImpresexParticelle_Movimentate([FromBody] GetImpresexParticelle_Movimentate getImpresexParticelle_Movimentate, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(getImpresexParticelle_Movimentate);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetImpresexParticelle_Movimentate_NG(request);
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
        [Route("Leggi_Centro_Dropdowns")]
        public ObjectResult Leggi_Centro_Dropdowns([FromBody] Parametri_ObjParametriAgenda_NG_GestioneRichieste InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CentroDropdownLists> result = new RispostaStandard<CentroDropdownLists>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste> requestData = new CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request = new CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Centro_Dropdowns(request);
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
        [Route(nameof(Carica_Centri_Impresa))]
        public ObjectResult Carica_Centri_Impresa([FromBody] AgronicaCoreDTOStd.InData.Metaschema.CaricaCentriImpresa carica_Centri_Impresa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<object> result = new RispostaStandard<object>();

            try
            {
                var request = getRequest(carica_Centri_Impresa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Carica_Centri_Impresa(request);
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
        [Route(nameof(DatiRelativiPercorsoBreadcrumbs))]
        public ObjectResult DatiRelativiPercorsoBreadcrumbs([FromBody] DatiRelativiPercorsoBreadcrumbs datiRelativiPercorsoBreadcrumbs, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(datiRelativiPercorsoBreadcrumbs);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).DatiRelativiPercorsoBreadcrumbs_NG(request);
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
        [Route("GetCodiceImpianto")]
        [ProducesResponseType(typeof(RispostaStandard<object>), StatusCodes.Status200OK)]
        public ObjectResult GetCodiceImpianto([FromBody] object InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<object> result = new RispostaStandard<object>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<object> requestData = new CoreWS_Generic<object>(objP, InData);
                CoreWSRequest<CoreWS_Generic<object>> request = new CoreWSRequest<CoreWS_Generic<object>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetCodiceImpianto(request);
                
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
        [Route("GenerateDescriptions")]
        [ProducesResponseType(typeof(RispostaStandard<List<object>>), StatusCodes.Status200OK)]
        public ObjectResult GenerateDescriptions([FromBody] string InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<object>> result = new RispostaStandard<List<object>>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<string> requestData = new CoreWS_Generic<string>(objP, InData);
                CoreWSRequest<CoreWS_Generic<string>> request = new CoreWSRequest<CoreWS_Generic<string>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GenerateDescriptions(request);
                
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
        [Route("ClosePlant")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult ClosePlant([FromBody] Esercizio InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new RispostaStandard<string>();

            try
            {
                CoreWS_GenericObjP objP = new CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti);
                CoreWS_Generic<Esercizio> requestData = new CoreWS_Generic<Esercizio>(objP, InData);
                CoreWSRequest<CoreWS_Generic<Esercizio>> request = new CoreWSRequest<CoreWS_Generic<Esercizio>>(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ClosePlant(request);

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

        #region DatiPrevisionaliColture

        [HttpPost]
        [Route(nameof(readDatiPrevisionaliColture))]
        public ObjectResult readDatiPrevisionaliColture([FromBody] DatiPrevisionaliColtureRequest requestData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<DatiPrevisionaliColture> result = new RispostaStandard<DatiPrevisionaliColture>();

            try
            {
                var request = getRequest(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readDatiPrevisionaliColture(request);
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
        [Route(nameof(readDatiPrevisionaliColtureDT))]
        public ObjectResult readDatiPrevisionaliColtureDT([FromBody] DatiPrevisionaliColtureRequest requestData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).readDatiPrevisionaliColtureDT(request);
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
        [Route(nameof(editDatiPrevisionaliColture))]
        public ObjectResult editDatiPrevisionaliColture([FromBody] DatiPrevisionaliColtureComplete requestData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).editDatiPrevisionaliColture(request);
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
        [Route(nameof(createDatiPrevisionaliColture))]
        public ObjectResult createDatiPrevisionaliColture([FromBody] DatiPrevisionaliColtureComplete requestData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).createDatiPrevisionaliColture(request);
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
        [Route(nameof(deleteDatiPrevisionaliColture))]
        public ObjectResult deleteDatiPrevisionaliColture([FromBody] DatiPrevisionaliColtureComplete requestData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                var request = getRequest(requestData);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).deleteDatiPrevisionaliColture(request);
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
