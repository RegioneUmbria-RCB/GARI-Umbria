using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Utility.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Utility.DAL.DataLayer
{
    public class BaseDALUtility : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALUtility(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
