using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.IsAlive;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.IsAlive;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Controllers
{
    public class IsAliveController : BaseController
    {
        public IsAliveController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer) { }

        /// <summary>
        /// Metodo che verifica che i servizi siano attivi
        /// </summary>
        /// <param name="checkIsAlive"> obj con params per IsAlive
        /// <param name="Authorization">Token di autorizzazione</param>
        /// {
        ///     "bearerToken": bearer token di accesso,
        ///     "sites": array di siti di cui verificare la raggiungibilità,
        ///     "version": campo interno, non presente se metodo chiamato da esterni 
        /// }
        /// </param>
        /// <returns>database reachable + lista di siti reachable + versione dei siti se passato version</returns>
        /// <remarks>
        /// 
        ///     POST /IsAlive
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="500">Errore durante l'operazione</response>
        [HttpPost]
        [Route("IsAlive")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RispostaStandard<JObject>), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsAlive([FromBody] CheckIsAliveIN checkIsAlive, [FromHeader] string Authorization)
        {
            IActionResult response = Unauthorized();
            RispostaStandard<JObject> result = new();

            try {
                if (!isAuthorized(Authorization)) { return StatusCode(StatusCodes.Status401Unauthorized, null); }
            }
            catch (Exception) {
                return StatusCode(StatusCodes.Status401Unauthorized, null);
            }

            try {
                CheckIsAliveOUT objRes = new();

                //Verifica la raggiungibilità dei db (super_server, server e utenti)
                var reqDB = getRequest("");
                var resultDB = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).reachableDB(reqDB);

                if (!resultDB.RispostaOK) { throw new Exception(result.Errore); }

                objRes.reachableDBs = resultDB.RispostaStringa;

                //Verifica la raggiungibilità dei siti richiesti
                if (!checkIsAlive.sites.Equals(null) && checkIsAlive.sites.Length > 0)
                {
                    var reqSites = getRequest(checkIsAlive.sites.ToList());
                    var resultSites = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).reachableSites(reqSites);

                    if (!resultSites.RispostaOK) { throw new Exception(resultSites.Errore); }

                    objRes.reachableSites = CheckReachableSites(resultSites.RispostaStringa, checkIsAlive.showVersion, checkIsAlive.showUrl);

                    JsonSerializerSettings settings = new() { NullValueHandling = NullValueHandling.Ignore };
                    result.RispostaStringa = JObject.Parse(JsonConvert.SerializeObject(objRes, settings));
                }

                response = Ok(result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex) {
                string errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                logErrore("Errore IsAlive: " + JsonConvert.SerializeObject(checkIsAlive) + "\n" + errore);
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return response;
        }

        /// <summary>
        /// Verifica se i siti passati sono raggiungibili
        /// </summary>
        /// <param name="sitesToCheck"></param>
        /// <param name="setVersion"></param>
        /// <param name="setUrl"></param>
        /// <returns></returns>
        private List<ReachableSiteOUT> CheckReachableSites(List<ReachableSiteIN> sitesToCheck, bool setVersion, bool setUrl)
        {
            List<ReachableSiteOUT> sitesChecked = new();

            foreach (var site in sitesToCheck)
            {
                var urlsToCheck = site.urls;
                if (urlsToCheck.Count == 0)
                {
                    sitesChecked.Add(new ReachableSiteOUT(site.key, setUrl ? "" : null, "Unreachable: wrong or empty site key.", setVersion ? "" : null, false));
                    continue;
                }

                foreach (var url in urlsToCheck)
                {
                    if (string.IsNullOrEmpty(url) || url.Equals("KO"))
                    {
                        sitesChecked.Add(new ReachableSiteOUT(site.key, setUrl ? url : null, "Unreachable: wrong or empty site key.", setVersion ? "" : null, false));
                        break;
                    }
                    else
                    {
                        try
                        {
                            switch (site.key.ToLower())
                            {
                                case "giasng":
                                    sitesChecked.Add(GiasNGRequest(site.key, url, setUrl));
                                    break;
                                case "giasbase":
                                    sitesChecked.Add(GiasBaseRequest(site.key, url, setUrl));
                                    break;
                                case "netcoreapi":
                                case "netcoredataexchange":
                                case "qdcacompliance":
                                    sitesChecked.Add(NetCoreApiRequest(hc, site.key, url, setVersion, setUrl));
                                    break;
                                default:
                                    sitesChecked.Add(GenericRequest(hc, site.key, url, setVersion, setUrl));
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            sitesChecked.Add(new ReachableSiteOUT(site.key, setUrl ? url : null, "Unreachable: " + ex.Message, setVersion ? "" : null, false));
                            break;
                        }
                    }
                }

                if (!setUrl)
                {
                    sitesChecked = sitesChecked
                        .GroupBy(g => g.key)
                        .Select(g => new ReachableSiteOUT(
                                    g.Key,
                                    null,
                                    g.All(x => string.IsNullOrEmpty(x.error)) ? "" : 
                                        g.Where(x => !string.IsNullOrEmpty(x.error))
                                            .Select(x => x.error).Distinct().ToList().Count == 1 ?
                                        g.First().error : "Unreachable.",
                                    g.First().version,
                                    g.All(x => x.reachable)
                                ))
                        .ToList();
                }
            }

            return sitesChecked;
        }

        /// <summary>   
        /// Richiesta a un sito (esclusi GiasBase e GiasNG)
        /// </summary>
        /// <param name="_hc"></param>
        /// <param name="siteKey"></param>
        /// <param name="request"></param>
        /// <param name="setVersion"></param>
        /// <param name="setUrl"></param>
        /// <returns></returns>
        private static ReachableSiteOUT GenericRequest(HttpClient _hc, string siteKey, string request, bool setVersion, bool setUrl)
        {
            ReachableSiteOUT resp = new(siteKey, setUrl ? request : null, "Unreachable.", setVersion ? "" : null, false);

            HttpMethod method = HttpMethod.Post;
            string content = "{\"InData\":\"" + siteKey + "\"}";

            try {
                Task<HttpResponseMessage> trm = null;

                if (method == HttpMethod.Post) {
                    HttpContent cont = null;
                    if (!string.IsNullOrEmpty(content))
                        cont = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                    trm = _hc.PostAsync(request, cont);
                }
                else {
                    if (method == HttpMethod.Get)
                        trm = _hc.GetAsync(request);
                }

                if (trm != null) {
                    HttpResponseMessage rm = trm.Result;
                    if (rm.IsSuccessStatusCode) {
                        Task<String> ts = rm.Content.ReadAsStringAsync();

                        JObject obj = JObject.Parse(ts.Result);
                        if (obj.ContainsKey("d"))
                        {
                            obj = (JObject)obj.GetValue("d");
                            RispostaStandard r = JsonConvert.DeserializeObject<RispostaStandard>(obj.ToString());
                            resp = new ReachableSiteOUT(siteKey, setUrl ? request : null, "", setVersion ? r.RispostaStringa : null, true);
                        }
                        else
                        {
                            resp = new(siteKey, setUrl ? request : null, "Unreachable.", setVersion ? "" : null, false);
                        }
                    }
                }
            }
            catch (Exception ex) {
                resp = new(siteKey, setUrl ? request : null, $"Unreachable: {Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex)}", setVersion ? "" : null, false);
            }

            return resp;
        }

        /// <summary>
        /// Richiesta a pagina Default.aspx di GiasBase
        /// </summary>
        /// <param name="siteKey"></param>
        /// <param name="url"></param>
        /// <param name="setUrl"></param>
        /// <returns></returns>
        private static ReachableSiteOUT GiasBaseRequest(string siteKey, string url, bool setUrl)
        {
            ReachableSiteOUT resp = new();

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (response.StatusCode == HttpStatusCode.OK) 
                        resp = new(siteKey, setUrl ? url : null, "", null, true);
                    else
                        resp = new(siteKey, setUrl ? url : null, $"Unreachable: {response.StatusCode}", null, false);
                }
            }
            catch (WebException ex) {
                resp = new(siteKey, setUrl ? url : null, $"Unreachable: {Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex)}", null, false);
            }

            return resp;
        }

        /// <summary>
        /// Richiesta a pagina x di GiasNG
        /// </summary>
        /// <param name="siteKey"></param>
        /// <param name="url"></param>
        /// <param name="setUrl"></param>
        /// <returns></returns>
        private static ReachableSiteOUT GiasNGRequest(string siteKey, string url, bool setUrl)
        {
            ReachableSiteOUT resp = new();

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    if (response.StatusCode == HttpStatusCode.OK) 
                        resp = new(siteKey, setUrl ? url : null, "", null,  true);
                    else
                        resp = new(siteKey, setUrl ? url : null, $"Unreachable: {response.StatusCode}", null, false);
                }
            }
            catch (WebException ex) {
                resp = new(siteKey, setUrl ? url : null, $"Unreachable: {Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex)}", null, false);
            }

            return resp;
        }

        /// <summary>   
        /// Richiesta a NetCoreApi
        /// </summary>
        /// <param name="_hc"></param>
        /// <param name="siteKey"></param>
        /// <param name="request"></param>
        /// <param name="setVersion"></param>
        /// <param name="setUrl"></param>
        /// <returns></returns>
        private static ReachableSiteOUT NetCoreApiRequest(HttpClient _hc, string siteKey, string request, bool setVersion, bool setUrl)
        {
            ReachableSiteOUT resp = new(siteKey, setUrl ? request : null, "Unreachable.", setVersion ? "" : null, false);

            HttpMethod method = HttpMethod.Post;
            string content = "";

            try
            {
                Task<HttpResponseMessage> trm = null;

                if (method == HttpMethod.Post)
                {
                    HttpContent cont = null;
                    if (!string.IsNullOrEmpty(content))
                        cont = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                    trm = _hc.PostAsync(request, cont);
                }
                else
                {
                    if (method == HttpMethod.Get)
                        trm = _hc.GetAsync(request);
                }

                if (trm != null)
                {
                    HttpResponseMessage rm = trm.Result;
                    if (rm.IsSuccessStatusCode)
                    {
                        Task<String> ts = rm.Content.ReadAsStringAsync();
                        JObject obj = JObject.Parse(ts.Result);

                        if (obj.ContainsKey("RispostaStringa"))
                        {
                            RispostaStandard r = JsonConvert.DeserializeObject<RispostaStandard>(obj.ToString());
                            resp = new ReachableSiteOUT(siteKey, setUrl ? request : null, "", setVersion ? r.RispostaStringa : null, true);
                        }
                        else
                        {
                            resp = new(siteKey, setUrl ? request : null, "Unreachable.", setVersion ? "" : null, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resp = new(siteKey, setUrl ? request : null, $"Unreachable: {Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex)}", setVersion ? "" : null, false);
            }

            return resp;
        }
    }
}
