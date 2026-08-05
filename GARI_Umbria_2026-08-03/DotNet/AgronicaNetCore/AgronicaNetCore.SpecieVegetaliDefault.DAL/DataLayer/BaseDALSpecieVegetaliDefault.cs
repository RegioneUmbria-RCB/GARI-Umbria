using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.SpecieVegetaliDefault.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SpecieVegetaliDefault.DAL.DataLayer
{
    public class BaseDALSpecieVegetaliDefault : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALSpecieVegetaliDefault(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
