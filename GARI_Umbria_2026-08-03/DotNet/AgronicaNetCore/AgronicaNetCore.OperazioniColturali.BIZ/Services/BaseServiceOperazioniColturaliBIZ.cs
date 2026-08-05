using AgronicaNetCore.Base.Base;
using AgronicaNetCore.OperazioniColturali.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.OperazioniColturali.BIZ.Services
{
    public class BaseServiceOperazioniColturaliBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceOperazioniColturaliBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
