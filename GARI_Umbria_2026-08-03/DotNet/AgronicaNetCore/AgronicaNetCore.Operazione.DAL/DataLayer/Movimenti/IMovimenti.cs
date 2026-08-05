using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti
{
    public interface IMovimenti
    {
        public Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(int Id_Mov, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> CreateAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer);

        public Task<int> ScriviModificaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovimenti> Movimenti, AgronicaCoreParametriServer objParametriServer);
    }
}
