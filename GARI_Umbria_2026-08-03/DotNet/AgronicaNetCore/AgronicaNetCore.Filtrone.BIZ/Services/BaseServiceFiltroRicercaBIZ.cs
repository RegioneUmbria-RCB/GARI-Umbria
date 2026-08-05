using AgronicaNetCore.Base.Base;
using AgronicaNetCore.FiltroRicerca.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.FiltroRicerca.BIZ.Services
{
    public class BaseServiceFiltroRicercaBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceFiltroRicercaBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
