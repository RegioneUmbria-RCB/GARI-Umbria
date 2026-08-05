using AgronicaNetCore.Base.Base;
using AgronicaNetCore.ProfilazioneImprese.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneImprese.BIZ.Services
{
    public class BaseServiceProfilazioneImpreseBIZ : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseServiceProfilazioneImpreseBIZ(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
