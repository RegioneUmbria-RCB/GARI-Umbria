using AgronicaNetCore.Base.Base;
using AgronicaNetCore.UtilityDB.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.UtilityDB.BIZ.Services
{
    public class BaseServiceUtilityDBBiz : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceUtilityDBBiz(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
