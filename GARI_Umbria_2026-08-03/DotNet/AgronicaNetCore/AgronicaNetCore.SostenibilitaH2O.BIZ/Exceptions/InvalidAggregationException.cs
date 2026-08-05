namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando i dati di input risultano inconsistenti per l'aggregazione aziendale:
    /// lista di esercizi vuota oppure superficie coltivata totale ≤ 0.
    /// Riferimento spec: DS09-BL AggregazionePayloadPerAzienda — InvalidAggregationException.
    /// </summary>
    public class InvalidAggregationException : Exception
    {
        /// <param name="message">Descrizione del problema di aggregazione rilevato.</param>
        public InvalidAggregationException(string message) : base(message) { }
    }
}
