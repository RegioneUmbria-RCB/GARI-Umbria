using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Dettagli
{
    public interface IMovimenti_Dettagli
    {
        public Task<DataTable> ReadAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, AgronicaCoreParametriServer objParametriServer, FiltroAggiuntivo? xFiltroAggiuntivo = null);
        public Task<bool> ExistAsync(int Id_Mov_Det, AgronicaCoreParametriServer objParametriServer);

        public Task<bool> CreateAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(string Piva, int Id_Agenda, int Id_Mov, int Id_Mov_Det, AgronicaCoreParametriServer objParametriServer);

        public Task<int> ScriviModificaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovDettagli> MovDettagli, AgronicaCoreParametriServer objParametriServer);
    }
}
