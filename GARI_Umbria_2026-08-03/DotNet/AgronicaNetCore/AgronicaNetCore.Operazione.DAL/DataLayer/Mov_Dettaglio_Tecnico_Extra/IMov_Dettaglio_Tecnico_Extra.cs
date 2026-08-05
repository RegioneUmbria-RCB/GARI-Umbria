using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico_Extra
{
    public interface IMov_Dettaglio_Tecnico_Extra
    {
        public Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> CreateAsync(WriteMovDettTecnicoExtra dtoMovDettTecnEx, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteMovDettTecnicoExtra dtoMovDettTecnEx, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, int Id_Reg_Det, AgronicaCoreParametriServer objParametriServer);

        public Task<int> ScriviModificaAsync(WriteMovDettTecnicoExtra dtoMovDettTecExtra, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovDettTecnicoExtra dtoMovDettTecExtra, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovDettTecnicoExtra> MovDettTecExtra, AgronicaCoreParametriServer objParametriServer);
    }
}
