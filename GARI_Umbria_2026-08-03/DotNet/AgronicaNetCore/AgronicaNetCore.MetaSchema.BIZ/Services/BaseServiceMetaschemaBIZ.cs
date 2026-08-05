using AgronicaNetCore.Base.Base;
using AgronicaNetCore.MetaSchema.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.BIZ.Services
{
    public class BaseServiceMetaschemaBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceMetaschemaBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
