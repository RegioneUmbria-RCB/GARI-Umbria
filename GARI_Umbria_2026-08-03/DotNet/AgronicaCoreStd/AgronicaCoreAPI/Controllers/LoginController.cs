using AgronicaCoreAPI.DTO;
using AgronicaCoreAPI.Helpers;
using AgronicaCoreAPI.models;
using AgronicaCoreAPI.Resources;
using AgronicaCoreDTOStd.InData.Provisioning;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.provisioning;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Services.Security;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.SuperServer.BIZ.Autenticazione;
using AgronicaNetCore.SuperServer.DAL.Autenticazione;
using AgronicaNetCore.Utenti.BIZ.Services.ControlloScadenzaPasswordAlLogin;
using AgronicaNetCore.Utenti.DAL.DataLayer.Utenti;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using anagrafiche = AgronicaCoreModelsSTD.anagrafiche;
using profilazione = AgronicaCoreModelsSTD.profilazione;

namespace AgronicaCoreAPI.Controllers
{

    public class LoginController : BaseController
    {
        public LoginController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
        }

        private Token SaveToken(CoreWS_Utenti_R payload, profilazione.LoginModel login)
        {
            objP_super_server = payload.objP_super_server;
            string idToken = Guid.NewGuid().ToString();
            RefreshToken refreshToken = GenerateRefreshToken();
            RispostaStandard resp = new RispostaStandard();
            Token token;

            try
            {
                var objRefreshToken = new CreaTokenJWT_In
                {
                    idToken = idToken,
                    objP_super_server = payload.objP_super_server,
                    objP_server = payload.objP_server,
                    objP_utenti = payload.objP_utenti,
                    codiceFiscale = payload.CodiceFiscale,
                    coreWSBaseURL = coreWSBaseURL,
                    pivaSuperUser = login.PivaSuperUser,
                    username = login.Username,
                    versioneApp = login.VersioneAPP,
                    refreshToken = refreshToken.Token,
                    dataCreazione = (DateTime)refreshToken.DataCreazione,
                    dataFineValidita = refreshToken.DataExpireToken,
                    IdDB = login.idDB
                };

                objP_server = payload.objP_server;
                objP_utenti = payload.objP_utenti;
                objP_super_server = payload.objP_super_server;
                var request = getRequest(objRefreshToken);
                resp = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).creaTokenJWT(request);

                if (!resp.RispostaOK) { throw new Exception(resp.Errore); }

                //crea l'oggetto token
                token = new Token()
                {
                    idToken = idToken,
                    objP_super_server = payload.objP_super_server,
                    objP_server = payload.objP_server,
                    objP_utenti = payload.objP_utenti,
                    CodiceFiscale = payload.CodiceFiscale,
                    CoreWSBaseURL = coreWSBaseURL,
                    Username = login.Username,
                    Password = login.Password,
                    PivaSuperUser = login.PivaSuperUser,
                    VersioneAPP = login.VersioneAPP,
                    Data_Creazione = DateTime.Now,
                    refreshToken = resp.RispostaStringa,
                    refreshToken_ExpirationDate = refreshToken.DataExpireToken
                };
            }
            catch (Exception ex)
            {
                string errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                //logErrore("Errore Login: " + JsonConvert.SerializeObject(token) + "\n" + errore);
                resp.RispostaOK = false;
                resp.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                token = null;
                throw new Exception(errore);
            }


