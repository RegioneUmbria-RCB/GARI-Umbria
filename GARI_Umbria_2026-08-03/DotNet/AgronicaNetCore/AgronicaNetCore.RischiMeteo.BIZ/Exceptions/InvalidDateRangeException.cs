namespace AgronicaNetCore.RischiMeteo.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione di warning sollevata quando <c>finestraEventi.start</c> &gt; <c>finestraEventi.end</c>.
    /// Le date vengono scambiate e il payload viene comunque costruito.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Validazione range date, InvalidDateRangeException.
    /// </summary>
    public class InvalidDateRangeException : Exception
    {
        /// <summary>Identificativo dell'Esercizio con l'intervallo date invertito.</summary>
        public string IdEsercizio { get; }

        public InvalidDateRangeException(string idEsercizio)
            : base($"finestraEventi.start > finestraEventi.end per l'Esercizio '{idEsercizio}'. Date scambiate automaticamente.")
        {
            IdEsercizio = idEsercizio;
        }
    }
}
