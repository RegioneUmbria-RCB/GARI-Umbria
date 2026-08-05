using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer
{
    public class BaseDALMetaschema : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALMetaschema(IServiceProvider provider, IStringLocalizer<Messages> localizer, bool securityBypass = false) : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
