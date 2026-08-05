using AgronicaCoreUtilityStd;                                    
using AgronicaNetCore.Base.Models;                             
using AgronicaNetCore.Base.Utility;                             
using AgronicaNetCore.Gis.BIZ;                                  
using AgronicaNetCore.Gis.BIZ.Services.Gis;                    
using AgronicaNetCoreApi.Resources;                            
using Microsoft.AspNetCore.Authorization;                      
using Microsoft.AspNetCore.Mvc;                             
using Microsoft.Extensions.Localization;                      
using Microsoft.Extensions.Options;                             
using Newtonsoft.Json;                                         
using System.Data;                                               
using System.Security.Claims;                                     
using AgronicaCoreDTOStd.InData.Gis;
using AgronicaCoreVarieBizSTD;
using InData.Gis;
using AgronicaDataProvider6.Models;                           

namespace AgronicaNetCoreApi.Controllers
{

    [ApiController] 
    [Authorize] 
    [Route("[controller]")] 
    public class GisController : BaseController
    {
        private readonly IGisService _gisService;
        private readonly IGisClusterConfigService _gisClusterConfigService;
        private readonly IGisClusteringCalculatorService _gisClusteringCalculatorService;


        public GisController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer)
            : base(provider, securitySettings, localizer)
        {
            _gisService = provider.GetRequiredService<IGisService>();
            _gisClusterConfigService = provider.GetRequiredService<IGisClusterConfigService>();
            _gisClusteringCalculatorService = provider.GetRequiredService<IGisClusteringCalculatorService>();
        }

