using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda
{
    public class DatiAziendaEntity
    {
        public ImpresaEntity azienda;

        public List<CentroAziendaleEntity> centriAziendali = new List<CentroAziendaleEntity>();
        public List<CampoEntity> campi = new List<CampoEntity>();
        public PianoColturaleEntity pianoColturale = new PianoColturaleEntity();

        public List<ProdottoEntity> prodotti = new List<ProdottoEntity>();
        public List<FabbricatoEntity> magazzini = new List<FabbricatoEntity>();
        public List<RilevamentoDiMagazzinoEntity> prodottiGiacenze = new List<RilevamentoDiMagazzinoEntity>();

        public List<ContattoEntity> contatti = new List<ContattoEntity>();
        public List<RisorseUmaneEntity> risorseUmane = new List<RisorseUmaneEntity>();
        public List<ContattoEntity> fornitori = new List<ContattoEntity>();
        public List<RisorseUmaneEntity> risorseFornitori = new List<RisorseUmaneEntity>();
        public List<ParcoMacchineEntity> parcoMacchine = new List<ParcoMacchineEntity>();

        public List<ProgettoEntity> progetti = new List<ProgettoEntity>();
        public List<CentroAziendaleAttivitaCDGEntity> centriAziendaliAttivitaCDG = new List<CentroAziendaleAttivitaCDGEntity>();

        public List<ImpostazioneEntity> impostazioni = new List<ImpostazioneEntity>();

        public class PianoColturaleEntity
        {
            public List<CentroAziendaleEntity> centriAziendali = new List<CentroAziendaleEntity>();
            public List<AppezzamentoEntity> appezzamenti = new List<AppezzamentoEntity>();
            public List<ImpiantoEntity> impianti = new List<ImpiantoEntity>();
            public List<EsercizioEntity> esercizi = new List<EsercizioEntity>();

            public List<DestinazioneUsoEntity> destinazioniUso = new List<DestinazioneUsoEntity>();
            public List<SpecieEntity> specie = new List<SpecieEntity>();
            public List<VarietaEntity> varieta = new List<VarietaEntity>();
            public List<GruppoFinalitaEntity> finalita = new List<GruppoFinalitaEntity>();

        }
    }

}
