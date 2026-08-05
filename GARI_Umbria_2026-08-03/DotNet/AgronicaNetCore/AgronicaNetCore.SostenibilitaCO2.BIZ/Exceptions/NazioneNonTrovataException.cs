namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione non bloccante usata per tracciare il fallback sulla nazione aziendale
    /// quando l'indirizzo di sede operativa non e' presente oppure non valorizza il campo stato.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Eccezioni.
    /// </summary>
    public class NazioneNonTrovataException : Exception
    {
        public NazioneNonTrovataException(string piva, string defaultNazione)
            : base($"Nazione non disponibile per PIVA '{piva}'. Usato default '{defaultNazione}'.")
        {
        }

        public NazioneNonTrovataException(string piva, string defaultNazione, Exception innerException)
            : base($"Nazione non disponibile per PIVA '{piva}'. Usato default '{defaultNazione}'.", innerException)
        {
        }
    }
}