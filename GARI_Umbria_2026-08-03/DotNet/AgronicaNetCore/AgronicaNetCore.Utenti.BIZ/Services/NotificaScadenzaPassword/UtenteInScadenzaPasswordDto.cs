namespace AgronicaNetCore.Utenti.BIZ.Services.NotificaScadenzaPassword
{
    /// <summary>
    /// Dati dell'utente la cui password è prossima alla scadenza, restituiti dal servizio di notifica.
    /// </summary>
    public class UtenteInScadenzaPasswordDto
    {
        public string UserName { get; set; } = string.Empty;
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
        public string Email { get; set; } = string.Empty;
        public int GiorniRimanenti { get; set; }
        public DateTime DataScadenza { get; set; }
        public int LinguaCod { get; set; } = 1; // 1 = IT (italiano)
    }
}
