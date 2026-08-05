using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Operazione.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Operazione.BIZ.Services
{
    public class BaseServiceOperazioneBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseServiceOperazioneBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            this._localizer = localizer;
        }
    }
}
