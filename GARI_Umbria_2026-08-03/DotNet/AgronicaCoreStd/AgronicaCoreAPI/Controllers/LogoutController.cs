using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreUtilityStd;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaDataProvider6.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using AgronicaCoreAPI.Resources;

namespace AgronicaCoreAPI.Controllers
{
    public class LogoutController:  BaseController
    {
        private List<string> cookiesToDelete = new List<string> { "auth_cookie", "refreshToken" };

        public LogoutController(IServiceProvider provider, IConfiguration config, IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer) : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult Logout([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<string>();

            try
            {

                var request = getRequest(cookiesToDelete);
                result = new CoreWSController(hc, coreWSBaseURL, bearerToken, Request).Logout(request);
                if (!result.RispostaOK) return StatusCode(StatusCodes.Status500InternalServerError, result);


                CookieOptions option = new CookieOptions();
                option.SameSite = SameSiteMode.None;
                option.Expires = DateTime.Now.AddDays(-1);
                option.Secure = true;
                cookiesToDelete.ForEach(c =>
                   {
                       if (HttpContext.Request.Cookies.ContainsKey(c))
                           Response.Cookies.Delete(c, option);
                           
                   });

                result.RispostaOK = true;
                result.RispostaStringa = "OK";
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
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
        [Route("LinkHomePageLogin")]
        [ProducesResponseType(typeof(RispostaStandard<string>), StatusCodes.Status200OK)]
        public ObjectResult LinkHomePageLogin([FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization)) return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<string>();

            try
            {
                string linkLogin = string.Empty;

                HttpContext.Request.Cookies.TryGetValue("LinkHomePageGlobale", out linkLogin);

                result.RispostaOK = true;
                result.RispostaStringa = linkLogin;
            }
            catch (CoreWSNotAuthenticatedException coreWsNotAuthenticatedException)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(coreWsNotAuthenticatedException);
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception ex)
            {
                result.RispostaOK = false;
                result.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return StatusCode(StatusCodes.Status200OK, result);

        }

        private void RemoveCookie(string key)
        {

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(-1);
            option.Secure = true;
            option.IsEssential = true;
            option.SameSite = SameSiteMode.None;
            Response.Cookies.Append(key, string.Empty, option);
            Response.Cookies.Delete(key);
        }

    }
}
