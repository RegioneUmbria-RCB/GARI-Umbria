using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoPrecipitazioniM6
{
    /// <summary>
    /// Contratto per il recupero delle precipitazioni totali (mm) nel periodo dell'esercizio
    /// colturale, tramite Engine Meteo M6.
    /// <para>
    /// Per ogni esercizio del perimetro:
    /// <list type="number">
    /// <item><description>Recupera il poligono GIS dell'appezzamento e ne estrae il primo punto (centroide WKT).</description></item>
    /// <item><description>Chiama M6 con le coordinate lat/lon e il periodo 1-gen / 31-dic dell'anno.</description></item>
    /// <item><description>Somma i valori orari PREC_HOURLY restituiti e restituisce il totale in mm.</description></item>
    /// <item><description>Se nessuna stazione meteo è disponibile, applica fallback a zero (non bloccante).</description></item>
    /// </list>
    /// </para>
    /// Riferimento spec: DS05-BL RecuperoPrecipitazioniM6.
    /// </summary>
    public interface IRecuperoPrecipitazioniService
    {
        /// <summary>
        /// Recupera il totale delle precipitazioni (mm) per la lista di esercizi e l'anno specificati.
        /// </summary>
        /// <param name="esercizi">
        /// Lista degli esercizi del perimetro selezionato.
        /// La lista non può essere vuota.
        /// </param>
        /// <param name="anno">Anno di riferimento per il calcolo (date range: 1-gen / 31-dic).</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <param name="objParametriSuperServer">Parametri super-server per la configurazione Engine M6.</param>
        /// <param name="cancellationToken">Token di cancellazione opzionale.</param>
        /// <returns>
        /// <see cref="PrecipitazioniResult"/> con i mm totali aggregati per esercizio e per azienda.
        /// In assenza di stazione meteo per un appezzamento, la pioggia è impostata a zero
        /// senza sollevare eccezione (fallback non bloccante).
        /// </returns>
        /// <exception cref="ArgumentNullException">Se <paramref name="esercizi"/> è <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Se la lista <paramref name="esercizi"/> è vuota.</exception>
        /// <exception cref="NoWeatherStationException">
        /// Sollevata internamente e gestita come fallback; non propagata al chiamante.
        /// </exception>
        Task<PrecipitazioniResult> GetPrecipitazioniAsync(
            List<EsercizioH2O> esercizi,
            int anno,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
