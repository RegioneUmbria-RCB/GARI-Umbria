namespace AgronicaNetCore.Utenti.DAL.Exceptions
{
    /// <summary>
    /// Eccezione sollevata dal layer DAL Utenti in caso di errori di connessione o errori SQL.
    /// Riferimento DS: DS01-BL §Eccezioni — UtentiDALException.
    /// </summary>
    public class UtentiDALException : Exception
    {
        public UtentiDALException(string message) : base(message) { }

        public UtentiDALException(string message, Exception innerException) : base(message, innerException) { }
    }
}
