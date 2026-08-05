namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando nessuna stazione meteo è disponibile nelle vicinanze
    /// del centroide dell'appezzamento richiesto (risposta HTTP 404 da Engine M6).
    /// <para>
    /// Secondo la spec DS05-BL, l'assenza di stazione meteo è un fallback non bloccante:
    /// il servizio <see cref="Services.RecuperoPrecipitazioniM6.RecuperoPrecipitazioniService"/>
    /// cattura questa eccezione e imposta la pioggia a zero per l'esercizio coinvolto.
    /// </para>
    /// Riferimento spec: DS05-BL RecuperoPrecipitazioniM6 — Eccezioni.
    /// </summary>
    public class NoWeatherStationException : Exception
    {
        /// <param name="message">Descrizione del motivo dell'assenza di stazione meteo.</param>
        public NoWeatherStationException(string message) : base(message) { }

        /// <param name="message">Descrizione del motivo dell'assenza di stazione meteo.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public NoWeatherStationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
