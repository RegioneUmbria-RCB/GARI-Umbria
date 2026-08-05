using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Rappresenta un singolo capo
    /// </summary>
    public class CapoAnimale : CapoAnimaleLight
    {
        #region Properties
        //public string partitaIva { get; set; }
        //public int codice { get; set; }
        //public string matricola { get; set; }

        //public Genere genere { get; set; }
        //public Specie specie { get; set; }
        //public Razza razza { get; set; }

        /// <summary>
        /// (= IPRO_COD)
        /// </summary>
        public IndirizzoProduttivo indirizzoProd { get; set; }

        /// <summary>
        /// Categoria di destinazione (= CAT_COD)
        /// </summary>
        public Categoria categoria { get; set; }

        /// <summary>
        /// Tipologia dell'Animale (= TIPO_COD)
        /// </summary>
        /// <example>Grasso</example>
        public TipologiaCapoAnimale tipologia { get; set; }

        //public string nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string collare { get; set; }

        //public string sesso { get; set; }
        //public bool flagCancellazione { get; set; }

        //public DateTime dataNascita { get; set; }
        //public IntervalloTemporale validita { get; set; }

        /// <summary>
        /// Riferimento alla madre dell'Animale
        /// </summary>
        public CapoAnimale madre{ get; set; }

        /// <summary>
        /// Riferimento al padre dell'Animale
        /// </summary>
        public CapoAnimale padre { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Contatto fornitore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lottoFornitore { get; set; }

        /// <summary>
        /// Corrisponde a enum_MetodoProduzione
        /// 1 = Convenzionale, 2 = InConversione, 3 = Biologico
        /// </summary>
        public MetodoProduzione metodoProduzione { get; set; }

        /// <summary>
        /// Validità conversione
        /// </summary>
        public IntervalloTemporale validitaConversione { get; set; }
        /// <summary>
        /// codiceFiscaleDetentore (corrisponde al codice fiscale del detentore in BDN)
        /// </summary>
        public string codiceFiscaleDetentore { get; set; }
        /// <summary>
        /// codiceFiscaleDetentore (corrisponde al codice fiscale del detentore in BDN)
        /// </summary>
        public string codiceFiscaleProprietario { get; set; }
        /// <summary>
        /// codiceAziendaNascita (corrisponde al codice dell'azienda di nascita in BDN)
        /// </summary>
        public string codiceAziendaNascita { get; set; }
        /// <summary>
        /// codiceAziendaFornitore (corrisponde al codice dell'azienda di provenienza in BDN)
        /// </summary>
        public string codiceAziendaFornitore { get; set; }

        /// <summary>
        /// Capo_Id_BDN (corrisponde al campo CAPO_ID presente in BDN)
        /// </summary>
        public int idCapo_BDN { get; set; }
        /// <summary>
        /// Id univoco in BDN del modello cartaceo di movimento relativo all’ingresso in stalla(modello 4)
        /// </summary>
        public string ingresso_mm_id { get; set; }
        /// <summary>
        /// Id univoco in BDN del modello cartaceo del movimento di uscita
        /// </summary>

        public string ingresso_modello4_numero { get; set; }
        /// <summary>
        /// Id univoco in BDN del modello cartaceo del movimento di uscita
        /// </summary>

        public string ingresso_modello4_prenotazione { get; set; }
        /// <summary>
        /// data di prenotazione del documento modello4 in ingresso
        /// </summary>
        public DateTime  ingresso_modello4_data_prenotazione { get; set; }
        /// <summary>
        /// Id univoco in BDN del modello cartaceo del movimento di uscita
        /// </summary>

        public string uscita_mm_id { get; set; }
        /// <summary>
        /// Certificato (corrisponde al campo NUM_CERTIFICATO in BDN)
        /// </summary>

        public string uscita_modello4_numero { get; set; }
        /// <summary>
        /// Id univoco in BDN del modello cartaceo del movimento di uscita
        /// </summary>

        public string uscita_modello4_prenotazione { get; set; }
        /// <summary>
        /// data di prenotazione del documento modello4 in uscita
        /// </summary>
        public DateTime uscita_modello4_data_prenotazione { get; set; }
        
        /// <summary>
        /// Id univoco in BDN del modello cartaceo del movimento di uscita
        /// </summary>

        public string Codice_Azienda_Uscita { get; set; }
        /// <summary>
        /// Id univoco in BDN del modello cartaceo del movimento di uscita
        /// </summary>

        public string numCertificato { get; set; }

        /// <summary>
        /// Capo validato (in app)
        /// </summary>
        public bool validato { get; set; }

        /// <summary>
        /// Note per anomalie del capo
        /// </summary>
        public string anomalieNote { get; set; }
        
        /// <summary>
        /// Note di scarico
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Stalla di svezzamento del capo
        /// </summary>
        public string stallaSvezzamento { get; set; }

        /// <summary>
        /// Causale morte da Lista_Causali_Morte
        /// </summary>
        public int causaleMorte { get; set; }

        /// <summary>
        /// Lista degli stati di accrescimento dell'Animale
        /// </summary>
        public List<StatoAccrescimento> statiAccrescimento { get; set; }

        //public List<EsercizioCapoAnimale> esercizi { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Costruttore base
        /// </summary>
        public CapoAnimale() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="codProgetto"></param>
        /// <param name="matricola"></param>
        public CapoAnimale(string piva, int codProgetto, string matricola) : base(piva, codProgetto, matricola)
        {

        }
        #endregion

    }
}
