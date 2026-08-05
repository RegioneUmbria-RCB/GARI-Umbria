using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Services.Culture;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.SuperServer.DAL.Autenticazione;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AgronicaCoreAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    // [Authorize]
    public class BaseController : ControllerBase
    {
        protected IConfiguration _config;
        protected ILog _log;
        protected readonly IHttpClientFactory _httpClientFactory;

        protected int siteRedirector;
        protected string agronicaWSBaseURL;
        protected string coreWSBaseURL;
        protected string bearerToken;
        protected string versioneClientChiamate;
        protected int minValiditaLogin;
        protected string RetailCoreWS_BaseURL;

        protected string objP_super_server;
        protected string objP_server;
        protected string objP_utenti;
        protected string user;
        protected string username;
        protected string pivaSuperUser;
        protected string userAgent;
        protected string host;
        protected HttpClient hc;
        protected IServiceProvider _serviceProvider;
        protected readonly ICultureService _cultureService;
        protected readonly IOptions<SecuritySettings> _securitySettings;
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _log = LogManager.GetLogger("CoreAPI");
            agronicaWSBaseURL = config.GetValue<string>("AgronicaWSBaseURL");
            coreWSBaseURL = ""; // config.GetValue<string>("CoreWSBaseURL");
            bearerToken = "";
            siteRedirector = 100;
            versioneClientChiamate = "1.0";
            minValiditaLogin = config.GetValue<int>("minValiditaLogin");
            hc = _httpClientFactory.CreateClient("base");

            RetailCoreWS_BaseURL = config.GetValue<string>("RetailCoreWS_BaseURL");
            _securitySettings = securitySettings;
            _localizer = localizer;
            _serviceProvider = provider;
            _cultureService = _serviceProvider.GetRequiredService<ICultureService>();
        }

        /// <summary>
        /// Decompressione dell'obj contenente: refreshToken, objP_super_server e coreWsBaseUrl
        /// e valorizzazione di quest'ultimi
        /// </summary>
        /// <param name="objRefTok_str"></param>
        /// <returns></returns>
        [Obsolete("Il refresh token è stato trasformato in una guid e non contiene più questi parametri all'interno")]
        protected string unzipObjRefreshToken(string objRefTok_str)
        {
            byte[] byteArray = Convert.FromBase64String(objRefTok_str);
            byte[] byteOut = unzip(byteArray);
            UnicodeEncoding enc = new UnicodeEncoding();

            JObject objParams = JObject.Parse(enc.GetString(byteOut));
            objP_super_server = (string)objParams["objP_super_server"];
            coreWSBaseURL = (string)objParams["coreWSBaseURL"];
            string username = (string)objParams["username"];

            return username;
        }

        /// <summary>
        /// Decompressione dell'obj contenente: objP (super_server, server e utenti) e coreWsBaseUrl
        /// e valorizzazione di quest'ultimi
        /// </summary>
        /// <param name="objParams_str"></param>
        /// <returns></returns>
        protected void unzipObjParams(string objParams_str)
        {
            int numberChars = objParams_str.Length;
            byte[] byteArray = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2) { byteArray[i / 2] = Convert.ToByte(objParams_str.Substring(i, 2), 16); }
            byte[] byteOut = unzip(byteArray);
            UnicodeEncoding enc = new UnicodeEncoding();

            JObject objParams = JObject.Parse(enc.GetString(byteOut));
            coreWSBaseURL = (string)objParams["coreWSBaseURL"];
            username = (string)objParams["codiceFiscale"];
            user = (string)objParams["userName"];
            pivaSuperUser = (string)objParams["pivaSuperUser"];
        }
        
        /// <summary>
        /// Restituisce true se l'utente è autenticato (in questo caso effettua anche alcune configurazioni inziiale - imposta la culture), altrimenti false
        /// </summary>
        /// <param name="Authorization"></param>
        /// <returns></returns>
        protected bool isAuthorized(string Authorization)
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            userAgent = HttpContext.Request.Headers.UserAgent;
            host = HttpContext.Request.Headers.Host;
            
            if (!string.IsNullOrWhiteSpace(Authorization))
            {
                if (Authorization.StartsWith("Bearer "))
                {
                    bearerToken = Authorization.Substring("Bearer ".Length).Trim();
                }
                else //non dovrebbe mai entrare nel else
                {
                    bearerToken = Authorization;
                }
            }
            
            if (identity != null) {

                if (identity.FindFirst("exp") != null && DateTime.Now >= DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(identity.FindFirst("exp").Value)).LocalDateTime) return false;

                // legge obj parametri da token jwt
                if (identity.FindFirst("tokenID") != null && identity.FindFirst("objParams") != null) {
                    string tok_str = identity.FindFirst("tokenID").Value;
                    
                    string objParams_str = identity.FindFirst("objParams").Value;
                    unzipObjParams(objParams_str);

                    //lettura obj params usando objP_super_server
                    var autenticazione = _serviceProvider.GetRequiredService<IAutenticazione>();

                    var connectionString = _config.GetValue<string>("ConnectionString");

                    // decodifica la stringa di connessione se criptata
                    if (_config.GetValue<bool>("ConnectionStringEncoded")) {
                        connectionString = Security.DecryptString(connectionString, _config.GetValue<string>("cr2"));
                    }

                    var objPSuperServer = new AgronicaCoreParametriSuperServer()
                    {
                        StringaConnessione = connectionString,
                        Lingua_Cod = 1,
                        LogDirectory = "C:\\GIASLAN",
                        LogFileName = "GiasOnline_log.txt",
                    };

                    var dt = autenticazione.LeggiObjParametri(tok_str, objPSuperServer);
                    objP_server = (string)dt.Rows[0]["objP_Server"];
                    objP_utenti = (string)dt.Rows[0]["objP_Utenti"];
                    objP_super_server = (string)dt.Rows[0]["objP_SuperServer"];

                    if (objP_super_server != null && objP_server != null && objP_utenti != null && coreWSBaseURL != null)
                    {
                        var objParametriServer = UtilityAgronica.convertStringtoOBJparametri(objP_server, _securitySettings);
                        _cultureService?.SetUICulture(objParametriServer.Lingua_Cod);
                        return true;
                    }
                }

            }

            return false;
        }

        protected CoreWSRequest<CoreWS_Generic<T>> getRequest<T>(T inData)
        {
            var objP = new CoreWS_GenericObjP() { objP_super_server = objP_super_server, objP_server = objP_server, objP_utenti = objP_utenti, user_Agent = userAgent, host = host };
            CoreWS_Generic<T> request = new CoreWS_Generic<T>(objP, inData);
            return new CoreWSRequest<CoreWS_Generic<T>>(request);
        }

        protected CoreWS_Generic<T> getGenericRequest<T>(T inData)
        {
            var objP = new CoreWS_GenericObjP() { objP_super_server = objP_super_server, objP_server = objP_server, objP_utenti = objP_utenti };
            return new CoreWS_Generic<T>(objP, inData);
        }

        protected void logInfo(string info) { _log.Info(info); }

        protected void logErrore(string errore) { _log.Error(errore); }

        protected void logErrore(string errore, Exception ex) { _log.Error(errore,ex); }

        protected string getErrorMessage(Exception ex) {
            if (ex is CoreAPIException)
            {
                var e = (CoreAPIException)ex;
                if (e.StatusCode == StatusCodes.Status400BadRequest)
                {
                    return e.ReasonPhrase;
                }
            }
            return Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, false);
        }

        /// <summary>
        /// Exchanges an api-key for a bearer token by calling the internal /Login/BackgroundAuthentication endpoint.
        /// </summary>
        /// <param name="fmisContext">The api-key header received from the caller.
        /// This string should be a Base-64 endcoded JSON string of type BackgroundAuthenticationModel</param>
        /// <returns>The bearer token to use for subsequent internal calls.</returns>
        /// <exception cref="ValidationException">If the apiKey is missing or empty.</exception>
        /// <exception cref="UnauthorizedAccessException">If the api-key is not authorized.</exception>
        protected async Task<string> GetBearerTokenViaBackendAuthentication(string fmisContext)
        {
            if (string.IsNullOrWhiteSpace(fmisContext))
            {
                throw new ValidationException("Header 'fmis-context' is required.");
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}".TrimEnd('/');
            var endpointUrl = $"{baseUrl}/Login/BackgroundAuthentication";

            using var httpClient = _httpClientFactory.CreateClient();
            var payload = new { key = fmisContext };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(endpointUrl, content);
            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException("Invalid api-key.");
            }

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new CoreAPIException { StatusCode = (int)response.StatusCode, ReasonPhrase = "Empty response from authentication service." };
            }

            var responseObj = JsonConvert.DeserializeObject<RispostaStandard>(responseBody);
            if (responseObj?.RispostaStringa == null)
            {
                throw new CoreAPIException { StatusCode = (int)response.StatusCode, ReasonPhrase = "Invalid JSON response from authentication service." };
            }

            if (!responseObj.RispostaOK)
            {
                var error = responseObj.Errore ?? "Authentication failed.";
                throw new UnauthorizedAccessException(error);
            }

            return responseObj.RispostaStringa;
        }

        public static byte[] zip(string dati, Encoding enc) {
            byte[] byteIn;

            switch (enc.GetType().Name){
                case (nameof(Encoding.ASCII) + "Encoding"):
                    byteIn = new ASCIIEncoding().GetBytes(dati);
                    break;
                case (nameof(Encoding.Unicode) + "Encoding"):
                    byteIn = new UnicodeEncoding().GetBytes(dati);
                    break;
                case (nameof(Encoding.UTF8) + "Encoding"):
                    byteIn = new UTF8Encoding().GetBytes(dati);
                    break;
                case (nameof(Encoding.UTF32) + "Encoding"):
                    byteIn = new UTF32Encoding().GetBytes(dati);
                    break;
                default:
                    byteIn = new UnicodeEncoding().GetBytes(dati);
                    break;
            }
            MemoryStream MemStream = new MemoryStream();
            Stream ZipStream = new GZipStream(MemStream, CompressionMode.Compress, true);
            ZipStream.Write(byteIn, 0, byteIn.Length);
            ZipStream.Close();
            MemStream.Position = 0;
            byte[] byteOut = new byte[MemStream.Length - 1 + 1];
            MemStream.Read(byteOut, 0, (int)MemStream.Length);
            return byteOut;
        }

        public static byte[] unzip(byte[] byteCompressed)
        {
            //byte[] byteCompressed = Convert.FromBase64String(datiCompressi);
            using (MemoryStream memStreamIn = new MemoryStream(byteCompressed))
            {
                using (GZipStream zipStreamIn = new GZipStream(memStreamIn, CompressionMode.Decompress))
                {
                    using (MemoryStream memStreamOut = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = zipStreamIn.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memStreamOut.Write(buffer, 0, bytesRead);
                        }
                        return memStreamOut.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Call controller that will make the call to the BE with try catch.
        /// This method was created to avoid code repetition in the controllers and to manage in a single point
        /// the exceptions and the relative log.
        /// It accepts a function that returns a RispostaStandard (or RispostaStandard<T>) and returns an ObjectResult
        /// with the correct status code based on the result of the function and the exceptions thrown.
        /// </summary>
        /// <param name="call">A lamba function that creates the request and calls the correct method of the controller.</param>
        protected ObjectResult CallController(Func<RispostaStandard> call)
        {
            var result = new RispostaStandard();
            try
            {
                result = call.Invoke();
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        /// Call controller that will make the call to the BE with try catch.
        /// This method was created to avoid code repetition in the controllers and to manage in a single point
        /// the exceptions and the relative log.
        /// It accepts a function that returns a RispostaStandard (or RispostaStandard<T>) and returns an ObjectResult
        /// with the correct status code based on the result of the function and the exceptions thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="call">A lamba function that creates the request and calls the correct method of the controller.</param>
        /// <example><code>
        /// 
        ///  if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);
        ///  var readWorkflowData = () => {
        ///      var request = getRequest("");
        ///      return new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).GetWorkflowManagementData(request);
        ///  };
        ///  return CallController(readWorkflowData);
        ///  
        /// </code></example>
        protected ObjectResult CallController<T>(Func<RispostaStandard<T>> call)
        {
            var result = new RispostaStandard<T>();
            try
            {
                result = call.Invoke();
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
