using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Note.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Note.BIZ.Services
{
    public class BaseServiceNoteBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseServiceNoteBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
