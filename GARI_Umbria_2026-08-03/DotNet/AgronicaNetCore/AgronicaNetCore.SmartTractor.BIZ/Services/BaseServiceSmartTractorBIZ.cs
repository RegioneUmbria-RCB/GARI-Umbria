using AgronicaNetCore.Base.Base;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SmartTractor.BIZ.Services
{
    public class BaseServiceSmartTractorBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceSmartTractorBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
