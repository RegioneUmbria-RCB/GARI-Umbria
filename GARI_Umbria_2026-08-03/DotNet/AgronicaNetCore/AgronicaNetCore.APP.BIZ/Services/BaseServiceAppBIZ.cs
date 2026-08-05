using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services
{
    public class BaseServiceAppBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseServiceAppBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider)
        {
            _localizer = localizer;
        }
    }
}
