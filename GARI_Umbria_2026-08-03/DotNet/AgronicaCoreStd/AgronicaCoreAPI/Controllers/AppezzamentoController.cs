using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreUtilityStd;
using AgronicaCoreDTOStd.InData.Anagrafica;
using System.Net.Http;
using AgronicaCoreModelsSTD.exceptions;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;
using Microsoft.Extensions.Options;
using AgronicaDataProvider6.Models;

namespace AgronicaCoreAPI.Controllers
{
    public class AppezzamentoController : BaseController
    {

        public AppezzamentoController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config,httpClientFactory, securitySettings, localizer)
        {

        }

        //// GET: AppezzamentoController
        //[HttpGet("{piva}/{sacod:int}")]
        //public ObjectResult List(string piva, int sacod)
        //{
        //    if (!isAuthorized())
        //    {
        //        return StatusCode(StatusCodes.Status401Unauthorized, null);
        //    }

        //    if (piva == "")
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, "Partita Iva non valorizzata");
        //    }

        //    if (sacod == 0)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, "Centro Aziendale non valorizzato");
        //    }

        //    RispostaStandard<List<Appezzamento>> result = new RispostaStandard<List<Appezzamento>>();

        //    try
        //    {
        //        //var objP = new CoreWS_GenericObjP() { objP_super_server = objP_super_server, objP_server = objP_server, objP_utenti = objP_utenti };
        //        //var request = getGenericRequest<Appezzamento>(new Appezzamento(new Appezzamento.PK(0, new CentroAziendale.PK(sacod, piva))));

        //        //result = new CoreWSController(coreWSBaseURL, tokenChiamateWS).LeggiAppezzamentoModello(request);
        //        //if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.RispostaOK = false;
        //        result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
        //        return StatusCode(StatusCodes.Status500InternalServerError, result);
        //    }

        //    return StatusCode(StatusCodes.Status200OK, result);
        //}

        [HttpGet("{piva}/{sacod:int}/{appezza:int}/{idreg:int}")]
        public ObjectResult Get(string piva, int sacod,int appezza,int idreg, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, null);
            }

            if (piva == "")
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Partita Iva non valorizzata");
            }

            if (sacod == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Centro Aziendale non valorizzato");
            }

            if (appezza == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Appezzamento non valorizzato");
            }

            RispostaStandard<Appezzamento> result = new RispostaStandard<Appezzamento>();

            try
            {
                var appezzamento = new Appezzamento(new Appezzamento.PK(appezza, new CentroAziendale.PK(sacod, piva)));
                if (idreg != 0)
                {
                    var impianto = new Impianto(new Impianto.PK(idreg, new Appezzamento.PK(appezza, new CentroAziendale.PK(sacod, piva))));
                    appezzamento.impianti = new List<Impianto>();
                    appezzamento.impianti.Add(impianto);
                }

                var leggiAppezzamento = new LeggiAppezzamento() { 
                    appezzamento = appezzamento,
                    filtroData = false,
                    leggiCatasto = false,
                    leggiDistinte = true,
                    leggiImpianti = true,
                    leggiIndirizzi = true,
                    leggiCartografia = true
                };
                var request = getRequest(leggiAppezzamento);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiAppezzamentoModello(request);
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
        public ObjectResult Scrivi([FromBody] Appezzamento appezzamento, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, null);
            }

            if (appezzamento == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Oggetto appezzamento non valorizzato");
            }

            RispostaStandard<Appezzamento> result = new RispostaStandard<Appezzamento>();

            try
            {
                var request = getRequest(appezzamento);

                if (appezzamento.primaryKey.codice == 0 && string.IsNullOrEmpty(appezzamento.guid))
                {
                    throw new Exception("Il campo guid non può essere vuoto in creazione!");
                }

                if (appezzamento.primaryKey.codice == 0 && !string.IsNullOrEmpty(appezzamento.guid))
                {
                    result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviAppezzamento(request);
                }
                else
                {
                    result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).ScriviAppezzamentiModello(request);
                }
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
