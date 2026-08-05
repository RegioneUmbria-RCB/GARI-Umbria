using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.Info;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Dettagli.Factory
{
    public interface IMovDettagliFactory
    {
        List<Movimento_Dettaglio> GetMovDettagliList(
            Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, decimal totalTreatedArea);
    }
}