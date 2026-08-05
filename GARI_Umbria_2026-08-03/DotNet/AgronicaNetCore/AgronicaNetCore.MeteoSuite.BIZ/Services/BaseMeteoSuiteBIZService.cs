using AgronicaNetCore.Base.Base;
using AgronicaNetCore.MeteoSuite.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services
{
    /// <summary>
    /// Classe base per i servizi BIZ del modulo Meteo Suite.
    /// </summary>
    public class BaseMeteoSuiteBIZService : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseMeteoSuiteBIZService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider)
        {
            _localizer = localizer;
        }
    }
}
