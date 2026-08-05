namespace AgronicaNetCore.RischiMeteo.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando un'azienda presente nel perimetro di calcolo
    /// non viene trovata nella tabella <c>Aziende</c> del database GIAS (inconsistenza DB).
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Eccezioni.
    /// </summary>
    public class DataNotFoundException : Exception
    {
        /// <summary>Partita IVA dell'azienda non trovata.</summary>
        public string Piva { get; }

        public DataNotFoundException(string piva)
            : base($"Azienda con PIVA '{piva}' non trovata nella tabella Aziende (inconsistenza DB).")
        {
            Piva = piva;
        }

        public DataNotFoundException(string piva, Exception innerException)
            : base($"Azienda con PIVA '{piva}' non trovata nella tabella Aziende (inconsistenza DB).", innerException)
        {
            Piva = piva;
        }
    }
}
