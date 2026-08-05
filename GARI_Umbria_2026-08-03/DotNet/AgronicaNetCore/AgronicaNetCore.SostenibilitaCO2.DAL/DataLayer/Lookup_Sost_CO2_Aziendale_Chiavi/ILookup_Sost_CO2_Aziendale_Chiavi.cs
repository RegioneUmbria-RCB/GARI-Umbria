using InData.FoodMetaVerse;
using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Chiavi
{
    public interface ILookup_Sost_CO2_Aziendale_Chiavi
    {
        Task<DataTable> ReadAsync(GetSostCO2Aziendale dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        Task<bool> ExistAsync(int id, AgronicaCoreParametri objP);

        Task<bool> CreateAsync(WriteLookupSostCO2AziendaleChiavi dto, AgronicaCoreParametri objP);
        Task<bool> UpdateAsync(WriteLookupSostCO2AziendaleChiavi dto, AgronicaCoreParametri objP);
        Task<bool> DeleteAsync(int id, string idInvocazione, AgronicaCoreParametri objP);

        /// <summary>
        /// Upsert: crea o aggiorna il record in base a <see cref="WriteLookupSostCO2AziendaleChiavi.Id_Invocazione"/>.
        /// Restituisce l'<c>id_invocazione</c> persistito.
        /// </summary>
        Task<string> ScriviModificaAsync(WriteLookupSostCO2AziendaleChiavi dto, AgronicaCoreParametri objP);

        Task<bool> EliminaAsync(WriteLookupSostCO2AziendaleChiavi dto, AgronicaCoreParametriServer objP);
        Task<bool> EliminaAsync(List<WriteLookupSostCO2AziendaleChiavi> dtos, AgronicaCoreParametriServer objP);
    }
}

