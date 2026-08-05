using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.Info;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti.Factory
{
    public interface IMovimentiFactory
    {
        Movimento GetMovimentoCampagna(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info);
    }
}
