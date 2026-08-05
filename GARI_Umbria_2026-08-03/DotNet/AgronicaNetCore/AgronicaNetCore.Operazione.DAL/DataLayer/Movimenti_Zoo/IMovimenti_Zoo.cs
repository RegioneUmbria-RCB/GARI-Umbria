using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Zoo
{
    public interface IMovimenti_Zoo
    {
        public Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> CreateAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> ScriviModificaAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovimentiZoo dtoMovZoo, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovimentiZoo> MovZoo, AgronicaCoreParametriServer objParametriServer);
    }
}
