using AgronicaNetCore.Base.Base;
using AgronicaNetCore.DSSDifesa.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services
{
    /// <summary>
    /// Classe base per i servizi BIZ del modulo DSS Difesa.
    /// </summary>
    public class BaseDSSDifesaBIZService : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseDSSDifesaBIZService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider)
        {
            _localizer = localizer;
        }
    }
}
