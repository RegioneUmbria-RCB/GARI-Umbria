using AgronicaNetCore.Base.Models;
using InData.Agenda;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Mov_Dettaglio_Tecnico
{
    public interface IMovDettTecnicoService
    {
        Task<int> ScriviModificaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer);
        Task ScriviMovDettTecnicoAsync(Movimento_Dettaglio_Tecnico tecnico, AgronicaCoreParametriServer objParametriServer);
        Task<bool> EliminaAsync(WriteMovDettTecnico dtoMovDettTec, AgronicaCoreParametriServer objParametriServer);
        Task<bool> EliminaAsync(List<WriteMovDettTecnico> MovDettTec, AgronicaCoreParametriServer objParametriServer);
    }
}
