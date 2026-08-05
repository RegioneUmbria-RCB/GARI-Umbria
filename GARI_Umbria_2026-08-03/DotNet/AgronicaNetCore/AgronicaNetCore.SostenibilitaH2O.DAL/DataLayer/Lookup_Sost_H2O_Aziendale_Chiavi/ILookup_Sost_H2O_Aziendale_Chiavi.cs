using AgronicaNetCore.Base.Models;
using InData.FoodMetaVerse;
using System.Data;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Chiavi
{
    /// <summary>
    /// Read access to Lookup_Sost_H20_Aziendale_Chiavi.
    /// See Database schema, table lookup_sost_h2o_aziendale_chiavi.
    /// </summary>
    public interface ILookup_Sost_H2O_Aziendale_Chiavi
    {
        /// <summary>
        /// Reads lookup key rows filtered by filiera/azienda/anno/id invocazione.
        /// See DS09-API, sezione "Parametri Query".
        /// </summary>
        Task<DataTable> ReadAsync(GetSostH2OAziendale dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        /// <summary>
        /// Checks if a row with the given primary key id already exists.
        /// See DS08-BL sezione transazionalita ACID.
        /// </summary>
        Task<bool> ExistAsync(int id, AgronicaCoreParametri objP);

        /// <summary>
        /// Inserts a new lookup key row.
        /// </summary>
        Task<bool> CreateAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametri objP);

        /// <summary>
        /// Updates an existing lookup key row.
        /// </summary>
        Task<bool> UpdateAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametri objP);

        /// <summary>
        /// Deletes a lookup key row by invocation id.
        /// </summary>
        Task<bool> DeleteAsync(string idInvocazione, AgronicaCoreParametri objP);

        /// <summary>
        /// Upsert behavior: create or update by invocation id and return persisted id.
        /// </summary>
        Task<string> ScriviModificaAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametri objP);

        /// <summary>
        /// Deletes a lookup row using the DTO key.
        /// </summary>
        Task<bool> EliminaAsync(WriteLookupSostH2OAziendaleChiavi dto, AgronicaCoreParametriServer objP);

        /// <summary>
        /// Deletes multiple lookup rows within one transaction.
        /// </summary>
        Task<bool> EliminaAsync(List<WriteLookupSostH2OAziendaleChiavi> dtos, AgronicaCoreParametriServer objP);
    }
}
