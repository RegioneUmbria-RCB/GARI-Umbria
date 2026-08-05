namespace AgronicaNetCore.Utenti.DAL.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando un utente cercato non esiste nella tabella <c>Utenti</c>.
    /// Riferimento DS: DS04-BL §Eccezioni — UtentiNotFoundException.
    /// </summary>
    public class UtentiNotFoundException : Exception
    {
        public UtentiNotFoundException(string username)
            : base($"L'utente '{username}' non è stato trovato nella tabella Utenti.") { }

        public UtentiNotFoundException(string username, Exception innerException)
            : base($"L'utente '{username}' non è stato trovato nella tabella Utenti.", innerException) { }
    }
}
