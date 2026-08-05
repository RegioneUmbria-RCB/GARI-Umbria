using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Utenti.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Utenti.DAL.DataLayer
{
    public class BaseDALUtenti : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALUtenti(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
