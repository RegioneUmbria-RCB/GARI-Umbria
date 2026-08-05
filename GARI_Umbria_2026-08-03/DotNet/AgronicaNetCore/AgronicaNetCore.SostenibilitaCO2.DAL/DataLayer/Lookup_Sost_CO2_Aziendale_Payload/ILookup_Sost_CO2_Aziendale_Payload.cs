using InData.FoodMetaVerse;
using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Payload
{
    public interface ILookup_Sost_CO2_Aziendale_Payload
    {
        Task<DataTable> ReadAsync(string? idInvocazione, short? inviato, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        Task<bool> ExistAsync(string idInvocazione, AgronicaCoreParametri objP);

        Task<bool> CreateAsync(WriteLookupSostCO2AziendalePayload dto, AgronicaCoreParametri objP);
        Task<bool> UpdateAsync(WriteLookupSostCO2AziendalePayload dto, AgronicaCoreParametri objP);
        Task<bool> DeleteAsync(string idInvocazione, AgronicaCoreParametri objP);

        /// <summary>
        /// Upsert: crea o aggiorna il record in base a <see cref="WriteLookupSostCO2AziendalePayload.Id_Invocazione"/>.
        /// Restituisce l'<c>id_invocazione</c> persistito.
        /// </summary>
        Task<string> ScriviModificaAsync(WriteLookupSostCO2AziendalePayload dto, AgronicaCoreParametri objP);

        Task<bool> EliminaAsync(WriteLookupSostCO2AziendalePayload dto, AgronicaCoreParametriServer objP);
        Task<bool> EliminaAsync(List<WriteLookupSostCO2AziendalePayload> dtos, AgronicaCoreParametriServer objP);
    }
}

