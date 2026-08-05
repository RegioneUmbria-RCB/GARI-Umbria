using AgronicaNetCore.Base.Base;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services
{
    /// <summary>
    /// Base BIZ class for H2O sustainability services.
    /// </summary>
    public class BaseServiceSostenibilitaH2OBiz : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseServiceSostenibilitaH2OBiz(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider)
        {
            _localizer = localizer;
        }
    }
}
