using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.OperazioniZoo.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.OperazioniZoo.DAL.DataLayer
{
    public class BaseDALOperazioniZoo : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALOperazioniZoo(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
