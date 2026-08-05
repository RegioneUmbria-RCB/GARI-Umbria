using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.SostenibilitaCO2.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer
{
    public class BaseDALSostenibilitaCO2 : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALSostenibilitaCO2(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
