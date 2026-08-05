namespace AgronicaNetCore.Base.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando si verifica un errore di connessione o un errore SQL
    /// durante la lettura della tabella <c>Configurazione_Siti</c>.
    /// Riferimento DS: DS02-BL §Eccezioni — ConfigurazioneDALException.
    /// </summary>
    public class ConfigurazioneDALException : Exception
    {
        public ConfigurazioneDALException(string message) : base(message) { }

        public ConfigurazioneDALException(string message, Exception innerException) : base(message, innerException) { }
    }
}
