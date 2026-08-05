using InData.FoodMetaVerse;
using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Payload
{
    public interface ILookup_Sost_CO2_Colture_Payload
    {
        Task<DataTable> ReadAsync(string? idInvocazione, short? inviato, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        Task<bool> ExistAsync(string idInvocazione, AgronicaCoreParametri objP);

        Task<bool> CreateAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametri objP);
        Task<bool> UpdateAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametri objP);
        Task<bool> DeleteAsync(string idInvocazione, AgronicaCoreParametri objP);

        /// <summary>
        /// Upsert: crea o aggiorna il record in base a <see cref="WriteLookupSostCO2ColturePayload.Id_Invocazione"/>.
        /// Restituisce l'<c>id_invocazione</c> persistito.
        /// </summary>
        Task<string> ScriviModificaAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametri objP);

        Task<bool> EliminaAsync(WriteLookupSostCO2ColturePayload dto, AgronicaCoreParametriServer objP);
        Task<bool> EliminaAsync(List<WriteLookupSostCO2ColturePayload> dtos, AgronicaCoreParametriServer objP);
    }
}

