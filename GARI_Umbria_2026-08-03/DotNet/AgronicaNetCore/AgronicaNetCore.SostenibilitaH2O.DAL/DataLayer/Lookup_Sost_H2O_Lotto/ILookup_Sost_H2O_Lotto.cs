using AgronicaNetCore.Base.Models;
using InData.FoodMetaVerse;
using System.Data;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Lotto
{
    /// <summary>
    /// Read and write access to Lookup_Sost_H20_Lotto.
    /// See Database schema, table lookup_sost_h2o_lotto.
    /// </summary>
    public interface ILookup_Sost_H2O_Lotto
    {
        /// <summary>
        /// Reads lookup rows filtered by filiera, optional azienda and cod_prodotto_fmp.
        /// See DS10-API query parameters.
        /// </summary>
        Task<DataTable> ReadAsync(GetSostH2OLotto dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        /// <summary>
        /// Reads lookup rows filtered by filiera, cod_prodotto_fmp and a batch of (azienda, lotto_raccolta).
        /// See DS11-API Endpoint POST /sostenibilita_h2o/sintesi_per_lotti_raccolti.
        /// </summary>
        Task<DataTable> ReadPerLottiBatchAsync(GetSostH2OLottiBatch dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        /// <summary>
        /// Inserts a single row into Lookup_Sost_H20_Lotto.
        /// See DS08-BL PersistenzaRisultatiPerColture — Regole di Business.
        /// </summary>
        Task<bool> CreateAsync(WriteLookupSostH2OLotto dto, AgronicaCoreParametri objP);

        /// <summary>
        /// Inserts a batch of rows into Lookup_Sost_H20_Lotto within a single atomic transaction.
        /// All rows share the same id_invocazione and data_calcolo.
        /// See DS08-BL PersistenzaRisultatiPerColture — Transazione ACID.
        /// </summary>
        Task<int> InsertBatchAsync(IReadOnlyList<WriteLookupSostH2OLotto> righe, AgronicaCoreParametri objP);
    }
}