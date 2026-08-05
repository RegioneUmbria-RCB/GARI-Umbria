using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class RifEsercizio : RifImpianto
    {
        public int progettoCod { get; set; }

        // Costruttore vuoto per consentire deserializzazioni della classe
        public RifEsercizio() : base()
        {
            progettoCod = 0;
        }

        public RifEsercizio(string _piva, int _saCod, int _appezza, int _idReg, int _progettoCod) : base(_piva, _saCod, _appezza, _idReg)
        {
            progettoCod = _progettoCod;
        }

        public RifEsercizio(string fullId, char separator = '_')
        {
            string[] strings = fullId.Split(separator);
            if (strings.Length != 5)
                throw new Exception("Errore nella deserializzazione di RifEsercizio");
            partitaIva = strings[0];
            saCod = int.Parse(strings[1]);
            appezza = int.Parse(strings[2]);
            idReg = int.Parse(strings[3]);
            progettoCod = int.Parse(strings[4]);
        }

        public String getEsercizioFullId(string separator = "_")
        {
            return $"{getImpiantoFullId(separator)}{separator}{progettoCod}";
        }
    }

    public class Esercizio : BaseCodeDescr
    {
        public IntervalloTemporale validita { get; set; }
        public Impianto.PK impiantoPK { get; set; }

        public string lotto { get; set; }        

        /// <summary>
        /// piante ha/impianto uno dei due è calcolato, l'altro è memorizzato, modificare di conseguenza
        /// </summary>
        public double piante_Ha { get; set; }
        public double piante_Impianto { get; set; }
        
        public int piante_Ha_Femmine { get; set; }
        public int Piante_Ha_Impianto_Femmine { get; set; }
        public int Piante_Ha_Maschi { get; set; }
        public int Piante_Ha_Impianto_Maschi { get; set; }

        public DateTime data_Semina_Trapianto_Prevista { get; set; }
        public DateTime data_Raccolta_Prevista { get; set; }
        public DateTime data_Fioritura_Prevista { get; set; }
        public double resa_prevista { get; set; }
        public double resa_effettiva { get; set; }              //campo calcolato

        public double superficie { get; set; }


        public Regolamenti regolamento { get; set; }
        public Disciplinare disciplinare { get; set; }

        public int id_tr { get; set; }  

        public List<ImpegniAggiuntiviFacoltativi> iaf { get; set; } = new List<ImpegniAggiuntiviFacoltativi>();

        public BaseCodeDescrStr capitolato_Privato { get; set; }

        public List<BaseCodeDescrStr> contributi { get; set; }
        public List<BaseCodeDescrStr> tecnico { get; set; }

        /// <summary>
        /// p.iva organismo referente
        /// </summary>
        public Contatto organismo_Referente { get; set; }

        public BaseCodeDescrStr modalita_liquidazione { get; set; }

        public BaseCodeDescrStr origine_prodotto { get; set; }

        public BaseCodeDescrStr residuo { get; set; }

        public List<BaseCodeDescr> certificazioneAziendale { get; set; }

        public BaseCodeDescrStr certificazioneProdotto { get; set; }

        public LicenzaColtivazione  licenza_Coltivazione { get; set; }
        public Contatto riferimento_Trasferimento_Dati { get; set; }


        public Fabbricato magazzino_Conferimento { get; set; }

        public BaseCodeDescrStr piano_Semina { get; set; }

        public bool esercizio_Chiuso { get; set; }
        public bool esercizioReplica { get; set; }
        public string codiceImpiantoRibaltato { get; set; } // contiene la chiave dell'esercizio replicato su altra azienda, se presente
        public string replicaGias { get; set; }

        public bool flagSecondoRaccolto { get; set; }

        public ApportoMacroelementi apportiMassimiMacroelementi { get; set; }

        public Vincolo vincolo { get; set; }

        public List<CatastoEsercizio> catastoEsercizio { get; set; }

        public List<CodiciAnagrafeValori> codici { get; set; }

        public Prodotto prodotto { get; set; }
        public bool flag_cancellazione { get; set; }

        public BaseCodeDescr gruppoRaccolta { get; set; }
        public BaseCodeDescrStr lavorazione { get; set; }
        public BaseCodeDescrStr specifica { get; set; }

        public List<LinkedContribute<KeyValuePair<int, string>>> acaContributes { get; set; }

        public Esercizio()
        {
            this.flag_cancellazione = false;
        }

        public Esercizio(int codice, string descrizione) : base (codice, descrizione)
        {
            this.flag_cancellazione = false;
        }

        public bool isActive()
        {
            return isActive(new DateTime());
        }
        public bool isActive(DateTime referenceDate)
        {
            return validita.isActive(referenceDate);
        }

        public RifEsercizio getRiferimento()
        {
            return new RifEsercizio
            (
                this.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                this.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                this.impiantoPK.appezzamentoPK.codice,
                this.impiantoPK.codice,
                this.codice
            );
        }

        public string GetKey(string separator = "|")
        {
            return impiantoPK.toString(separator) + separator + codice;
        }

        public class PK
        {
            public int codice { get; set; }
            public Impianto.PK impiantoPK { get; set; }

            public PK(int codice, Impianto.PK impiantoPK)
            {
                this.codice = codice;
                this.impiantoPK = impiantoPK;
            }

            public PK(int codice, int idReg,int appezza, int saCod, string piva)
            {
                this.codice = codice;
                this.impiantoPK = new Impianto.PK(idReg, new Appezzamento.PK(appezza, saCod, piva));
            }

            public PK() { }

            public int idReg => impiantoPK.codice;
            public int appezza => impiantoPK.appezzamentoPK.codice;
            public int saCod => impiantoPK.appezzamentoPK.centroAziendalePK.codice;
            public string piva => impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva;

            public string toString(string separator)
            {
                return this.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + separator +
                       this.impiantoPK.appezzamentoPK.centroAziendalePK.codice + separator +
                       this.impiantoPK.appezzamentoPK.codice + separator +
                       this.codice;
            }
        }
    }
}
