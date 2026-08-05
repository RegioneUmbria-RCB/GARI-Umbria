using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.note_intervento;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.contabilita;
using AgronicaCoreModelsSTD.documenti;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita
{


    public class RifAttivita : RifCentroAziendale
    {
        public int agenda { get; set; }

        // Costruttore vuoto per consentire deserializzazioni della classe
        public RifAttivita() : base("", 0)
        {
            agenda = 0;
        }

        public RifAttivita(string _piva, int _saCod, int _agenda) : base(_piva, _saCod)
        {
            agenda = _agenda;
        }

        public String getAgendaId(string separator = "_")
        {
            return $"{getCentroAziendaleFullId()}{separator}{agenda}";
        }
    }


    /// <summary>
    /// Attivita corrisponde ad un record della tabella Agenda
    /// </summary>
    public class Attivita
    {
        /// <summary>
        /// Agenda.Id_Agenda o Ricette_Operazioni.Ricetta_Operazione_Cod
        /// </summary>
        public string codice { get; set; }       
        

        /// <summary>
        /// non usare per comporre il des_lib
        /// </summary>
        public string descrizione { get; set; }

        /// <summary>
        /// Memorizzata due volte su db: Agenda.Validita_Inizio e nei record Movimenti.Data_Movimento
        /// </summary>
        public DateTime inizio { get; set; }

        /// <summary>
        /// Agenda.Validita_Fine su db (al momento non è gestita in AgronicaCoreModello) 'TODO: approfondire con Vanni
        /// </summary>
        public DateTime fine { get; set; }

        public DateTime oraInizio { get; set; }

        public DateTime oraFine { get; set; }

        public CentroAziendale centroAziendale { get; set; }

        public int fabbricatoCod { get; set; }

        public List<CentroDiCosto> centriDiCosto { get; set; }

        public List<Risorsa> risorse { get; set; }

        public List<Documento> documenti { get; set; }

        /// <summary>
        /// Disciplinare di produzione che regola l'attività
        /// </summary>
        public Disciplinare disciplinare { get; set; }

        public UtilizzoTerreno utilizzoTerreno { get; set; }

        public RegistrazioneContabile registrazioneContabile { get; set; }

        public int raccoglitore { get; set; }

        /// <summary>
        /// Epoca in cui rientra l'attività (Movimenti.extra_int per trattamento e fertilizzazione)
        /// </summary>
        public Epoca epoca { get; set; }

        /// <summary>
        /// Movimenti.extra_int per raccolta
        /// </summary>
        public Tipo_Raccolta tipoRaccolta { get; set; }

        public List<NoteIntervento> noteIntervento { get; set; }

        /// <summary>
        /// Lavorazione (campagna), economica (Controllo gestione), ecc...: in Tabella si memorizza il codice del job se di classType lavorazione (oppure fisso 4500) in  Agenda.Lav_Cod
        /// </summary>
        public Job job { get; set; }

        public Tipo_Attivita tipo { get; set; }

        public Tipo_Ricetta tipoRicetta { get; set; }

        public Stati stato { get; set; }

        public string riferimentoPianificata { get; set; }

        public StatiWorkflowQdC statoWorkflow { get; set; }

        public List<Attivita> attivitaCollegate { get; set; }

        /// <summary>
        /// Movimenti.mov_desc
        /// </summary>
        public string note { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public string guid { get; set; }

        public string versione { get; set; }

        /// <summary>Campo usato per indicare l'origine dell'attività quando le attività vengono mandate verso l'app</summary>
        public string origineAttivita { get; set; }

        public bool cancellato { get; set; }

        public bool daRemoto { get; set; }

        public string codicePerVerificaConformita { get; set; }

        /// <summary>
        /// Movimenti.modalita
        /// </summary>
        public int modalita{ get; set; }

        /// <summary>
        /// Agenda.Id_Attivita
        /// </summary>
        public AttivitaPersonalizzata attivitaPersonalizzata { get; set; }


        /// <summary>
        /// Corrisponde alla tabella "Ricette", che è la testata di Ricette_Operazioni (che invece corrispondono concettualmente alla Agenda)
        /// </summary>
        public TestataRicetta testataRicetta { get; set; }

        /// Corrisponde alla tabella "RicettexAgenda"
        public AssociazionePK associazionePK { get; set; }

        /// <summary>
        /// Corrisponde alla campo "Invia_App" di Ricette_Operazioni
        /// </summary>
        public bool inviaRicetta { get; set; }  //DT: usato anche dalla APP

        ///////////////////////////////////////////////////////////////
        //DT: campi usati solo dalla APP in scrittura, NON USARE IN ALTRI CONTESTI
        public string codiceOperazioneRicetta { get; set; } //DT: sostituito da associazionePK.Ricetta_Operazione_Cod, ma mantenuto il vecchio campo per la APP
        ///////////////////////////////////////////////////////////////

        /// <summary>
        /// Corrisponde alla campo "Origine" di Agenda/Ricetta
        /// </summary>
        public string origine { get; set; }


        /// <summary>
        /// Corrisponde alla campo "APP_Ricetta_Operazione_ID" di Ricette_Operazioni
        /// </summary>
        public string appRicettaOperazioneID { get; set; }

        public Blocco blocco { get; set; }

        /// <summary>
        /// Corrisponde al campo Modalita_Applicazione della Movimenti valorizzato solo nel QdC NG
        /// </summary>
        public BaseCodeDescr modalitaApplicazione { get; set; }

        /// <summary>
        /// Da struttura dati "Ricette" indica il tipo della ricetta (vedi enum completo su tipiEnumerativi.Enum_TipoRicetta)
        /// </summary>
        public enum Tipo_Ricetta
        {
            Standard = 0,
            Costi = 1,
            PUA = 2,
            Budget_Globale = 3,
            Budget_Utente = 4,
            Standard_Destinazioni = 5,
            PianoDistribuzioneConcimi = 6,
            ControlloDiGestione = 7,
            Standard_Destinazioni_Planning = 8,
            PianoDistribuzionePua = 9,
            RichiestaUMA = 10,
            Zoo = 11
        }

        public enum Tipo_Attivita
        {
            QuadernoDiCampagna = 1,
            Ricetta = 2
        }

        public enum Tipo_Raccolta
        {
            Fast = 10, //data e impianti
            Leggera = 20, //data, impianti e qta prodotto
            Leggera_Con_Dettagli_Magazzino = 30 //data, impianti, qta prodotto e carico magazzino
        }

        /// <summary>
        /// Stato rispetto al workflow delle ricette
        /// </summary>
        public enum Stati
        {
            Da_Eseguire = 300,
            Eseguita = 301
        }

        public enum StatiWorkflowQdC
        {
            Non_Definito = 0,
            Da_Eseguire = 400,
            Eseguito = 401
        }

        public Attivita()
        {
            centriDiCosto = new List<CentroDiCosto>();
            risorse = new List<Risorsa>();
            note = "";
            testataRicetta = new TestataRicetta();
            origine = "";
            blocco = new Blocco();
        }

        public Attivita Clona()
        {
            try
            {

                string output = JsonConvert.SerializeObject(this);
                Attivita deserializedObject = JsonConvert.DeserializeObject<Attivita>(output);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                throw new Exception("Attivita.Clona: " + ex.Message);
            }

        }
    }

    public class AssociazionePK
    {
        public int id_agenda { get; set; }

        public int Ricetta_Cod { get; set; }

        public int Ricetta_Operazione_Cod { get; set; }

    }
}
