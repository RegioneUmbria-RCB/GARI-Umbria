using System.Data;
using InData.Agenda;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.RicettexAgenda
{
    public interface IRicettexAgenda
    {
        public Task<DataTable> ReadAsync(int Ricetta_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, int RicettaOp_Cod = 0);
        public Task<bool> ExistAsync(int Ricetta_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, int RicettaOp_Cod = 0);

        public Task<bool> CreateAsync(WriteRicettexAgenda dtoRxA, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> UpdateAsync(WriteRicettexAgenda dtoRxA, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> DeleteAsync(int Ricetta_Cod, int Id_Agenda, AgronicaCoreParametriServer objParametriServer, int RicettaOp_Cod = 0);

        public Task<bool> ScriviModificaAsync(WriteRicettexAgenda dtoRicxAg, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteRicettexAgenda dtoRicxAg, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteRicettexAgenda> RicettexAgenda, AgronicaCoreParametriServer objParametriServer);
    }
}
