using AgronicaNetCore.AuthDispatcher.BIZ.Resources;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.AuthDispatcher.BIZ
{
    public class BaseServiceAuthDispatcherBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseServiceAuthDispatcherBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}