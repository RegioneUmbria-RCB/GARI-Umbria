using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Services.Culture;
using AgronicaNetCore.Base.Services.Email;
using AgronicaNetCore.Utenti.BIZ.Resources;
using AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationConfig;
using AgronicaNetCore.Utenti.DAL.DataLayer.Utenti;
using InData.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using DataProviderMessages = AgronicaDataProvider6.Resources.Messages;

namespace AgronicaNetCore.Utenti.BIZ.Services.NotificaScadenzaPassword
{
    /// <summary>
    /// Servizio BIZ che recupera gli utenti la cui password scadrà entro un numero di giorni configurabile.
    /// Legge il parametro PASSWORD_EXPIRATION_DAYS da Configurazione_Siti tramite IPasswordExpirationConfigService
    /// e poi delega la query al DAL.
    /// </summary>
    public class UtentiNotificaScadenzaPasswordService : BaseServiceUtentiBIZ, IUtentiNotificaScadenzaPasswordService
    {
        private readonly IPasswordExpirationConfigService _passwordExpirationConfigService;
        private readonly IUtenti _utentiDal;
        private readonly ISecurityLayerDAL _securityLayerDAL;
        private readonly IEmailService _emailService;
        private readonly ICultureService _cultureService;
        private readonly IStringLocalizer<DataProviderMessages> _mailLocalizer;

        public UtentiNotificaScadenzaPasswordService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _passwordExpirationConfigService = _serviceProvider.GetRequiredService<IPasswordExpirationConfigService>();
            _utentiDal = _serviceProvider.GetRequiredService<IUtenti>();
            _securityLayerDAL = _serviceProvider.GetRequiredService<ISecurityLayerDAL>();
            _emailService = _serviceProvider.GetRequiredService<IEmailService>();
            _cultureService = _serviceProvider.GetRequiredService<ICultureService>();
            _mailLocalizer = _serviceProvider.GetRequiredService<IStringLocalizer<DataProviderMessages>>();
        }

