using AgronicaNetCore.Base.Base;
using AgronicaNetCore.DSSNutrizione.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services
{
    /// <summary>
    /// Classe base per i servizi BIZ del modulo DSS Nutrizione.
    /// </summary>
    public class BaseDSSNutrizioneBIZService : BaseService
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseDSSNutrizioneBIZService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider)
        {
            _localizer = localizer;
        }
    }
}
