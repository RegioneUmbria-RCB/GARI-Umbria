using AgronicaCoreModelloSTD;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using AgronicaCoreDTOStd.InData.Gis;
using AgronicaCoreModelsSTD.Gis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CoreApi.BusinessLayer.Services;
using AgronicaCoreModelsSTD.Gis.PermessiLayer;
using AgronicaCoreAPI.DTO;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreDTOStd.InData.Gis.MUZ;
using AgronicaCoreModelsSTD.Gis.MUZ;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.OutData.Gis;
using InData.Gis;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace AgronicaCoreAPI.Controllers
{
    public class GisController : BaseController
    {

        private readonly IGisDataReadParam gisDataReadParamValidator;

        public GisController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IGisDataReadParam gisDataReadParamValidator, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
            this.gisDataReadParamValidator = gisDataReadParamValidator;
        }

        [HttpPost]
        [Route("TestData")]
        public ObjectResult TestData([FromBody] CoreWS_Gis<GisDataReadParam> request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<GisDataReadRval<GeoJSONAgroGisProp>> result = new();

            try
            {
                request.objP_super_server = objP_super_server;
                request.objP_server = objP_server;
                request.objP_utenti = objP_utenti;
                // CoreWS_Gis<GisDataReadParam> request = new CoreWS_Gis<GisDataReadParam>(objP_super_server, objP_server, objP_utenti, gisDataReadParam);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GisTestData(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("LeggiElencoEntitaGeoJson")]
        [ProducesResponseType(typeof(RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>), StatusCodes.Status200OK)]
        //public ObjectResult LeggiElencoEntitaGeoJson([FromBody] CoreWSRequest<GisDataReadParam> CoreWSRequestGisDataReadParam)
        public ObjectResult LeggiElencoEntitaGeoJson([FromBody] GisDataReadParam requestGisDataReadParam, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (!gisDataReadParamValidator.IsValid(requestGisDataReadParam).Result) return StatusCode(StatusCodes.Status400BadRequest, null);

            RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> result = new();
                                 
            try
            {
                CoreWS_Gis<GisDataReadParam> request = new CoreWS_Gis<GisDataReadParam>(objP_super_server, objP_server, objP_utenti, requestGisDataReadParam);
                result = new CoreWSController(hc,coreWSBaseURL, bearerToken, Request).GisLeggiElencoEntitaGeoJson(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("LeggiDateImpiantiFiltroTemporale")]
        [ProducesResponseType(typeof(RispostaStandard<DateTime>), StatusCodes.Status200OK)]
        public ObjectResult LeggiDateImpiantiFiltroTemporale([FromBody] LeggiDateImpiantiPerFiltroTemporale_In requestGisDataReadParam, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<DateTime> result = new();

            try
            {
                CoreWS_Gis<LeggiDateImpiantiPerFiltroTemporale_In> request = new CoreWS_Gis<LeggiDateImpiantiPerFiltroTemporale_In>(objP_super_server, objP_server, objP_utenti, requestGisDataReadParam);
                result = new CoreWSController(hc,coreWSBaseURL, bearerToken, Request).GisLeggiDateImpiantiFiltroTemporale(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("LeggiPoligono")]
        public ObjectResult LeggiPoligono([FromBody] CoreWS_Gis<STBufferGeoJsonPolygonInData> request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<STBufferGeoJsonPolygonOutData> result = new();

            try
            {
                request.objP_super_server = objP_super_server;
                request.objP_server = objP_server;
                request.objP_utenti = objP_utenti;
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GisLeggiPoligono(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("ScriviElementoGrafico")]
        public ObjectResult ScriviElementoGrafico([FromBody] CoreWS_Gis<CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoInData> request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoOutData> result = new();

            try
            {
                request.objP_super_server = objP_super_server;
                request.objP_server = objP_server;
                request.objP_utenti = objP_utenti;
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GisScriviElementoGrafico(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("ModificaImpianto")]
        public ObjectResult ModificaImpianto([FromBody] CoreWS_Gis<CoreWSGisEndPoints_ModificaImpianto2019InData> request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CoreWSGisEndPoints_ModificaImpianto2019OutData> result = new();

            try
            {
                request.objP_super_server = objP_super_server;
                request.objP_server = objP_server;
                request.objP_utenti = objP_utenti;
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GisModificaImpianto(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Popola il GIS con le varie entita
        /// </summary>
        /// <returns>Lista di obj TipoEntita_Out</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /GIS_TipoEntita_popola
        ///     
        /// </remarks>
        /// <response code="200">Lista di Entita GIS (obj TipoEntita_Out)</response>
        /// <response code="400">Se non trova nessun'entità presente nel GIS</response>
        [HttpGet]
        [Route("GIS_TipoEntita_popola")]
        [ProducesResponseType(typeof(RispostaStandard<List<TipoEntita_Out>>), StatusCodes.Status200OK)]
        public ObjectResult GIS_TipoEntita_popola([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<TipoEntita_Out>> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GIS_TipoEntita_Popola(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Scrive sul DB l'xml di passaggio tra i vari siti
        /// </summary>
        /// <param name="LeggiEntita">Entita_Cod</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>URL di redirect per Sito Analisi + parametri Analisi2010</returns>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST /ApriSitoAnalisixVisualizzazione
        ///     {
        ///         "Entita_Cod": "31344",
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Viene restituito l'URL per il redirect + obj di tipo params Analisi_2010</response>
        /// <response code="400">Non è stato trovato l'url</response>
        [HttpPost]
        [Route("ApriSitoAnalisixVisualizzazione")]
        [ProducesResponseType(typeof(RispostaStandard<ApriSitoAnalisi_Out>), StatusCodes.Status200OK)]
        public ObjectResult ApriSitoAnalisixVisualizzazione([FromBody] LeggiEntita_In LeggiEntita, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if(LeggiEntita == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto LeggiEntita non valorizzato");

            RispostaStandard<ApriSitoAnalisi_Out> result = new();

            try
            {
                var request = getRequest(LeggiEntita);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ApriSitoAnalisixVisualizzazione(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Carica uno o più obj di tipo Impianto all’interno del GIS
        /// </summary>
        /// <param name="ChiaveImpianto">Piva, Sa_Cod, Appezza, Id_Reg</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un obj di tipo SingoloObjImpianto_Out</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaSingoloOggetto_Impianto
        ///     {
        ///         "Piva": "02969160544",
        ///         "Sa_Cod": 130023425,
        ///         "Appezza": 130023430,
        ///         "Id_Reg" 130023426,
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">obj SingoloObjImpianto_Out</response>
        /// <response code="400">Non è stato trovato nessun Impianto</response>
        [HttpPost]
        [Route("CaricaSingoloOggetto_Impianto")]
        [ProducesResponseType(typeof(RispostaStandard<SingoloObjImpianto_Out>), StatusCodes.Status200OK)]
        public ObjectResult CaricaSingoloOggetto_Impianto([FromBody] ChiaveImpianto_In ChiaveImpianto, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (ChiaveImpianto == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto ChiaveImpianto non valorizzato");

            RispostaStandard<SingoloObjImpianto_Out> result = new();

            try
            {
                var request = getRequest(ChiaveImpianto);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaSingoloOggetto_Impianto(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Genera URL per il redirect verso una pagina di Analisi (meteo, modelli o rilievi)
        /// </summary>
        /// <param name="AnalisiMeteo">Lat, Long, obj ChiaveAlbero, Veg_Cod, Tipo_Analisi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>URL di redirect per pagina Analisi e params Agenda2010</returns>
        /// <remarks>
        /// 
        ///     POST /AnalisiMeteoBs
        ///     {
        ///         "Lat": "",
        ///         "Long": "",
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///         "Veg_Cod": ,
        ///         "Tipo_Analisi": 1, 2 o 3 (enum_GIS_TipoAnalisiBs),
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Restituisce l'URL per il redirect alla pagina di Analisi richiesta + parametri Agenda2010</response>
        /// <response code="400">Errore durante l'operazione di componimento dell'url</response>
        [HttpPost]
        [Route("AnalisiMeteoBs")]
        [ProducesResponseType(typeof(RispostaStandard<AnalisiMeteo_Out>), StatusCodes.Status200OK)]
        public ObjectResult AnalisiMeteoBs([FromBody] AnalisiMeteo_In AnalisiMeteo, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            
            if (AnalisiMeteo == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto AnalisiMeteo non valorizzato");

            RispostaStandard<AnalisiMeteo_Out> result = new();

            try
            {
                var request = getRequest(AnalisiMeteo);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AnalisiMeteoBs(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue un update dell'Appezzamento passato
        /// </summary>
        /// <param name="BufferZone">obj ChiaveAlbero, DistBZ_CorpiIdrici, DistBZ_AreeResPub, DistBZ_Allevamenti, DistBZ_VegNatNonColt, SupBZ_Riduzione</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Messaggio di riuscita esecuzione</returns>
        /// <remarks>
        /// 
        ///     POST /BufferZone_Aggiorna
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///         "DistBZ_CorpiIdrici": ,
        ///         "DistBZ_AreeResPub": ,
        ///         "DistBZ_Allevamenti": ,
        ///         "DistBZ_VegNatNonColt": ,
        ///         "SupBZ_Riduzione": ,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">"Operazione di aggiornamento eseguita correttamente."</response>
        /// <response code="400">Messaggio di errore: esecuzione fallita</response>
        [HttpPost]
        [Route("BufferZone_Aggiorna")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult BufferZone_Aggiorna([FromBody] BufferZone_In BufferZone, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (BufferZone == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto BufferZone non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(BufferZone);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).BufferZone_Aggiorna(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge e ritorna un oggetto di tipo Appezzamento, calcolandone la BufferZone 
        /// </summary>
        /// <param name="BufferZone">Entita_Cod, obj ChiaveAlbero</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un obj LeggiBufferZone_Out</returns>
        /// <remarks>
        /// 
        ///     POST /BufferZone_Leggi
        ///     {
        ///         "Entita_Cod": 31344,
        ///         ChiaveAlbero: obj ChiaveAlbero,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Obj LeggiBufferZone_Out</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("BufferZone_Leggi")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiBufferZone_Out>), StatusCodes.Status200OK)]
        public ObjectResult BufferZone_Leggi([FromBody] BufferZone_In BufferZone, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (BufferZone == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto BufferZone non valorizzato");

            RispostaStandard<LeggiBufferZone_Out> result = new();

            try
            {
                var request = getRequest(BufferZone);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).BufferZone_Leggi(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Carica i codici degli elementi Impresa utilizzando il chiave (= Piva + Id_Cod)
        /// </summary>
        /// <param name="CaricaImpreseCod">Piva, Id_Cod</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di Codici_Impresa</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaImpreseCodici
        ///     {
        ///         "Piva": "02969160544",
        ///         "Id_cod": 1107,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Restituisce Val_Cod</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CaricaImpreseCodici")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CaricaImpreseCodici([FromBody] CaricaImpreseCod_In CaricaImpreseCod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (CaricaImpreseCod == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto CaricaImpreseCod non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(CaricaImpreseCod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaImpreseCodici(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Gestisce la colorazione automatica
        /// </summary>
        /// <param name="TipologiaLayer_cod"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Messaggio di riuscita esecuzione</returns>
        /// <remarks>
        ///     
        ///     POST /ColorazioneAutomatica(1)
        /// 
        /// </remarks>
        /// <response code="200">"OK " + "True/False" </response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ColorazioneAutomatica")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ColorazioneAutomatica(int TipologiaLayer_cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(TipologiaLayer_cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ColorazioneAutomatica(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Ritorna il CodiceFiscaleTecnico aggiornato leggendolo da DB in Utenti_xGruppi_Utente
        /// </summary>
        /// <param name="Codice_Fiscale_Tecnico"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Codice_Fiscale_Tecnico</returns>
        /// <remarks>
        /// 
        ///     POST /getCodiceFiscaleTecnico("NTRGKN71L08C721J")
        /// 
        /// </remarks>
        /// <response code="200">Codice_Fiscale_Tecnico</response>
        /// <response code="400">Non è stato trovato nessun Codice_Fiscale_Tecnico</response>
        [HttpPost]
        [Route("getCodiceFiscaleTecnico")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult Get_CodiceFiscaleTecnico(string Codice_Fiscale_Tecnico, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(Codice_Fiscale_Tecnico);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Get_CodiceFiscaleTecnico(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce la ChiaveAlbero come oggetto ChiaveAlbero dato l'Id_Agenda
        /// </summary>
        /// <param name="Id_Agenda"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce obj ChiaveAlbero</returns>
        /// <remarks>
        /// 
        ///     POST /GetChiave_ID_Agenda_Selezionato(195896)
        /// 
        /// </remarks>
        /// <response code="200">obj ChiaveAlbero</response>
        /// <response code="400">Non è stato trovata nessuna ChiaveAlbero per quell'Id_Agenda</response>
        [HttpPost]
        [Route("GetChiave_ID_Agenda_Selezionato")]
        [ProducesResponseType(typeof(RispostaStandard<ChiaveAlbero>), StatusCodes.Status200OK)]
        public ObjectResult GetChiave_ID_Agenda_Selezionato(string Id_Agenda, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (Id_Agenda == "") return StatusCode(StatusCodes.Status400BadRequest, "Id_Agenda non valorizzato");

            RispostaStandard<ChiaveAlbero> result = new();

            try
            {
                var request = getRequest(Id_Agenda);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetChiave_ID_Agenda_Selezionato(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Ritorna l'elenco delle operazioni di agenda disposte per l'utente corrente
        /// </summary>
        /// <returns>Lista di obj ObjInputHTML_Out contenenti operazioni d'agenda disponibili</returns>
        /// <remarks>
        /// 
        ///     GET /ElencoOperazioniAgendaGraficabili
        /// 
        /// </remarks>
        /// <response code="200">Operazioni disponibili come List(Of ObjInputHTML_Out)</response>
        /// <response code="400">Non è stato trovata nessuna Operazione d'agenda disponibile per quell'utente</response>
        [HttpGet]
        [Route("ElencoOperazioniAgendaGraficabili")]
        [ProducesResponseType(typeof(RispostaStandard<List<ObjInputHTML_Out>>), StatusCodes.Status200OK)]
        public ObjectResult ElencoOperazioniAgendaGraficabili([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<ObjInputHTML_Out>> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ElencoOperazioniAgendaGraficabili(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Salva uno o più nuovi elementi MultiPoint e restituisce la lista di essi
        /// </summary>
        /// <param name="SalvaNuovoMultiPoint">obj ChiaveAlbero, hiddenPunti_M, DialogMultipointDes</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista dei nuovi obj hiddenPoint_Nuovi salvati (in SalvaNuovoMultiPoint_Out)</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaNuovoMultipoint
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///         "hiddenPunti_M": stringa di array di obj hiddenPunti,
        ///         "DialogMultipointDes": "descrizione dell'elemento grafico"
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">obj SalvaNuovoMultiPoint_Out</response>
        /// <response code="400">Errore durante l'operazione (salvataggio)</response>
        [HttpPost]
        [Route("SalvaNuovoMultipoint")]
        [ProducesResponseType(typeof(RispostaStandard<SalvaNuovoMultiPoint_Out>), StatusCodes.Status200OK)]
        public ObjectResult SalvaNuovoMultipoint([FromBody] SalvaNuovoMultiPoint_In SalvaNuovoMultiPoint, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SalvaNuovoMultiPoint == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto SalvaNuovoMultiPoint non valorizzato");

            RispostaStandard<SalvaNuovoMultiPoint_Out> result = new();

            try
            {
                var request = getRequest(SalvaNuovoMultiPoint);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaNuovoMultipoint(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// A seconda del caso, estrae App_Nome o Programmazione_Des dell'Appezzamento
        /// </summary>
        /// <param name="ChiaveAlb">obj ChiaveAlbero</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>App_nome OR Programmazione_Des</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaAppNomeProgrammazione_des
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Restituisce App_nome o Programmazione_Des</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CaricaAppNomeProgrammazione_des")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CaricaAppNomeProgrammazione_des([FromBody] ChiaveAlbero ChiaveAlb, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (ChiaveAlb == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto ChiaveAlb non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(ChiaveAlb);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaAppNomeProgrammazione_des(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Ritorna il metodo di sementi utilizzato
        /// </summary>
        /// <param name="LeggiGisPurpose">Sementi, Sementi_MappaturaLibera</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce il tipo di utilizzo delle Sementi</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiGisPurpose
        ///     {
        ///         "Sementi": "",
        ///         "Sementi_MappaturaLibera": "",
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Restituisce enum_GisPurpose (SementiSportello = 1 o SementiMappaturaLibera = 2)</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiGisPurpose")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LeggiGisPurpose([FromBody] LeggiGisPurpose_In LeggiGisPurpose, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (LeggiGisPurpose == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto LeggiGisPurpose non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(LeggiGisPurpose);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGisPurpose(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Verifica la cancellazione dell'entità tramite Entita_Cod
        /// </summary>
        /// <param name="LeggiEntita">Entita_Cod</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna True/False a seconda della riuscita esecuzione</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaCancella
        ///     {
        ///         "Entita_Cod": 31344,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">True/False</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaCancella")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult VerificaCancella([FromBody] LeggiEntita_In LeggiEntita, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (LeggiEntita == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto LeggiEntita non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(LeggiEntita);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaCancella(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Dato un poligono, ne calcola il rateo variabile e crea ne crea gli elementi di conseguenza
        /// </summary>
        /// <param name="PfRateoSrv">obj ChiaveAlbero, DescrizioneDelPiano, cellsize, DataRiferimentoPerLetturaDatiSentinel, oCfgLetturaPF</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna il numero degli elementi memorizzati</returns>
        /// <remarks>
        /// 
        ///     POST /pfRateoSrv
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///         "DescrizioneDelPiano": "",
        ///         "cellsize": 10,
        ///         "DataRiferimentoPerLetturaDatiSentinel": "1/1/1900",
        ///         "oCfgLetturaPF": stringa che verrà Deserializzata in obj PrecisionModel_CFGLetturaDati,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio con numero elementi memorizzati</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("pfRateoSrv")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult pfRateoSrv([FromBody] PfRateoSrv_In PfRateoSrv, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (PfRateoSrv == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto PfRateoSrv non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(PfRateoSrv);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).pfRateoSrv(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Dato un poligono, ed un tipo di rischio richiama un servizio esterno per recuperare i valori medi presi da mappe satellitari
        /// </summary>
        /// <param name="PfRateoSrv">obj ChiaveAlbero, tipoIndice, wktPolygon,  cellsize, DataRiferimento, srid</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna il numero degli elementi memorizzati</returns>
        /// <remarks>
        /// 
        ///     POST /pfIndiceRischioSrv
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///         "tipoIndice": "",
        ///         "wktPolygon": "",
        ///         "cellsize": 10,
        ///         "DataRiferimento": "1/1/1900",
        ///         "srid": sistema cartografico di riferimento,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio con numero elementi memorizzati</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("pfIndiceRischioSrv")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult pfIndiceRischioSrv([FromBody] pfIndiciRischio_In PfRateoSrv, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (PfRateoSrv == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto pfIndiciRischio_In non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(PfRateoSrv);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).pfIndiceRischioSrv(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Dato un poligono, ne calcola il rateo variabile e crea ne crea gli elementi di conseguenza
        /// </summary>
        /// <param name="request">obj ChiaveAlbero, DescrizioneDelPiano, file</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna un messagio ok\ko</returns>
        /// <remarks>
        /// 
        ///     POST /pfPrescriptionUpload
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///         "DescrizioneDelPiano": "",
        ///         "rateColumnName" : nome della colonna del dbf da cui recuperare il dosaggio della prescrizione
        ///         "fileZip": oggetto IFormFile dove impostare il file zip contenente la mappa di prescrizione
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio con numero elementi memorizzati</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("pfPrescriptionUpload")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult pfPrescriptionUpload([FromForm] pfPrescriptionUpload_In request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            if (request == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto request non valorizzato");
            RispostaStandard result = new();

            try
            {
                using(var str = new System.IO.MemoryStream())
                {
                    request.fileZip.CopyTo(str);
                    var req = getRequest(new pfPrescriptionUploadCoreWS_In(request.ChiaveAlbero,
                                                                      request.DescrizionePiano,
                                                                      request.psw_SuperUser,
                                                                      request.rateColumnName,
                                                                      request.fileZip.FileName,
                                                                      str.ToArray()));
                    result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).pfPrescriptionUpload(req);
                    if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Dato un codice di allegato ed una stringa GEOJsons serializzata, viene aggiornato il dato per la mappa di prescrizione
        /// </summary>
        /// <param name="pfPrescriptionUpdate">oggetto con i relativi codici e stringa</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna un messagio ok\ko</returns>
        /// <remarks>
        /// 
        ///     POST /pfPrescriptionUpdate
        ///     {
        ///         "allegati_documenti_cod": id allegati documenti,
        ///         "jsonMap": oggetto IFormFile dove impostare il file zip contenente la mappa di prescrizione
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio con numero elementi memorizzati</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("pfPrescriptionUpdate")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult pfPrescriptionUpdate([FromBody] pfPrescriptionUpdate_In pfPrescriptionUpdate, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            if (pfPrescriptionUpdate == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto request non valorizzato");
            RispostaStandard result = new();

            try
            {
                var request = getRequest(pfPrescriptionUpdate);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).pfPrescriptionUpdate(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge la posizione del Centro richiesto
        /// </summary>
        /// <param name="CoordinateFromImpresa">Piva, Sa_Cod</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna la posizione del Centro in obj CoordsFromViaCentro_Out</returns>
        /// <remarks>
        /// 
        ///     POST /CoordinateFromViaCentro
        ///     {
        ///         "Piva": "01704430519",
        ///         "Sa_Cod": 130023434,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">obj CoordsFromViaCentro_Out</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CoordinateFromViaCentro")]
        [ProducesResponseType(typeof(RispostaStandard<CoordsFromViaCentro_Out>), StatusCodes.Status200OK)]
        public ObjectResult CoordinateFromViaCentro([FromBody] CoordinateFromImpresa_In CoordinateFromImpresa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (CoordinateFromImpresa == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto CoordinateFromImpresa non valorizzato");

            RispostaStandard<CoordsFromViaCentro_Out> result = new();

            try
            {
                var request = getRequest(CoordinateFromImpresa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CoordinateFromViaCentro(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue un controllo sull'esistenza dell'elemento Catasto passato, identificato per ChiaveAlbero
        /// </summary>
        /// <param name="LeggiEntita">obj ChiaveAlbero</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna True/False a seconda della riuscita esecuzione</returns>
        /// <remarks>
        /// 
        ///     POST /ControllaSeEsisteCatasto
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">True/False</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ControllaSeEsisteCatasto")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ControllaSeEsisteCatasto([FromBody] LeggiEntita_In LeggiEntita, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (LeggiEntita == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto LeggiEntita non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(LeggiEntita);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ControllaSeEsisteCatasto(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Crea e ritorna una tabella degli elementi grafici presenti 
        /// </summary>
        /// <param name="LayerElementiGrafici_Cod"></param>
        /// <returns>strTabella (HTML table)</returns>
        /// <remarks>
        /// 
        ///     POST /Caricaplace_tabella_Dettagli(50)
        /// 
        /// </remarks>
        /// <response code="200">Restituisce la tabella HTML di elementi grafici</response>
        /// <response code="400">Errore durante l'operazione</response>

        //[HttpPost]
        //[Route("Caricaplace_tabella_Dettagli")]
        //[ProducesResponseType(typeof(RispostaStandard<Caricaplace_TabHTML_Out>), StatusCodes.Status200OK)]
        //public ObjectResult Caricaplace_tabella_Dettagli(int LayerElementiGrafici_Cod)
        //{
        //    if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);
        //    if (LayerElementiGrafici_Cod < 0) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto LayerElementiGrafici_Cod non valorizzato correttamente");
        //    RispostaStandard<Caricaplace_TabHTML_Out> result = new();
        //    try
        //    {
        //        var request = getRequest(LayerElementiGrafici_Cod);
        //        result = new CoreWSController(hc, coreWSBaseURL, tokenChiamateWS, Request).Caricaplace_tabella_Dettagli(request);
        //        if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.RispostaOK = false;
        //        result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
        //        return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //    return StatusCode(StatusCodes.Status200OK, result);
        //}

        /// <summary>
        /// Ritorna una tabella degli elementi grafici presenti
        /// </summary>
        /// <param name="LayerElementiGrafici_Cod"></param>
        /// <returns>kendo DataTable</returns>
        /// <remarks>
        /// 
        ///     POST /Caricaplace_tabella_Dettagli2(50)
        /// 
        /// </remarks>
        /// <response code="200">Restituisce la kendo DataTable di elementi grafici</response>
        /// <response code="400">Errore durante l'operazione</response>
        
        //[HttpPost]
        //[Route("Caricaplace_tabella_Dettagli2")]
        //[ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        //public ObjectResult Caricaplace_tabella_Dettagli2(int LayerElementiGrafici_Cod)
        //{
        //    if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);
        //    if (LayerElementiGrafici_Cod < 0) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto LayerElementiGrafici_Cod non valorizzato correttamente");
        //    RispostaStandard result = new();
        //    try
        //    {
        //        var request = getRequest(LayerElementiGrafici_Cod);
        //        result = new CoreWSController(hc, coreWSBaseURL, tokenChiamateWS, Request).Caricaplace_tabella_Dettagli2(request);
        //        if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.RispostaOK = false;
        //        result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
        //        return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //    return StatusCode(StatusCodes.Status200OK, result);
        //}

        /// <summary>
        /// Popola la DDL Tipologia_Layer da DB
        /// </summary>
        /// <returns>Elenco di option per DDL Tipologia_Layer</returns>
        /// <remarks>
        /// 
        ///     GET /Carica_ddlTipologiaLayer
        /// 
        /// </remarks>
        /// <response code="200">Lista di tag "option" per DDL Tipologia_Layer</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpGet]
        [Route("Carica_ddlTipologiaLayer")]
        [ProducesResponseType(typeof(RispostaStandard<List<ObjOptionHTML_Out>>), StatusCodes.Status200OK)]
        public ObjectResult Carica_ddlTipologiaLayer([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<ObjOptionHTML_Out>> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Carica_ddlTipologiaLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Controllo sulle operazioni d'agenda possibili
        /// </summary>
        /// <param name="ChiaveAlb">obj ChiaveAlbero</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Ritorna True/False a seconda della riuscita esecuzione</returns>
        /// <remarks>
        /// 
        ///     POST /ControllaSeEsistonoOperazioni
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">True/False</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ControllaSeEsistonoOperazioni")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ControllaSeEsistonoOperazioni([FromBody] ChiaveAlbero ChiaveAlb, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (ChiaveAlb == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto ChiaveAlb non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(ChiaveAlb);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ControllaSeEsistonoOperazioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Ritorna le proprietà di un'entità del GIS (appezzamento, ricetta, catasto...) cercando per Entita_Cod
        /// </summary>
        /// <param name="GetProprieta">Entita_Cod, Tipo_GetProp</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Proprietà dell'entità del GIS come obj GetProprieta_Out</returns>
        /// <remarks>
        /// 
        ///     //Appezzamento
        ///     POST /GetProprieta
        ///     {
        ///         "Entita_Cod": 31344
        ///         "Tipo_GetProp": 1
        ///     }
        /// 
        ///     //Impianto
        ///     POST /GetProprieta
        ///     {
        ///         "Entita_Cod": 31879
        ///         "Tipo_GetProp": 2
        ///     }
        ///     
        ///     //Catasto
        ///     POST /GetProprieta
        ///     {
        ///         "Entita_Cod": 33172
        ///         "Tipo_GetProp": 2
        ///     }
        ///     
        /// 
        /// </remarks>
        /// <response code="200">Proprietà entità obj JSON</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("GetProprieta")]
        [ProducesResponseType(typeof(RispostaStandard<GetProprieta_Out>), StatusCodes.Status200OK)]
        public ObjectResult GetProprieta([FromBody] GetProprieta_In GetProprieta, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (GetProprieta == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto GetProprieta non valorizzato");

            RispostaStandard<GetProprieta_Out> result = new();

            try
            {
                var request = getRequest(GetProprieta);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetProprieta(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Ritorna un obj contenente le liste di Codici riguardanti Ricette e AllegatiDoc
        /// </summary>
        /// <param name="CaricaDatiPrecision">Lista_RicettaOp_Cod, TipoCodici</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un obj Lista_DatiPrecision_XmlAllegati_Out</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaDatiPrecisionXmlDaAllegati
        ///     {
        ///         "Lista_RicettaOp_Cod": lista di RicettaOperazione_Cod,
        ///         "TipoCodici": 0, 1, //flag codici Ricette/Allegati
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">obj Lista_DatiPrecision_XmlAllegati_Out</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CaricaDatiPrecisionXmlDaAllegati")]
        [ProducesResponseType(typeof(RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>), StatusCodes.Status200OK)]
        public ObjectResult CaricaDatiPrecisionXmlDaAllegati([FromBody] CaricaDatiPrecision_In CaricaDatiPrecision, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (CaricaDatiPrecision == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto CaricaDatiPrecision non valorizzato");

            RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> result = new();

            try
            {
                var request = getRequest(CaricaDatiPrecision);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaDatiPrecisionXmlDaAllegati(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Verifica la vicinanza con il poligono selezionato (può essere chiamata coem VerificaInterferenze o come CampoPiuVicino (CampoPiuVicino = 1)
        /// </summary>
        /// <param name="VerificaInterferenze">Veg_Cod, Grva_Cod, Data_Inizio, Data_Fine, HiddenPunti_Nuovo, CampoPiuVicino, Sementi, SementiMappaturaLibera</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un obj VerificaInterferenze_Out</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaInterferenze
        ///     {
        ///         "Veg_Cod": 12,
        ///         "Grva_Cod": 0,
        ///         "Data_Inizio": "1/1/1900",
        ///         "Data_Fine": "31/12/2100",
        ///         "HiddenPunti_Nuovo": obj JSON come stringa,
        ///         "CampoPiuVicino": 0, 1, //flag caso VerificaInterferenze/CampoPiuVicino
        ///         "Sementi": "",
        ///         "SementiMappaturaLibera": "",
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Elenco interferenze + Messaggio di esito</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaInterferenze")]
        [ProducesResponseType(typeof(RispostaStandard<VerificaInterferenze_Out>), StatusCodes.Status200OK)]
        public ObjectResult VerificaInterferenze([FromBody] VerificaInterferenze_In VerificaInterferenze, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (VerificaInterferenze == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto VerificaInterferenze non valorizzato");

            RispostaStandard<VerificaInterferenze_Out> result = new();

            try
            {
                var request = getRequest(VerificaInterferenze);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaInterferenze(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce le entità vicine a quella passata (utilizzando o meno Entita_Cod)
        /// </summary>
        /// <param name="VerificaVicini">Veg_Cod, Grva_Cod, Data_Inizio, Data_Fine, HiddenPunti_Nuovo/Modifica (a seconda di Entita_Cod), 
        /// Entita_Cod, Sementi, SementiMappaturaLibera</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un obj VerificaVicini_Out</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaVicini
        ///     {
        ///         "Veg_Cod": 12,
        ///         "Grva_Cod": 0,
        ///         "Data_Inizio": "1/1/1900",
        ///         "Data_Fine": "31/12/2100",
        ///         "HiddenPunti_Nuovo/Modifica": obj JSON come stringa,
        ///         "Entita_Cod": "31344",
        ///         "Sementi": "",
        ///         "SementiMappaturaLibera": "",
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">obj VerificaVicini_Out</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaVicini")]
        [ProducesResponseType(typeof(RispostaStandard<VerificaVicini_Out>), StatusCodes.Status200OK)]
        public ObjectResult VerificaVicini([FromBody] VerificaVicini_In VerificaVicini, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (VerificaVicini == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto VerificaVicini non valorizzato");

            RispostaStandard<VerificaVicini_Out> result = new();

            try
            {
                var request = getRequest(VerificaVicini);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaVicini(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce i dati sulle Entità tracciate come interferenze
        /// </summary>
        /// <param name="InfoInterferenze">Codice_Fiscale_Tecnico, Veg_Cod, Grva_Cod, Data_Inizio, Data_Fine, Sementi, SementiMappaturaLibera, 
        /// HiddenPunti, Entita_Cod</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista delle interferenze (Entità)</returns>
        /// <remarks>
        /// 
        ///     POST /InfoInterferenze
        ///     {
        ///         "Codice_Fiscale_Tecnico": "01704430519",
        ///         "Veg_Cod": 12,
        ///         "Grva_Cod": 0,
        ///         "Data_Inizio": "1/1/1900",
        ///         "Data_Fine": "31/12/2100",
        ///         "Sementi": "",
        ///         "SementiMappaturaLibera": "",
        ///         "HiddenPunti": obj JSON come stringa,
        ///         "Entita_Cod": "31344",
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Lista interferenze come obj List(Of Entita_Info_Out)</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("InfoInterferenze")]
        [ProducesResponseType(typeof(RispostaStandard<Entita_Info_Out>), StatusCodes.Status200OK)]
        public ObjectResult InfoInterferenze([FromBody] InfoInterferenze_In InfoInterferenze, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (InfoInterferenze == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto InfoInterferenze non valorizzato");

            RispostaStandard<List<Entita_Info_Out>> result = new();

            try
            {
                var request = getRequest(InfoInterferenze);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).InfoInterferenze(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Cancella una o più entità del GIS
        /// </summary>
        /// <param name="Canc">Entita_Cod, Sementi, SementiMappaturaLibera, DatiPassaggio, Elimina_Grafica, 
        /// <param name="Authorization">Token di autorizzazione</param>
        /// Elimina_Impianto, Elimina_PrecisionFarming, Elimina_PrecisionFarmingABLine, Elimina_DatoGiasPalm</param>
        /// <returns>Restituisce stringa con messaggio in base alla riuscita cancellazione</returns>
        /// <remarks>
        /// 
        ///     POST /Cancella
        ///     {
        ///         "Entita_Cod": "31344",
        ///         "Sementi": "",
        ///         "SementiMappaturaLibera": "",
        ///         "DatiPassaggio": array come stringa,
        ///         "Elimina_Grafica": True,
        ///         "Elimina_Impianto": False,
        ///         "Elimina_PrecisionFarming": False,
        ///         "Elimina_PrecisionFarmingABLine": False,
        ///         "Elimina_DatoGiasPalm": False,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Ritorna oggetto Cancella_Out</response>
        /// <response code="401">Errore autorizzazione</response>
        /// <response code="400">Errore dati in ingresso </response>
        /// <response code="406">Errore durante operazione</response>
        /// <response code="500">Errore server inaspettato</response>
        [HttpPost]
        [Route("Cancella")]
        [ProducesResponseType(typeof(RispostaStandard<Cancella_Out>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RispostaStandard<Cancella_Out>), StatusCodes.Status406NotAcceptable)]
        public ObjectResult Cancella([FromBody] Cancella_In Canc, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (Canc == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto Cancella_In non valorizzato");

            RispostaStandard<Cancella_Out> result = new();

            try
            {
                var request = getRequest(Canc);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Cancella(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status406NotAcceptable, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Cancella una lista di Entita_Grafiche nel GIS con una lista di Entita_Cod
        /// </summary>
        /// <param name="Lista_EntitaCod">List(Of Entita_Cod)</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un messaggio (composto in HTML dentro una stringa) con il numero di entità cancellate e non</returns>
        /// <remarks>
        /// 
        ///     POST /CancellaEntitaGrafiche
        ///     {
        ///         "lista_Entita_Cod": array di Entita_Cod (string),
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Numero di entità eliminate e non</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CancellaEntitaGrafiche")]
        [ProducesResponseType(typeof(RispostaStandard<Cancella_EntitaGraf_Out>), StatusCodes.Status200OK)]
        public ObjectResult CancellaEntitaGrafiche([FromBody] List<string> Lista_EntitaCod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (Lista_EntitaCod.Count == 0) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto Lista_EntitaCod non valorizzato");

            RispostaStandard<Cancella_EntitaGraf_Out> result = new();

            try
            {
                var request = getRequest(Lista_EntitaCod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CancellaEntitaGrafiche(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Cancella un elenco di hiddenPunti (entita grafiche del GIS)
        /// </summary>
        /// <param name="Lista_HiddenPunti">List(Of obj HiddenPunti)</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Messaggio con numero elementi eliminati</returns>
        /// <remarks>
        /// 
        ///     POST /CancellaMultipoint
        ///     {
        ///         "lista_HiddenPunti": array di HiddenPunti (obj JSON),
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Numero di HiddenPunti eliminate</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CancellaMultipoint")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CancellaMultipoint([FromBody] List<string> Lista_HiddenPunti, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (Lista_HiddenPunti.Count == 0) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto Lista_HiddenPunti non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(Lista_HiddenPunti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CancellaMultipoint(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Verifica che il test sull'orientamento del poligono vada a buon fine
        /// </summary>
        /// <param name="Poligono"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un messaggio di esito</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaPoligono("(44.1, 12.3) (44.1, 12.4) (44.15, 12.4) (44.15, 12.3)")
        ///     
        /// </remarks>
        /// <response code="200">"OK"</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaPoligono")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult VerificaPoligono(string Poligono, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (Poligono == "") return StatusCode(StatusCodes.Status400BadRequest, "Poligono non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(Poligono);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaPoligono(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue un salvataggio di un dato cartografico nel GIS
        /// </summary>
        /// <param name="SalvaNuovoAB">obj ChiaveAlbero, hiddenPunti_A, hiddenPunti_B, hiddenPunti_AB</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un oggetto SalvaNuovoAB_Out contenente i tre obj SalvaGrafica dei rispettivi hiddenPunti (A, B, AB)</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaNuovoAB
        ///     {
        ///         "ChiaveAlbero": obj ChiaveAlbero,
        ///         "hiddenPunti_A": stringa contenente obj HiddenPunti,
        ///         "hiddenPunti_B": stringa contenente obj HiddenPunti,
        ///         "hiddenPunti_AB": stringa contenente obj HiddenPunti,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">oggetto SalvaNuovoAB_Out</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("SalvaNuovoAB")]
        [ProducesResponseType(typeof(RispostaStandard<SalvaNuovoAB_Out>), StatusCodes.Status200OK)]
        public ObjectResult SalvaNuovoAB([FromBody] SalvaNuovoAB_In SalvaNuovoAB, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SalvaNuovoAB == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto SalvaNuovoAB non valorizzato");

            RispostaStandard<SalvaNuovoAB_Out> result = new();

            try
            {
                var request = getRequest(SalvaNuovoAB);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaNuovoAB(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce Validita_Inizio e Validita_Fine
        /// </summary>
        /// <param name="CaricaDate_Default">Sementi, SementiMappaturaLibera</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce Validita_Inizio e Validita_Fine come obj DateDefault_Out</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaDateDefault
        ///     {
        ///         "Sementi": "",
        ///         "SementiMappaturaLibera": "",
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">obj DateDefault_Out</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CaricaDateDefault")]
        [ProducesResponseType(typeof(RispostaStandard<DateDefault_Out>), StatusCodes.Status200OK)]
        public ObjectResult CaricaDateDefault([FromBody] CaricaDate_Default_In CaricaDate_Default, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (CaricaDate_Default == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto CaricaDate_Default non valorizzato");

            RispostaStandard<DateDefault_Out> result = new();

            try
            {
                var request = getRequest(CaricaDate_Default);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaDateDefault(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue un controllo delle specie permesse
        /// </summary>
        /// <param name="SementiML_SpecieVegPermessa">SementiMappaturaLibera, Veg_Cod, Finalita</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce True/False in base all'esito</returns>
        /// <remarks>
        /// 
        ///     POST /SementiMappaturaLiberaSpecieVegetalePermessa
        ///     {
        ///         "SementiMappaturaLibera": "",
        ///         "Veg_Cod": 12,
        ///         "Finalita": 0,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">True/False</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("SementiMappaturaLiberaSpecieVegetalePermessa")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SementiMappaturaLiberaSpecieVegetalePermessa([FromBody] SementiMappaturaLibera_SpecieVegPermessa_In SementiML_SpecieVegPermessa, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SementiML_SpecieVegPermessa == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto SementiML_SpecieVegPermessa non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(SementiML_SpecieVegPermessa);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SementiMappaturaLiberaSpecieVegetalePermessa(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Controlla permessi utente per poter disegnare elemento sementi
        /// </summary>
        /// <param name="DatiPassaggio"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un messaggio di esito come HTML</returns>
        /// <remarks>
        /// 
        ///     POST /SementiSportelloPossoDisegnare("0|0|0|0|-1")
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("SementiSportelloPossoDisegnare")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SementiSportelloPossoDisegnare(string DatiPassaggio, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (DatiPassaggio == "") return StatusCode(StatusCodes.Status400BadRequest, "DatiPassaggio non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(DatiPassaggio);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SementiSportelloPossoDisegnare(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// 
        /// </summary>
        /// <param name="PoligonoWKT"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista di OverLayer</returns>
        /// <remarks>
        ///
        ///     POST /CustomMapOverlayBase_InizializzaCalendario
        ///     {
        ///         "PoligonoWKT": come stringa,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">AgronicaCoreModelloInSviluppo.Gis_Sat_Sentinel_Overlay_list</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CustomMapOverlayBase_InizializzaCalendario")]
        [ProducesResponseType(typeof(RispostaStandard<Lista_GisSat_SentinelOverlay_Out>), StatusCodes.Status200OK)]
        public ObjectResult CustomMapOverlayBase_InizializzaCalendario(string PoligonoWKT, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (PoligonoWKT == "") return StatusCode(StatusCodes.Status400BadRequest, "PoligonoWKT non valorizzato");

            RispostaStandard<Lista_GisSat_SentinelOverlay_Out> result = new();

            try
            {
                var request = getRequest(PoligonoWKT);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CustomMapOverlayBase_InizializzaCalendario(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// 
        /// </summary>
        /// <param name="EntitaCod"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista di OverLayer</returns>
        /// <remarks>
        ///
        ///     POST /CustomMapOverlayBaseEntitaGIS_InizializzaCalendario
        ///     {
        ///         "EntitaCod": come stringa,
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">AgronicaCoreModelloInSviluppo.Gis_Sat_Sentinel_Overlay_list</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CustomMapOverlayBaseEntitaGIS_InizializzaCalendario")]
        [ProducesResponseType(typeof(RispostaStandard<Lista_GisSat_SentinelOverlay_Out>), StatusCodes.Status200OK)]
        public ObjectResult CustomMapOverlayBaseEntitaGIS_InizializzaCalendario(int EntitaCod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (EntitaCod == 0) return StatusCode(StatusCodes.Status400BadRequest, "EntitaCod non valorizzata");

            RispostaStandard<Lista_GisSat_SentinelOverlay_Out> result = new();

            try
            {
                var request = getRequest(EntitaCod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CustomMapOverlayBaseEntitaGIS_InizializzaCalendario(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge elenco tipologie layer
        /// </summary>
        /// <param name="AggiornaElencoTipologieIn">Parametri necessari per lettura tipologie layer</param>
        /// <returns>Restituisce un oggetto con un elenco delle tipologie layer</returns>
        /// <remarks>
        /// 
        ///     POST /AggiornaElencoTipologie3
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>

        //[HttpPost]
        //[Route("AggiornaElencoTipologie3")]
        //[ProducesResponseType(typeof(RispostaStandard<ElencoTipologieLayer>), StatusCodes.Status200OK)]
        //public ObjectResult AggiornaElencoTipologie3([FromBody] AggiornaElencoTipologie_In AggiornaElencoTipologieIn)
        //{
        //    if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);
        //    RispostaStandard<ElencoTipologieLayer> result = new();
        //    try
        //    {
        //        var request = getRequest(AggiornaElencoTipologieIn);
        //        result = new CoreWSController(hc, coreWSBaseURL, tokenChiamateWS, Request).AggiornaElencoTipologie3(request);
        //        if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.RispostaOK = false;
        //        result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
        //        return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //    return StatusCode(StatusCodes.Status200OK, result);
        //}

        /// <summary>
        /// Salva i colori da applicare ai layer o ai temi
        /// </summary>
        /// <param name="AggiornaElencoTipologieIn">Parametri necessari per lettura tipologie layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un oggetto con un elenco delle tipologie layer</returns>
        /// <remarks>
        /// 
        ///     POST /AggiornaElencoTipologie3
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("AggiornaElencoTipologie4")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoTipologieLayer>), StatusCodes.Status200OK)]
        public ObjectResult AggiornaElencoTipologie4([FromBody] AggiornaElencoTipologie_In AggiornaElencoTipologieIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoTipologieLayer> result = new();

            try
            {
                var request = getRequest(AggiornaElencoTipologieIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaElencoTipologie4(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// <param name="SalvaColoriLayer2">Tipologia, DatiLayer o DatiTema</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce "OK" se tutto va a buon fine</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaColoriLayer2
        ///     {
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Route("SalvaColoriLayer2")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SalvaColoriLayer2([FromBody] SalvaColoriLayer2_In SalvaColoriLayer2, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SalvaColoriLayer2 == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto SalvaColoriLayer2");

            if (SalvaColoriLayer2.Tipologia != 1 && SalvaColoriLayer2.Tipologia != 2)  return StatusCode(StatusCodes.Status400BadRequest, "Tipologia salvataggio non indicata");

            if (SalvaColoriLayer2.Tipologia == 1)
            {
                if (SalvaColoriLayer2.ListaDatiLayer.Count == 0) return StatusCode(StatusCodes.Status400BadRequest, "Non sono presenti layer da salvare");

                foreach (DatiLayer datiLayer in SalvaColoriLayer2.ListaDatiLayer)
                {
                    if (datiLayer.TipologiaLayer_Cod == null) return StatusCode(StatusCodes.Status400BadRequest, "Esistono layer con TipologiaLayer_Cod non definita");
                    if (datiLayer.Colore_Primario != null && datiLayer.Colore_Primario.Length != 6) return StatusCode(StatusCodes.Status400BadRequest, "Esistono layer con Colore_Primario non corretto");
                    if (datiLayer.Colore_Secondario != null && datiLayer.Colore_Secondario.Length != 6) return StatusCode(StatusCodes.Status400BadRequest, "Esistono layer con Colore_Secondario non corretto");
                }
            }

            if (SalvaColoriLayer2.Tipologia == 2)
            {
                if (SalvaColoriLayer2.ListaDatiTema.Count == 0) return StatusCode(StatusCodes.Status400BadRequest, "Non sono presenti temi da salvare");

                foreach (DatiTema datiTema in SalvaColoriLayer2.ListaDatiTema)
                {
                    if (datiTema.TipologiaLayer_Cod == null) return StatusCode(StatusCodes.Status400BadRequest, "Esistono temi con TipologiaLayer_Cod non definita");
                    if (datiTema.Colore_Primario != null && datiTema.Colore_Primario.Length != 6) return StatusCode(StatusCodes.Status400BadRequest, "Esistono temi con Colore_Primario non corretto");
                    if (datiTema.Colore_Secondario != null && datiTema.Colore_Secondario.Length != 6) return StatusCode(StatusCodes.Status400BadRequest, "Esistono temi con Colore_Secondario non corretto");
                }
            }

            RispostaStandard result = new();

            try
            {
                var request = getRequest(SalvaColoriLayer2);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaColoriLayer2(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Salva la visibilità dei layer
        /// </summary>
        /// <param name="SalvaVisibilitaLayer">Tipologia Layer, Dati Visibilità</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce "OK" se tutto va a buon fine</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaVisibilitaLayer
        ///          {
        ///            "tipologiaLayer_Cod": "1",
        ///            "datiVisibilitaLayer": [
        ///              {
        ///                "id": "1",
        ///                "flag_Visibile": 1
        ///              }
        ///            ]
        ///          }
        /// 
        /// </remarks>
        [HttpPost]
        [Route("SalvaVisibilitaLayer")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SalvaVisibilitaLayer([FromBody] SalvaVisibilitaLayer_In SalvaVisibilitaLayer, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SalvaVisibilitaLayer == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto SalvaVisibilitaLayer nullo");

            if (SalvaVisibilitaLayer.DatiVisibilitaLayer.Count == 0) return StatusCode(StatusCodes.Status400BadRequest, "Non sono presenti elementi da salvare");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(SalvaVisibilitaLayer);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaVisibilitaLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Salva la visibilità dei layer
        /// </summary>
        /// <param name="SalvaFlagVisibilitaLayer">Tipologia Layer, Dati Visibilità</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce "OK" se tutto va a buon fine</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaFlagVisibilitaLayer
        ///          {
        ///            "tipologiaLayer_Cod": "1",
        ///            "datiVisibilitaLayer": [
        ///              {
        ///                "id": "1",
        ///                "flag_Visibile": 1,
        ///                "flag_Attivo": 1
        ///              }
        ///            ]
        ///          }
        /// 
        /// </remarks>
        [HttpPost]
        [Route("SalvaFlagVisibilitaLayer")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SalvaFlagVisibilitaLayer([FromBody] SalvaFlagVisibilitaLayer_In SalvaFlagVisibilitaLayer, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SalvaFlagVisibilitaLayer == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto SalvaFlagVisibilitaLayer nullo");

            if (SalvaFlagVisibilitaLayer.DatiVisibilitaLayer.Count == 0) return StatusCode(StatusCodes.Status400BadRequest, "Non sono presenti elementi da salvare");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(SalvaFlagVisibilitaLayer);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaFlagVisibilitaLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Scrittura nuovo layer personalizzato
        /// </summary>
        /// <param name="ScriviNuovoLayerPersonalizzato">Nome layer, Mostra descrizione associata</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce codice layer inserito</returns>
        /// <remarks>
        /// 
        ///     POST /ScriviNuovoLayerPersonalizzato
        ///     {
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Route("ScriviNuovoLayerPersonalizzato")]
        [ProducesResponseType(typeof(RispostaStandard<ScriviNuovoLayerPersonalizzato_Out>), StatusCodes.Status200OK)]
        public ObjectResult ScriviNuovoLayerPersonalizzato([FromBody] ScriviNuovoLayerPersonalizzato_In ScriviNuovoLayerPersonalizzato, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (ScriviNuovoLayerPersonalizzato == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto ScriviNuovoLayerPersonalizzato nullo");

            if (string.IsNullOrEmpty(ScriviNuovoLayerPersonalizzato.NomeLayer)) return StatusCode(StatusCodes.Status400BadRequest, "Nome layer non impostato");

            RispostaStandard<ScriviNuovoLayerPersonalizzato_Out> result = new();

            try
            {
                var request = getRequest(ScriviNuovoLayerPersonalizzato);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviNuovoLayerPersonalizzato(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Eliminazione layer personalizzato
        /// </summary>
        /// <param name="customLayerCd">LayerCod</param>
        /// <returns>Restituisce un bool che indica il successo dell'operazione</returns>
        /// <remarks>
        /// 
        ///     POST /DeleteCustomLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Route(nameof(DeleteCustomLayer))]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult DeleteCustomLayer([FromBody] int customLayerCod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (customLayerCod == null) return StatusCode(StatusCodes.Status400BadRequest, "Codice layer non impostato");

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(customLayerCod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).DeleteCustomLayer(request);
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

        /// <summary>
        /// Salva nuovo elemento grafico da chiave albero con appezzamento
        /// </summary>
        /// <param name="SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza">Dati salvataggio elemento grafico</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>RispostaOK indica se il salvataggio è andato a buon fine</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza
        ///     {
        ///         "ChiaveAlbero": "string",
        ///         "HiddenPuntiNuovo": "string",
        ///         "Area": "string",
        ///         "FlagGps": "string",
        ///         "TipoOperazioneDB": 0,
        ///         "EntitaCod": 0
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Route("SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza")]
        [ProducesResponseType(typeof(RispostaStandard<Obj_SalvaGrafica>), StatusCodes.Status200OK)]
        public ObjectResult SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza([FromBody] SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_In SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza nullo");

            RispostaStandard<Obj_SalvaGrafica> result = new();

            try
            {
                var request = getRequest(SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Recupero informazioni WMS
        /// </summary>
        /// <param name="Url">Indirizzo web servizio recupero informazioni WMS</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce codice layer inserito</returns>
        /// <remarks>
        /// 
        ///     POST /WmsGetFeatureInfo("Url")
        /// 
        /// </remarks>
        [HttpPost]
        [Route("WmsGetFeatureInfo")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult WmsGetFeature(string Url, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (Url == null || Url == "") return StatusCode(StatusCodes.Status400BadRequest, "Url nullo o non impostato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(Url);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).WmsGetFeature(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge elenco struttura attributi da anagrafica layer
        /// </summary>
        /// <param name="LayerElementiGraficiCod">Codice layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce elenco struttura attributi gestiti dal layer</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiElencoStrutturaAttributiLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiElencoStrutturaAttributiLayer")]
        [ProducesResponseType(typeof(RispostaStandard<DatiStrutturaAttributiLayer>), StatusCodes.Status200OK)]
        public ObjectResult LeggiElencoStrutturaAttributiLayer(string LayerElementiGraficiCod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (LayerElementiGraficiCod == null || LayerElementiGraficiCod == "") {
                return StatusCode(StatusCodes.Status400BadRequest, "Codice elemento grafico nullo o non impostato"); 
            }

            RispostaStandard<DatiStrutturaAttributiLayer> result = new();

            try
            {
                var request = getRequest(LayerElementiGraficiCod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoStrutturaAttributiLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Salva entità ed elemento grafico con attributi
        /// </summary>
        /// <param name="SalvaEntitaConAttributiIn">Parametri ingresso</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce elenco entita / elementi grafici inseriti</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaEntitaConAttributi
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("SalvaEntitaConAttributi")]
        [ProducesResponseType(typeof(RispostaStandard<SalvaEntitaConAttributi_Out>), StatusCodes.Status200OK)]
        public ObjectResult SalvaEntitaConAttributi([FromBody] SalvaEntitaConAttributi_In SalvaEntitaConAttributiIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (SalvaEntitaConAttributiIn == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Parametri ingresso nulli");
            }

            RispostaStandard<SalvaEntitaConAttributi_Out> result = new();

            try
            {
                var request = getRequest(SalvaEntitaConAttributiIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaEntitaConAttributi(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Verifica esistenza entita per impianti
        /// </summary>
        /// <param name="VerificaEsistenzaEntitaPerImpiantiIn">Codice layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce elenco entita per impianti</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaEsistenzaEntitaPerImpianti
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaEsistenzaEntitaPerImpianti")]
        [ProducesResponseType(typeof(RispostaStandard<VerificaEsistenzaEntitaPerImpianti_Out>), StatusCodes.Status200OK)]
        public ObjectResult VerificaEsistenzaEntitaPerImpianti([FromBody] VerificaEsistenzaEntitaPerImpianti_In VerificaEsistenzaEntitaPerImpiantiIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (VerificaEsistenzaEntitaPerImpiantiIn == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Parametri di filtro nulli");
            }

            RispostaStandard<VerificaEsistenzaEntitaPerImpianti_Out> result = new();

            try
            {
                var request = getRequest(VerificaEsistenzaEntitaPerImpiantiIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaEsistenzaEntitaPerImpianti(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// AggiornaFiltroImpianti
        /// </summary>
        /// <param name="AggiornaFiltroImpiantiIn">Codice layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce elenco entita per impianti</returns>
        /// <remarks>
        /// 
        ///     POST /AggiornaFiltroImpianti
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("AggiornaFiltroImpianti")]
        [ProducesResponseType(typeof(RispostaStandard<AggiornaFiltroImpianti_Out>), StatusCodes.Status200OK)]
        public ObjectResult AggiornaFiltroImpianti([FromBody] AggiornaFiltroImpianti_In AggiornaFiltroImpiantiIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (AggiornaFiltroImpiantiIn == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Parametri aggiorna filtro impianti nulli");
            }

            RispostaStandard<AggiornaFiltroImpianti_Out> result = new();

            try
            {
                var request = getRequest(AggiornaFiltroImpiantiIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaFiltroImpianti(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Inizializza le configurazioni per i sistemi di riferimento cartografia
        /// </summary>
        /// <returns>Restituisce i dati di riferimento cartografia</returns>
        /// <remarks>
        /// 
        ///     GET /CfgGestioneSistemaDiRiferimentoPredefinito
        /// 
        /// </remarks>
        /// <response code="200">obj Cfg_GestioneSistemaRif_Out</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpGet]
        [Route("CfgGestioneSistemaDiRiferimentoPredefinito")]
        [ProducesResponseType(typeof(RispostaStandard<Cfg_GestioneSistemaRif_Out>), StatusCodes.Status200OK)]
        public ObjectResult CfgGestioneSistemaDiRiferimentoPredefinito([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<Cfg_GestioneSistemaRif_Out> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CfgGestioneSistemaDiRiferimentoPredefinito(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// <param name="impostaCfgAlbero">Parametri necessari per configurazione albero</param>
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
        [Route("ConfiguraAlbero")]
        [ProducesResponseType(typeof(RispostaStandard<CfgAlbero_CfgGisUtente>), StatusCodes.Status200OK)]
        public ObjectResult ConfiguraAlbero([FromBody] ImpostaConfigurazioneAlbero impostaCfgAlbero, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CfgAlbero_CfgGisUtente> result = new();

            try
            {
                var request = getRequest(impostaCfgAlbero);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ConfiguraAlbero(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i dati di configurazione GIS specifici per utente
        /// </summary>
        /// <returns>Restituisce oggetto configurazioni GIS utente</returns>
        /// <remarks>
        /// 
        ///     GET /CfgGIS_Leggi
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpGet]
        [Route("CfgGIS_Leggi")]
        [ProducesResponseType(typeof(RispostaStandard<ConfigurazioneGisUtente>), StatusCodes.Status200OK)]
        public ObjectResult CfgGIS_Leggi([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ConfigurazioneGisUtente> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CfgGIS_Leggi(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i dati di configurazione GIS specifici per utente
        /// </summary>
        /// <returns>Restituisce oggetto configurazioni GIS utente</returns>
        /// <remarks>
        /// 
        ///     GET /CfgGIS_Leggi
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpGet]
        [Route("CfgGIS_Leggi_Default")]
        [ProducesResponseType(typeof(RispostaStandard<ConfigurazioneGisUtente>), StatusCodes.Status200OK)]
        public ObjectResult CfgGIS_Leggi_Default([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ConfigurazioneGisUtente> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CfgGIS_Leggi_Default(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Salva i dati di configurazione GIS specifici per utente
        /// </summary>
        /// <param name="scriviCfgAlbero">Parametri per scrittura dati configurazione GIS specifici per utente</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce esito in risposta standard</returns>
        /// <remarks>
        /// 
        ///     POST /CfgGIS_Salva
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CfgGIS_Salva")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CfgGIS_Salva([FromBody] ScriviConfigurazioneGisUtente scriviCfgAlbero, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(scriviCfgAlbero);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CfgGIS_Salva(request);
                if (!result.RispostaOK)
                {
                    if(result.Errore.IndexOf("INVALID_USER") != -1) return StatusCode(StatusCodes.Status403Forbidden, result);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i dati di configurazione GIS generali
        /// </summary>
        /// <param name="leggiCfgGeneraliGis">Parametri necessari per lettura dati configurazione GIS generali</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto configurazioni GIS generali</returns>
        /// <remarks>
        /// 
        ///     POST /CfgGIS_Leggi_Generali
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CfgGIS_Leggi_Generali")]
        [ProducesResponseType (typeof (RispostaStandard<ConfigurazioniGisGenerali>), StatusCodes.Status200OK)]
        public ObjectResult CfgGIS_Leggi_Generali([FromBody] LeggiConfigurazioniGeneraliGis leggiCfgGeneraliGis, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ConfigurazioniGisGenerali> result = new();

            try
            {
                var request = getRequest(leggiCfgGeneraliGis);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CfgGIS_Leggi_Generali(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i permessi applicati al singolo Layer
        /// </summary>
        /// <param name="requestBoby">Parametri necessari per lettura elenco permessi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto LeggiPermessiLayerUtenti_Out con l'elenco per dei permessi di tutti gli utenti sullo specifico layer</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiElencoPermessiLayerUtenti
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiElencoPermessiLayerUtenti")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiPermessiLayerUtenti_Out>), StatusCodes.Status200OK)]
        public ObjectResult LeggiElencoPermessiLayerUtenti([FromBody] LeggiPermessiLayerUtenti_In requestBoby, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LeggiPermessiLayerUtenti_Out> result = new();
            try
            {
                var request = getRequest(requestBoby);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoPermessiLayerUtenti(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i permessi di un utente e dei gruppi a cui appartiene per un singolo Layer
        /// </summary>
        /// <param name="requestBoby">Parametri necessari per lettura elenco permessi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto LeggiPermessiLayerUtenti_Out con l'elenco dei permessi dell'utente desiderato sullo specifico layer</returns>
        /// <remarks>
        /// 
        ///     POST /PermessiUtenteSuSingoloLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("PermessiUtenteSuSingoloLayer")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiPermessiLayerUtenti_Out>), StatusCodes.Status200OK)]
        public ObjectResult PermessiUtenteSuSingoloLayer([FromBody] PermessiUtenteSuSingoloLayer_In requestBoby, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LeggiPermessiLayerUtenti_Out> result = new();
            try
            {
                var request = getRequest(requestBoby);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PermessiUtenteSuSingoloLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i permessi applicati al singolo Layer
        /// </summary>
        /// <param name="requestBoby">Parametri necessari per scrittura elenco permessi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto rispostastandard </returns>
        /// <remarks>
        /// 
        ///     POST /SalvaPermessiLayerUtenti
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>

        [HttpPost]
        [Route("SalvaPermessiLayerUtenti")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SalvaPermessiLayerUtenti([FromBody] SalvaPermessiLayerUtenti_In requestBoby, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();
            try
            {
                var request = getRequest(requestBoby);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaPermessiLayerUtenti(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i permessi applicati al singolo Layer
        /// </summary>
        /// <param name="requestBoby">Parametri necessari per lettura elenco permessi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto LeggiPermessiLayerGruppiUtente_Out con l'elenco per dei permessi di tutti i gruppi utente sullo specifico layer</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiElencoPermessiLayerUtenti
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiElencoPermessiLayerGruppiUtente")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiPermessiLayerGruppiUtente_Out>), StatusCodes.Status200OK)]
        public ObjectResult LeggiElencoPermessiLayerGruppiUtente([FromBody] LeggiPermessiLayerGruppiUtente_In requestBoby, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LeggiPermessiLayerGruppiUtente_Out> result = new();
            try
            {
                var request = getRequest(requestBoby);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoPermessiLayerGruppiUtente(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i permessi applicati al singolo Layer
        /// </summary>
        /// <param name="requestBoby">Parametri necessari per scrittura elenco permessi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce oggetto rispostastandard </returns>
        /// <remarks>
        /// 
        ///     POST /SalvaPermessiLayerUtenti
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="400">Errore durante l'operazione</response>

        [HttpPost]
        [Route("SalvaPermessiLayerGruppiUtente")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SalvaPermessiLayerGruppiUtente([FromBody] SalvaPermessiLayerGruppiUtente_In requestBoby, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();
            try
            {
                var request = getRequest(requestBoby);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaPermessiLayerGruppiUtente(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge l'elenco degli attributi di un layer per gestirne le impostazioni avanzate
        /// </summary>
        /// <param name="id">Id del layer, passato implicitamente nella route</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un oggetto con un elenco degli attributi del layer selezionato</returns>
        /// <remarks>
        /// 
        ///     GET /LeggiImpostazioniAvanzateLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpGet]
        [Route("LeggiImpostazioniAvanzateLayer/{id}")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiImpostazioniAvanzateLayer>), StatusCodes.Status200OK)]
        public ObjectResult LeggiImpostazioniAvanzateLayer([FromRoute] int id, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LeggiImpostazioniAvanzateLayer> result = new();

            try
            {
                var LeggiImpostazioniAvanzateLayerIn = new LeggiImpostazioniAvanzateLayer_In();
                LeggiImpostazioniAvanzateLayerIn.IdLayer = id.ToString();

                var request = getRequest(LeggiImpostazioniAvanzateLayerIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpostazioniAvanzateLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Effettua le operazioni CRUD sugli attributi di un layer
        /// </summary>
        /// <param name="AttributoLayerIn">Parametri necessari per l'esecuzione del metodo desiderato</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una risposta standard OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /AttributoLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("AttributoLayer")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult AttributoLayer([FromBody] AttributoLayer_In AttributoLayerIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(AttributoLayerIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AttributoLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Attiva o disattiva l'attributo di un layer
        /// </summary>
        /// <param name="AttivaAttributoLayerIn">Parametri necessari all'attivazione dell'attributo identificato da ID</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una risposta standard OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /ImpostaAttivazioneAttributoLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ImpostaAttivazioneAttributoLayer")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ImpostaAttivazioneAttributoLayer([FromBody] AttivaAttributoLayer_In AttivaAttributoLayerIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(AttivaAttributoLayerIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImpostaAttivazioneAttributoLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Imposta la visualizzazione etichetta per l'attributo selezionato
        /// </summary>
        /// <param name="ImpostaVisualizzazioneEtichettaIn">Parametri necessari alla visualizzazione dell'etichetta dell'attributo layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una risposta standard OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /ImpostaVisualizzazioneEtichettaLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ImpostaVisualizzazioneEtichettaLayer")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ImpostaVisualizzazioneEtichettaLayer([FromBody] ImpostaVisualizzazioneEtichetta_In ImpostaVisualizzazioneEtichettaIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(ImpostaVisualizzazioneEtichettaIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImpostaVisualizzazioneEtichetta(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Imposta la visualizzazione etichetta per l'attributo selezionato
        /// </summary>
        /// <param name="ImpostaCampoChiaveLayerIn">Parametri necessari alla visualizzazione dell'etichetta dell'attributo layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una risposta standard OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /ImpostaCampoChiaveLayer
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ImpostaCampoChiaveLayer")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ImpostaCampoChiaveLayer([FromBody] ImpostaCampoChiaveLayer_In ImpostaCampoChiaveLayerIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(ImpostaCampoChiaveLayerIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ImpostaCampoChiaveLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Aggiorna le informazioni della tabella GIS_LayerElementiGraficiXTipoOggetto
        /// </summary>
        /// <param name="AggiornaElementoGraficoPerTipoOggettoIn">Parametri necessari alla visualizzazione dell'etichetta dell'attributo layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una risposta standard OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /AggiornaElementoGraficoPerTipoOggetto
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("AggiornaElementoGraficoPerTipoOggetto")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult AggiornaElementoGraficoPerTipoOggetto([FromBody] AggiornaElementoGraficoPerTipoOggetto_In AggiornaElementoGraficoPerTipoOggettoIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(AggiornaElementoGraficoPerTipoOggettoIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AggiornaElementoGraficoPerTipoOggetto(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge le informazioni della tabella GIS_LayerElementiGraficiXTipoOggetto
        /// </summary>
        /// <param name="ElementoGraficoPerTipoOggettoIn">Parametri necessari alla visualizzazione dell'etichetta dell'attributo layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista di tipi oggetto</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiTipiOggettoPerElementoGrafico
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiTipiOggettoPerElementoGrafico")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiTipiOggettoPerElementoGrafico>), StatusCodes.Status200OK)]
        public ObjectResult LeggiTipiOggettoPerElementoGrafico([FromBody] ElementoGraficoPerTipoOggetto_In ElementoGraficoPerTipoOggettoIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LeggiTipiOggettoPerElementoGrafico> result = new();

            try
            {
                var request = getRequest(ElementoGraficoPerTipoOggettoIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiTipiOggettoPerElementoGrafico(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge la lista degli allegati data una ricetta di destinazione
        /// </summary>
        /// <param name="LeggiAllegatiDaRicettaDestinazioneIn">Parametri necessari alla visualizzazione dell'etichetta dell'attributo layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista di allegati identificati da codice / descrizione</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiAllegatiDaRicettaDestinazione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiAllegatiDaRicettaDestinazione")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiAllegatiDaRicettaDestinazione>), StatusCodes.Status200OK)]
        public ObjectResult LeggiAllegatiDaRicettaDestinazione([FromBody] LeggiAllegatiDaRicettaDestinazione_In LeggiAllegatiDaRicettaDestinazioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LeggiAllegatiDaRicettaDestinazione> result = new();

            try
            {
                var request = getRequest(LeggiAllegatiDaRicettaDestinazioneIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAllegatiDaRicettaDestinazione(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Verifica l'esistenza di un impianto e/o di un appezzamento dati i parametri chiave
        /// </summary>
        /// <param name="VerificaEsistenzaEntitaAnagraficheIn">Campi chiave dell'impianto/appezzamento di cui si vuole verificare l'esistenza</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce un oggetto contenente due valori booleani cha indicano l'esistenza dell'entità di cui si vuole eseguire la verifica</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaEsistenzaEntitaAnagrafiche
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaEsistenzaEntitaAnagrafiche")]
        [ProducesResponseType(typeof(RispostaStandard<VerificaEsistenzaEntitaAnagrafiche>), StatusCodes.Status200OK)]
        public ObjectResult VerificaEsistenzaEntitaAnagrafiche([FromBody] VerificaEsistenzaEntitaAnagrafiche_In VerificaEsistenzaEntitaAnagraficheIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<VerificaEsistenzaEntitaAnagrafiche> result = new();

            try
            {
                var request = getRequest(VerificaEsistenzaEntitaAnagraficheIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaEsistenzaEntitaAnagrafiche(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Richiede la lista delle posizioni rilevate nel report percorsi dei tecnici sul campo
        /// </summary>
        /// <param name="LettureTecniciInCampoIn">Oggetto contenente l'intervallo di date in cui effettuare la ricerca</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista delle posizioni rilevate nel report percorsi dei tecnici sul campo</returns>
        /// <remarks>
        /// 
        ///     POST /PosizioniRilevate
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("PosizioniRilevate")]
        [ProducesResponseType(typeof(RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>), StatusCodes.Status200OK)]
        public ObjectResult PosizioniRilevate([FromBody] LettureTecniciInCampo_In LettureTecniciInCampoIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> result = new();

            try
            {
                var request = getRequest(LettureTecniciInCampoIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).PosizioniRilevate(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Richiede la lista delle ultime posizioni dei tecnici sul campo
        /// </summary>
        /// <param name="LettureTecniciInCampoIn">Oggetto contenente l'intervallo di date in cui effettuare la ricerca</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista delle ultime posizioni rilevate dei tecnici sul campo</returns>
        /// <remarks>
        /// 
        ///     POST /UltimaPosizione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("UltimaPosizione")]
        [ProducesResponseType(typeof(RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>), StatusCodes.Status200OK)]
        public ObjectResult UltimaPosizione([FromBody] LettureTecniciInCampo_In LettureTecniciInCampoIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> result = new();

            try
            {
                var request = getRequest(LettureTecniciInCampoIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).UltimaPosizione(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Dati relativi ad un sesonsore satellitare
        /// </summary>
        /// <param name="LetturaSensoridIn">Oggetto contenente l'intervallo di date in cui effettuare la ricerca</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Restituisce una lista di dati per sensore</returns>
        /// <remarks>
        /// 
        ///     POST /UltimaPosizione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LetturaDatiElaboratiSuSensoreListaValori")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LetturaDatiElaboratiSuSensoreListaValori([FromBody] LetturaDatiElaboratiSuSensoreListaValori_In LetturaSensoridIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var request = getRequest(LetturaSensoridIn);
            RispostaStandard result = new();

            try
            {

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LetturaDatiElaboratiSuSensoreListaValori(request);
                if (!result.RispostaOK)
                {
                    if(result.RispostaStringa.IndexOf("BAD_REQUEST") != -1)
                    {
                        result.RispostaStringa = result.RispostaStringa.Replace("BAD_REQUEST - ", "");
                        return StatusCode(StatusCodes.Status400BadRequest, result);
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue una operazione su di una lista di oggetti di tipo TipologiaLabel 
        /// </summary>
        /// <param name="LayerTilesDescrizioneIn">Parametro contenente la lista di oggetti su cui eseguire l'operazione richiesta</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /LayerTilesDescrizione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LayerTilesDescrizione")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult LayerTilesDescrizione([FromBody] LayerTilesDescrizione_In LayerTilesDescrizioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var request = getRequest(LayerTilesDescrizioneIn);
            RispostaStandard result = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LayerTilesDescrizione(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che legge una lista di oggetti di tipo TipologiaLabel in base ai parametri chiave
        /// </summary>
        /// <param name="LeggiLayerTilesDescrizioneIn"> Parametro contenente i valori chiave con cui eseguire la select</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di oggetti di tipo TipologiaLabel che corrispondono ai parametri di ricerca</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiLayerTilesDescrizione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiLayerTilesDescrizione")]
        [ProducesResponseType(typeof(RispostaStandard<List<TipologiaLabel>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiLayerTilesDescrizione([FromBody] LeggiLayerTilesDescrizione_In LeggiLayerTilesDescrizioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var request = getRequest(LeggiLayerTilesDescrizioneIn);
            RispostaStandard<List<TipologiaLabel>> result = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiLayerTilesDescrizione(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che legge una lista di sistemi di riferimento
        /// </summary>
        /// <param name="LeggiSistemiRiferimentoIn"> Parametro contenente i valori chiave con cui eseguire la select</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di sistemi di riferimento che corrispondono ai parametri di ricerca</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiSistemiRiferimento
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiSistemiRiferimento")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiSistemiRiferimento([FromBody] LeggiSistemiRiferimento_In LeggiSistemiRiferimentoIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var request = getRequest(LeggiSistemiRiferimentoIn);
            RispostaStandard<string> result = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSistemiRiferimento(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che carica un file shape
        /// </summary>
        /// <param name="CaricaFileShapeIn"> Dati del file da caricare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaFileShape
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK/KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CaricaFileShape")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CaricaFileShape([FromForm] CaricaFileShape_In CaricaFileShapeIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                using (var str = new System.IO.MemoryStream())
                {
                    CaricaFileShapeIn.fileZip.CopyTo(str);
                    var request = getRequest(new CaricaFileCompletoCoreWS_In(CaricaFileShapeIn.codice_sistemaRiferimento,
                                                                             str.ToArray(),
                                                                             CaricaFileShapeIn.fileZip.FileName,
                                                                             CaricaFileShapeIn.layer_cod,
                                                                             CaricaFileShapeIn.tipologiaShape_cod,
                                                                             CaricaFileShapeIn.progressivoGIAS,
                                                                             CaricaFileShapeIn.Validita_Inizio,
                                                                             CaricaFileShapeIn.Validita_Fine,
                                                                             CaricaFileShapeIn.Description,
                                                                             CaricaFileShapeIn.PixelSize,
                                                                             CaricaFileShapeIn.datiImpianto,
                                                                             null));

                    result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaFileShape(request);
                    if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }
        /*
                /// <summary>
                /// Metodo che carica un file raster
                /// </summary>
                /// <param name="CaricaFileShapeIn"> Dati del file da caricare</param>
                /// <returns>OK/KO</returns>
                /// <remarks>
                /// 
                ///     POST /CaricaFileRaster
                ///     {
                ///     }
                /// 
                /// </remarks>
                /// <response code="200">OK/KO</response>
                /// <response code="401">Utente non autorizzato</response>
                /// <response code="500">Errore durante l'operazione</response>
                [HttpPost]
                [Route("CaricaFileRaster")]
                [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
                public ObjectResult CaricaFileRaster([FromForm] CaricaFileShape_In CaricaFileShapeIn)
                {
                    if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);

                    RispostaStandard result = new();

                    try
                    {
                        using (var str = new System.IO.MemoryStream())
                        {
                            CaricaFileShapeIn.fileZip.CopyTo(str);
                            var request = getRequest(new CaricaFileCompletoCoreWS_In(CaricaFileShapeIn.codice_sistemaRiferimento,
                                                                                     str.ToArray(),
                                                                                     CaricaFileShapeIn.fileZip.FileName,
                                                                                     CaricaFileShapeIn.layer_cod,
                                                                                     CaricaFileShapeIn.tipologiaShape_cod,
                                                                                     CaricaFileShapeIn.dati_catastali,
                                                                                     CaricaFileShapeIn.progressivoGIAS,
                                                                                     CaricaFileShapeIn.Validita_Inizio,
                                                                                     CaricaFileShapeIn.Validita_Fine,
                                                                                     CaricaFileShapeIn.Description,
                                                                                     CaricaFileShapeIn.datiImpianto,
                                                                                     null));

                            result = new CoreWSController(hc, coreWSBaseURL, tokenChiamateWS, Request).CaricaFileShape(request);
                            if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.RispostaOK = false;
                        result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                        return StatusCode(StatusCodes.Status500InternalServerError, result);
                    }

                    return StatusCode(StatusCodes.Status200OK, result);
                }
        */
        /// <summary>
        /// Metodo che carica un file shape con informazioni catastali
        /// </summary>
        /// <param name="CaricaFileCompletoIn"> Dati del file da caricare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaFileCatasto
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK/KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>

        [HttpPost]
        [Route("CaricaFileCatasto")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CaricaFileCatasto([FromForm] CaricaFileShapeCompleto_In CaricaFileCompletoIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                using (var str = new System.IO.MemoryStream())
                {
                    CaricaFileCompletoIn.fileZip.CopyTo(str);
                    var request = getRequest(new CaricaFileCompletoCoreWS_In(CaricaFileCompletoIn.codice_sistemaRiferimento,
                                                                             str.ToArray(),
                                                                             CaricaFileCompletoIn.fileZip.FileName,
                                                                             CaricaFileCompletoIn.layer_cod,
                                                                             CaricaFileCompletoIn.tipologiaShape_cod,
                                                                             CaricaFileCompletoIn.progressivoGIAS,
                                                                             CaricaFileCompletoIn.Validita_Inizio,
                                                                             CaricaFileCompletoIn.Validita_Fine,
                                                                             CaricaFileCompletoIn.Description,
                                                                             0,
                                                                             CaricaFileCompletoIn.datiImpianto,
                                                                             CaricaFileCompletoIn.datiCatasto));

                    result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaFileShape(request);
                    if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce una lista di algoritmi di proiezione
        /// </summary>
        /// <returns>Elenco degli algoritmi di proiezione</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /LeggiAlgoritmiProiezione
        ///     
        /// </remarks>
        /// <response code="200">Lista algoritmi</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpGet]
        [Route("LeggiAlgoritmiProiezione")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoAlgoritmi>), StatusCodes.Status200OK)]
        public ObjectResult LeggiAlgoritmiProiezione([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoAlgoritmi> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAlgoritmiProiezione(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce una lista di configurazioni di proiezione
        /// </summary>
        /// <returns>Elenco delle configurazioni di proiezione</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /LeggiConfigurazioniProiezione
        ///     
        /// </remarks>
        /// <response code="200">Lista configurazioni</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpGet]
        [Route("LeggiConfigurazioniProiezione")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoConfigurazioniProiezione>), StatusCodes.Status200OK)]
        public ObjectResult LeggiConfigurazioniProiezione([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoConfigurazioniProiezione> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiConfigurazioniProiezione(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce una lista di configurazioni di proiezione
        /// </summary>
        /// <param name="cfg"> Rappresenta la configurazione su cui andare a filtrare l'elenco delle configurazioni, in formato JSON</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Elenco delle configurazioni di proiezione</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiConfigurazioniProiezioneFiltrati
        ///     
        /// </remarks>
        /// <response code="200">Lista configurazioni</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiConfigurazioniProiezioneFiltrati")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoConfigurazioniProiezione>), StatusCodes.Status200OK)]
        public ObjectResult LeggiConfigurazioniProiezioneFiltrati([FromBody] string cfg, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoConfigurazioniProiezione> result = new();

            try
            {
                var request = getRequest(cfg);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiConfigurazioniProiezioneFiltrati(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce una lista di configurazioni di proiezione applicabili ad un Layer/ElementoGrafico
        /// </summary>
        /// <param name="leggiElencoConfigurazioniIn"> Parametro contenente il layer / l'ElementoGrafico da filtrare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Elenco delle configurazioni di proiezione applicabili al Layer/ElementoGrafico richiesto</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiConfigurazioniProiezioneSuLayer
        ///     
        /// </remarks>
        /// <response code="200">Lista configurazioni</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiConfigurazioniProiezioneSuLayer")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoConfigurazioniSuLayer>), StatusCodes.Status200OK)]
        public ObjectResult LeggiConfigurazioniProiezioneSuLayer([FromBody] LeggiElencoConfigurazioni_In leggiElencoConfigurazioniIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoConfigurazioniSuLayer> result = new();

            try
            {
                var request = getRequest(leggiElencoConfigurazioniIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiConfigurazioniProiezioneSuLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che effettua un salvataggio di una configurazione di proiezione tra layer
        /// </summary>
        /// <param name="ConfigurazioneProiezioneIn"> Parametro contenente i valori chiave con cui eseguire la select</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /SalvaConfigurazioneProiezione
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK/KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("SalvaConfigurazioneProiezione")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SalvaConfigurazioneProiezione([FromBody] ConfigurazioneProiezione ConfigurazioneProiezioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var request = getRequest(ConfigurazioneProiezioneIn);
            RispostaStandard result = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SalvaConfigurazioneProiezione(request);

                if (!result.RispostaOK)
                {
                    if (result.Errore.Equals("NOT_ALLOWED")) return StatusCode(StatusCodes.Status401Unauthorized, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route(nameof(ActivateAlgorithm))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ActivateAlgorithm([FromBody] ConfigurazioneProiezione ConfigurazioneProiezioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var request = getRequest(ConfigurazioneProiezioneIn);
            RispostaStandard result = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ActivateAlgorithm(request);

                if (!result.RispostaOK)
                {
                    if (result.Errore.Equals("NOT_ALLOWED")) return StatusCode(StatusCodes.Status401Unauthorized, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route(nameof(SaveAlgorithmConfigurationCfg))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult SaveAlgorithmConfigurationCfg([FromBody] ConfigurazioneProiezione ConfigurazioneProiezioneIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var request = getRequest(ConfigurazioneProiezioneIn);
            RispostaStandard result = new();

            try
            {
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SaveAlgorithmConfigurationCfg(request);

                if (!result.RispostaOK)
                {
                    if (result.Errore.Equals("NOT_ALLOWED")) return StatusCode(StatusCodes.Status401Unauthorized, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce una lista di permessi utente per le configurazioni di proiezione
        /// </summary>
        /// <param name="LayerAnalysisConfig_Cod"> Codice della configurazione</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Elenco dei permessi utente per le configurazioni di proiezione</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiPermessiUtenteConfigurazioni
        ///     
        /// </remarks>
        /// <response code="200">Lista permessi utente</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiPermessiUtenteConfigurazioni")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiConfigurazione>), StatusCodes.Status200OK)]
        public ObjectResult LeggiPermessiUtenteConfigurazioni([FromBody] Int32 LayerAnalysisConfig_Cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoPermessiConfigurazione> result = new();

            try
            {
                var request = getRequest(LayerAnalysisConfig_Cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPermessiUtenteConfigurazioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce una lista di permessi dei gruppi utente per le configurazioni di proiezione
        /// </summary>
        /// <param name="LayerAnalysisConfig_Cod"> Codice della configurazione</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Elenco dei permessi dei gruppi utente per le configurazioni di proiezione</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiPermessiGruppiUtenteConfigurazioni
        ///     
        /// </remarks>
        /// <response code="200">Lista permessi dei gruppi utente</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiPermessiGruppiUtenteConfigurazioni")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiConfigurazione>), StatusCodes.Status200OK)]
        public ObjectResult LeggiPermessiGruppiUtenteConfigurazioni([FromBody] Int32 LayerAnalysisConfig_Cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoPermessiConfigurazione> result = new();

            try
            {
                var request = getRequest(LayerAnalysisConfig_Cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPermessiGruppiUtenteConfigurazioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce la lista di permessi per le configurazioni di proiezione dell'utente loggato
        ///     I permessi restituiti sono la combinazione (OR) dei permessi dell'utente e di tutti i gruppi
        ///     a cui appartiene
        /// </summary>
        /// <returns>Elenco dei permessi utente per le configurazioni di proiezione</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /LeggiPermessiConfigurazioniDaUtente
        ///     
        /// </remarks>
        /// <response code="200">Lista permessi utente</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpGet]
        [Route("LeggiPermessiConfigurazioniDaUtente")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiUtente>), StatusCodes.Status200OK)]
        public ObjectResult LeggiPermessiConfigurazioniDaUtente([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoPermessiUtente> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPermessiConfigurazioniDaUtente(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue una serie di operazioni sui permessi utente per le configurazioni di proiezione
        /// </summary>
        /// <param name="elencoModifiche"> Codice della configurazione</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /OperazioniPermessiProiezioni
        ///     
        /// </remarks>
        /// <response code="200">Operazione completata con successo</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("OperazioniPermessiProiezioni")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiConfigurazione>), StatusCodes.Status200OK)]
        public ObjectResult OperazioniPermessiProiezioni([FromBody] ModifichePermessiConfigurazione elencoModifiche, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(elencoModifiche);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).OperazioniPermessiProiezioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Attiva o disattiva una configurazione su di un layer oppure su di un poligono
        /// </summary>
        /// <param name="attivazioneConfigurazione_In"> Contiene le informazioni sulla configurazione da attivare/disattivare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /AttivaDisattivaConfigurazione
        ///     
        /// </remarks>
        /// <response code="200">OK/KO</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("AttivaDisattivaConfigurazione")]
        [ProducesResponseType(typeof(RispostaStandard<AttivaDisattivaConfigurazione_Out>), StatusCodes.Status200OK)]
        public ObjectResult AttivaDisattivaConfigurazione([FromBody] AttivazioneConfigurazioneAlgoritmiCartografici attivazioneConfigurazione_In, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AttivaDisattivaConfigurazione_Out> result = new();

            try
            {
                var request = getRequest(attivazioneConfigurazione_In);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AttivaDisattivaConfigurazione(request);
                if (!result.RispostaOK)
                {
                    if(result.Errore.Equals("NOT_ALLOWED")) return StatusCode(StatusCodes.Status403Forbidden, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge la lista di esecuzioni configurazione per una data entità
        /// </summary>
        /// <returns>Lista di esecuzioni configurazione</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiLogEsecuzioniConfigurazione
        ///     
        /// </remarks>
        /// <response code="200">lista di esecuzioni configurazione</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiLogEsecuzioniConfigurazione")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoLogEsecuzioniConfigurazioniProiezione>), StatusCodes.Status200OK)]
        public ObjectResult LeggiLogEsecuzioniConfigurazione([FromBody] Int32 entita_cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard< ElencoLogEsecuzioniConfigurazioniProiezione> result = new();

            try
            {
                var request = getRequest(entita_cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiLogEsecuzioniConfigurazione(request);
                if (!result.RispostaOK)
                {
                    if (result.Errore.Equals("NOT_ALLOWED")) return StatusCode(StatusCodes.Status403Forbidden, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge la lista di Maschere presenti per un dato layer raster
        /// </summary>
        /// <param name="maschereLayerFiltroIn"> Contiene i dati del layer per il quale viene richiesta la lista delle maschere</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di maschere</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiMaschereLayerRaster
        ///     
        /// </remarks>
        /// <response code="200">lista di maschere</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiMaschereLayerRaster")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoMaschereLayerRaster>), StatusCodes.Status200OK)]
        public ObjectResult LeggiMaschereLayerRaster([FromBody] MaschereLayerFiltro_In maschereLayerFiltroIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoMaschereLayerRaster> result = new();

            try
            {
                var request = getRequest(maschereLayerFiltroIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMaschereLayerRaster(request);
                if (!result.RispostaOK)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue una operazione su di una maschera:
        /// Elenco operazioni:
        ///     - INSERT = 1
		///     - UPDATE = 2
		///     - DELETE = 3
        /// </summary>
        /// <param name="operazioneMascheraLayerRasterIn"> Contiene i dati della maschera e l'operazione richiesta sui dati in input</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /OperazioniMaschereLayerRaster
        ///     
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("OperazioniMaschereLayerRaster")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoMaschereLayerRaster>), StatusCodes.Status200OK)]
        public ObjectResult OperazioniMaschereLayerRaster([FromBody] OperazioneMascheraLayerRaster_In operazioneMascheraLayerRasterIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(operazioneMascheraLayerRasterIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).OperazioniMaschereLayerRaster(request);
                if (!result.RispostaOK)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge la lista di permessi utente su di una data maschera
        /// </summary>
        /// <param name="maschera_Cod"> Il codice della maschera di cui si richiedono i permessi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista permessi utente per la maschera richiesta</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiPermessiUtenteMaschera
        ///     
        /// </remarks>
        /// <response code="200">lista di permessi utente</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiPermessiUtenteMaschera")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiMaschera>), StatusCodes.Status200OK)]
        public ObjectResult LeggiPermessiUtenteMaschera([FromBody] Int32 maschera_Cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoPermessiMaschera> result = new();

            try
            {
                var request = getRequest(maschera_Cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPermessiUtenteMaschera(request);
                if (!result.RispostaOK)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge la lista di permessi per i gruppi utente su di una data maschera
        /// </summary>
        /// <param name="maschera_Cod"> Il codice della maschera di cui si richiedono i permessi</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista permessi per i gruppi utente sulla maschera richiesta</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /LeggiPermessiGruppiUtenteMaschera
        ///     
        /// </remarks>
        /// <response code="200">lista di permessi per i gruppi utente</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("LeggiPermessiGruppiUtenteMaschera")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiMaschera>), StatusCodes.Status200OK)]
        public ObjectResult LeggiPermessiGruppiUtenteMaschera([FromBody] Int32 maschera_Cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoPermessiMaschera> result = new();

            try
            {
                var request = getRequest(maschera_Cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPermessiGruppiUtenteMaschera(request);
                if (!result.RispostaOK)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue le operazioni CRUD su di una lista di permessi utente / gruppi utente per una data maschera
        /// </summary>
        /// <param name="modifichePermessiMaschera"> Contiene tre liste per le operazioni principali di insert/update/delete</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /OperazioniPermessiMaschera
        ///     
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        [HttpPost]
        [Route("OperazioniPermessiMaschera")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiMaschera>), StatusCodes.Status200OK)]
        public ObjectResult OperazioniPermessiMaschera([FromBody] ModifichePermessiMaschera modifichePermessiMaschera, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(modifichePermessiMaschera);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).OperazioniPermessiMaschera(request);
                if (!result.RispostaOK)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Attiva / disattiva una maschera per un dato utente
        /// </summary>
        /// <param name="attivazioneMascheraLayerRaster"> Contiene la maschera richiesta ed un boolean per richiedere l'attivazione/disattivazione</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /AttivaDisattivaMaschera
        ///     
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="500">Errore interno</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        [HttpPost]
        [Route("AttivaDisattivaMaschera")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoPermessiMaschera>), StatusCodes.Status200OK)]
        public ObjectResult AttivaDisattivaMaschera([FromBody] AttivazioneMascheraLayerRaster attivazioneMascheraLayerRaster, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(attivazioneMascheraLayerRaster);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).AttivaDisattivaMaschera(request);
                if (!result.RispostaOK)
                {
                    if (result.Errore.Equals("NOT_ALLOWED")) return StatusCode(StatusCodes.Status403Forbidden, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("RecuperaStaticMapsSuElencoEntita")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult RecuperaStaticMapsSuElencoEntita([FromBody] ElencoEntitaCodXRecuperoStaticMaps elenco, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            if (elenco == null) return StatusCode(StatusCodes.Status400BadRequest, "Oggetto ElencoEntitaCodXRecuperoStaticMaps non valorizzato");

            RispostaStandard result = new();

            try
            {
                var request = getRequest(elenco);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ElencoEntitaCodXRecuperoStaticMaps(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge la lista degli allegati dato un layer
        /// </summary>
        /// <param name="leggiDatiLayerIn">Dati del layer di cui si richiede la lista degli allegati</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista allegati del layer</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiAllegatiLayer
        /// 
        /// </remarks>
        /// <response code="200">Lista allegati del layer</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiAllegatiLayer")]
        [ProducesResponseType(typeof(RispostaStandard<ElencoAllegatiLayer_Out>), StatusCodes.Status200OK)]
        public ObjectResult LeggiAllegatiLayer([FromBody] LeggiDatiLayer_In leggiDatiLayerIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<ElencoAllegatiLayer_Out> result = new();

            try
            {
                var request = getRequest(leggiDatiLayerIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAllegatiLayer(request);
                if (!result.RispostaOK)
                {
                    if (result.Errore.IndexOf("INVALID_USER") != -1) return StatusCode(StatusCodes.Status403Forbidden, result);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i dati dell'allegato da codice
        /// </summary>
        /// <param name="allegati_Documenti_Cod">Codice Allegato</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Dati dell'allegato per il download</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiAllegatoDocumento
        /// 
        /// </remarks>
        /// <response code="200">Dati dell'allegato per il download</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiAllegatoDocumento")]
        [ProducesResponseType(typeof(RispostaStandard<AllegatoFile>), StatusCodes.Status200OK)]
        public ObjectResult LeggiAllegatoDocumento([FromBody] int allegati_Documenti_Cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<AllegatoFile> result = new();

            try
            {
                var request = getRequest(allegati_Documenti_Cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAllegatoDocumento(request);
                if (!result.RispostaOK)
                {
                    if (result.Errore.IndexOf("INVALID_USER") != -1) return StatusCode(StatusCodes.Status403Forbidden, result);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esporta gli shape di una lista di entità identificate da codice per un dato layer
        /// </summary>
        /// <param name="esportaShapeEntitaIn">Elenco entità da esportare con il layer di appartenenza</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /EsportaShapeEntita
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("EsportaShapeEntita")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult EsportaShapeEntita([FromBody] EsportaShapeEntita_In esportaShapeEntitaIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(esportaShapeEntitaIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).EsportaShapeEntita(request);
                if (!result.RispostaOK)
                {
                    if(result.Errore.IndexOf("INVALID_USER") != -1) return StatusCode(StatusCodes.Status403Forbidden, result);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        //[HttpPost]
        //[Route("TestElaborazioneAlgoritmo")]
        //[ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        //public ObjectResult TestElaborazioneAlgoritmo()
        //{
        //    if (!isAuthorized()) return StatusCode(StatusCodes.Status401Unauthorized, null);

        //    RispostaStandard result = new();

        //    try
        //    {
        //        var request = getRequest("");
        //        result = new CoreWSController(hc, coreWSBaseURL, tokenChiamateWS, Request).TestElaborazioneAlgoritmo(request);
        //        if (!result.RispostaOK)
        //        {
        //            if (result.Errore.Equals("NOT_ALLOWED")) return StatusCode(StatusCodes.Status401Unauthorized, null);
        //            return StatusCode(StatusCodes.Status500InternalServerError, result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.RispostaOK = false;
        //        result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
        //        return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }

        //    return StatusCode(StatusCodes.Status200OK, result);
        //}

        /// <summary>
        /// Modifica i dati di una estrazione shape
        /// </summary>
        /// <param name="allegato">Dati da modificare dell'allegato</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /ModificaEstrazioneShape
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("ModificaEstrazioneShape")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult ModificaEstrazioneShape([FromBody] AllegatoLayerModifica allegato, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(allegato);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ModificaEstrazioneShape(request);
                if (!result.RispostaOK)
                {
                    if (result.Errore.IndexOf("INVALID_USER") != -1) return StatusCode(StatusCodes.Status403Forbidden, result);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Endpoint che riceve i dati delle elaborazioni di Piano Concimazione eseguite dalla piattaforma GEE
        /// </summary>
        /// <param name="elaborazione">Risultati dell'esecuzione.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /NuovaElaborazionePianoConcimazioneGEE
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("NuovaElaborazionePianoConcimazioneGEE")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult NuovaElaborazionePianoConcimazioneGEE([FromBody] PianoConcimazioneGEE elaborazione, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(elaborazione);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).NuovaElaborazionePianoConcimazioneGEE(request);
                if (!result.RispostaOK)
                {
                    if (result.Errore.IndexOf("INVALID_USER") != -1) return StatusCode(StatusCodes.Status403Forbidden, result);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Endpoint che riceve i dati delle elaborazioni eseguite dalla piattaforma GEE
        /// </summary>
        /// <param name="elaborazione">Risultati dell'esecuzione.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /NuovaElaborazionePiattaformaGEE
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("NuovaElaborazionePiattaformaGEE")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult NuovaElaborazionePiattaformaGEE([FromBody] NuovaElaborazioneGEE elaborazione, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(elaborazione);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).NuovaElaborazionePiattaformaGEE(request);
                if (!result.RispostaOK)
                {
                    if (result.Errore.IndexOf("INVALID_USER") != -1) return StatusCode(StatusCodes.Status403Forbidden, result);
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Endpoint che riceve i dati delle elaborazioni eseguite dalla piattaforma GEE
        /// </summary>
        /// <param name="satCloudEvent">Risultati dell'esecuzione.</param>
        /// <param name="fmisContext">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /NuovaElaborazionePiattaformaSAT
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("NuovaElaborazionePiattaformaSAT")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<ObjectResult> NuovaElaborazionePiattaformaSAT([FromBody] SatCloudEvent satCloudEvent, [FromHeader(Name = "fmis-context")] string fmisContext)
        {
            RispostaStandard result = new();

            try
            {
                if (satCloudEvent == null)
                {
                    result.RispostaOK = false;
                    result.Errore = "Request body is required.";
                    return StatusCode(StatusCodes.Status400BadRequest, result);
                }

                var backendBearerToken = await GetBearerTokenViaBackendAuthentication(fmisContext);

                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}".TrimEnd('/');
                var internalUrl = $"{baseUrl}/Gis/NuovaElaborazionePiattaformaSAT_Internal";

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", backendBearerToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(satCloudEvent), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(internalUrl, jsonContent);

                var responseBody = await response.Content.ReadAsStringAsync();
                object responseObject;
                try
                {
                    responseObject = JsonConvert.DeserializeObject(responseBody);
                }
                catch
                {
                    responseObject = responseBody;
                }

                return StatusCode((int)response.StatusCode, responseObject);
            }
            catch (ValidationException vex)
            {
                result.RispostaOK = false;
                result.Errore = vex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (UnauthorizedAccessException uex)
            {
                result.RispostaOK = false;
                result.Errore = uex.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }


        
        /// <summary>
        /// Endpoint che riceve i dati delle elaborazioni eseguite dalla piattaforma GEE
        /// </summary>
        /// <param name="satCloudEvent">Risultati dell'esecuzione.</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /NuovaElaborazionePiattaformaSAT_Internal
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="403">Operazione non autorizzata</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("NuovaElaborazionePiattaformaSAT_Internal")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult NuovaElaborazionePiattaformaSAT_Internal([FromBody] SatCloudEvent satCloudEvent, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
            
            RispostaStandard result = new();

            try
            {
                var request = getRequest(satCloudEvent);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).NuovaElaborazionePiattaformaSAT(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Endpoint webhook non autenticato che riceve le notifiche di esito dall'Engine Mappe Prescrizione.
        /// Autentica il contesto tramite fmis-context e inoltra la richiesta all'endpoint interno.
        /// </summary>
        /// <param name="mappePrescrizioneCloudEvent">Payload CloudEvent ricevuto dall'Engine.</param>
        /// <param name="fmisContext">Contesto di autenticazione FMIS (Base64-encoded BackgroundAuthenticationModel).</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /NuovaElaborazioneMappePrescrizione
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="400">Payload mancante o non valido</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("NuovaElaborazioneMappePrescrizione")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<ObjectResult> NuovaElaborazioneMappePrescrizione([FromBody] MappePrescrizioneCloudEvent mappePrescrizioneCloudEvent, [FromHeader(Name = "fmis-context")] string fmisContext)
        {
            RispostaStandard result = new();

            try
            {
                if (mappePrescrizioneCloudEvent == null)
                {
                    result.RispostaOK = false;
                    result.Errore = "Request body is required.";
                    return StatusCode(StatusCodes.Status400BadRequest, result);
                }

                var backendBearerToken = await GetBearerTokenViaBackendAuthentication(fmisContext);

                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}".TrimEnd('/');
                var internalUrl = $"{baseUrl}/Gis/NuovaElaborazioneMappePrescrizione_Internal";

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", backendBearerToken);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(mappePrescrizioneCloudEvent), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(internalUrl, jsonContent);

                var responseBody = await response.Content.ReadAsStringAsync();
                object responseObject;
                try
                {
                    responseObject = JsonConvert.DeserializeObject(responseBody);
                }
                catch
                {
                    responseObject = responseBody;
                }

                return StatusCode((int)response.StatusCode, responseObject);
            }
            catch (ValidationException vex)
            {
                result.RispostaOK = false;
                result.Errore = vex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (UnauthorizedAccessException uex)
            {
                result.RispostaOK = false;
                result.Errore = uex.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        /// <summary>
        /// Endpoint interno autenticato che processa la notifica webhook dall'Engine Mappe Prescrizione
        /// e la inoltra al layer SOAP tramite CoreWSController.
        /// </summary>
        /// <param name="mappePrescrizioneCloudEvent">Payload CloudEvent ricevuto dall'Engine.</param>
        /// <param name="Authorization">Token di autorizzazione Bearer.</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /NuovaElaborazioneMappePrescrizione_Internal
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("NuovaElaborazioneMappePrescrizione_Internal")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult NuovaElaborazioneMappePrescrizione_Internal([FromBody] MappePrescrizioneCloudEvent mappePrescrizioneCloudEvent, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(mappePrescrizioneCloudEvent);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).NuovaElaborazionePiattaformaMappePrescrizione(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Endpoint che restituisce il baseURL per l'endpoint mappe satellitari
        /// </summary>
        /// <returns>stringa contenente il baseUrl dell'endpoint per le mappe satellitari</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiBaseUrlMappeSatellitari
        /// 
        /// </remarks>
        /// <response code="200">stringa contenente il baseUrl dell'endpoint per le mappe satellitari</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiBaseUrlMappeSatellitari")]
        [ProducesResponseType(typeof(RispostaStandard<EndpointMappeSatellitari>), StatusCodes.Status200OK)]
        public ObjectResult LeggiBaseUrlMappeSatellitari([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard< EndpointMappeSatellitari> result = new();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiBaseUrlMappeSatellitari(request);
                if (!result.RispostaOK)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Elimina il contenuto di un layer a patto che nessuno degli elementi sia stato usato in una esecuzione di algoritmo
        /// </summary>
        /// <param name="LayerElementiGrafici_Cod">Codice Layer</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /EliminazioneTotaleDatiLayer
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="400">Codice Layer non consentito</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("EliminazioneTotaleDatiLayer")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult EliminazioneTotaleDatiLayer([FromBody] int LayerElementiGrafici_Cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                if(LayerElementiGrafici_Cod < 1000000)
                {
                    result.RispostaOK = false;
                    result.Errore = "Codice Layer non consentito.";
                    return StatusCode(StatusCodes.Status400BadRequest, result);
                }

                var request = getRequest(LayerElementiGrafici_Cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).EliminazioneTotaleDatiLayer(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue l'operazione desiderata su di un bookmark
        /// </summary>
        /// <param name="operazioniBookmarkIn">Bookmark su cui operare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /OperazioniBookmark
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("OperazioniBookmark")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult OperazioniBookmark([FromBody] OperazioniBookmark_In operazioniBookmarkIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(operazioniBookmarkIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).OperazioniBookmark(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i dati di un bookmark specifico oppure la lista dei bookmark per l'utente loggato
        /// </summary>
        /// <param name="bookmark_cod">Bookmark da leggere oppure 0 per leggere tutti i bookmark dell'utente</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista dei bookmark desiderati</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiBookmark
        /// 
        /// </remarks>
        /// <response code="200">Lista dei bookmark desiderati</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiBookmark")]
        [ProducesResponseType(typeof(RispostaStandard<List<Bookmark>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiBookmark([FromBody] Int32 bookmark_cod, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Bookmark>> result = new();

            try
            {
                var request = getRequest(bookmark_cod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiBookmark(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Legge i dati delle MUZ che si trovano in una area
        /// </summary>
        /// <param name="leggiDatiMUZVisibiliIn">Area in cui effettuare la ricerca di appezzamenti</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista delle MUZ desiderate</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiDatiMUZVisibili
        /// 
        /// </remarks>
        /// <response code="200">Lista delle MUZ desiderate</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiDatiMUZVisibili")]
        [ProducesResponseType(typeof(RispostaStandard<List<DatiMUZVisibili_Out>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiDatiMUZVisibili([FromBody] LeggiDatiMUZVisibili_In leggiDatiMUZVisibiliIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<DatiMUZVisibili_Out>> result = new();

            try
            {
                var request = getRequest(leggiDatiMUZVisibiliIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDatiMUZVisibili(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Ritorna la lista dei gruppi delle MUZ appartenenti ad una data partita IVA
        /// </summary>
        /// <param name="Piva">Partita IVA dell'azienda richiedente</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista dei gruppi delle MUZ desiderate</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiListaGruppiMUZ
        /// 
        /// </remarks>
        /// <response code="200">Lista dei gruppi delle MUZ desiderate</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiListaGruppiMUZ")]
        [ProducesResponseType(typeof(RispostaStandard<List<GruppoAreaOmogenea>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiListaGruppiMUZ([FromBody] string Piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<GruppoAreaOmogenea>> result = new();

            try
            {
                var request = getRequest(Piva);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiListaGruppiMUZ(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue le operazioni di:
        ///     - Aggiunta MUZ ad un gruppo
        ///     - Clonazione di un gruppo MUZ
        /// </summary>
        /// <param name="operazioniGruppiMUZIn">Dati necessari per effettuare l'operazione richiesta</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /OperazioniGruppiMUZ
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("OperazioniGruppiMUZ")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult OperazioniGruppiMUZ([FromBody] OperazioniGruppiMUZ_In operazioniGruppiMUZIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(operazioniGruppiMUZIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).OperazioniGruppiMUZ(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Esegue le operazioni di creazione e modifica su di una MUZ
        /// </summary>
        /// <param name="MUZIn">La MUZ su cui eseguire l'operazione desiderata</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK / KO</returns>
        /// <remarks>
        /// 
        ///     POST /OperazioniMUZ
        /// 
        /// </remarks>
        /// <response code="200">OK / KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("OperazioniMUZ")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult OperazioniMUZ([FromBody] MUZ MUZIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                var request = getRequest(MUZIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).OperazioniMUZ(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce la lista delle MUZ dato un Area_Cod o una Piva
        /// </summary>
        /// <param name="LeggiMUZIn">La MUZ su cui eseguire l'operazione desiderata</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista delle MUZ desiderate</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiMUZ
        /// 
        /// </remarks>
        /// <response code="200">Lista delle MUZ desiderate</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiMUZ")]
        [ProducesResponseType(typeof(RispostaStandard<List<MUZ>>), StatusCodes.Status200OK)]
        public ObjectResult LeggiMUZ([FromBody] LeggiMUZ_In LeggiMUZIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<MUZ>> result = new();

            try
            {
                var request = getRequest(LeggiMUZIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiMUZ(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce le informazioni da mostrare al click sul raster in interfaccia
        /// </summary>
        /// <param name="rasterInfoClickIn">L'elemento grafico per cui mostrare le info</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Informazioni sul raster</returns>
        /// <remarks>
        /// 
        ///     POST /RasterInfoClick
        /// 
        /// </remarks>
        /// <response code="200">Informazioni sul raster</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("RasterInfoClick")]
        [ProducesResponseType(typeof(RispostaStandard<List<RasterInfoClick_Out>>), StatusCodes.Status200OK)]
        public ObjectResult RasterInfoClick([FromBody] RasterInfoClick_In rasterInfoClickIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<RasterInfoClick_Out>> result = new();

            try
            {
                var request = getRequest(rasterInfoClickIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).RasterInfoClick(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce la 'chiave albero' associata ad un'entità
        /// </summary>
        /// <param name="entitaCod">Il codice dell'entità</param>
        /// <param name="authorization">Token di autorizzazione</param>
        /// <returns>Chiave albero associata all'entità specificata</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiChiaveAlbero
        ///     CALCOLO CHIAVE ALBERO INCOMPLETO, TESTARE PRIMA DI UTILIZZARE
        /// 
        /// </remarks>
        /// <response code="200">Chiave albero associata all'entità specificata</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiChiaveAlbero")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LeggiChiaveAlbero([FromQuery] int entitaCod, [FromHeader] string authorization)
        {
            if (!isAuthorized(authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(entitaCod);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiChiaveAlbero(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che carica una palette colori in formato SLD
        /// </summary>
        /// <param name="CaricaPaletteSLDFFIn"> Palette da caricare</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>OK/KO</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaPaletteSLD
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">OK/KO</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("CaricaPaletteSLD")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public ObjectResult CaricaPaletteSLD([FromForm] CaricaPaletteSLDFF_In CaricaPaletteSLDFFIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new();

            try
            {
                using (var str = new System.IO.MemoryStream())
                {
                    CaricaPaletteSLDFFIn.filePalette.CopyTo(str);
                    
                    var request = getRequest(new CaricaPaletteSLD_In(CaricaPaletteSLDFFIn.LayerElementiGrafici_Cod,
                                                                     CaricaPaletteSLDFFIn.TipologiaLayer_Cod, 
                                                                     str.ToArray()));

                    result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CaricaPaletteSLD(request);
                    if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che verifica la presenza di una palette colori per un dato layer
        /// </summary>
        /// <param name="VerificaEsistenzaPaletteIn"> Layer da verificare </param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Boolean di esistenza palette</returns>
        /// <remarks>
        /// 
        ///     POST /CaricaPaletteSLD
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Boolean di esistenza palette</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaEsistenzaPalette")]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult VerificaEsistenzaPalette([FromBody] VerificaEsistenzaPalette_In VerificaEsistenzaPaletteIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(VerificaEsistenzaPaletteIn);

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaEsistenzaPalette(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("getEnvelopeWKT")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult getEnvelopeWKT([FromBody] int layerId, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<string> result = new();

            try
            {
                var request = getRequest(layerId);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getEnvelopeWKT(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che sottomette
        /// </summary>
        /// <param name="elencoAziende"> Parametri per attivare\disattivare il controllo Compliance ISCC da una o più aziende   </param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Boolean richiesta sottomessa</returns>
        /// <remarks>
        /// 
        ///     POST /IncludiEsludiImpresaDaControlloComplianceISCC
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Boolean aggiornamento effettuato</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("IncludiEsludiImpresaDaControlloComplianceISCC")]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult IncludiEsludiImpresaDaControlloComplianceISCC([FromBody] IncludiEsludiImpresaISCC_In elencoAziende, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(elencoAziende);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).IncludiEsludiImpresaDaControlloComplianceISCC(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che sottomette
        /// </summary>
        /// <param name="SottomettiElaborazioneMassivaGISCheckListIn"> Parametri per la sottomissione di una elaborazione batch di CheckList per il GIS (Ad es: compliance ISCC)  </param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Boolean richiesta sottomessa</returns>
        /// <remarks>
        /// 
        ///     POST /SottomettiElaborazioneMassivaGISCheckList
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Boolean richiesta sottomessa</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("SottomettiElaborazioneMassivaGISCheckList")]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult SottomettiElaborazioneMassiva_GISCheckList([FromBody] SottomettiElaborazioneMassivaGISCheckList_In SottomettiElaborazioneMassivaGISCheckListIn, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(SottomettiElaborazioneMassivaGISCheckListIn);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).SottomettiElaborazioneMassivaGISCheckList(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che sottomette
        /// </summary>
        /// <param name="parametri"> Parametri per verifica se una azienda è abilitata su un particolare tipo di CheckList per il GIS (Ad es: compliance ISCC)  </param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Azienda abilitata\non abilitata</returns>
        /// <remarks>
        /// 
        ///     POST /VerificaAziendaAbilitataISCC
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Boolean richiesta sottomessa</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("VerificaAziendaAbilitataISCC")]
        [ProducesResponseType(typeof(RispostaStandard<bool>), StatusCodes.Status200OK)]
        public ObjectResult VerificaAziendaAbilitataISCC([FromBody] VerificaAziendaAbilitataISCC_In parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<bool> result = new();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).VerificaAziendaAbilitataISCC(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Metodo che sottomette
        /// </summary>
        /// <param name="parametri"> Parametri per la ricerca del dettaglio di elaborazione delle CheckList per il GIS (Ad es: compliance ISCC)  </param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>json con elenco delle elaboazioni per tipologia di checklist e relativi dettagli</returns>
        /// <remarks>
        /// 
        ///     POST /LeggiElencoElaborazioniMassiveGISCheckList
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Boolean richiesta sottomessa</response>
        /// <response code="401">Utente non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("LeggiElencoElaborazioniMassiveGISCheckList")]
        [ProducesResponseType(typeof(RispostaStandard<LeggiElencoElaborazioniMassive_Out>), StatusCodes.Status200OK)]
        public ObjectResult LeggiElencoElaborazioniMassiveGISCheckList([FromBody] LeggiElencoElaborazioniMassive_In parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<LeggiElencoElaborazioniMassive_Out> result = new();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiElencoElaborazioniMassiveGISCheckList(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce l'immagine PNG della mappa di prescrizione
        /// </summary>
        /// <param name="piva">Partita IVA</param>
        /// <param name="saCod">Codice SA</param>
        /// <param name="appezza">Codice Appezzamento</param>
        /// <param name="idImp">ID Impianto</param>
        /// <param name="ricettaOperazioneCod">Codice Operazione Ricetta</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Immagine PNG della mappa di prescrizione</returns>
        /// <response code="200">Immagine PNG</response>
        /// <response code="400">Parametro obbligatorio mancante</response>
        /// <response code="401">Non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpGet]
        [Route("mappa-prescrizione-image")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public IActionResult MappaPrescrizioneImage(
            [FromQuery] string piva,
            [FromQuery] int saCod,
            [FromQuery] int appezza,
            [FromQuery] int idImp,
            [FromQuery] int ricettaOperazioneCod,
            [FromQuery] int allegatoCod,
            [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            try
            {
                var payload = new MappaPrescrizioneImage_In { Piva = piva, SaCod = saCod, Appezza = appezza, IdImp = idImp, RicettaOperazioneCod = ricettaOperazioneCod, AllegatoCod = allegatoCod };
                var result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).MappaPrescrizioneImage(getRequest(payload));
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
                var imageBytes = Convert.FromBase64String(result.RispostaStringa);
                return File(imageBytes, "image/png");
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex));
            }
        }

        /// <summary>
        /// Restituisce l'immagine PNG della mappa di prescrizione e il bounding box WGS84
        /// per il rendering di un GroundOverlay su Google Maps SDK.
        /// Il PNG viene generato tramite GDAL se non già presente, altrimenti viene letto dalla cache.
        /// </summary>
        /// <param name="allegatoCod">Codice allegato del file raster</param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Oggetto con PngBase64 e Bounds (north/south/east/west)</returns>
        /// <response code="200">Oggetto MappaPrescrizioneGroundOverlay_Out</response>
        /// <response code="401">Non autorizzato</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpGet]
        [Route("mappa-prescrizione-ground-overlay")]
        [ProducesResponseType(typeof(MappaPrescrizioneGroundOverlay_Out), StatusCodes.Status200OK)]
        public IActionResult MappaPrescrizioneGroundOverlay(
            [FromQuery] int allegatoCod,
            [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            try
            {
                var result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).MappaPrescrizioneGroundOverlay(getRequest(allegatoCod));
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
                return Ok(result.RispostaStringa);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex));
            }
        }

    }
}