        /// <inheritdoc/>
        public async Task<List<UtenteInScadenzaPasswordDto>> GetUtentiInScadenzaAsync(
            string pivaSuperUser,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            ArgumentNullException.ThrowIfNull(pivaSuperUser);
            ArgumentNullException.ThrowIfNull(objParametriServer);
            ArgumentNullException.ThrowIfNull(objParametriUtenti);

            int? giorniScadenza = await _passwordExpirationConfigService
                .LeggiParametroScadenzaPasswordAsync(objParametriServer);

            if (giorniScadenza is null)
            {
                LogInformation(
                    "PASSWORD_EXPIRATION_DAYS non configurato: feature disabilitata. Nessun utente da notificare.",
                    objParametriServer);
                return new List<UtenteInScadenzaPasswordDto>();
            }

            int? giorniPreavviso = await _passwordExpirationConfigService
                .LeggiParametroGiorniPreavvisoAsync(objParametriServer);

            if (giorniPreavviso is null)
            {
                LogInformation(
                    "GIORNI_PREAVVISO_SCADENZA_PASSWORD non configurato: notifiche di preavviso disabilitate. Nessun utente da notificare.",
                    objParametriServer);
                return new List<UtenteInScadenzaPasswordDto>();
            }

            DataTable dt = await _utentiDal.LeggiUtentiInScadenzaPasswordAsync(
                giorniScadenza.Value, giorniPreavviso.Value, objParametriUtenti);

            var lista = new List<UtenteInScadenzaPasswordDto>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new UtenteInScadenzaPasswordDto
                {
                    UserName        = row.Field<string>("UserName") ?? string.Empty,
                    Nome            = row.Field<string>("Nome"),
                    Cognome         = row.Field<string>("Cognome"),
                    Email           = row.Field<string>("Email") ?? string.Empty,
                    GiorniRimanenti = row.Field<int>("GiorniRimanenti"),
                    DataScadenza    = row.Field<DateTime>("DataScadenza"),
                    LinguaCod       = row["Lingua_cod"] == DBNull.Value ? 1 : Convert.ToInt32(row["Lingua_cod"])
                });
            }

            return lista;
        }

        /// <inheritdoc/>
        public async Task<UtentiInScadenzaConUrlResult> GetUtentiInScadenzaConUrlAsync(
            string pivaSuperUser,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            ArgumentNullException.ThrowIfNull(pivaSuperUser);

            List<UtenteInScadenzaPasswordDto> utenti = await GetUtentiInScadenzaAsync(
                pivaSuperUser, objParametriServer, objParametriUtenti);

            string? url = await ComposizioneUrlCambioPasswordAsync(pivaSuperUser, objParametriServer);

            return new UtentiInScadenzaConUrlResult
            {
                Utenti = utenti,
                UrlCambioPassword = url
            };
        }

        /// <summary>
        /// Compone l'URL della pagina di login per il cambio password leggendo
        /// <c>LinkAgronicaAgenda2010</c> (e facoltativamente <c>LanToWebSiteBasePath</c>)
        /// da <c>Configurazione_Siti</c> sul DB server.
        /// Restituisce null se la chiave non è configurata.
        /// </summary>
        private async Task<string?> ComposizioneUrlCambioPasswordAsync(
            string pivaSuperUser,
            AgronicaCoreParametriServer objParametriServer)
        {
            DataTable dtLink = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(
                "LinkAgronicaAgenda2010", objParametriServer);

            if (dtLink.Rows.Count == 0) return null;

            string linkAgenda = dtLink.Rows[0]["Valore"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(linkAgenda)) return null;

            string baseDomain;
            string pathSenzaFile;

            if (Uri.TryCreate(linkAgenda, UriKind.Absolute, out Uri? linkUri))
            {
                baseDomain = linkUri.GetLeftPart(UriPartial.Authority);
                pathSenzaFile = linkUri.AbsolutePath.Substring(0, linkUri.AbsolutePath.LastIndexOf('/') + 1);
            }
            else
            {
                DataTable dtBase = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(
                    "LanToWebSiteBasePath", objParametriServer);

                if (dtBase.Rows.Count == 0) return null;

                string lanBasePath = dtBase.Rows[0]["Valore"]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(lanBasePath)) return null;

                baseDomain = lanBasePath.TrimEnd('/');
                string path = linkAgenda.StartsWith("/") ? linkAgenda : "/" + linkAgenda;
                pathSenzaFile = path.Substring(0, path.LastIndexOf('/') + 1);
            }

            return baseDomain + pathSenzaFile + "index.aspx?pivasuperuser=" + Uri.EscapeDataString(pivaSuperUser);
        }

        /// <inheritdoc/>
        public async Task<NotificheScadenzaPasswordResult> InviaNotificheScadenzaPasswordAsync(
            string pivaSuperUser,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            ArgumentNullException.ThrowIfNull(pivaSuperUser);

            var result = new NotificheScadenzaPasswordResult();

            UtentiInScadenzaConUrlResult datiNotifiche = await GetUtentiInScadenzaConUrlAsync(
                pivaSuperUser, objParametriServer, objParametriUtenti);

            if (datiNotifiche.Utenti.Count == 0)
            {
                result.LogMessages.Add("Nessun utente con password in scadenza entro il periodo configurato.");
                return result;
            }

            result.LogMessages.Add($"Trovati {datiNotifiche.Utenti.Count} utenti con password in scadenza. Avvio invio email.");

            DataTable dtMailFrom = await _securityLayerDAL.LeggiConfigurazioneSitiAsync("MailFrom_smtp", objParametriServer);
            string mailFrom = dtMailFrom.Rows.Count > 0 ? dtMailFrom.Rows[0]["Valore"]?.ToString() ?? string.Empty : string.Empty;

            foreach (UtenteInScadenzaPasswordDto utente in datiNotifiche.Utenti)
            {
                if (!IsEmailValida(utente.Email))
                {
                    result.LogMessages.Add($"Utente {utente.UserName}: email assente o non valida ('{utente.Email}'), notifica saltata.");
                    continue;
                }

                try
                {
                    string dataScadenzaFormatted = utente.DataScadenza.ToString("dd/MM/yyyy");

                    var previousUICulture = CultureInfo.CurrentUICulture;
                    try
                    {
                        _cultureService.SetUICulture(utente.LinguaCod);

                        string subjectTemplate = _mailLocalizer["PasswordExpirationMailSubject"].Value;
                        string bodyTemplate = _mailLocalizer["PasswordExpirationMailBody"].Value;

                        string subject = ReplaceMailPlaceholders(subjectTemplate, utente, dataScadenzaFormatted, datiNotifiche.UrlCambioPassword);
                        string body = ReplaceMailPlaceholders(bodyTemplate, utente, dataScadenzaFormatted, datiNotifiche.UrlCambioPassword);

                        var emailData = new EmailData
                        {
                            From = mailFrom,
                            To = new List<string> { utente.Email },
                            Subject = subject,
                            Text = body,
                            IsBodyHtml = false
                        };

                        bool inviata = await _emailService.SendEmailAsync(objParametriServer, emailData);
                        if (!inviata)
                            throw new Exception($"SendEmailAsync ha restituito false per l'utente {utente.UserName} ({utente.Email}).");
                    }
                    finally
                    {
                        CultureInfo.CurrentUICulture = previousUICulture;
                    }

                    result.LogMessages.Add($"Email inviata a {utente.Email} per l'utente {utente.UserName} ({utente.GiorniRimanenti} giorni rimanenti).");
                    result.EmailInviate++;
                }
                catch (Exception ex)
                {
                    result.LogMessages.Add($"Errore invio email utente {utente.UserName}: {ex.Message}");
                    result.EmailFallite++;
                }
            }

            result.LogMessages.Add($"Notifiche completate: {result.EmailInviate} inviate, {result.EmailFallite} fallite.");
            return result;
        }

        private static bool IsEmailValida(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try { _ = new MailAddress(email); return true; }
            catch { return false; }
        }

        private static string ReplaceMailPlaceholders(
            string template,
            UtenteInScadenzaPasswordDto utente,
            string dataScadenzaFormatted,
            string? urlCambioPassword)
        {
            return template
                .Replace("{Nome}", utente.Nome ?? string.Empty)
                .Replace("{Cognome}", utente.Cognome ?? string.Empty)
                .Replace("{GiorniRimanenti}", utente.GiorniRimanenti.ToString(CultureInfo.InvariantCulture))
                .Replace("{DataScadenza}", dataScadenzaFormatted)
                .Replace("{UrlCambioPassword}", urlCambioPassword ?? string.Empty);
        }
    }
}
