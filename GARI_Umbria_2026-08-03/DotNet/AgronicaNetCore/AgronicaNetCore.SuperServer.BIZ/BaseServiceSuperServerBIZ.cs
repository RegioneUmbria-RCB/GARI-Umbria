using AgronicaNetCore.Base.Base;
using AgronicaNetCore.SuperServer.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SuperServer.BIZ
{
    public class BaseServiceSuperServerBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceSuperServerBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
