namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando non vengono trovate operazioni di raccolta
    /// per il perimetro e l'anno specificati.
    /// <para>
    /// Secondo la spec DS04-BL, se nessuna raccolta è presente la resa è considerata
    /// zero, ma la condizione diventa bloccante quando il benchmark viene applicato
    /// nel calcolo del bilancio idrico. Il chiamante (<c>CalcoloSostenibilitaH2OService</c>)
    /// decide se propagare o assorbire questa eccezione.
    /// </para>
    /// Riferimento spec: DS04-BL RecuperoResaProduttiva — Eccezioni.
    /// </summary>
    public class NoHarvestDataException : Exception
    {
        /// <param name="message">Descrizione del motivo dell'assenza di dati.</param>
        public NoHarvestDataException(string message) : base(message) { }

        /// <param name="message">Descrizione del motivo dell'assenza di dati.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public NoHarvestDataException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