        [HttpGet]
        [Route(nameof(GetGISProcessingAlgorithmsCleaningAlgorithm))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGISProcessingAlgorithmsCleaningAlgorithm()
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();
                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                DataTable GIS_ProcessingAlgorithms_Cleaning_Algorithm = await _gisService.LeggiGIS_ProcessingAlgorithms_Cleaning_AlgorithmAsync(objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(GIS_ProcessingAlgorithms_Cleaning_Algorithm, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (Exception ex)
            {  
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        #region "API per Configurazione Clustering GIS"

        // ====================================================================================================
        // 1- Lettura della configurazione
        // ====================================================================================================
        /// <summary>
        /// Endpoint per leggere una specifica configurazione di clustering GIS.
        /// </summary>
        /// <param name="inputDto">Un DTO contenente i parametri per identificare la configurazione.</param>
        /// <returns>La configurazione completa (testata e dettagli) in formato JSON.</returns>
        [HttpPost] 
        [Route(nameof(GetGisClusterConfig))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGisClusterConfig([FromBody] GisClusterConfigGet_InData inputDto)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var config = await _gisClusterConfigService.GetConfigurationAsync(
                    inputDto.LayerElementiGraficiConfigTypeCod,
                    inputDto.LayerElementiGraficiCod,
                    inputDto.Utente,
                    objParametriServer);

                // Se il BIZ restituisce null, significa che la configurazione non è stata trovata.
                if (config == null)
                {
                    resp.RispostaOK = true;
                    resp.RispostaStringa = null; 
                    return Ok(resp);
                }

                // Se trovata, serializza il DTO di ritorno in una stringa JSON.
                resp.RispostaStringa = JsonConvert.SerializeObject(config, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        // ====================================================================================================
        // 1- Lettura della configurazione
        // ====================================================================================================
        /// <summary>
        /// Endpoint per leggere tutte le configurazioni di clustering GIS di un utente.
        /// </summary>
        /// <returns>Le configurazioni relative all'utente loggato in formato JSON.</returns>
        [HttpPost] 
        [Route(nameof(GetGisClusterConfigs))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGisClusterConfigs()
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity,
                    Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                var config = await _gisClusterConfigService.GetConfigurationsAsync(
                    objParametriServer.UtenteUsername,
                    objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(config, Formatting.Indented);
                resp.RispostaOK = true;
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        // ====================================================================================================
        // 2- Creazione (Scrittura) della configurazione
        // ====================================================================================================
        /// <summary>
        /// Endpoint per creare una nuova configurazione di clustering.
        /// </summary>
        /// <param name="inputDto">Un DTO completo (testata + dettagli) con i dati da inserire.</param>
        /// <returns>Una risposta di successo (OK) o un errore.</returns>
        [HttpPost]
        [Route(nameof(CreateGisClusterConfig))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateGisClusterConfig([FromBody] GisClusterConfigSave_InData inputDto)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                // Chiama il servizio BIZ per eseguire l'operazione di creazione.
                await _gisClusterConfigService.CreateConfigurationAsync(inputDto, objParametriServer);

                //la risposta in caso di successo è un semplice OK/KO.
                resp.RispostaOK = true;
                return Ok(resp);
            }
            catch (InvalidOperationException ex) 
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                return BadRequest(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        // ====================================================================================================
        // 3- Aggiornamento della configurazione
        // ====================================================================================================
        /// <summary>
        /// Endpoint per aggiornare una configurazione di clustering esistente.
        /// </summary>
        /// <param name="inputDto">Un DTO completo con i dati aggiornati.</param>
        /// <returns>La configurazione completa aggiornata in formato JSON.</returns>
        [HttpPut] // Si usa il verbo HTTP PUT per le operazioni di aggiornamento/sostituzione.
        [Route(nameof(UpdateGisClusterConfig))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGisClusterConfig([FromBody] GisClusterConfigSave_InData inputDto)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                // Chiama il servizio BIZ per eseguire l'aggiornamento.
                var updatedConfig = await _gisClusterConfigService.UpdateConfigurationAsync(inputDto, objParametriServer);

                // la risposta contiene l'oggetto aggiornato.
                resp.RispostaStringa = JsonConvert.SerializeObject(updatedConfig, Formatting.Indented);
                resp.RispostaOK = true;
                return Ok(resp);
            }
            catch (InvalidOperationException ex) 
            {
                resp.RispostaOK = false;
                resp.Errore = ex.Message;
                // Restituisce 404 Not Found
                return NotFound(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        // ====================================================================================================
        // 4- Cancellazione della configurazione
        // ====================================================================================================
        /// <summary>
        /// Endpoint per cancellare una specifica configurazione di clustering.
        /// </summary>
        /// <param name="inputDto">Un DTO contenente i parametri per identificare la configurazione da cancellare.</param>
        /// <returns>Una risposta di successo (OK) o un errore.</returns>
        [HttpDelete] 
        [Route(nameof(DeleteGisClusterConfig))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteGisClusterConfig([FromBody] GisClusterConfigGet_InData inputDto)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                // Chiama il servizio BIZ per eseguire la cancellazione.
                await _gisClusterConfigService.DeleteConfigurationAsync(
                    inputDto.LayerElementiGraficiConfigTypeCod,
                    inputDto.LayerElementiGraficiCod,
                    inputDto.Utente,
                    objParametriServer);

                resp.RispostaOK = true;
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        #endregion

        #region "API per il Processo Batch di Clustering"

        /// <summary>
        /// Endpoint per sottomettere un job di calcolo dei centroidi in background.
        /// </summary>
        /// <param name="inputDto">Un DTO contenente i parametri per identificare la configurazione da cancellare.</param>
        /// <returns>Una risposta di successo (OK) o un errore.</returns>

        [HttpPost]
        [Route(nameof(CalcoloCentroidi))]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> CalcoloCentroidi([FromBody] GisCalculateCentroid_In inputDto)
        {
            var resp = new RispostaStandard();
            try
            {
                var auth = _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers).GetAwaiter().GetResult();
                if (!auth.status) return Unauthorized();

                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);

                if (!await _gisClusteringCalculatorService.StartJob_SaveCentroidsAsync(
                                inputDto.PivaSuperUser,
                                inputDto.LayerElementiGraficiCod,
                                inputDto.ElementoGraficoCod,
                                inputDto.BatchSize,
                                objParametriServer))
                    throw new Exception("Errore nella sottomissione del job di calcolo centroidi, consultare il log.");


                resp.RispostaOK = true;
                resp.RispostaStringa = "Job di calcolo centroidi terminato consultare il log per le informazioni di dettaglio.";

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }

        #endregion

        [HttpPost]
        [Route("layer-translations")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLayerTranslations([FromBody] TraduzioniLayer_In input)
        {
            RispostaStandard resp = new();
            
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();
                
                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var result = await _gisService.UpdateLayerTranslations(input, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }
            
        [HttpPost]
        [Route("layer-label-translations")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLayerLabelTranslations([FromBody] TraduzioniLayerLabel_In input)
        {
            RispostaStandard resp = new();
            try
            {
                var auth = await _identityService.IsAuthorizedAsync(HttpContext.User.Identity as ClaimsIdentity, Request.Headers);
                if (!auth.status) return Unauthorized();
        
                var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
                var result = await _gisService.UpdateLayerLabelTranslations(input, objParametriServer);

                resp.RispostaStringa = JsonConvert.SerializeObject(result, Formatting.Indented);
                resp.RispostaOK = true;

                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, resp);
            }
        }
    }
}