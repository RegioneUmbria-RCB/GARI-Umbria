namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando non è possibile trovare il codice FAOSTAT corrispondente
    /// al codice varietà GIAS (Profitosan) nella tabella di trascodifica <c>CAC_Codifica_Veg_Cod</c>.
    /// <para>
    /// Questa eccezione è bloccante: senza la varietà nel database FAOSTAT il calcolo
    /// del benchmark non può procedere.
    /// </para>
    /// Riferimento spec: DS06-BL LookupBenchmarkWaterFootprint — Eccezioni.
    /// </summary>
    public class MalformedVarietaException : Exception
    {
        /// <param name="message">Descrizione del codice varietà non trovato.</param>
        public MalformedVarietaException(string message) : base(message) { }

        /// <param name="message">Descrizione del codice varietà non trovato.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public MalformedVarietaException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
