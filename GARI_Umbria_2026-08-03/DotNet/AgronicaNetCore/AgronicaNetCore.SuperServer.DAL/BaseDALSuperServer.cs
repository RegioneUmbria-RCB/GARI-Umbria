using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.SuperServer.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SuperServer.DAL
{
    public class BaseDALSuperServer : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALSuperServer(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
