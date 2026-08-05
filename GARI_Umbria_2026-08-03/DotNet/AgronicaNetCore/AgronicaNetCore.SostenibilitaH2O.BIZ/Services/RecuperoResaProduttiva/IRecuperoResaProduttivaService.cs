using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoResaProduttiva
{
    /// <summary>
    /// Contratto per il servizio di recupero della resa produttiva dal database GIAS.
    /// <para>
    /// Interroga le operazioni di raccolta (Lav_Cod = <c>LAVCOD_RACCOLTA</c>) tramite
    /// <c>IAgenda.LeggiImpiantiAsync</c> e <c>IAgenda.LeggiProdottiAsync</c>,
    /// filtrando per anno di riferimento ed esercizi in stato "Chiuso".
    /// Restituisce la resa aggregata in tonnellate sia per esercizio che per azienda.
    /// </para>
    /// Riferimento spec: DS04-BL RecuperoResaProduttiva.
    /// </summary>
    public interface IRecuperoResaProduttivaService
    {
        /// <summary>
        /// Recupera la resa produttiva per gli esercizi del perimetro nell'anno specificato.
        /// </summary>
        /// <param name="esercizi">Lista degli esercizi del perimetro di calcolo.</param>
        /// <param name="anno">Anno solare di riferimento per il filtro temporale.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="ResaProduttivaResult"/> con la resa in tonnellate aggregata per esercizio e per azienda.
        /// </returns>
        /// <exception cref="Exceptions.NoHarvestDataException">
        /// Nessuna operazione di raccolta trovata per il perimetro e l'anno.
        /// </exception>
        /// <exception cref="Exceptions.DatabaseQueryException">
        /// Errore durante la query su GIAS.
        /// </exception>
        Task<ResaProduttivaResult> GetResaAsync(
            List<EsercizioH2O> esercizi,
            int anno,
            AgronicaCoreParametriServer objParametriServer);
    }
}
