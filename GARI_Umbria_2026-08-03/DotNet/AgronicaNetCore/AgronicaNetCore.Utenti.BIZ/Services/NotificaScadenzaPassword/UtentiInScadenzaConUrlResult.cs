namespace AgronicaNetCore.Utenti.BIZ.Services.NotificaScadenzaPassword
{
    /// <summary>
    /// Risultato di <see cref="IUtentiNotificaScadenzaPasswordService.GetUtentiInScadenzaConUrlAsync"/>:
    /// lista utenti in scadenza password e URL precomposto per il cambio password.
    /// </summary>
    public class UtentiInScadenzaConUrlResult
    {
        /// <summary>Utenti la cui password scadrà entro il periodo di preavviso configurato.</summary>
        public List<UtenteInScadenzaPasswordDto> Utenti { get; set; } = new();

        /// <summary>
        /// URL della pagina di login per cambio password (index.aspx?pivasuperuser=...).
        /// Null se <c>LinkAgronicaAgenda2010</c> non è configurato in <c>Configurazione_Siti</c>.
        /// </summary>
        public string? UrlCambioPassword { get; set; }
    }
}
