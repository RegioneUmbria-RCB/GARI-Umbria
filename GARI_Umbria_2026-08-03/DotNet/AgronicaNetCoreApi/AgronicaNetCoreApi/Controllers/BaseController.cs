using AgronicaCoreDTOStd.Identity;
using AgronicaDataProvider6.Models;
using AgronicaNetCore.Authentication.BIZ.Services.Authentication;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Services.Culture;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCoreApi.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace AgronicaNetCoreApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IOptions<SecuritySettings> _securitySettings;
        protected IServiceProvider _provider;
        protected readonly IStringLocalizer<Messages> _localizer;
        protected readonly IIdentityService _identityService;
        protected readonly ICultureService? _cultureService;

        public BaseController(IServiceProvider provider, IOptions<SecuritySettings> securitySettings, IStringLocalizer<Messages> localizer)
        {
            _provider = provider;
            _securitySettings = securitySettings;
            _localizer = localizer;
            _identityService = _provider.GetRequiredService<IIdentityService>();
            _cultureService = _provider.GetRequiredService<ICultureService>();
        }

        protected AgronicaCoreParametriServer ExtractObjParametriServerAndSetCulture(AuthenticationCheckResult auth)
        {
            var objParametriServer = UtilityAgronica.ConvertStringToObjParametriServer(auth.objP.objP_server, _securitySettings);
            
            //necessario impostare la culture nel thread principale della request (e non nei vari task)
            _cultureService?.SetUICulture(objParametriServer.Lingua_Cod);
            return objParametriServer;
        }

        protected AgronicaCoreParametriUtenti ExtractObjParametriUtentiAndSetCulture(AuthenticationCheckResult auth)
        {
            var objParametriUtenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);

            //necessario impostare la culture nel thread principale della request (e non nei vari task)
            _cultureService?.SetUICulture(objParametriUtenti.Lingua_Cod);
            return objParametriUtenti;
        }

        protected AgronicaCoreParametriDouble ExtractObjParametriDoubleAndSetCulture(AuthenticationCheckResult auth)
        {
            var objParametriServer = ExtractObjParametriServerAndSetCulture(auth);
            var objParametriUtenti = UtilityAgronica.ConvertStringToObjParametriUtenti(auth.objP.objP_utenti, _securitySettings);
            return new AgronicaCoreParametriDouble(objParametriServer, objParametriUtenti);
        }

        protected AgronicaCoreParametriTriple ExtractObjParametriTripleAndSetCulture(AuthenticationCheckResult auth)
        {
            var objParametriDouble = ExtractObjParametriDoubleAndSetCulture(auth);
            var objParametriSuperServer = UtilityAgronica.ConvertStringToObjParametriSuperServer(auth.objP.objP_super_server, _securitySettings);
            return new AgronicaCoreParametriTriple(objParametriDouble.ObjParametriServer, objParametriDouble.ObjParametriUtenti, objParametriSuperServer);
        }
    }
}
