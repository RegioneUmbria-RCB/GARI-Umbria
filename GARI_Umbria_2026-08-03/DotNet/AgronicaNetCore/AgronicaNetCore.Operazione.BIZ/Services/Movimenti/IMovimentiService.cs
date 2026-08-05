using InData.Agenda;
using AgronicaNetCore.Base.Models;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti
{
    public interface IMovimentiService
    {
        public Task<int> ScriviModificaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovimenti dtoMovimenti, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovimenti> Movimenti, AgronicaCoreParametriServer objParametriServer);
        Task ScriviMovimentoAsync(Movimento movimento, AgronicaCoreParametriServer bjParametriServer);
    }
}
