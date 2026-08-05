using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.PrecipitazioniM6
{
    /// <summary>
    /// Contratto per il recupero delle precipitazioni dal servizio Engine Meteo M6 (HYPERMETEO).
    /// <para>
    /// La singola chiamata restituisce la somma di tutti i valori orari PREC_HOURLY nel periodo,
    /// espressa in millimetri (mm). Ritorna <see langword="null"/> se nessuna stazione meteo
    /// è disponibile nelle vicinanze delle coordinate fornite (HTTP 404 da M6).
    /// </para>
    /// Riferimento spec: DS05-BL RecuperoPrecipitazioniM6.
    /// </summary>
    public interface IPrecipitazioniM6DAL
    {
        /// <summary>
        /// Recupera il totale delle precipitazioni (mm) per le coordinate e il periodo indicati.
        /// </summary>
        /// <param name="lat">Latitudine WGS84 del punto di osservazione.</param>
        /// <param name="lon">Longitudine WGS84 del punto di osservazione.</param>
        /// <param name="dataInizio">Data inizio periodo in formato ISO 8601 (es. <c>2024-01-01T00:00:00Z</c>).</param>
        /// <param name="dataFine">Data fine periodo in formato ISO 8601 (es. <c>2024-12-31T23:59:59Z</c>).</param>
        /// <param name="objParametriServer">Parametri di connessione al server GIAS.</param>
        /// <param name="objParametriSuperServer">Parametri super-server per la configurazione Engine M6.</param>
        /// <param name="cancellationToken">Token di cancellazione opzionale.</param>
        /// <returns>
        /// Somma delle precipitazioni orarie nel periodo in mm,
        /// oppure <see langword="null"/> se nessuna stazione meteo è disponibile.
        /// </returns>
        Task<decimal?> GetPioggiaTotaleMmAsync(
            decimal lat,
            decimal lon,
            string dataInizio,
            string dataFine,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
