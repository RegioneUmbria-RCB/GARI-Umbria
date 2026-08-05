using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utility.DAL.DataLayer.FiltriTabelle
{
    public interface ITmpAgenda
    {
        Task<bool> CancellaRecordDaIDTestataTempAsync(int idTestataTemp, AgronicaCoreParametriServer objParametriServer);
        Task<bool> ScriviAsync(int idTestataTemp, string piva, int idAgenda, int lavCod, AgronicaCoreParametriServer objParametriServer);
    }
}