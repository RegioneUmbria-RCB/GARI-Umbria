using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico
{
    public interface IMov_Dettaglio_Tecnico
    {
        public Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> CreateAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer);

        public Task<int> ScriviModificaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovDettTecnico> MovDettTec, AgronicaCoreParametriServer objParametriServer);
    }
}
