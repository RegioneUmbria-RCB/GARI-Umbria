using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Utenti.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Utenti.BIZ.Services
{
    public class BaseServiceUtentiBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceUtentiBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
