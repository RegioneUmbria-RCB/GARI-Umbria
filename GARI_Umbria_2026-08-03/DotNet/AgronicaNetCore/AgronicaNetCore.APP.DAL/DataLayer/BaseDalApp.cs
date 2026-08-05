using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.APP.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.DAL.DataLayer
{
    public class BaseDalApp : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseDalApp(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, false)
        {
            _localizer = localizer;
        }
    }
}
