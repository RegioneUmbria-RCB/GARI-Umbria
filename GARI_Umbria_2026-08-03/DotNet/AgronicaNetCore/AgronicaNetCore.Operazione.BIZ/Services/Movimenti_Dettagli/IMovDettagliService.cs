using InData.Agenda;
using AgronicaNetCore.Base.Models;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Dettagli
{
    public interface IMovDettagliService
    {
        public Task<int> ScriviModificaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovDettagli dtoMovDettagli, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovDettagli> MovDettagli, AgronicaCoreParametriServer objParametriServer);
        Task ScriviMovimentoDettaglioAsync(Movimento_Dettaglio movDett, AgronicaCoreParametriServer objParametriServer, bool documentoPrevisionale = false, bool usaDataModifica = false);
    }
}
