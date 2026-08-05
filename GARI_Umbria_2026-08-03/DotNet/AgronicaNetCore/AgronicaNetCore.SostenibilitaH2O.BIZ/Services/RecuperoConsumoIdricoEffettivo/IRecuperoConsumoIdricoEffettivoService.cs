using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoConsumoIdricoEffettivo
{
    /// <summary>
    /// Contratto per il recupero del volume totale di acqua utilizzata per
    /// irrigazione (Lav_Cod=1) e fertirrigazione (Lav_Cod=26) nell'anno di riferimento,
    /// per gli impianti del perimetro selezionato.
    /// <para>
    /// Il risultato è aggregato sia per esercizio (modalità "Per Colture") che
    /// per azienda (modalità "Aziendale"), in modo che il chiamante scelga
    /// la proiezione appropriata.
    /// </para>
    /// Riferimento spec: DS03-BL RecuperoConsumoIdricoEffettivo.
    /// </summary>
    public interface IRecuperoConsumoIdricoEffettivoService
    {
        /// <summary>
        /// Recupera il consumo idrico effettivo (irrigazioni e fertirrigazioni)
        /// per la lista di esercizi e l'anno specificati.
        /// </summary>
        /// <param name="esercizi">
        /// Lista degli esercizi del perimetro selezionato.
        /// Ogni esercizio corrisponde a un <c>Progetto_Cod</c> in <c>Imprese_Progetti</c>.
        /// La lista non può essere vuota.
        /// </param>
        /// <param name="anno">Anno di riferimento per il filtro temporale sulle operazioni.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="VolumiIrrigazioneResult"/> con i volumi in m³ aggregati per esercizio e per azienda.
        /// Se non esistono operazioni di irrigazione, i dizionari risultanti sono vuoti;
        /// la specifica prevede volume = 0 in assenza di dati (nessuna eccezione sollevata di default).
        /// </returns>
        /// <exception cref="NoIrrigationDataException">
        /// Sollevata se la business rule richiede che almeno un'operazione di irrigazione esista.
        /// </exception>
        /// <exception cref="DatabaseQueryException">
        /// Sollevata in caso di errore SQL durante l'interrogazione del database GIAS.
        /// </exception>
        Task<VolumiIrrigazioneResult> GetVolumiAsync(
            List<EsercizioH2O> esercizi,
            int anno,
            AgronicaCoreParametriServer objParametriServer);
    }
}
