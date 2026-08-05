using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.PersistenzaRisultatiPerColture
{
    /// <summary>
    /// Contratto per la persistenza batch dei risultati di sostenibilità idrica in modalità "Per Colture"
    /// nella tabella <c>Lookup_Sost_H20_Lotto</c>.
    /// Riferimento spec: DS08-BL PersistenzaRisultatiPerColture.
    /// </summary>
    public interface IPersistenzaRisultatiPerColtureService
    {
        /// <summary>
        /// Valida e inserisce in modo atomico (unica transazione) tutte le righe presenti in
        /// <see cref="PersistenzaRisultatiPerColtureInput.Risultati"/> nella tabella
        /// <c>Lookup_Sost_H20_Lotto</c>.
        /// <para>
        /// Le righe non conformi (superficie ≤ 0, chiavi candidate mancanti) sono saltate con log
        /// warning senza rilanciare eccezioni al caller.
        /// </para>
        /// <para>
        /// Se l'inserimento database fallisce, viene sollevata
        /// <see cref="Exceptions.TransactionFailureException"/> che causa il rollback di tutta la batch.
        /// </para>
        /// Riferimento spec: DS08-BL — Regole di Business, Eccezioni.
        /// </summary>
        /// <param name="input">Dati del batch da persistere.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>Output con numero di righe inserite e id_invocazione.</returns>
        Task<PersistenzaRisultatiPerColtureOutput> EseguiAsync(
            PersistenzaRisultatiPerColtureInput input,
            AgronicaCoreParametriServer objParametriServer);
    }
}
