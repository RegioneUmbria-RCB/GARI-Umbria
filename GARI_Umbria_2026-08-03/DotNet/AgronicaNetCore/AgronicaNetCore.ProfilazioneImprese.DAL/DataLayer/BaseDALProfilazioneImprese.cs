using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.ProfilazioneImprese.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.ProfilazioneImprese.DAL.DataLayer
{
    public class BaseDALProfilazioneImprese : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BaseDALProfilazioneImprese(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider)
        {
            _localizer = localizer;
        }
    }
}
