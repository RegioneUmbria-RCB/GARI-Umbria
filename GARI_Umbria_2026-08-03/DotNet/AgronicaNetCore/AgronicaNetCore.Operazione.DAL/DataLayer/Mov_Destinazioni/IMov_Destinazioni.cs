using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Destinazioni
{
    public interface IMov_Destinazioni
    {
        public Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Dest, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Destinazione, AgronicaCoreParametriServer objParametriServer);
        
        public Task<bool> CreateAsync(WriteMovDestinazioni dtoMovDestinazioni, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteMovDestinazioni dtoMovDestinazioni, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Dest, AgronicaCoreParametriServer objParametriServer);

        public Task<int> ScriviModificaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovDestinazioni> MovDestinazioni, AgronicaCoreParametriServer objParametriServer);
    }
}