            return token;
        }

        private string GenerateJSONWebToken(
            string CodiceFiscale,
            string coreWSBaseURL,
            string tokenID,
            string Username,
            string PivaSuperUser,
            int MinutiValiditaLoginMemorizzato,
            out DateTime scadenzaToken)
        {
            var key = Environment.GetEnvironmentVariable(_config["Jwt:KeyName"] ?? "JWT_KEY") ?? _config["Jwt:Key"] ??
                throw new ApplicationException("JWT key is not configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)); 
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims;
            if (string.IsNullOrEmpty(tokenID))
            {
                claims = new[] {
                    new Claim("CodiceFiscale", CodiceFiscale),
                    new Claim("CoreWSBaseURL", coreWSBaseURL),
                    new Claim("UserName", Username),
                    new Claim("PivaSuperUser", PivaSuperUser)
                };
            }
            else
            {
                JObject objParams = new JObject();
                objParams.Add("coreWSBaseURL", coreWSBaseURL);
                objParams.Add("codiceFiscale", CodiceFiscale);
                objParams.Add("userName", Username);
                objParams.Add("pivaSuperUser", PivaSuperUser);
                byte[] byteOut = zip(objParams.ToString(), Encoding.Unicode);
                string objParams_str = BitConverter.ToString(byteOut).Replace("-", "");

                claims = new[] {
                    new Claim("tokenID", tokenID),
                    new Claim("objParams", objParams_str)
                };
            }
            scadenzaToken = DateTime.UtcNow.AddMinutes(MinutiValiditaLoginMemorizzato);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: scadenzaToken,
              signingCredentials: credentials);

            var claimsIdentity = new ClaimsIdentity(token.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var signIn = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string LeggiProfilazione(string PivaSuperUser, string VersioneAPP)
        {
            string webService = agronicaWSBaseURL + "/WS_Autenticazione.asmx/InizializzazioneProfilazione";
            EseguiChiamateApi xChiamata = new EseguiChiamateApi(webService, bearerToken, versioneClientChiamate);
            RispostaStandard risposta = xChiamata.ChiamaWS("{\"PivaSuperUser\":\"" + PivaSuperUser + "|" + VersioneAPP + "\"}");
            string CoreWsUrl = null;
            if (risposta != null && risposta.RispostaStringa != null)
            {
                string[] urls = risposta.RispostaStringa.Split(';');
                CoreWsUrl = urls.Length > 1 ? urls[0] : risposta.RispostaStringa;
            }
            return CoreWsUrl;
        }

        private List<Connessioni> LeggiConnessioni(string PivaSuperUser, string VersioneAPP)
        {
            var request = new ConnessioniModel
            {
                PivaSuperUser = PivaSuperUser,
                SiteRedirector = siteRedirector
            };
            if (string.IsNullOrEmpty(coreWSBaseURL))
            {
                coreWSBaseURL = LeggiProfilazione(PivaSuperUser, VersioneAPP);
            }
            string webService = coreWSBaseURL + "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/ListaConnessioni";
            EseguiChiamateApi xChiamata = new EseguiChiamateApi(webService, bearerToken, versioneClientChiamate);
            RispostaStandard risposta = xChiamata.ChiamaWS(JsonConvert.SerializeObject(request));
            List<Connessioni> listConnessioni = null;
            if (risposta != null && risposta.RispostaStringa != null)
            {
                listConnessioni = JsonConvert.DeserializeObject<List<Connessioni>>(risposta.RispostaStringa);
            }
            return listConnessioni;
        }

        private CoreWS_Utenti_R AuthenticateUser(profilazione.LoginModel login)
        {
            CoreWS_Utenti_R autenticazione = null;
            string datiConnessione = null;
            string pivaSuperUser = login.PivaSuperUser;
            string versione = login.VersioneAPP;
            if (string.IsNullOrEmpty(pivaSuperUser)) return null;
            if (string.IsNullOrEmpty(versione)) versione = "Release";

            // ricavo i dati della connessione se non passati
            if (!string.IsNullOrEmpty(login.CoreWSBaseURL))
            {
                login.CoreWSBaseURL = convertCoreWSLink(login.CoreWSBaseURL);
                coreWSBaseURL = login.CoreWSBaseURL;
            }
            else
            {
                coreWSBaseURL = LeggiProfilazione(pivaSuperUser, versione);
            }

            // leggo dati connessione
            if (!string.IsNullOrEmpty(coreWSBaseURL))
            {
                var connessioni = LeggiConnessioni(login.PivaSuperUser, versione);
                if (login.idDB == 0 && versione.Split("_").Length > 1)
                {
                    login.idDB = int.Parse(versione.Split("_")[1]);
                }
                if (login.idDB > 0)
                {
                    datiConnessione = (from c in connessioni where c.chiave.Split("|")[0] == login.idDB.ToString() select c.chiave).FirstOrDefault();
                }
                else
                {
                    datiConnessione = connessioni.Count > 0 ? connessioni.First().chiave : null;
                }
            }

            if (!string.IsNullOrEmpty(datiConnessione))
            {

                AutenticazioneModel request = new()
                {
                    Username = login.Username,
                    Password = login.Password,
                    DatiConnessioneSelezionata = datiConnessione
                };

                string webService = coreWSBaseURL + "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/Autenticazione";
                EseguiChiamateApi xChiamata = new EseguiChiamateApi(webService, bearerToken, versioneClientChiamate);
                RispostaStandard<CoreWS_Utenti_R> risposta = xChiamata.ChiamaWS<CoreWS_Utenti_R>(JsonConvert.SerializeObject(request));
                if (risposta != null && risposta.RispostaStringa != null)
                {
                    autenticazione = risposta.RispostaStringa;
                }
            }

            return autenticazione;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] profilazione.LoginModel login)
        {
            IActionResult response = Unauthorized();
            RispostaStandard result = new RispostaStandard();

            try
            {
                if (login.idDB > 0)
                {
                    //Login da Online
                    //Dall'App non arriva IDDB
                    siteRedirector = 300;
                }
                else
                {
                    //Login da App
                    siteRedirector = 100;
                }

                var payload = AuthenticateUser(login);
                if (payload != null)
                {
                    // DS11-API §Dipendenze Business Logic — Controllo Scadenza Password (DS10-BL).
                    // Eseguito PRIMA di SaveToken per non creare record di sessione ordinaria in caso di password scaduta.
                    // Riferimento DS: DS11-API §Descrizione — in caso di password scaduta blocca il login ordinario.
                    var objParametriServer = UtilityAgronica.ConvertStringToObjParametriServer(payload.objP_server, _securitySettings);
                    var objParametriUtenti = UtilityAgronica.ConvertStringToObjParametriUtenti(payload.objP_utenti, _securitySettings);
                    var controlloScadenza = _serviceProvider.GetRequiredService<IControlloScadenzaPasswordAlLoginService>();
                    var risultatoScadenza = await controlloScadenza.ControllaScadenzaAsync(
                        login.Username, login.PivaSuperUser, objParametriUtenti, objParametriServer);

                    // DS11-API §Risposte — 403 Password Scaduta.
                    // Il controller NON crea la sessione ordinaria; segnala al client il cambio password obbligatorio.
                    if (risultatoScadenza.AzioneRichiesta == AzioneControlloScadenza.RedirigWizard)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new
                        {
                            success = false,
                            message = "Password scaduta. Cambio password richiesto",
                            reason = "PASSWORD_EXPIRED"
                        });
                    }

                    // DS11-API §Risposte — 200 Login Ordinario Riuscito (Password Valida).
                    Token token = SaveToken(payload, login);

                    var tokenString = GenerateJSONWebToken(payload.CodiceFiscale, coreWSBaseURL, token.idToken, login.Username,
                                                           login.PivaSuperUser, payload.MinutiValiditaLoginMemorizzato, out DateTime scadenzaToken);

                    var authCookie = ExtractAuthCookieFromCookies();

                    var autenticazioneService = _serviceProvider.GetRequiredService<IAutenticazioneService>();
                    var campi = new AggiornaUtentiTokenJWTCampi(authCookie, tokenString, scadenzaToken);
                    var authCookieUpdated = await autenticazioneService.AggiornaUtentiTokenJWTAsync(token.idToken, campi, UtilityAgronica.ConvertStringToObjParametriSuperServer(objP_super_server, _securitySettings));

                    SetRefreshTokenInCookie(token.refreshToken, token.refreshToken_ExpirationDate);

                    result.RispostaOK = true;
                    result.RispostaStringa = tokenString;
                    // var loginUtente = new LoginUtente { token = tokenString, autenticazione = payload };
                    // result.RispostaStringa = JsonConvert.SerializeObject(loginUtente);
                    // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, HttpContext.User);
                    response = Ok(result);
                }
            }
            catch (Exception ex)
            {
                string errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                logErrore("Errore Login: " + JsonConvert.SerializeObject(login) + "\n" + errore);
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            IActionResult response = Unauthorized();
            RispostaStandard result = new RispostaStandard();

            try
            {
                string tok_str = Request.Cookies["refreshToken"];

                Token tokenOld = new Token();
                if (!string.IsNullOrEmpty(tok_str))
                {
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

                    string newTokenID = Guid.NewGuid().ToString();
                    RefreshToken newRefreshToken = GenerateRefreshToken();

                    RefreshTokenJWT_In paramsRefreshToken = new RefreshTokenJWT_In()
                    {
                        oldRefreshToken = tok_str,
                        newTokenID = newTokenID,
                        newRefreshToken = newRefreshToken.Token,
                        objP_super_server = objP_super_server,
                        dataCreazione = (DateTime)newRefreshToken.DataCreazione,
                        dataFineValidita = newRefreshToken.DataExpireToken
                    };
                    var autenticazioneService = _serviceProvider.GetRequiredService<IAutenticazioneService>();

                    var newTokenInfo = await autenticazioneService.RefreshTokenAsync(paramsRefreshToken, objPSuperServer);

                    //var requestRefresh = getRequest(paramsRefreshToken);
                    //respRefresh = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).refreshTokenJWT(requestRefresh);

                    objP_server = newTokenInfo.objP_server;
                    objP_utenti = newTokenInfo.objP_utenti;
                    objP_super_server = newTokenInfo.objP_super_server;
                    username = newTokenInfo.username;
                    user = newTokenInfo.codiceFiscale;
                    pivaSuperUser = newTokenInfo.pivaSuperUser;
                    coreWSBaseURL = newTokenInfo.coreWSBaseURL;

                    SetRefreshTokenInCookie(newTokenInfo.refreshToken, newTokenInfo.dataFineValidita);

                    var request = getRequest("");
                    RispostaStandard<int> minutiResult = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).getMinutiValiditaLoginMemorizzato(request);
                    int minutiValiditaLogin = minutiResult.RispostaStringa;

                    string tokenString = GenerateJSONWebToken(newTokenInfo.codiceFiscale,
                                                              coreWSBaseURL, newTokenID, newTokenInfo.username, newTokenInfo.pivaSuperUser,
                                                              minutiValiditaLogin, out DateTime scadenzaToken);

                    //trovo il primo StringValues che contiene all'interno una stringa dove auth_cookie è presente
                    var authCookie = ExtractAuthCookieFromCookies();

                    var campi = new AggiornaUtentiTokenJWTCampi(authCookie, tokenString, scadenzaToken);
                    var authCookieUpdated = await autenticazioneService.AggiornaUtentiTokenJWTAsync(newTokenID, campi, UtilityAgronica.ConvertStringToObjParametriSuperServer(objP_super_server, _securitySettings));

                    result.RispostaOK = true;
                    result.RispostaStringa = tokenString;
                    response = Ok(result);
                }
            }
            catch (Exception ex)
            {
                logErrore(ex.Message, ex);
                string errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                //logErrore("Errore Login: " + JsonConvert.SerializeObject(token) + "\n" + errore);
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return response;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return new string[] { accessToken };
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Versione")]
        public string GetVersione()
        {
            string versione = "";
            string path = "GiasVersioneCorrente.txt";

            if (System.IO.File.Exists(path))
            {
                versione = System.IO.File.ReadAllText(path);
            }

            return versione;
        }

        [HttpGet]
        [Route("Autenticazione")]
        public ObjectResult LeggiAutenticazione([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard<CoreWS_Utenti_R> result = new RispostaStandard<CoreWS_Utenti_R>();

            try
            {
                result.RispostaStringa = new(objP_super_server, objP_server, objP_utenti, username, minValiditaLogin > 0 ? minValiditaLogin : 240);
                result.RispostaOK = true;
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Demetra")]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<ObjectResult> LoginByDemetra([FromHeader] string authorization, AuthDemetraIn authDemetra)
        {
            RispostaStandard result = new RispostaStandard();

            try
            {
                if (string.IsNullOrEmpty(authorization))
                    return StatusCode(StatusCodes.Status401Unauthorized, null);

                var auth = authorization.Split(' ');
                if (auth.Length != 2)
                    return StatusCode(StatusCodes.Status401Unauthorized, null);

                authorization = auth[1];

                string authUrlDemetra = null!;
                if (authDemetra == null)
                    return StatusCode(StatusCodes.Status401Unauthorized, null);
                else if (string.IsNullOrEmpty(authDemetra.DemetraBaseURL))
                    return StatusCode(StatusCodes.Status401Unauthorized, null);
                else
                    authUrlDemetra = new Uri(new Uri(authDemetra.DemetraBaseURL), "anag-api/api/v1/user/validate-token").ToString();

                int idDb = 0;
                string versione = authDemetra.VersioneAPP;
                if (string.IsNullOrEmpty(versione))
                    return StatusCode(StatusCodes.Status401Unauthorized, null);
                else if (versione.Split('_').Length != 2)
                    return StatusCode(StatusCodes.Status401Unauthorized, null);

                int.TryParse(versione.Substring(versione.LastIndexOf('_') + 1), out idDb);
                versione = versione.Substring(0, versione.LastIndexOf('_'));

                // Chiamata a Demetra per verifica del token d'autenticazione
                AuthDemetraOut authDemetraResponse = null;
                var response = await new HttpClientHelper("JWT", authorization).GetAsync(authUrlDemetra);
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    authDemetraResponse = JsonConvert.DeserializeObject<AuthDemetraOut>(content);
                else
                    response.EnsureSuccessStatusCode();

                if (authDemetraResponse == null || !authDemetraResponse.validated)
                    return StatusCode(StatusCodes.Status401Unauthorized, null);

                // Recupero objParametri per accedere ai DB
                var _securityService = _serviceProvider.GetService<ISecurityService>();
                AgronicaCoreParametriTriple objParametri = _securityService.GetAgronicaCoreParametriTriple(idDb);

                // recupero url per accedere al core WS per l'autenticazione
                var _securityLayerDAL = _serviceProvider.GetService<ISecurityLayerDAL>();
                coreWSBaseURL = await _securityLayerDAL.LeggiConfigurazioneSitiScalareAsync(
                    "GiasOnline_WS_Core_AgroWS_Core", objParametri.ObjParametriServer, objParametri.ObjParametriSuperServer
                );

                // verifica esistenza utente
                string password = null!;
                IUtenti utenti = _serviceProvider.GetRequiredService<IUtenti>();
                DataTable utenza = await utenti.LeggiUtenteAsync(authDemetraResponse.Username, objParametri.ObjParametriUtenti);
                if (authDemetraResponse.is_socio)
                {
                    // utenza di tipo socio
                    if (utenza != null && utenza.Rows.Count > 0)
                    {
                        // verifica allienamento tra utenza e cuaa
                        string numeroTessera = await utenti.LeggiNumeroTesseraByCuaaAsync(authDemetraResponse.Username,
                                authDemetraResponse.cuaa, objParametri.ObjParametriServer);
                        if (string.IsNullOrEmpty(numeroTessera))
                            return StatusCode(StatusCodes.Status401Unauthorized, null);

                        password = utenza.Rows[0]["Password"].ToString();
                    }
                    else
                    {
                        // Ricopia utente demetra su GIAS
                        string piva = await utenti.LeggiPivaByCuaaAsync(authDemetraResponse.cuaa, objParametri.ObjParametriServer);
                        if (string.IsNullOrEmpty(piva))
                            return StatusCode(StatusCodes.Status401Unauthorized, null);

                        // Recupero le informazioni base dell'utente (persona giuridica/fisica)
                        anagrafiche.UtenteColdiretti datiColdiretti = GetCodiceUtenteColdiretti(authDemetraResponse.cuaa);
                        if (datiColdiretti == null)
                            return StatusCode(StatusCodes.Status401Unauthorized, null);
                        else if (authDemetraResponse.Username != datiColdiretti.GetUserCode())
                            return StatusCode(StatusCodes.Status401Unauthorized, null);

                        int profiloDefault = 0;
                        int.TryParse(_config["SincronizzazioneDemetra:BaseUrlColdirettiWebServicePortaleSocio"].ToString(), out profiloDefault);

                        profilazione.LeggiScriviVisibilitaUtente profiloUtente = await utenti.MapUserToAgronicaUtente(
                            authDemetra.PivaSuperUser, piva, authDemetraResponse.Username, datiColdiretti, profiloDefault, objParametri.ObjParametriServer);

                        var request = getRequest(profiloUtente);
                        result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).CreaUtenteConVisibilita(request);
                        if (!result.RispostaOK)
                            return StatusCode(StatusCodes.Status500InternalServerError, result);

                        password = profiloUtente.Utente.Password;
                    }
                }
                else
                {
                    // non è un'utenza socio
                    if (utenza != null && utenza.Rows.Count > 0)
                        password = utenza.Rows[0]["Password"].ToString();
                    else
                        return StatusCode(StatusCodes.Status401Unauthorized, null);
                }

                var login = new profilazione.LoginModel()
                {
                    PivaSuperUser = authDemetra.PivaSuperUser,
                    CoreWSBaseURL = coreWSBaseURL,
                    VersioneAPP = "",
                    Username = authDemetraResponse.Username,
                    Password = password,
                    idDB = idDb
                };

                result = await GetTokenAutenticazione(login);
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
        [Route("Impostazioni")]
        public ObjectResult LeggiImpostazioni([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpostazioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("ImpreseImpostazioni")]
        public ObjectResult LeggiImpreseImpostazioni(string piva, [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                APICallsBasic request = new CoreWS_Imprese_Impostazioni(piva, objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiImpreseImpostazioni(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("Utente")]
        public ObjectResult LeggiDatiUtente([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                CoreWS_Operatore request = new CoreWS_Operatore(objP_super_server, objP_server, objP_utenti, user, false);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiDatiUtente(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("PermessiAPP")]
        public ObjectResult LeggiPermessiAPP([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            RispostaStandard result = new RispostaStandard();

            try
            {
                APICallsBasic request = new APICallsBasic(objP_super_server, objP_server, objP_utenti);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).LeggiPermessiAPP(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                SameSite = SameSiteMode.None,
                Secure = true
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private RefreshToken GenerateRefreshToken()
        {
            DateTime dataCreazione = DateTime.UtcNow;
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                DataCreazione = dataCreazione,
                DataExpireToken = dataCreazione.AddDays(7),
                DataRevoke = null
            };
        }

        /// <summary>
        /// Metodo che esegue l'autenticazione tramite chiave hash
        /// </summary>
        /// <param name="key"> stringa codificata base64 che contiene il seguente json
        /// {
        ///     "access_token" : "token_preso_da_utenti_token",
        ///     "corewsbaseurl" : "url base corews",
        ///     "iddb" : 0
        /// }
        /// </param>
        /// <returns>JWT autenticazione core api</returns>
        /// <remarks>
        /// 
        ///     POST /BackgroundAuthentication
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Messaggio in HTML</response>
        /// <response code="500">Errore durante l'operazione</response>

        [HttpPost]
        [Route("BackgroundAuthentication")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RispostaStandard), StatusCodes.Status200OK)]
        public async Task<IActionResult> BackgroundAuthentication([FromBody] KeyAuth key)
        {
            IActionResult response = Unauthorized();
            RispostaStandard result = new RispostaStandard();
            if (key.key.Equals(""))
            {
                throw new AccessViolationException("Wrong key");
            }

            var decrypt = JsonConvert.DeserializeObject<BackgroundAuthenticationModel>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(key.key)));

            try
            {

                if (decrypt.access_token.Equals("") || decrypt.corewsbaseurl.Equals(""))
                {
                    throw new JsonException("Incorrect connection parameters");
                }

                var coreWsBaseUrl = convertCoreWSLink(decrypt.corewsbaseurl);
                var res = new CoreWSController(hc, coreWsBaseUrl, "", Request).LeggiParametriConnessioneDaAccessToken(new LoginAccessTokenModel() { access_token = decrypt.access_token });
                if (!res.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, res);

                siteRedirector = 300;   //Lavez - 19/02/2025 - fix per bypassare il filtro sulla colonna DisponibilePerGiasAPP

                var login = new profilazione.LoginModel()
                {
                    PivaSuperUser = res.RispostaStringa.PivaSuperUser,
                    CoreWSBaseURL = decrypt.corewsbaseurl,
                    VersioneAPP = "",
                    Username = res.RispostaStringa.Username,
                    Password = res.RispostaStringa.UserPwd,
                    idDB = decrypt.iddb == 0 ? res.RispostaStringa.iddb_server : decrypt.iddb
                };

                var payload = AuthenticateUser(login);
                if (payload != null)
                {
                    Token token = SaveToken(payload, login);
                    var tokenString = GenerateJSONWebToken(
                        payload.CodiceFiscale,
                        coreWSBaseURL,
                        token.idToken,
                        login.Username,
                        login.PivaSuperUser,
                        payload.MinutiValiditaLoginMemorizzato, out DateTime scadenzaToken);

                    var authCookie = ExtractAuthCookieFromCookies();

                    var autenticazioneService = _serviceProvider.GetRequiredService<IAutenticazioneService>();
                    var campi = new AggiornaUtentiTokenJWTCampi(authCookie, tokenString, scadenzaToken);
                    var authCookieUpdated = await autenticazioneService.AggiornaUtentiTokenJWTAsync(token.idToken, campi, UtilityAgronica.ConvertStringToObjParametriSuperServer(payload.objP_super_server, _securitySettings));

                    SetRefreshTokenInCookie(token.refreshToken, token.refreshToken_ExpirationDate);
                    result.RispostaOK = true;
                    result.RispostaStringa = tokenString;
                    // var loginUtente = new LoginUtente { token = tokenString, autenticazione = payload };
                    // result.RispostaStringa = JsonConvert.SerializeObject(loginUtente);
                    // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, HttpContext.User);
                    response = Ok(result);
                }
            }
            catch (Exception ex)
            {
                string errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                logErrore("Errore BackgroundAuthentication: " + JsonConvert.SerializeObject(decrypt) + "\n" + errore);
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return response;
        }

        private string ExtractAuthCookieFromCookies()
        {
            var authCookieValue = string.Empty;
            StringValues cookieValues = this.Response.Headers.Values.FirstOrDefault(v => v.Any(s => s.Contains("auth_cookie")));
            if (cookieValues.Count > 0)
            {
                var authCookie = cookieValues.FirstOrDefault(c => c.Contains("auth_cookie="));
                if (authCookie is not null)
                {
                    var authCookieContent = authCookie.Split(";").First(s => s.Contains("auth_cookie")).Split("auth_cookie=")[1];
                    if (authCookieContent.Contains("chunks"))
                    {
                        var chunksNr = Convert.ToInt32(authCookieContent.Split("-")[1]);
                        for (int i = 1; i <= chunksNr; i++)
                        {
                            var chunkAuthCookie = cookieValues.FirstOrDefault(s => s.Contains($"auth_cookieC{i}"));

                            if (chunkAuthCookie is not null)
                            {
                                chunkAuthCookie = chunkAuthCookie.Split(";").First(s => s.Contains($"auth_cookieC{i}")).Split("=")[1];
                                authCookieValue = string.Format("{0}{1}", authCookieValue, chunkAuthCookie);
                            }
                        }
                    }
                    else
                    {
                        authCookieValue = authCookieContent;
                    }
                }
            }

            return authCookieValue;
        }


        private string convertCoreWSLink(string baseAddr)
        {
            if (_config["CoreWSCallConfiguration:CoreWSOnLocalhost"] == "true")
            {
                string protocol = "http://";
                string port = "80";
                string dns = "localhost";
                if (!string.IsNullOrEmpty(_config["CoreWSCallConfiguration:CoreWSLocalhostProtocol"]))
                {
                    protocol = _config["CoreWSCallConfiguration:CoreWSLocalhostProtocol"];
                    
                    if (!protocol.Contains("://"))
                    {
                        protocol = protocol + "://";
                    }
                    
                    if (!string.IsNullOrEmpty(_config["CoreWSCallConfiguration:CoreWSOnDns"]))
                    {
                        dns = _config["CoreWSCallConfiguration:CoreWSOnDns"];
                    }
                    
                    if (!string.IsNullOrEmpty(_config["CoreWSCallConfiguration:CoreWSOnPort"]))
                    {
                        port = _config["CoreWSCallConfiguration:CoreWSOnPort"];
                    }
                }
                baseAddr = localhostBaseAddr(baseAddr, protocol, dns, port);
            }

            // If baseAddr is not a valid absolute URL (e.g. "/coreWS"), reconstruct it
            // using the current request origin so that downstream logic always receives a proper URL.
            if (!Uri.TryCreate(baseAddr, UriKind.Absolute, out _))
            {
                baseAddr = $"{Request.Scheme}://{Request.Host}/{baseAddr.TrimStart('/')}";
            }
            return baseAddr;
        }

        private string localhostBaseAddr(string baseAddr, 
                                        string CoreWSProtocol, 
                                        string CoreWSDns,
                                        string CoreWSPort)
        {
            string baseString = "";
            string protocol = "";
            string[] baseArray;
            string returnBaseAddr = "";
            if (baseAddr.Contains("https"))
            {
                protocol = "https://";
                baseString = baseAddr.Replace(protocol, "");
                baseArray = baseString.Split("/");
            }
            else if (baseAddr.Contains("http"))
            {
                protocol = "http://";
                baseString = baseAddr.Replace(protocol, "");
                baseArray = baseString.Split("/");
            }
            else
            {
                return baseAddr;
            }

            returnBaseAddr += CoreWSProtocol + CoreWSDns +":" + CoreWSPort ;
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(baseArray[i]) && i != 0)
                {
                    returnBaseAddr += "/" + baseArray[i];
                }
            }
            return returnBaseAddr;
        }

        private anagrafiche.UtenteColdiretti GetCodiceUtenteColdiretti(string CUAA)
        {
            anagrafiche.UtenteColdiretti result = null!;
            Uri uri = new Uri(_config["SincronizzazioneDemetra:BaseUrlColdirettiWebServicePortaleSocio"]);
            string url = new Uri(uri, $"/anagrafica/v1/anagrafica/persone-giuridiche/SGL_CODFIS/{CUAA}").ToString();
            string resp = new Http().chiamaWS(null, null, url, "application/json", "GET", "application/json", "", TimeOut: 600);
            result = JsonConvert.DeserializeObject<anagrafiche.UtenteColdiretti>(resp);
            if (result == null || string.IsNullOrEmpty(result.GetUserCode()))
            {
                url = new Uri(uri, $"/anagrafica/v1/anagrafica/persone-fisiche/SGL_CODFIS/{CUAA}").ToString();
                resp = new Http().chiamaWS(null, null, url, "application/json", "GET", "application/json", "", TimeOut: 600);
                result = JsonConvert.DeserializeObject<anagrafiche.UtenteColdiretti>(resp);
                if (result == null || string.IsNullOrEmpty(result.GetUserCode()))
                    result = null!;
            }
            return result;
        }

        private async Task<RispostaStandard> GetTokenAutenticazione(profilazione.LoginModel login)
        {
            RispostaStandard result = new RispostaStandard();
            var payload = AuthenticateUser(login);
            if (payload != null)
            {
                Token token = SaveToken(payload, login);
                var tokenString = GenerateJSONWebToken(
                    payload.CodiceFiscale,
                    coreWSBaseURL,
                    token.idToken,
                    login.Username,
                    login.PivaSuperUser,
                    payload.MinutiValiditaLoginMemorizzato, out DateTime scadenzaToken);

                var authCookie = ExtractAuthCookieFromCookies();

                var autenticazioneService = _serviceProvider.GetRequiredService<IAutenticazioneService>();
                var campi = new AggiornaUtentiTokenJWTCampi(authCookie, tokenString, scadenzaToken);
                var authCookieUpdated = await autenticazioneService.AggiornaUtentiTokenJWTAsync(token.idToken,
                    campi, UtilityAgronica.ConvertStringToObjParametriSuperServer(payload.objP_super_server, _securitySettings));

                SetRefreshTokenInCookie(token.refreshToken, token.refreshToken_ExpirationDate);
                result.RispostaOK = true;
                result.RispostaStringa = tokenString;
            }
            return result;
        }

    }

    public class KeyAuth
    {
        public string key { get; set; }
    }

    public class LoginAccessTokenModel
    {
        public string access_token { get; set; }
    }

    public class AutenticazioneModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string DatiConnessioneSelezionata { get; set; }
    }

    public class ConnessioniModel
    {
        [Required]
        public string PivaSuperUser { get; set; }
        [Required]
        public int SiteRedirector { get; set; }
    }

    public class Connessioni
    {
        public string chiave { get; set; }
        public string valore { get; set; }
    }

    public class Impostazioni
    {
        public int Impostazione_Cod { get; set; }
        public string Impostazione_Valore_1 { get; set; }
    }

    public class LoginUtente
    {
        public string token { get; set; }
        public CoreWS_Utenti_R autenticazione { get; set; }
        public List<Impostazioni> impostazioni { get; set; }
    }

    public class RefreshToken
    {
        [Obsolete("Non più presente all'interno del refresh token")]
        public int Id { get; set; }
        public string Token { get; set; }
        [Obsolete("Non più presente all'interno del refresh token")]
        public string UserName { get; set; }
        public DateTime DataExpireToken { get; set; }
        public bool IsExpired => DateTime.UtcNow >= DataExpireToken;
        public DateTime? DataCreazione { get; set; }
        [Obsolete("Non più presente all'interno del refresh token")]
        public string IpCreazione { get; set; }
        public DateTime? DataRevoke { get; set; }
        [Obsolete("Non più presente all'interno del refresh token")]
        public string IpRevoke { get; set; }
        [Obsolete("Non più presente all'interno del refresh token")]
        public bool IsActive => DataRevoke == null && !IsExpired;
    }


}
