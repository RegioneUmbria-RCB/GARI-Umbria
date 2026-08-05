using AgronicaNetCore.Base.Base;
using AgronicaNetCore.DefaultPianiColturali.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.DefaultPianiColturali.BIZ.Services
{
    public class BaseServiceDefaultPianiColturaliBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceDefaultPianiColturaliBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
