using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.ProfilazioneMacchine.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneMacchine.DAL.DataLayer
{
    public class BaseDALProfilazioneMacchine : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALProfilazioneMacchine(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
