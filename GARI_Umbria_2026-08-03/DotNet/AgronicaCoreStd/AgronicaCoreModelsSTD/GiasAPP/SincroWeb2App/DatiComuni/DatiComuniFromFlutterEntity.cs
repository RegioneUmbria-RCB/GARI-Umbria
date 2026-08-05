using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni
{
    public class DatiComuniFromFlutterEntity
    {
        public List<LavorazioneEntity> lavorazioni = new List<LavorazioneEntity>();
        public List<OperazioniCombinazioniEntity> operazioniCombinazioni = new List<OperazioniCombinazioniEntity>(); 

        public List<AvversitaEntity> avversita = new List<AvversitaEntity>();
        public List<GruppoAvversitaEntity> gruppiAvversita = new List<GruppoAvversitaEntity>();
        public List<GruppoAvversitaAttiveEntity> gruppiAvversitaAttive = new List<GruppoAvversitaAttiveEntity>();
        public List<InfestantiAttiveEntity> infestantiAttive = new List<InfestantiAttiveEntity>();
        public List<AvversitaSpecieEntity> avversitaSpecie = new List<AvversitaSpecieEntity>();

        public List<CategorieUnitaMisuraEntity> categorieUnitaMisura = new List<CategorieUnitaMisuraEntity>();
        public List<MisuraAvversitaEntity> misureAvversita = new List<MisuraAvversitaEntity>();
        public List<MisuraDanniRaccoltaEntity> misureDanni = new List<MisuraDanniRaccoltaEntity>();
        public List<MisuraIndiciMaturitaEntity> misureIndiciMaturita = new List<MisuraIndiciMaturitaEntity>();
        public List<IndiciMaturitaSpecieVegetaliEntity> indiciMaturitaSpecieVegetali = new List<IndiciMaturitaSpecieVegetaliEntity>();
        public List<IndiciMaturitaEntity> indiciMaturita = new List<IndiciMaturitaEntity>();
        public List<SpecieVegetaliStadiCrescitaEntity> specieVegetaliStadiCrescita = new List<SpecieVegetaliStadiCrescitaEntity>();

        public List<TipoMacchinaEntity> tipiMacchine = new List<TipoMacchinaEntity>();

        public List<SpecieEntity> specie = new List<SpecieEntity>();
        public List<VarietaEntity> varieta = new List<VarietaEntity>();
        public List<GruppoFinalitaEntity> finalita = new List<GruppoFinalitaEntity>();
        public List<DisciplinareEntity> disciplinari = new List<DisciplinareEntity>();

        public List<NazioneEntity> nazioni = new List<NazioneEntity>();
        public List<RegioneEntity> regioni = new List<RegioneEntity>();
        public List<ProvinciaEntity> province = new List<ProvinciaEntity>();
        public List<ComuneEntity> comuni = new List<ComuneEntity>();

        public List<SpecieZootecnicaEntity> specieZootecniche = new List<SpecieZootecnicaEntity>();

        public List<ImpiantoIrrigazioneEntity> impiantiIrrigazioni = new List<ImpiantoIrrigazioneEntity>();
        public List<RapportiContabiliEntity> rapportiContabili = new List<RapportiContabiliEntity>();
    }
}
