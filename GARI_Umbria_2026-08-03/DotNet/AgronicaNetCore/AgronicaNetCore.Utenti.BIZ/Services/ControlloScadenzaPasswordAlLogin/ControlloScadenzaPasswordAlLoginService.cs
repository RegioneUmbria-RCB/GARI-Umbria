using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationCalculator;
using AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationConfig;
using AgronicaNetCore.Utenti.BIZ.Services.UtentiPassword;
using AgronicaNetCore.Utenti.DAL.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Utenti.BIZ.Services.ControlloScadenzaPasswordAlLogin
{
    /// <summary>
    /// Servizio BIZ che orchestra il controllo di scadenza password durante il flusso di login.
    /// È il punto di integrazione principale nel pipeline di autenticazione: invoca DS01-BL,
    /// Riferimento DS: DS10-BL — ControlloScadenzaPasswordAlLogin §Descrizione, §Regole di Business.
    /// </summary>
    public class ControlloScadenzaPasswordAlLoginService : BaseServiceUtentiBIZ, IControlloScadenzaPasswordAlLoginService
    {
        private readonly IUtentiPasswordService _utentiPasswordService;
        private readonly IPasswordExpirationConfigService _passwordExpirationConfigService;
        private readonly IPasswordExpirationCalculatorService _passwordExpirationCalculatorService;

        public ControlloScadenzaPasswordAlLoginService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _utentiPasswordService = _serviceProvider.GetRequiredService<IUtentiPasswordService>();
            _passwordExpirationConfigService = _serviceProvider.GetRequiredService<IPasswordExpirationConfigService>();
            _passwordExpirationCalculatorService = _serviceProvider.GetRequiredService<IPasswordExpirationCalculatorService>();
        }

        /// Riferimento DS: DS10-BL §Regole di Business §1-4, §Eccezioni.
        public async Task<RisultatoControlloScadenzaPassword> ControllaScadenzaAsync(
            string username,
            string pivaSuperUser,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer)
        {
            // Fail-fast: validazione parametri obbligatori — DS10-BL §Input.
            if (username is null) throw new ArgumentNullException(nameof(username));
            if (pivaSuperUser is null) throw new ArgumentNullException(nameof(pivaSuperUser));
            if (objParametriUtenti is null) throw new ArgumentNullException(nameof(objParametriUtenti));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (username.Length == 0) throw new ArgumentException("Lo username non può essere una stringa vuota.", nameof(username));
            if (pivaSuperUser.Length == 0) throw new ArgumentException("La partita IVA del super-user non può essere una stringa vuota.", nameof(pivaSuperUser));

            // DS10-BL §Regola 2 — Retrieval Parametro Configurazione (DS02-BL).
            // Eseguito PRIMA della lettura della colonna DataUltimaModificaPassword:
            // se il parametro è assente la feature è disabilitata e la colonna potrebbe non esistere nel DB.
            // null = parametro assente = feature non abilitata → login ordinario senza verifica.
            int? giorniValidita =
                await _passwordExpirationConfigService.LeggiParametroScadenzaPasswordAsync(objParametriServer);

            if (giorniValidita is null)
            {
                return new RisultatoControlloScadenzaPassword(
                    PasswordScaduta: false,
                    AzioneRichiesta: AzioneControlloScadenza.ProcediLoginOrdinario);
            }

            // DS10-BL §Regola 1 — Retrieval Data Modifica (DS01-BL).
            // Eseguito solo se la feature è abilitata (giorniValidita != null).
            var (dataUltimaModificaPassword, utenteTrovato) =
                await _utentiPasswordService.LeggiDataUltimaModificaPasswordAsync(username, objParametriUtenti);

            // DS10-BL §Regola 1 — Se utente non trovato: eccezione (non deve verificarsi se autenticazione riuscita).
            if (!utenteTrovato)
                throw new UtentiNotFoundException(username);

            StatoScadenzaPassword stato = _passwordExpirationCalculatorService.CalcolaStatoScadenza(
                dataUltimaModificaPassword,
                giorniValidita.Value,
                DateTime.UtcNow);

            if (!stato.PasswordScaduta)
            {
                return new RisultatoControlloScadenzaPassword(
                    PasswordScaduta: false,
                    AzioneRichiesta: AzioneControlloScadenza.ProcediLoginOrdinario);
            }

            // DS10-BL §Regola 5 — Se scaduta: log di tipo INFO con dettagli giorni.
            LogInformation(
                SanitizeLogMessage($"ControlloScadenzaPasswordAlLogin: password scaduta per l'utente '{username}' " +
                $"— giorni dalla modifica: {stato.GiorniDallaModifica}, " +
                $"giorni rimanenti: {stato.GiorniRimanenti}, " +
                $"data scadenza prevista: {stato.DataScadenzaPrevista:yyyy-MM-dd}. " +
                $"Azione: {AzioneControlloScadenza.RedirigWizard}."),
                objParametriServer);

            return new RisultatoControlloScadenzaPassword(
                PasswordScaduta: true,
                AzioneRichiesta: AzioneControlloScadenza.RedirigWizard);
        }
    }
}
