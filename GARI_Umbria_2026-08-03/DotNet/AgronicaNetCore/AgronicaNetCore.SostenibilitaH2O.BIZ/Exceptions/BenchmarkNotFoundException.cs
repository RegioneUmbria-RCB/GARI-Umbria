namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando nessuna riga viene trovata nella tabella
    /// <c>waterfootprint</c> per la combinazione (paese, codice FAOSTAT, wf_type='Green+Blue').
    /// <para>
    /// Questa eccezione è bloccante: senza il valore di benchmark il calcolo del bilancio
    /// idrico non può procedere.
    /// </para>
    /// Riferimento spec: DS06-BL LookupBenchmarkWaterFootprint — Eccezioni.
    /// </summary>
    public class BenchmarkNotFoundException : Exception
    {
        /// <param name="message">Descrizione della combinazione di parametri non trovata.</param>
        public BenchmarkNotFoundException(string message) : base(message) { }

        /// <param name="message">Descrizione della combinazione di parametri non trovata.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public BenchmarkNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
