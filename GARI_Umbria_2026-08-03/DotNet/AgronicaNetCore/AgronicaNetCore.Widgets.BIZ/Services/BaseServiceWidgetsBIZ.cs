using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Widgets.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Widgets.BIZ.Services
{
    public class BaseServiceWidgetsBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceWidgetsBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
