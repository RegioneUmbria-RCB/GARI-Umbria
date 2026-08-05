using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Widgets.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Widgets.DAL.DataLayer
{
    public class BaseDALWidgets : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALWidgets(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
