using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.SostenibilitaH2O.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer
{
    /// <summary>
    /// Base DAL class for H2O sustainability lookup access.
    /// See DS09-API and Database schema sections related to lookup tables.
    /// </summary>
    public class BaseDALSostenibilitaH2O : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseDALSostenibilitaH2O(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false)
            : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
