using InData.Agenda;
using AgronicaNetCore.Base.Models;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Mov_Destinazioni
{
    public interface IMovDestinazioniService
    {
        public Task<int> ScriviModificaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(WriteMovDestinazioni dtoMovDest, AgronicaCoreParametriServer objParametriServer);
        public Task<bool> EliminaAsync(List<WriteMovDestinazioni> MovDestinazioni, AgronicaCoreParametriServer objParametriServer);
        Task ScriviMovimentoDestinazioneAsync(Movimento_Destinazione movDest, AgronicaCoreParametriServer objParametriServer);
    }
}
