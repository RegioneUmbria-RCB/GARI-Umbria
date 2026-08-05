namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
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
            : base($"Impresa con PIVA '{piva}' non trovata.")
        {
            Piva = piva;
        }

        public DataNotFoundException(string piva, string campo)
            : base($"Per l'impresa con PIVA '{piva}' il campo '{campo}' non è valorizzato")
        {            
            Piva = piva;
        }
    }
}
