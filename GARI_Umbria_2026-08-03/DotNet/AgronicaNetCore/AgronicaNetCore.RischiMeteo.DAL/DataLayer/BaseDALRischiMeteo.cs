using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.RischiMeteo.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.RischiMeteo.DAL.DataLayer
{
    public class BaseDALRischiMeteo : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseDALRischiMeteo(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)
            : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
