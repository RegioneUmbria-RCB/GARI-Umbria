using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.UtilityDB.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.UtilityDB.DAL.DataLayer
{
    public class BaseDALUtilityDB : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALUtilityDB(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
