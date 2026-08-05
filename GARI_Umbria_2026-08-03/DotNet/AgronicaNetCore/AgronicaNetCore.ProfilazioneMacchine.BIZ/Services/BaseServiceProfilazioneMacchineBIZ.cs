using AgronicaNetCore.Base.Base;
using AgronicaNetCore.ProfilazioneMacchine.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneMacchine.BIZ.Services
{
    public class BaseServiceProfilazioneMacchineBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceProfilazioneMacchineBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
