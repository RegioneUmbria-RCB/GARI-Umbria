using AgronicaNetCore.Base.Base;
using AgronicaNetCore.RischiMeteo.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services
{
    public class BaseServiceRischiMeteoBiz : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseServiceRischiMeteoBiz(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider)
        {
            _localizer = localizer;
        }
    }
}
