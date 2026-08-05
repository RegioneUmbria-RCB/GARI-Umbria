using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.DSSNutrizione.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer
{
    /// <summary>
    /// Classe base per i DAL del modulo DSS Nutrizione.
    /// Fornisce accesso al data provider e alla localizzazione.
    /// </summary>
    public class BaseDSSNutrizioneDAL : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseDSSNutrizioneDAL(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            bool securityBypass = false)
            : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
