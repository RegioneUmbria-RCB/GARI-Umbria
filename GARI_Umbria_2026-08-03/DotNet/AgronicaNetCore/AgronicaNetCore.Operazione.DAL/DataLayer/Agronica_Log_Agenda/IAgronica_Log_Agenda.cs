using AgronicaNetCore.Base.Models;
using InData.Agenda;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Agronica_Log_Agenda
{
    public interface IAgronica_Log_Agenda
    {
        //public Task<DataTable> ReadAsync(string int lavCod, DateTime? data, AgronicaCoreParametri objP);

        public Task<bool> CreateAsync(WriteLogAgenda dtoLogAgenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteLogAgenda dtoLogAgenda, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(int ID, string Piva, int Id_Agenda, AgronicaCoreParametriServer objParametriServer);
    }
}
