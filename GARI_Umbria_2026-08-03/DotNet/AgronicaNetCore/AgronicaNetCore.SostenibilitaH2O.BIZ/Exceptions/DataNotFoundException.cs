namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando un dato atteso non viene trovato durante il calcolo
    /// del bilancio idrico (es. CUAA non trovato per una PIVA filiera).
    /// </summary>
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string message)
            : base(message)
        {
        }

        public DataNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
