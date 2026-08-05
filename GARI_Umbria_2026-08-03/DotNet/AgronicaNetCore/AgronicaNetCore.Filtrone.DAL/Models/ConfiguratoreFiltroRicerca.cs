
using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.profilazione;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaCoreModelsSTD.Models.ConfiguratoreFiltroRicerca
{
    public class ConfiguratoreFiltroRicerca
    {
        #region "COLONNE VISUALIZZATE"
        private bool _caricaIndirizzoAzienda;
        private bool _caricaIndirizzoCentroAziendale;
        private bool _caricaIndirizziAppezzamento;

        private bool _caricaDatiServizi = false;
        private bool _caricaDatiGISImpianto = false;
        private bool _caricaContributiACA = false;

        private bool _caricaDatiCatastaliAppezzamento = false;

        private bool _caricaImpreseReferenti = false;

        private bool _caricaNumeroLibroSociImpresaReferente = false;
        private bool _caricaDataIscrizioneLibroSociImpresaReferente = false;

        private bool _caricaDatiGerarchia = false;

        public bool CaricaImpreseReferenti
        {
            get { return _caricaImpreseReferenti; }
            set
            {
                _caricaImpreseReferenti = value;
                joinImpreseReferenti = value;
               
            }
        }

        public bool CaricaNumeroLibroSociImpresaReferente
        {
            get { return _caricaNumeroLibroSociImpresaReferente; }
            set
            {
                _caricaNumeroLibroSociImpresaReferente = value;
               if (_caricaNumeroLibroSociImpresaReferente)
                    CaricaImpreseReferenti = value;
            }
        }  
        
        public bool CaricaDataIscrizioneLibroSociImpresaReferente
        {
            get { return _caricaDataIscrizioneLibroSociImpresaReferente; }
            set
            {
                _caricaDataIscrizioneLibroSociImpresaReferente = value;
               if (_caricaDataIscrizioneLibroSociImpresaReferente)
                    CaricaImpreseReferenti = value;
            }
        }

        public bool CaricaLegaleRappresentante;
        public bool CaricaIndirizzoAzienda
        {
            get { return _caricaIndirizzoAzienda; }
            set
            {
                _caricaIndirizzoAzienda = value;
                joinIndirizzoAzienda = value;
            }
        }
        public bool CaricaIndirizzoCentroAziendale
        {
            get { return _caricaIndirizzoCentroAziendale; }
            set
            {
                _caricaIndirizzoCentroAziendale = value;
                joinIndirizzoCentroAziendale = value;
                if (CaricaIndirizzoAzienda)
                    specificaIndirizzo = value;
            }
        }
        public bool CaricaIndirizziAppezzamento
        {
            get { return _caricaIndirizziAppezzamento; }
            set
            {
                _caricaIndirizziAppezzamento = value;
                specificaIndirizzo = value;
            }
        }
        public bool CaricaDatiCatastaliCentroAziendale;
        public bool CaricaDatiCatastaliAppezzamento
        {
            get { return _caricaDatiCatastaliAppezzamento; }
            set
            {
                _caricaDatiCatastaliAppezzamento = value;
                joinRiparto = value;
            }
        }
        public bool CaricaDatiCatastaliCampo;
        public bool CaricaDatiServizi
        {
            get { return _caricaDatiServizi; }
            set
            {
                _caricaDatiServizi = value;
                joinServizio = value;
            }
        }
        public bool CaricaDatiGISImpianto
        {
            get { return _caricaDatiGISImpianto; }
            set
            {
                _caricaDatiGISImpianto = value;
                joinGIS = value;
            }
        }

        public bool CaricaContributiACA
        {
            get { return _caricaContributiACA; }
            set
            {
                _caricaContributiACA = value;
                joinContributiACA = value;
            }
        }

        public bool joinIndirizzoAzienda;
        public bool joinIndirizzoCentroAziendale;

        public bool specificaIndirizzo;
        public bool specificaCoordinate;
        #endregion

        #region "LOGICHE TIPO MOSTRA E JOIN"
        private bool _isMostraAziende = false;
        private bool _isMostraCentriAziendali = false;
        private bool _isMostraCampi = false;
        private bool _isMostraAppezzamenti = false;
        private bool _isMostraImpianti = false;
        private bool _isMostraEsercizi = false;
        private bool _isMostraPianoColturale = false;
        private bool _isMostraFabbricati = false;
        private bool _isMostraMovimenti = false;

        public bool isMostraAziende
        {
            get { return _isMostraAziende; }
            set
            {
                _isMostraAziende = value;
            }
        }
        public bool isMostraCentriAziendali
        {
            get { return _isMostraCentriAziendali; }
            set
            {
                _isMostraCentriAziendali = value;
                CentriAziendaliBase = value;
            }
        }
        public bool isMostraCampi
        {
            get { return _isMostraCampi; }
            set
            {
                _isMostraCampi = value;
                CampiBase = value;
            }
        }
        public bool isMostraAppezzamenti
        {
            get { return _isMostraAppezzamenti; }
            set
            {
                _isMostraAppezzamenti = value;
                AppezzamentiBase = value;
            }
        }
        public bool isMostraImpianti
        {
            get { return _isMostraImpianti; }
            set
            {
                _isMostraImpianti = value;
                ImpiantiBase = value;
                EserciziBase = value;
                needColumnsEsercizi = value;
                specificaCoordinate = value;
            }
        }
        public bool isMostraEsercizi
        {
            get { return _isMostraEsercizi; }
            set
            {
                _isMostraEsercizi = value;
                EserciziBase = value;
                specificaCoordinate = value;
                specificaIndirizzo = value;
            }
        }
        public bool isMostraPianoColturale
        {
            get { return _isMostraPianoColturale; }
            set
            {
                _isMostraPianoColturale = value;
                isMostraEsercizi = value;
                EserciziBase = value;
            }
        }
        public bool isMostraFabbricati
        {
            get { return _isMostraFabbricati; }
            set
            {
                _isMostraFabbricati = value;
                FabbricatiBase = value;
            }
        }
        public bool isMostraMovimenti
        {
            get { return _isMostraMovimenti; }
            set
            {
                _isMostraMovimenti = value;
                MovimentiBase = value;
            }
        }

        public bool needColumnsCentriAziendali;
        public bool needColumnsCampi;
        public bool needColumnsAppezzamenti;
        public bool needColumnsImpianti;
        public bool needColumnsEsercizi;
        public bool needColumnsFabbricati;


        private bool _joinAppezzamento;
        private bool _joinImpianto;
        private bool _joinEsercizio;
        private bool _joinFabbricato;
        private bool _joinMovimento;

        private bool _joinGIS;
        private bool _joinGISAnomalie;

        private bool _joinRiparto;

        public bool joinCentro;
        public bool joinCampo;
        public bool joinAppezzamento
        {
            get { return _joinAppezzamento; }
            set
            {
                _joinAppezzamento = value;
                joinCentro = value;
            }
        }
        public bool joinImpianto
        {
            get { return _joinImpianto; }
            set
            {
                _joinImpianto = value;
                joinAppezzamento = value;
            }
        }
        public bool joinEsercizio
        {
            get { return _joinEsercizio; }
            set
            {
                _joinEsercizio = value;
                joinImpianto = value;
            }
        }
        public bool joinFabbricato
        {
            get { return _joinFabbricato; }
            set
            {
                _joinFabbricato = value;
                joinCentro = value;
            }
        }
        public bool joinMovimento
        {
            get { return _joinMovimento; }
            set
            {
                _joinMovimento = value;
                joinEsercizio = value;
            }
        }
        public bool joinServizio;
        public bool joinGIS
        {
            get { return _joinGIS; }
            set
            {
                _joinGIS = value;
                joinImpianto = value;
            }
        }
        public bool joinGISAnomalie
        {
            get { return _joinGISAnomalie; }
            set
            {
                _joinGISAnomalie = value;
                joinGIS = value;
            }
        }
        public bool joinRiparto
        {
            get { return _joinRiparto; }
            set
            {
                _joinRiparto = value;
                joinAppezzamento = value;
            }
        }

        public bool joinImpreseReferenti;
        public bool joinContributiACA;

        /// <summary>
        /// Abilita il LEFT JOIN su GerarchiaImprese per ottenere Foglia, Livello e Padre per ogni azienda.
        /// </summary>
        public bool joinGerarchiaBase;
        public bool CaricaDatiGerarchia
        {
            get { return _caricaDatiGerarchia; }
            set
            {
                _caricaDatiGerarchia = value;
                joinGerarchiaBase = value;
            }
        }

        public bool joinOperazionixAzienda;
        public bool joinOperazionixCentroAziendale;
        public bool joinOperazionixCampo;
        public bool joinOperazionixAppezzamento;
        public bool joinOperazionixImpianto;
        public bool joinOperazionixEsercizio;

        public bool joinBudget;

        private bool _CentriAziendaliBase = false;
        private bool _CampiBase = false;
        private bool _AppezzamentiBase = false;
        private bool _ImpiantiBase = false;
        private bool _EserciziBase = false;
        private bool _FabbricatiBase = false;
        private bool _MovimentiBase = false;

        public bool AziendeBase { get; set; }
        public bool CentriAziendaliBase
        {
            get { return _CentriAziendaliBase; }
            set
            {
                _CentriAziendaliBase = value;
                needColumnsCentriAziendali = value;
                joinCentro = value;
            }
        }
        public bool CampiBase
        {
            get { return _CampiBase; }
            set
            {
                _CampiBase = value;
                CentriAziendaliBase = value;
                needColumnsCampi = value;
                joinCampo = value;
            }
        }
        public bool AppezzamentiBase
        {
            get { return _AppezzamentiBase; }
            set
            {
                _AppezzamentiBase = value;
                CampiBase = value;
                needColumnsAppezzamenti = value;
                joinAppezzamento = value;
            }
        }
        public bool ImpiantiBase
        {
            get { return _ImpiantiBase; }
            set
            {
                _ImpiantiBase = value;
                AppezzamentiBase = value;
                needColumnsImpianti = value;
                joinImpianto = value;
            }
        }
        public bool EserciziBase
        {
            get { return _EserciziBase; }
            set
            {
                _EserciziBase = value;
                ImpiantiBase = value;
                needColumnsEsercizi = value;
                joinEsercizio = value;
            }
        }
        public bool FabbricatiBase
        {
            get { return _FabbricatiBase; }
            set
            {
                _FabbricatiBase = value;
                CentriAziendaliBase = value;
                needColumnsFabbricati = value;
                joinFabbricato = value;
            }
        }
        public bool MovimentiBase
        {
            get { return _MovimentiBase; }
            set
            {
                _MovimentiBase = value;
                EserciziBase = value;
                joinMovimento = value;
            }
        }

        public bool CaricaInfoTecnicoImpresa;

        public int IncludiEscludiPoligoni = -1;
        public int IncludiEscludiRiparto = -1;
        public int IncludiEscludiDestinazioniUso = -1;
        public int IncludiEscludiOperazioniAgenda = -1;

        public bool ApplicaFiltroImpreseReferenti = false;
        public bool ApplicaFiltroServizi = false;

        public bool ApplicaFiltroSpecie = false;
        public bool ApplicaFiltroOperazione = false;
        public bool ApplicaFiltroContributiACA = false;
        #endregion

        #region "DETTAGLI"
        public bool AziendeDettagli;
        public List<CodiceAnagrafeBase> CodiciAzienda = new();
        public bool CentriAziendaliDettagli;
        public List<CodiceAnagrafeBase> CodiciCentri = new();
        public bool CampiDettagli;
        public List<CodiceAnagrafeBase> CodiciCampi = new();
        public bool AppezzamentiDettagli;
        public List<CodiceAnagrafeBase> CodiciAppezzamenti = new();
        public bool ImpiantiDettagli;
        public List<CodiceAnagrafeBase> CodiciImpianti = new();
        public bool EserciziDettagli;
        public List<CodiceAnagrafeBase> CodiciEsercizi = new();
        public List<CodiceAnagrafeBase> CodiciPianoColturale = new();
        public bool FabbricatiDettagli;
        public List<CodiceAnagrafeBase> CodiciFabbricati = new();
        #endregion


        public ConfiguratoreFiltroRicerca(Enum_TipoMostra_FiltroRicerca TipoMostra, CaricaDatiAggiuntivi CaricaDati)
        {

            CaricaImpreseReferenti = CaricaDati.ImpreseReferenti;
            CaricaNumeroLibroSociImpresaReferente = CaricaDati.DatiIscrizioneLibroSoci != null ?  CaricaDati.DatiIscrizioneLibroSoci.NumeroIscrizione: false;
            CaricaDataIscrizioneLibroSociImpresaReferente = CaricaDati.DatiIscrizioneLibroSoci != null ? CaricaDati.DatiIscrizioneLibroSoci.DataIscrizione : false;
            
            CaricaLegaleRappresentante = CaricaDati.LegaleRappresentante;

            CaricaIndirizzoAzienda = CaricaDati.IndirizzoAzienda;
            CaricaIndirizzoCentroAziendale = CaricaDati.IndirizzoCentroAziendale;
            CaricaIndirizziAppezzamento = CaricaDati.IndirizziPianoColturale;

            CaricaDatiCatastaliAppezzamento = CaricaDati.CatastoAppezzamento;

            CaricaDatiCatastaliCentroAziendale = CaricaDati.CatastoCentroAziendale;

            CaricaDatiCatastaliCampo = CaricaDati.CatastoCampo;

            CaricaDatiGISImpianto = CaricaDati.GISImpianto;

            CaricaContributiACA = CaricaDati.ContributiACA;

            CaricaDatiServizi = CaricaDati.Servizi;

            CaricaDatiGerarchia = CaricaDati.DatiGerarchia;

            //Imprese base sempre true! tutte le query partono dalla tabella Imprese
            AziendeBase = true;

            switch (TipoMostra)
            {
                case Enum_TipoMostra_FiltroRicerca.Aziende:
                    isMostraAziende = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.CentriAziendali:
                    isMostraCentriAziendali = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Campi:
                    isMostraCampi = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Appezzamenti:
                    isMostraAppezzamenti = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Impianti:
                    isMostraImpianti = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Fabbricati:
                    isMostraFabbricati = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Movimenti:
                    isMostraMovimenti = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Esercizi:
                    isMostraEsercizi = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.PianoColturale:
                case Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget:
                    isMostraPianoColturale = true;
                    break;
            }


            AziendeDettagli = CaricaDati.CodiciAzienda.Count > 0;
            CodiciAzienda = CaricaDati.CodiciAzienda;

            CentriAziendaliDettagli = CaricaDati.CodiciCentroAziendale.Count > 0;
            CodiciCentri = CaricaDati.CodiciCentroAziendale;

            CampiDettagli = CaricaDati.CodiciCampo.Count > 0;
            CodiciCampi = CaricaDati.CodiciCampo;

            AppezzamentiDettagli = CaricaDati.CodiciAppezzamento.Count > 0;
            CodiciAppezzamenti = CaricaDati.CodiciAppezzamento;

            ImpiantiDettagli = CaricaDati.CodiciImpianto.Count > 0;
            CodiciImpianti = CaricaDati.CodiciImpianto;

            EserciziDettagli = CaricaDati.CodiciEsercizio.Count > 0;
            CodiciEsercizi = CaricaDati.CodiciEsercizio;

            FabbricatiDettagli = CaricaDati.CodiciFabbricato.Count > 0;
            CodiciFabbricati = CaricaDati.CodiciFabbricato;
        }
    }
}
