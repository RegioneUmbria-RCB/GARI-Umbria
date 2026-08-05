using System.Security.Claims;
using System.Text;
using System.IO.Compression;
using AgronicaCoreDTOStd.Identity;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.SuperServer.BIZ.Autenticazione;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AgronicaNetCore.Base.Utility;


namespace AgronicaNetCore.Authentication.BIZ.Services.Authentication
{
    public class IdentityService : Serilog_Base,IIdentityService
    {
        private readonly IConfiguration _config;
        public IdentityService(IServiceProvider provider, IConfiguration config) : base(provider) 
        {
            _config = config;
        }

        public async Task<AuthenticationCheckResult> IsAuthorizedAsync(ClaimsIdentity? identity, IHeaderDictionary headers)
        {
            var ret = new AuthenticationCheckResult();
            try
            {
                if (identity != null)
                {
                    if (identity.FindFirst("exp") != null && DateTime.Now >= DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(identity.FindFirst("exp")!.Value)).LocalDateTime)
                    {
                        ret.status = false;
                    }
                    else
                    {
                        var tokenId = identity.FindFirst("tokenID");
                        if (tokenId != null && identity.FindFirst("objParams") != null)
                        {
                            await unzipObjTokenAsync(ret, identity.FindFirst("objParams")!.Value.ToString(), tokenId.Value);
                            if (ret.claims == null)
                                throw new Exception(String.Format("Claims per token {0} non recuperati.", identity.FindFirst("tokenID")!.ToString()));

                            if (ret.objP.objP_super_server != null &&
                                ret.objP.objP_server != null &&
                                ret.objP.objP_utenti != null &&
                                ret.claims.coreWSBaseURL != null)
                                ret.status = true;
                        }
                        else
                        {
                            ret.objP.objP_super_server = identity.FindFirst("objP_super_server")!.Value;
                            ret.objP.objP_server = identity.FindFirst("objP_server")!.Value;
                            ret.objP.objP_utenti = identity.FindFirst("objP_utenti")!.Value;
                            ret.claims.coreWSBaseURL = identity.FindFirst("coreWSBaseURL")!.Value;
                            ret.claims.username = identity.FindFirst("CodiceFiscale")!.Value;
                            ret.claims.user = identity.FindFirst("UserName")!.Value;
                            ret.claims.pivaSuperUser = identity.FindFirst("PivaSuperUser")!.Value;
                            ret.status = true;
                        }
                    }

                    ret.claims.bearerToken = headers["Authorization"];
                }
                else
                {
                    // ricava dall'header della chiamata
                    ret.objP.objP_super_server = headers["objP_super_server"];
                    ret.objP.objP_server = headers["objP_server"];
                    ret.objP.objP_utenti = headers["objP_utenti"];
                    ret.claims.coreWSBaseURL = headers["coreWSBaseURL"];
                    ret.claims.username = headers["CodiceFiscale"];

                    ret.status = !string.IsNullOrEmpty(ret.claims.username);
                }

                ret.claims.userAgent = headers["User-Agent"];
                ret.claims.host = headers["Host"];
            }catch(Exception ex)
            {
                LogError("", null, ex);
            }
            return ret;
        }

        private async Task unzipObjTokenAsync(AuthenticationCheckResult result, string objParams_str, string tokenId)
        {
            try
            {
                int numberChars = objParams_str.Length;
                byte[] byteArray = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2) { byteArray[i / 2] = Convert.ToByte(objParams_str.Substring(i, 2), 16); }
                byte[] byteOut = unzip(byteArray);
                UnicodeEncoding enc = new UnicodeEncoding();

                JObject objParams = JObject.Parse(enc.GetString(byteOut));

                result.claims.coreWSBaseURL = objParams["coreWSBaseURL"]!.ToString();
                result.claims.username = objParams["codiceFiscale"]!.ToString();
                result.claims.user = objParams["userName"]!.ToString();
                result.claims.pivaSuperUser = objParams["pivaSuperUser"]!.ToString();

                var connectionString = _config.GetValue<string>("ConnectionString");
                // decodifica la stringa di connessione se criptata
                if (_config.GetValue<bool>("ConnectionStringEncoded")) {
                    connectionString = Security.DecryptString(connectionString, _config.GetValue<string>("cr2"));
                }

                //lettura obj params usando objP_super_server
                var autenticazioneService = _serviceProvider.GetRequiredService<IAutenticazioneService>();
                var objPSuperServer = new AgronicaCoreParametriSuperServer()
                {
                    StringaConnessione = connectionString,
                    Lingua_Cod = 1,
                    LogDirectory = "C:\\GIASLAN",
                    LogFileName = "GiasOnline_log.txt",
                };

                var dt = await autenticazioneService.LeggiObjParametriAsync(tokenId, objPSuperServer);
                result.objP.objP_server = (string)dt.Rows[0]["objP_Server"];
                result.objP.objP_utenti = (string)dt.Rows[0]["objP_Utenti"];
                result.objP.objP_super_server = (string)dt.Rows[0]["objP_SuperServer"];
                if (dt.Rows[0]["IdDB"] is not null)
                {
                    result.IdDbServer = (int)dt.Rows[0]["IdDB"];
                }
            }
            catch (Exception ex)
            {
                LogError("", null, ex);
            }
        }

        private byte[] unzip(byte[] byteCompressed)
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
    }
}
