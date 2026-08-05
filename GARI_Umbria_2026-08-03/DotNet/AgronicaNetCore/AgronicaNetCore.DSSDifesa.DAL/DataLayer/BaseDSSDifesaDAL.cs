using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.DSSDifesa.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.DSSDifesa.DAL.DataLayer
{
    /// <summary>
    /// Classe base per i DAL del modulo DSS Difesa.
    /// Fornisce accesso al data provider e alla localizzazione.
    /// </summary>
    public class BaseDSSDifesaDAL : DAL_Base
    {
        protected readonly IStringLocalizer<Messages> _localizer;

        public BaseDSSDifesaDAL(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            bool securityBypass = false)
            : base(provider, securityBypass)
        {
            _localizer = localizer;
        }
    }
}
