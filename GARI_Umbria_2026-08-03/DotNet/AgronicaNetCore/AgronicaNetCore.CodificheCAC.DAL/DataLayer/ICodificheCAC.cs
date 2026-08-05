using System.Data;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.CodificheCAC.DAL.Model;

namespace AgronicaNetCore.CodificheCAC.DAL.DataLayer
{
    public interface ICodificheCAC
    {
        /// <summary>
        /// Recupera tutti i record dalla tabella CAC_Codifica_Dati_SistemiEsterni
        /// </summary>
        /// <returns>Lista di codifiche CAC</returns>
        Task<DataTable> GetCodificaCACAsync(AgronicaCoreParametriServer objParametriServer);
        Task<bool> DeleteCodificaCACAsync(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer);
        Task<bool> UpdateCodificaCACAsync(CodificaCACModel model, AgronicaCoreParametriServer objParametriServer);
        Task<bool> InsertCodificaCACAsync(CodificaCACModel model, AgronicaCoreParametriServer objParametriServer);
    }
}
