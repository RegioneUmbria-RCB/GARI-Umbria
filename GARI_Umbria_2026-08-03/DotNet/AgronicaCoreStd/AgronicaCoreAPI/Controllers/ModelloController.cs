using AgronicaCoreAPI.adapters;
using AgronicaCoreAPI.models.entities;
using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using AgronicaDataProvider6.Models;
using InData.Anagrafica;
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
    public class ModelloController : BaseController
    {
        public ModelloController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        [HttpGet]
        [Route("Aziende")]
        public ObjectResult GetAziende([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<ImpresaEntity>>();

            try
            {
                var adapter = new ModelloAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                var r = ws.LeggiImpreseAPP(getRequest(new FiltroAziendeAPP()));
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaOK = true;
                result.RispostaStringa = adapter.leggiAziende(r.RispostaStringa);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("AziendeByMap")]
        public ObjectResult GetAziendeByMap([FromBody] FiltroAziendeMappaAPP InData, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<ImpresaEntity>>();

            try
            {
                var adapter = new ModelloAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                var r = ws.LeggiImpreseAPP_GIS(getRequest(InData));
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaOK = true;
                result.RispostaStringa = adapter.leggiAziende(r.RispostaStringa);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("TipologieGerarchiaAziende")]
        public ObjectResult GetTipologieGerarchiaAziende([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<GerarchiaImpreseTipologiaEntity>>();

            try
            {                
                var adapter = new ModelloAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                
                var r = ws.LeggiTipologieGerarchiaImprese(new APICallsBasic(objP_super_server, objP_server, objP_utenti));
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                result.RispostaOK = true;
                result.RispostaStringa = adapter.leggiTipologieGerarchiaImprese(r.RispostaStringa);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("DestinazioniUso")]
        public ObjectResult GetDestinazioniUso([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<DestinazioneUso>> result = new RispostaStandard<List<DestinazioneUso>>();

            try
            {
                var request = getRequest(new LeggiDestinazioniUso());
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDestinazioniUso(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Specie")]
        [ProducesResponseType(typeof(RispostaStandard<List<Specie>>), StatusCodes.Status200OK)]
        public ObjectResult GetSpecie([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Specie>> result = new RispostaStandard<List<Specie>>();

            try
            {
                var request = getRequest(new LeggiSpecie());
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSpecie(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Specie")]
        [ProducesResponseType(typeof(RispostaStandard<List<Specie>>), StatusCodes.Status200OK)]
        public ObjectResult PostSpecie(LeggiSpecie parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Specie>> result = new RispostaStandard<List<Specie>>();

            try
            {
                var request = getRequest(parametri); //getRequest(new LeggiSpecie());
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiSpecie(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Varieta/{specie}")]
        public ObjectResult GetVarieta(int specie, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Varieta>> result = new RispostaStandard<List<Varieta>>();

            try
            {
                var request = getRequest(new LeggiCultivar() { specie = new Specie(specie) });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiVarieta(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("GruppoFinalita/{specie}")]
        public ObjectResult GetGruppoFinalita(int specie, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<GruppoFinalita>> result = new RispostaStandard<List<GruppoFinalita>>();

            try
            {
                var request = getRequest(new LeggiFinalita() { specie = new Specie(specie) });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiGruppoFinalita(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Disciplinari/{specie}/{flag}/{data}")]
        [ProducesResponseType(typeof(RispostaStandard<List<Disciplinare>>), StatusCodes.Status200OK)]
        public ObjectResult GetDisciplinari(int specie, bool flag,string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Disciplinare>> result = new RispostaStandard<List<Disciplinare>>();

            try
            {
                //nota la data è passata in formato YYYYMMDD

                //CoreWSDisciplinari request = new CoreWSDisciplinari(objP_super_server, objP_server, objP_utenti, specie, flag);
                //request.data = data;var request = getr
                var request = getRequest(new LeggiDisciplinari() { specie = new Specie() { codice = specie }, data = DateTime.Parse(data), privato = flag });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDisciplinari(request);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Restituisce un elenco di DPI filtrato sulla base dei parametri passati nel Body della requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Authorization">Token di autorizzazione</param>
        /// <returns>Lista di DPI</returns>
        /// <remarks>
        /// Sample request (lavorazione: rilievo avversita', su vite a maggio 2023, includi DPI privati):
        ///
        ///     POST /Disciplinari/Leggi_Disciplinari_Testata_conRegolamentoConcimazione
        ///     {
        ///     	"lavorazioni": [
        ///     		{
        ///     			"categoriaOperazione": null,
        ///     			"primaryKey": {
        ///     				"classType": "Lavorazione",
        ///     				"codice": "113"
        ///     			},
        ///     			"descrizione": "Rilievi Avversita' in Campo",
        ///     			"tipo": 0
        ///     		}
        ///     	],
        ///     	"specie": {
        ///     		"codice": 64,
        ///     		"descrizione": "Vite"
        ///     	},
        ///        "privato": true,
        ///     	"data": "2023-05-19T00:00:00"
        ///     }
        /// </remarks>
        [HttpPost]
        [Route("Disciplinari/Leggi_Disciplinari_Testata_conRegolamentoConcimazione")]
        [ProducesResponseType(typeof(RispostaStandard<List<Disciplinare>>), StatusCodes.Status200OK)]
        public ObjectResult Leggi_Disciplinari_Testata_conRegolamentoConcimazione([FromBody] LeggiDisciplinari request, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Disciplinare>> result = new RispostaStandard<List<Disciplinare>>();

            try
            {
                //nota la data è passata in formato YYYYMMDD

                //CoreWSDisciplinari request = new CoreWSDisciplinari(objP_super_server, objP_server, objP_utenti, specie, flag);
                //request.data = data;var request = getr
                //List<Lavorazione> ll = new List<Lavorazione>();
                //ll.Add(new Lavorazione { primaryKey = new Job.PK("Lavorazione", "113"), descrizione = "Rilievi Avversita' in Campo" });
                //var request = getRequest(new LeggiDisciplinari() {lavorazioni = ll.ToArray(), specie = new Specie() { codice = specie }, data = DateTime.Parse(data), privato = flag });

                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Leggi_Disciplinari_Testata_conRegolamentoConcimazione(getRequest(request));
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Esercizi")]
        public ObjectResult GetEsercizi(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Esercizio>> result = new RispostaStandard<List<Esercizio>>();

            try
            {
                CoreWS_Impianti request = new CoreWS_Impianti(objP_super_server, objP_server, objP_utenti, piva, data);
                RispostaStandard risposta = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpianti(request);

                if (!risposta.RispostaOK && !string.IsNullOrEmpty(risposta.Errore))
                {
                    result.RispostaOK = false;
                    result.Errore = risposta.Errore;
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

                List<Esercizio> esercizi = AnagraficaAdapter.leggiEsercizi(risposta.RispostaStringa);

                result.RispostaOK = true;
                result.RispostaStringa = esercizi;
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Impianti")]
        public ObjectResult GetImpianti(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Impianto>> result = new RispostaStandard<List<Impianto>>();

            try
            {
                CoreWS_Impianti request = new CoreWS_Impianti(objP_super_server, objP_server, objP_utenti, piva, data);
                RispostaStandard risposta = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpianti(request);

                if (!risposta.RispostaOK && !string.IsNullOrEmpty(risposta.Errore))
                {
                    result.RispostaOK = false;
                    result.Errore = risposta.Errore;
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

                List<Impianto> impianti = AnagraficaAdapter.leggiImpianti(risposta.RispostaStringa);

                result.RispostaOK = true;
                result.RispostaStringa = impianti;
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Appezzamenti")]
        public ObjectResult GetAppezzamenti(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Appezzamento>> result = new RispostaStandard<List<Appezzamento>>();

            try
            {
                CoreWS_Impianti request = new CoreWS_Impianti(objP_super_server, objP_server, objP_utenti, piva, data);
                RispostaStandard risposta = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpianti(request);

                if (!risposta.RispostaOK && !string.IsNullOrEmpty(risposta.Errore))
                {
                    result.RispostaOK = false;
                    result.Errore = risposta.Errore;
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

                List<Appezzamento> appezzamenti = AnagraficaAdapter.leggiAppezzamenti(risposta.RispostaStringa);

                result.RispostaOK = true;
                result.RispostaStringa = appezzamenti;
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("UtilizzoTerreno")]
        public ObjectResult GetUtilizzoTerreno(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<UtilizzoTerreno>> result = new RispostaStandard<List<UtilizzoTerreno>>();

            try
            {
                CoreWS_Impianti request = new CoreWS_Impianti(objP_super_server, objP_server, objP_utenti, piva, data);
                RispostaStandard risposta = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpianti(request);

                if (!risposta.RispostaOK && !string.IsNullOrEmpty(risposta.Errore))
                {
                    result.RispostaOK = false;
                    result.Errore = risposta.Errore;
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

                List<UtilizzoTerreno> utilizzoTerreno = AnagraficaAdapter.leggiUtilizziTerreno(risposta.RispostaStringa);

                result.RispostaOK = true;
                result.RispostaStringa = utilizzoTerreno;
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("CentriAziendali")]
        public ObjectResult GetCentriAziendali(string piva, string data, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<CentroAziendale>> result = new RispostaStandard<List<CentroAziendale>>();

            try
            {
                CoreWS_Impianti request = new CoreWS_Impianti(objP_super_server, objP_server, objP_utenti, piva, data);
                RispostaStandard risposta = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpianti(request);

                if (!risposta.RispostaOK && !string.IsNullOrEmpty(risposta.Errore))
                {
                    result.RispostaOK = false;
                    result.Errore = risposta.Errore;
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }

                List<CentroAziendale> centriAziendali = AnagraficaAdapter.leggiCentriAziendali(risposta.RispostaStringa);

                result.RispostaOK = true;
                result.RispostaStringa = centriAziendali;
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Nazioni")]
        public ObjectResult GetNazioni([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<CodiciNazioniISO3166>> result = new RispostaStandard<List<CodiciNazioniISO3166>>();

            try
            {
                var request = getRequest("");
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiNazioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Province")]
        public ObjectResult GetProvince(string cod_stato, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Provincia>> result = new RispostaStandard<List<Provincia>>();

            try
            {
                var request = getRequest(new GetProvincie() { stato = cod_stato });
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProvince(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Province")]
        public ObjectResult PostProvince(GetProvincie parametri, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Provincia>> result = new RispostaStandard<List<Provincia>>();

            try
            {
                var request = getRequest(parametri);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiProvince(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Comuni/{cod_provincia}")]
        public ObjectResult GetComuni(string cod_provincia, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<List<Comune>> result = new RispostaStandard<List<Comune>>();

            try
            {
                var request = getRequest(cod_provincia);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiComuni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Returns the list of companies visible to the authenticated user, simplified to two cases:
        /// an empty array (no companies or multiple companies) or an array with a single element containing
        /// the piva of the unique visible company.
        /// Spec: DS01-API - GET /api/v1/user/companies/visible (FS002, DS01-BL).
        /// </summary>
        /// <param name="Authorization">Bearer JWT token.</param>
        /// <returns>
        /// 200 with <see cref="RispostaStandard{T}"/> containing exactly 1 <see cref="ImpresaEntity"/> if exactly
        /// one company is visible; empty list otherwise.
        /// 401 if the token is missing or invalid.
        /// 500 on unexpected server errors.
        /// </returns>
        [HttpGet]
        [Route("SingolaAzienda")]
        [ProducesResponseType(typeof(RispostaStandard<List<ImpresaEntity>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ObjectResult GetVisibleCompaniesForUser([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<List<ImpresaEntity>>();

            try
            {
                var adapter = new ModelloAdapter();
                var ws = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request);
                // ContaImpreseVisibiliAPP calls SELECT TOP 2 server-side — returns at most 2 rows,
                // enough to resolve the 0 / 1 / ≥2 visibility cases without a full table scan.
                var r = ws.ContaImpreseVisibiliAPP(getRequest(new FiltroAziendeAPP()));
                if (!r.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, r);

                // Return the single-company list if exactly 1 company is visible; empty list otherwise.
                // This is the core logic defined in FS002 and DS01-BL (RilevamentoVisibilitaAziendaSingola).
                result.RispostaOK = true;
                result.RispostaStringa = adapter.leggiAziendeVisibili(r.RispostaStringa);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
