using AgronicaNetCore.Base.Base;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services
{
    public class BaseServiceSostenibilitaCO2Biz : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceSostenibilitaCO2Biz(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
