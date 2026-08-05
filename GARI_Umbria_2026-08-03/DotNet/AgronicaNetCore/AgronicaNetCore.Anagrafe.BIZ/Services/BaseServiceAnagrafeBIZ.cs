using AgronicaNetCore.Anagrafe.BIZ.Resources;
using AgronicaNetCore.Base.Base;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.BIZ.Services
{
    public class BaseServiceAnagrafeBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceAnagrafeBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
