using AgronicaCoreModelsSTD.analisi;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.Gis;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{

    public class RifAppezzamento : RifCentroAziendale
    {
        public int appezza { get; set; }

        // Costruttore vuoto per consentire deserializzazioni della classe
        public RifAppezzamento() : base()
        {
            appezza = 0;
        }

        public RifAppezzamento(string _piva, int _saCod, int _appezza) : base(_piva, _saCod)
        {
            appezza = _appezza;
        }

        public String getAppezzamentoFullId(string separator = "_")
        {
            return $"{getCentroAziendaleFullId(separator)}{separator}{appezza}";
        }
    }
    
    public class Appezzamento 
    {
        // public int Id { get; set; }
        // public int codice { get; set; }
        // public CentroAziendale centroAziendale { get; set; }
        public PK primaryKey { get; set; }
        public string descrizione { get; set; }
        
        public List<Impianto> impianti { get; set; }

        public Campo.PK campoPK { get; set; }

        public List<CatastoAppezzamento> catastoAppezzamento { get; set; }

        public List<CodiciAnagrafeValori> codici { get; set; }

        /// <summary>
        /// dato cartografico associato
        /// </summary>
        public string cartografia { get; set; }

        /// <summary>
        /// flag identificativo se l'appezzamento è disegnato tramite tramite il gps
        /// </summary>
        public bool flag_gps { get; set; }

        /// <summary>
        /// se lasciato null si suppone che il dato espresso nella proprietà cartografia è sullo SRID 4326
        /// </summary>
        public SistemiRiferimentoCartografia sistemaRiferimentoCartografia { get; set; }

        /// <summary>
        /// contiene un anteprima in formato PNG in una stringa Base64
        /// </summary>
        public string immagineBase64 { get; set; }

        // Dati Generali
        public double superficie { get; set; }
        public double superficieGis { get; set; }

        public string rif_Appezzamento { get; set; }
        public string isola { get; set; }

        public bool terrenoInutilizzato { get; set; }
        public bool terrenoDegradato { get; set; }
        public bool lowILUC { get; set; }

        public IntervalloTemporale validita { get; set; }

        /// <summary>
        /// Codice riconosciuto e condiviso anche con altri sistemi
        /// </summary>
        public MetodoProduzione metodo_Produzione { get; set; }

        // Rotazioni Colturali
        public Specie coltura_Precedente_1_Anno { get; set; }
        public Specie coltura_Precedente_2_Anno { get; set; }
        public Specie coltura_Precedente_3_Anno { get; set; }
        public Specie coltura_Precedente_4_Anno { get; set; }

        // Posizione Appezzamento
        public double pendenza { get; set; }
        public BaseCodeDescrStr esposizione { get; set; }
        public BaseCodeDescrStr ubicazione { get; set; }

        // Coordinate Baricentro
        public double lat { get; set; }
        public double lng { get; set; }
        public double altitudine { get; set; }

        // Buffer Zone
        /// <summary>
        /// Creare Classe per buffer
        /// </summary>
        public double distBZ_CorpiIdrici { get; set; }
        public double distBZ_AreeResPub { get; set; }
        public double distBZ_Allevamenti { get; set; }
        public double distBZ_VegNatNonColt { get; set; }
        public double supBZ_Riduzione { get; set; }

        // Biologico
        public string n_App_Bio { get; set; }
        public List<BaseCodeDescr> utilizzo_Terreno { get; set; }
        public string confini_A_Rischio { get; set; }
        public DateTime fine_Impiego_Prod_Non_Conformi { get; set; }

        public bool flag_cancellazione { get; set; }
        public DatiSementieri dati_sementieri { get; set; }

        // passato dall'app per evitare duplicati
        public string guid { get; set; }

        public List<IndirizzoAssociato> indirizzi { get; set; }

        public GisDataReadRval_New<GeoJSONAgroGisProp> obj_app { get; set; }
        public GeoJSONAgroGisPropTreeNode nodeInfo_app { get; set; }

        #region "Chiavi tracciato Agea"
        public string Agea_idSchedaValidazione { get; set; }
        public string Agea_identificativoPianoColtivazione { get; set; }
        //public string Agea_idParcella { get; set; }
        public string Agea_codiBarrScheVali { get; set; }
        public string Agea_identificativoIsola { get; set; }
        public string Agea_identificativoAppezzamento { get; set; }
        public string Agea_idAppezzamentoOrig { get; set; }
        #endregion

        public BlockAppezzamento blkAppezzamento { get; set; }

        public List<LinkedMachine<Appezzamento.PK>> linkedMachines { get; set; }
        
        public decimal? sabbia { get; set; }
        public decimal? limo { get; set; }
        public decimal? argilla { get; set; }
        public ClasseTessitura classeTessitura { get; set; }

        public Appezzamento()
        {
            flag_cancellazione = false;
        }

        public Appezzamento(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            this.flag_cancellazione = false;
        }

        public RifAppezzamento getRiferimento()
        {
            return new RifAppezzamento (
                this.primaryKey.centroAziendalePK.partitaIva,
                this.primaryKey.centroAziendalePK.codice,
                this.primaryKey.codice
            );
        }

        public class DatiSementieri
        {
            public string Sementi { get; set; }
            public string SementiMappaturaLibera { get; set; }
            public int Entita_Cod { get; set; }
            public string DatiPassaggio { get; set; }
        }

        public class BlockAppezzamento
        {
            public bool blkFlag { get; set; }
            public DateTime blkInizioData { get; set; }
            public string blkInizioUsername { get; set; }
            public string blkInizioNote { get; set; }
            public DateTime blkFineData { get; set; }
            public string blkFineUsername { get; set; }
            public string blkFineNote { get; set; }
        }

        public class PK
        {
            public int codice { get; set; }
            public CentroAziendale.PK centroAziendalePK { get; set; }

            public PK(int codice, CentroAziendale.PK centroAziendalePK)
            {
                this.codice = codice;
                this.centroAziendalePK = centroAziendalePK;
            }

            public PK(int codice, int saCod, string piva)
            {
                this.codice = codice;
                this.centroAziendalePK = new CentroAziendale.PK(saCod, piva);
            }

            public PK()
            {

            }

        }
    }

    public class AppezzamentoJoinDescrizioni : Appezzamento 
    {
        public string saNome { get; set; }

        public string ragSoc { get; set; }

        public AppezzamentoJoinDescrizioni(PK primaryKey) : base(primaryKey) { }
    }
}
