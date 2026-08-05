using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Gis.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Gis.DAL.DataLayer
{
    public class BaseDALGis : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALGis(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
