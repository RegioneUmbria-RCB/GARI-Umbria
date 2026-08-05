namespace AgronicaNetCore.RischiMeteo.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione generica sollevata durante la costruzione di un payload M2.
    /// Il campo <see cref="Causa"/> descrive il motivo del fallimento.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Eccezioni, PayloadConstructionException.
    /// </summary>
    public class PayloadConstructionException : Exception
    {
        /// <summary>Descrizione sintetica della causa del fallimento.</summary>
        public string Causa { get; }

        public PayloadConstructionException(string causa)
            : base($"Errore nella costruzione del payload Rischi Meteo: {causa}")
        {
            Causa = causa;
        }

        public PayloadConstructionException(string causa, Exception innerException)
            : base($"Errore nella costruzione del payload Rischi Meteo: {causa}", innerException)
        {
            Causa = causa;
        }
    }
}
