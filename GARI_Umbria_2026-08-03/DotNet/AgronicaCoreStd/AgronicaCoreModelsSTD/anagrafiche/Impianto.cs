using AgronicaCoreModelsSTD.analisi;
using AgronicaCoreModelsSTD.Gis;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.DensitaImpianto;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{

    public class RifImpianto : RifAppezzamento
    {
        public int idReg { get; set; }

        // Costruttore vuoto per consentire deserializzazioni della classe
        public RifImpianto() : base()
        {
            idReg = 0;
        }

        public RifImpianto(string _piva, int _saCod, int _appezza, int _idReg) : base(_piva, _saCod, _appezza)
        {
            idReg = _idReg;
        }

        public String getImpiantoFullId(string separator = "_")
        {
            return $"{getAppezzamentoFullId(separator)}{separator}{idReg}";
        }
    }

    public class Impianto 
    {
        // public int Id { get; set; }
        // public int codice { get; set; }
        // public Appezzamento appezzamento { get; set; }
        public PK primaryKey { get; set; }
        public string descrizione { get; set; }        
        public UtilizzoTerreno utilizzoTerreno { get; set; }

        public double superficie { get; set; }
        public double superficieGis { get; set; }

        /// <summary>
        /// valore superficie in unita di misura alternativa
        /// </summary>
        public double superficieAlternativa { get; set; }

        /// <summary>
        /// unita di misura alternativa a ettari legato a superficie alternativa
        /// </summary>
        public UnitaDiMisura_Alternativa unitaMisuraAlternativa { get; set; }

        public GruppoFinalita gruppoFinalita { get; set; }

        public GruppoVarietale gruppoVarietale { get; set; }

        public DateTime data_Creazione { get; set; }

        public DateTime data_Innesto_Varieta { get; set; }
        public DateTime data_Inizio_Produzione { get; set; }

        public DateTime data_Inizio_Portinnesto { get; set; }

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

        // 
        // Dati Accessori
        // 


        /// <summary>
        /// Codice riconosciuto e condiviso anche con altri sistemi
        /// </summary>
        public string codiceImpianto { get; set; }
        public string algoritmoCodifica { get; set; }

        // Sez Densita Impianto
        public bool cover_Crops { get; set; }
        public bool monitorato { get; set; }
        
        public FormaAllevamento formaAllevamento { get; set; }
        public Portinnesto portinnesto { get; set; }
        
        public SeminaTrapianto seminaTrapianto { get; set; }

        public ProvenienzaSeme provenienzaSeme { get; set; }

        public TecnicaConduzioneTraFila tecnicaConduzioneTraFila { get; set; }
        public TecnicaConduzioneSuFila tecnicaConduzioneSuFila { get; set; }

        public Irrigazione irrigazione { get; set; }
        public List<ParcoMacchine> macchineIrrigazione { get; set; }

        /// <summary>
        /// approfondire come gestire la consociazione
        /// </summary>
        public Impianto.PK consociazionePK { get; set; }

        public bool impiantoConsociato { get; set; }
        public bool maschi_in_Sesto { get; set; }

        public double tra_Fila_M { get; set; }
        public double su_Fila_M { get; set; }


        //copertura
        public Copertura copertura { get; set; }
        public DateTime cop_Data_Inizio { get; set; }
        public DateTime cop_Data_Fine { get; set; }

        public int unita_Vitata { get; set; }


        public bool impianto_Ibrido { get; set; }

        // Linea Maschio
        public string codBMBDBT_M { get; set; }
        public string codBMBDBT_F { get; set; }
        public string genetica_M { get; set; }
        public string genetica_F { get; set; }
        public string offType_M { get; set; }
        public string offType_F { get; set; }
        public double distanzaSuFila_F { get; set; }
        public double distanzaTraFila_F { get; set; }

        // Pannello Tuberi
        public double partiTuberi { get; set; }

        public TagliatoIntero tagliatoIntero { get; set; }

        public double interbina { get; set; }
        public int germinabilita { get; set; }

        public baseClass.BaseCodeDescrStr codiceZona { get; set; }

        public DettaglioVarietaPersonalizzato dettaglio_varieta_personalizzato { get; set; }

        public IntervalloTemporale validita { get; set; }
        public List<Esercizio> esercizi { get; set; }
        public List<CodiciAnagrafeValori> codici { get; set; }
        public bool flag_cancellazione { get; set; }

        public GisDataReadRval_New<GeoJSONAgroGisProp> obj_imp { get; set; }
        public GeoJSONAgroGisPropTreeNode nodeInfo_imp { get; set; }

        //Chiavi tracciato Agea - start
        public string Agea_idColt { get; set; }
        //Chiavi tracciato Agea - end

        public DateTime data_Inizio_Impianto { get; set; }

        //Lavez - 23/06/2025 - per integrazione DataPublish (gestione pubblicazione impianto da Agea)
        public string flagCessata { get; set; }

        public Impianto(PK primaryKey)
        {
            this.primaryKey = primaryKey;
            esercizi = new List<Esercizio>();
            this.flag_cancellazione = false;
            this.flagCessata = "";
        }

        public Impianto()
        {
            flag_cancellazione = false;
            this.flagCessata = "";
        }

        public Esercizio getActiveEsercizio()
        {
            return getActiveEsercizio(new DateTime());
        }

        public Esercizio getActiveEsercizio(DateTime referenceDate)
        {
            foreach (var e in esercizi)
            {
                if (e.isActive(referenceDate)) return e;
            }
            
            return null;
        }

        public RifImpianto getRiferimento()
        {
            return new RifImpianto
            (
                this.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                this.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                this.primaryKey.appezzamentoPK.codice,
                this.primaryKey.codice
            );
        }

        public class PK
        {
            public int codice { get; set; }
            public Appezzamento.PK appezzamentoPK { get; set; }

            public PK(int codice, Appezzamento.PK appezzamentoPK)
            {
                this.codice = codice;
                this.appezzamentoPK = appezzamentoPK;
            }

            public PK(int codice, int appezza, int saCod, string piva)
            {
                this.codice = codice;
                this.appezzamentoPK = new Appezzamento.PK(appezza, saCod, piva);
            }

            public PK() { }

            public int appezza => appezzamentoPK.codice;
            public int saCod => appezzamentoPK.centroAziendalePK.codice;
            public string piva => appezzamentoPK.centroAziendalePK.partitaIva;

            public string toString(string separator)
            {
                return this.appezzamentoPK.centroAziendalePK.partitaIva + separator +
                       this.appezzamentoPK.centroAziendalePK.codice + separator +
                       this.appezzamentoPK.codice + separator +
                       this.codice;
            }
        }
        
        public CentroAziendale.PK getCentroAziendale()
        {
            return primaryKey.appezzamentoPK.centroAziendalePK;
        }
    }
}
