using AgronicaCoreAPI.Resources;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreUtilityStd;
using AgronicaCoreVarieBizSTD;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.Utenti.BIZ.Services.NotificaScadenzaPassword;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Controllers
{
    public class PasswordExpirationNotificheController : BaseController
    {
        private readonly IUtentiNotificaScadenzaPasswordService _utentiNotificaScadenzaPasswordService;

        public PasswordExpirationNotificheController(
            IServiceProvider provider,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            IOptions<SecuritySettings> securitySettings,
            IStringLocalizer<Messages> localizer,
            IUtentiNotificaScadenzaPasswordService utentiNotificaScadenzaPasswordService)
            : base(provider, config, httpClientFactory, securitySettings, localizer)
        {
            _utentiNotificaScadenzaPasswordService = utentiNotificaScadenzaPasswordService;
        }

        /// <summary>
        /// Recupera gli utenti in scadenza password, compone l'URL di cambio password e invia
        /// le email di notifica. Tutta la logica è delegata al BIZ.
        /// Usato dal job Quartz JobNotificaScadenzaPassword.
        /// </summary>
        [HttpPost]
        [Route("InviaNotificheScadenzaPassword")]
        public async Task<ObjectResult> InviaNotificheScadenzaPassword(
            [FromHeader] string Authorization)
        {
            if (!isAuthorized(Authorization))
                return StatusCode(StatusCodes.Status401Unauthorized, null);

            var result = new RispostaStandard<NotificheScadenzaPasswordResult>();

            try
            {
                var objParametriServer = UtilityAgronica.ConvertStringToObjParametriServer(objP_server, _securitySettings);
                var objParametriUtenti = UtilityAgronica.ConvertStringToObjParametriUtenti(objP_utenti, _securitySettings);

                NotificheScadenzaPasswordResult bizResult = await _utentiNotificaScadenzaPasswordService.InviaNotificheScadenzaPasswordAsync(
                    pivaSuperUser,
                    objParametriServer,
                    objParametriUtenti);

                result.RispostaOK = true;
                result.RispostaStringa = bizResult;
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
