using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni
{
    public class DatiComuniEntity
    {
        public List<LavorazioneEntity> lavorazioni = new List<LavorazioneEntity>();
        public List<AttivitaCDGEntity> attivitaCDG = new List<AttivitaCDGEntity>();
        public List<LavorazioneAttivitaCDGEntity> lavorazioniAttivitaCDG = new List<LavorazioneAttivitaCDGEntity>();
        public List<OperazioniCombinazioniEntity> operazioniCombinazioni = new List<OperazioniCombinazioniEntity>(); 

        public List<AvversitaEntity> avversita = new List<AvversitaEntity>();
        public List<GruppoAvversitaEntity> gruppiAvversita = new List<GruppoAvversitaEntity>();
        public List<GruppoAvversitaAttiveEntity> gruppiAvversitaAttive = new List<GruppoAvversitaAttiveEntity>();
        public List<InfestantiAttiveEntity> infestantiAttive = new List<InfestantiAttiveEntity>();
        public List<AvversitaSpecieEntity> avversitaSpecie = new List<AvversitaSpecieEntity>();

        public List<CategorieUnitaMisuraEntity> categorieUnitaMisura = new List<CategorieUnitaMisuraEntity>();
        public List<MisuraAvversitaEntity> misureAvversita = new List<MisuraAvversitaEntity>();
        public List<MisuraDanniRaccoltaEntity> misureDanni = new List<MisuraDanniRaccoltaEntity>();
        public List<MisuraAvversitaAnagraficheEntity> misureAvversitaAnagrafiche = new List<MisuraAvversitaAnagraficheEntity>();
        public List<MisuraIndiciMaturitaEntity> misureIndiciMaturita = new List<MisuraIndiciMaturitaEntity>();
        public List<MisuraIndiciMaturitaAnagraficheEntity> misureIndiciMaturitaAnagrafiche = new List<MisuraIndiciMaturitaAnagraficheEntity>();
        public List<IndiciMaturitaSpecieVegetaliEntity> indiciMaturitaSpecieVegetali = new List<IndiciMaturitaSpecieVegetaliEntity>();
        public List<IndiciMaturitaEntity> indiciMaturita = new List<IndiciMaturitaEntity>();
        public List<SpecieVegetaliStadiCrescitaEntity> specieVegetaliStadiCrescita = new List<SpecieVegetaliStadiCrescitaEntity>();

        public List<ContattoEntity> contatti = new List<ContattoEntity>();
        public List<RisorseUmaneEntity> risorseUmane = new List<RisorseUmaneEntity>();
        public List<ContattoEntity> fornitori = new List<ContattoEntity>();
        public List<RisorseUmaneEntity> risorseFornitori = new List<RisorseUmaneEntity>();
        public List<ParcoMacchineEntity> parcoMacchine = new List<ParcoMacchineEntity>();
        public List<TipoMacchinaEntity> tipiMacchine = new List<TipoMacchinaEntity>();
        public List<OperazioneCausaleEntity> operazioniCausali = new List<OperazioneCausaleEntity>(); //non sono dati comuni, tabella di db_Server

        public List<AreaTipologieDocumentoEntity> areeTipologie = new List<AreaTipologieDocumentoEntity>();
        public List<TipologiaDocumentoEntity> tipologie = new List<TipologiaDocumentoEntity>();

        public List<ProdottoEntity> prodotti = new List<ProdottoEntity>();
        public List<CodificaProdottoEntity> codificaProdotti = new List<CodificaProdottoEntity>();

        public List<DestinazioneUsoEntity> destinazioniUso = new List<DestinazioneUsoEntity>();
        public List<SpecieEntity> specie = new List<SpecieEntity>();
        public List<VarietaEntity> varieta = new List<VarietaEntity>();
        public List<GruppoFinalitaEntity> finalita = new List<GruppoFinalitaEntity>();
        public List<DisciplinareEntity> disciplinari = new List<DisciplinareEntity>();

        public List<NazioneEntity> nazioni = new List<NazioneEntity>();
        public List<RegioneEntity> regioni = new List<RegioneEntity>();
        public List<ProvinciaEntity> province = new List<ProvinciaEntity>();
        public List<ComuneEntity> comuni = new List<ComuneEntity>();

        public List<SpecieZootecnicaEntity> specieZootecniche = new List<SpecieZootecnicaEntity>();
        // public List<GerarchiaImpreseTipologiaEntity> tipologieGerarchiaImprese = new List<GerarchiaImpreseTipologiaEntity>();        
    }
}
