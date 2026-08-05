namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando il payload M4 assemblato non è serializzabile
    /// oppure contiene campi obbligatori nulli dopo la fase di assembly.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Eccezioni.
    /// </summary>
    public class PayloadStructureException : Exception
    {
        public PayloadStructureException(string message)
            : base(message)
        {
        }

        public PayloadStructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
