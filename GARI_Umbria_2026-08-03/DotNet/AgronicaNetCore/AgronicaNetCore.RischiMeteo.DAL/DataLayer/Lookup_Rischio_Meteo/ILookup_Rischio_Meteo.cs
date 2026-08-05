using InData.FoodMetaVerse;
using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.RischiMeteo.DAL.DataLayer.Lookup_Rischio_Meteo
{
    public interface ILookup_Rischio_Meteo
    {
        Task<(int CodSpecie, int CodVarieta)?> ResolveCodSpecieVarietaDaProdottoAsync(int elemCod, int matCod, AgronicaCoreParametri objP);

        Task<DataTable> ReadAsync(GetLookupRischioMeteo dto, AgronicaCoreParametri objP, FiltroAggiuntivo? xFiltroAggiuntivo = null);

        Task<bool> ExistAsync(string cuaaFiliera, string cuaaAzienda, int idAppezzamento, int idEsercizio, AgronicaCoreParametri objP);

        Task<bool> CreateAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametri objP);
        Task<bool> UpdateAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametri objP);
        Task<bool> DeleteAsync(int id, string cuaaFiliera, string cuaaAzienda, int idAppezzamento, int idEsercizio, AgronicaCoreParametri objP);
        Task<int> RetrieveIdAsync(string cuaaFiliera, string cuaaAzienda, int idAppezzamento, int idEsercizio, AgronicaCoreParametri objP);

        /// <summary>
        /// Upsert: crea o aggiorna il record in base alla chiave logica
        /// (cuaa_filiera, cuaa_azienda, id_appezzamento, id_esercizio).
        /// Restituisce l'<c>id_esercizio</c> persistito.
        /// </summary>
        Task<int> ScriviModificaAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametri objP);

        Task<bool> EliminaAsync(WriteLookupRischioMeteo dto, AgronicaCoreParametriServer objP);
        Task<bool> EliminaAsync(List<WriteLookupRischioMeteo> dtos, AgronicaCoreParametriServer objP);
    }
}