using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.FiltroRicerca.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.FiltroRicerca.DAL.DataLayer
{
    public class BaseDALFiltroRicerca : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALFiltroRicerca(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
