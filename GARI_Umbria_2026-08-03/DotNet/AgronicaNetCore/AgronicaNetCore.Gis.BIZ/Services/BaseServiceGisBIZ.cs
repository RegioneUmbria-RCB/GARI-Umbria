using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Gis.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Gis.BIZ.Services
{
    public class BaseServiceGisBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceGisBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
