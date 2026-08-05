using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.documenti;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ParcoMacchine
    {
        public string partitaIva { get; set; }
        public CentroAziendale.PK centroPK { get; set; }

        public Contatto contatto { get; set; }

        public string descrizione { get; set; }
        public DittaMacchina marca { get; set; }
        public string modello { get; set; }
        public FinalitaMacchina finalita { get; set; }

        /// <summary>
        /// class_cod su database, arriva da tabella macchine
        /// </summary>
        public Macchine tipo { get; set; }

        public MacchineDettaglio1 dettaglio_1 { get; set; }
        public MacchineDettaglio2 dettaglio_2 { get; set; }

        public int codice { get; set; }

        public int codiceOrigine { get; set; }

        public string codice_stringa { get; set; }

        public bool visibilitaPubblica { get; set; }
        public bool? visibileControlloGestione { get; set; }

        public IntervalloTemporale validita { get; set; }

        public DateTime Data_Carico { get; set; }
        public DateTime Data_Scarico { get; set; }

        public string Username_Creazione { get; set; }
        public string Username_Modifica { get; set; }

        public TitoloDiPossesso titolo_Possesso { get; set; }
        public string proprietario { get; set; }
        public string CUAA_Proprietario { get; set; }

        /// Dati Tecnici
        public string targa { get; set; }

        public TipoTarga tipo_Targa { get; set; }

        public string telaio { get; set; }

        public string n_Immatricolazione { get; set; }
        public DateTime data_Immatricolazione { get; set; }
        public string n_Immatricolazione_Rimorchio { get; set; }
        public string N_Autorizzazione_Trasporto { get; set; }
        public DateTime data_Rilascio_Autorizzazione { get; set; }

        public DateTime data_Ultima_Revisione { get; set; }
        public DateTime data_Ultima_Manutenzione { get; set; }
        public Carburante alimentazione { get; set; }
        public AgronicaCoreModelsSTD.Utility.Immagine immagineGrande { get; set; }
        public AgronicaCoreModelsSTD.Utility.Immagine immaginePiccola { get; set; }

        public String VIN { get; set; }
        public String BTM_Serial { get; set; }

        public String ExternalAPIKey { get; set; }

        public BaseCodeDescr HubIoT_PlatformDestination { get; set; }

        public MacchinaGerarchia gerarchiaPadre { get; set; }

        public List<MacchinaGerarchia> gerarchiaFigli { get; set; }

        // 'Taratura Ugello
        public decimal taratura_Ugello { get; set; }
        public string numero_certificato { get; set; }
        public DateTime data_Ultima_Taratura { get; set; }
        public DateTime scadenza_Taratura { get; set; }

        public string stato_Utilizzo { get; set; }
        public string potenza { get; set; }
        public UnitaDiMisura unita_Misura { get; set; }
        public string note { get; set; }
        public bool flag_cancellazione { get; set; }

        public int portata { get; set; }
        public double efficienza { get; set; }
        public int codice_impianto { get; set; }

        public MacchineCodificaAgea ageaCod { get; set; }

        public ParcoMacchine()
        {
            flag_cancellazione = false;
            Documenti = new List<DocumentoPerMacchina>();
        }

        public List<ParcoMacchineCaratteristiche> caratteristiche { get; set; }

        public List<CostoUnitario> costi { get; set; }

        public List<RateoTempo> rateiTempo { get; set; }

        public string Distinta_Installazione { get; set; }

        public string Contratto_Installazione { get; set; }

        public string Tipologia_Installazione { get; set; }
        public DateTime? Data_Inizio_Installazione { get; set; }
        public DateTime? Data_Fine_Installazione { get; set; }
        public string Stato_Installazione { get; set; }
        public string Provincia_Istat_Installazione { get; set; }
        public string Comune_Istat_Installazione { get; set; }
        public string Indirizzo_Installazione { get; set; }
        public double Latitudine_Installazione { get; set; }
        public double Longitudine_Installazione { get; set; }

        //documenti Allegati valorizzati solo da APP
        public List<DocumentoPerMacchina> Documenti { get; set; }

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

    public class MacchinaGerarchia
    {
        public int ID { get; set; }
        public ParcoMacchine macchina { get; set; }
        public BaseCodeDescr legame { get; set; }
        public String desclegame { get; set; }
        public UnitaDiMisura udm { get; set; }
        public int qta { get; set; }
        public DateTime validita_inizio { get; set; }
        public DateTime validita_fine { get; set; }
    }

    public class LinkedMachine<T> : ParcoMacchine
    {
        public IntervalloTemporale linkValidity { get; set; }
        public T linkedItemPK { get; set; }

        public LinkedMachine() : base()
        {

        }
    }
}
