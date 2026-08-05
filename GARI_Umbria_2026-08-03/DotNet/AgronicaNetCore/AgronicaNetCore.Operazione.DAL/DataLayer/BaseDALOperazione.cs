using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Operazione.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Operazione.DAL.DataLayer
{
    public class BaseDALOperazione : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALOperazione(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
