using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer
{
    public class BaseDALAnagrafe : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALAnagrafe(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
