using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.attivita.risorse;
using InData.Anagrafica;

namespace AgronicaNetCore.Operazione.BIZ.Services.Mov_Destinazioni.Factory
{
    public interface IMovDestinazioniFactory
    {
        List<Movimento_Destinazione>? GetMovDestinazioni(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, decimal area, decimal dose);
        List<Movimento_Destinazione>? GetMovDestinazioniRaccolta(Movimento_Dettaglio mov, Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, decimal area, decimal dose, List<RisorsaProdotto> resources);
        List<Movimento_Destinazione> GetMovDestinazioniRilievo(IEnumerable<DettaglioRilievo> dettagliPerChiave, Attivita attivita, decimal superficieAccumulata, InfoOperazione info);
    }
}