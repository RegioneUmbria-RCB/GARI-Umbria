using AgronicaNetCore.Base.Models;
using InData.FoodMetaVerse;
using System.Data;

namespace AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Payload
{
    /// <summary>
    /// Read access to Lookup_Sost_H20_Aziendale_Payload.
    /// See Database schema, table lookup_sost_h2o_aziendale_payload.
    /// </summary>
    public interface ILookup_Sost_H2O_Aziendale_Payload
    {
        /// <summary>
        /// Reads payload rows by invocation id.
        /// </summary>
        Task<DataTable> ReadAsync(string idInvocazione, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        /// <summary>
        /// Checks if a payload row exists for invocation id.
        /// </summary>
        Task<bool> ExistAsync(string idInvocazione, AgronicaCoreParametri objP);

        /// <summary>
        /// Inserts a payload row.
        /// </summary>
        Task<bool> CreateAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametri objP);

        /// <summary>
        /// Updates an existing payload row.
        /// </summary>
        Task<bool> UpdateAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametri objP);

        /// <summary>
        /// Deletes a payload row by invocation id.
        /// </summary>
        Task<bool> DeleteAsync(string idInvocazione, AgronicaCoreParametri objP);

        /// <summary>
        /// Upsert behavior: create or update by invocation id and return persisted id.
        /// </summary>
        Task<string> ScriviModificaAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametri objP);

        /// <summary>
        /// Deletes a payload row using the DTO key.
        /// </summary>
        Task<bool> EliminaAsync(WriteLookupSostH2OAziendalePayload dto, AgronicaCoreParametriServer objP);

        /// <summary>
        /// Deletes multiple payload rows within one transaction.
        /// </summary>
        Task<bool> EliminaAsync(List<WriteLookupSostH2OAziendalePayload> dtos, AgronicaCoreParametriServer objP);
    }
}
