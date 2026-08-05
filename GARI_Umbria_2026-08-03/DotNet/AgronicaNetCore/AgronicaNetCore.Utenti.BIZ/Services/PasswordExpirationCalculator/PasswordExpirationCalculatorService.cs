using AgronicaNetCore.Utenti.BIZ.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationCalculator
{
    /// <summary>
    /// Servizio BIZ che calcola in-memory lo stato di scadenza della password.
    /// Non accede al database — la logica è puramente computazionale.
    /// Riferimento DS: DS03-BL — CalcoloStatoScadenzaPassword, §Descrizione, §Persistenze Coinvolte.
    /// </summary>
    public class PasswordExpirationCalculatorService : BaseServiceUtentiBIZ, IPasswordExpirationCalculatorService
    {
        public PasswordExpirationCalculatorService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
        }

        /// <summary>
        /// Calcola lo stato di scadenza della password in modo sincrono e bloccante.
        /// Riferimento DS: DS03-BL §Regole di Business, §Output, §Eccezioni.
        /// </summary>
        /// <param name="dataUltimaModificaPassword">
        /// Timestamp dell'ultima modifica della password. Se null, la password è considerata SCADUTA
        /// (security-first — DS03-BL §Regole di Business, prima regola).
        /// </param>
        /// <param name="giorniValidita">Numero di giorni di validità della password (deve essere positivo).</param>
        /// <param name="dataRiferimento">Data di riferimento per il calcolo (tipicamente DateTime.Now).</param>
        /// <returns><see cref="StatoScadenzaPassword"/> con lo stato calcolato e le metriche derivate.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Sollevata se <paramref name="giorniValidita"/> è zero o negativo.</exception>
        public StatoScadenzaPassword CalcolaStatoScadenza(
            DateTime? dataUltimaModificaPassword,
            int giorniValidita,
            DateTime dataRiferimento)
        {
            if (giorniValidita <= 0)
                throw new ArgumentOutOfRangeException(nameof(giorniValidita), "Il parametro giorniValidita deve essere un intero positivo.");

            // Riferimento DS: DS03-BL §Regole di Business — data_ultima_modifica null: security-first, password SCADUTA.
            if (dataUltimaModificaPassword is null)
            {
                LogWarning(
                    "DataUltimaModificaPassword è null: la password è considerata SCADUTA per criterio security-first (DS03-BL).");
                return new StatoScadenzaPassword(
                    PasswordScaduta: true,
                    GiorniDallaModifica: giorniValidita,
                    GiorniRimanenti: 0,
                    DataScadenzaPrevista: null);
            }

            // Riferimento DS: DS03-BL §Regole di Business — calcolo al granulo di giorni (floor).
            int giorniDallaModifica = (int)Math.Floor((dataRiferimento - dataUltimaModificaPassword.Value).TotalDays);

            // Riferimento DS: DS03-BL §Regole di Business — giorni_dalla_modifica >= giorni_validita → SCADUTA.
            bool passwordScaduta = giorniDallaModifica >= giorniValidita;

            int giorniRimanenti = giorniValidita - giorniDallaModifica;
            DateTime dataScadenzaPrevista = dataUltimaModificaPassword.Value.AddDays(giorniValidita);

            return new StatoScadenzaPassword(
                PasswordScaduta: passwordScaduta,
                GiorniDallaModifica: giorniDallaModifica,
                GiorniRimanenti: giorniRimanenti,
                DataScadenzaPrevista: dataScadenzaPrevista);
        }
    }
}
