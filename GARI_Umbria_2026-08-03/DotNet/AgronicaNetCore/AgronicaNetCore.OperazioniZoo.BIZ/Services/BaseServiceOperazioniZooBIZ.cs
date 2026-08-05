using AgronicaNetCore.Base.Base;
using AgronicaNetCore.OperazioniZoo.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services
{
    public class BaseServiceOperazioniZooBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceOperazioniZooBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
