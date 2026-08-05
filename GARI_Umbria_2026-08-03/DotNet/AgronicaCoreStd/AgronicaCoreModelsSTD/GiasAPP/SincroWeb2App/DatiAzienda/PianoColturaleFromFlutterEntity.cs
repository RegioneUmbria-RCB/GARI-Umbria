using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda
{
    public class PianoColturaleFromFlutterEntity
    {
        public List<AppezzamentoEntity> appezzamenti = new List<AppezzamentoEntity>();
        public List<ImpiantoEntity> impianti = new List<ImpiantoEntity>();
        public List<EsercizioEntity> esercizi = new List<EsercizioEntity>();

        public List<DestinazioneUsoEntity> destinazioniUso = new List<DestinazioneUsoEntity>();
        public List<SpecieEntity> specie = new List<SpecieEntity>();
        public List<VarietaEntity> varieta = new List<VarietaEntity>();
        public List<GruppoFinalitaEntity> finalita = new List<GruppoFinalitaEntity>();
    }
}
