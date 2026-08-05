using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    /// <summary>
    /// Classe contenente le colonne di output per la ricerca delle statistiche del quaderno per la dashboard
    /// </summary>
    public class AttivitaStatistiche
    {
        public string Piva { get; set; }

        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Id_Mov { get; set; }

        public int Id_Mov_Det { get; set; }

        public int Lav_Cod { get; set; }

        public String Des_Lib { get; set; }

        public DateTime Data_Movimento { get; set; }

        public DateTime Ora { get; set; }

        public String Mov_Desc { get; set; }

        public String Username_Creazione { get; set; }

        public String Cau_Mov { get; set; }

        public int Cul_Cod { get; set; }

        public int Veg_Cod { get; set; }

        public String Veg_Des { get; set; }

        public int Raccoglitore_Cod { get; set; }

        public String Tecnico { get; set; }

        public int Tipo_Destinazione { get; set; }

        public int Appezza { get; set; }

        public String App_Nome { get; set; }

        public int Elem_Cod { get; set; }

        public int Mat_Cod { get; set; }

        public int Pro_Cod { get; set; }

        public String TipoProdotto { get; set; }

        public String NomeProdotto { get; set; }

        public String Cod_Articolo { get; set; }

        public String sa_nome { get; set; }

        public String rag_soc { get; set; }

        public String lav_des { get; set; }

        public String gru_des { get; set; }

        public String tipo { get; set; }

        public String cul_des { get; set; }

        public String campo_des { get; set; }

        public int IdImpianto { get; set; }

        public float SupApp { get; set; }

        public float QtaImp { get; set; }

        public float SupTrattata { get; set; }

        public String LottoImpianto { get; set; }

        public int DestinazioneTerreniNudi_Cod { get; set; }

        public String DestinazioneTerreniNudi_Des { get; set; }

        public DateTime Data_Ultima_Modifica_Intervento { get; set; }

        public DateTime validita_inizio_destinazione { get; set; }

        public int Qta_Extra_Totale { get; set; }

        public int QtaProd { get; set; }

        public float QTA_EXTRA { get; set; }

        public String UdmProd { get; set; }

        public String UdmProdSim { get; set; }

        public String UdmImp { get; set; }

        public String UdmImpSim { get; set; }

        public String UdmExtra { get; set; }

        public String UdmExtraSim{ get; set; }

        public int num_impianti { get; set; }

        public int num_operazioni { get; set; }

        public int num_poligoni { get; set; }

        public int num_poligoni_mancanti { get; set; }

        public String Utente_Creazione_Op { get; set; }

        public DateTime Data_Creazione_Op { get; set; } 

        public int Anno_Creazione_Op { get; set; }

        public int Mese_Creazione_Op { get; set; }

        public String Utente_Creazione_Imp { get; set; }

        public DateTime Data_Creazione_Imp { get; set; }

        public int Anno_Creazione_Imp { get; set; }

        public int Mese_Creazione_Imp { get; set; }

        public String Utente_Creazione_Azienda { get; set; }

        public DateTime Data_Creazione_Azienda { get; set; }

        public int Anno_Creazione_Azienda { get; set; }

        public int Mese_Creazione_Azienda { get; set; }

        /// <summary>
        /// La ragione sociale della prima azienda padre dalla gerarchia
        /// </summary>
        public string Azienda_Padre { get; set; }

        /// <summary>
        /// La piva della prima azienda padre dalla gerarchia
        /// </summary>
        public string Piva_Padre { get; set; }

        /// <summary>
        /// Suddivisione territoriale dell'appezzamento - Nazione
        /// </summary>
        public string Stato { get; set; }

        /// <summary>
        /// Suddivisione territoriale dell'appezzamento - Contea
        /// </summary>
        public string Regione { get; set; }

        /// <summary>
        /// Suddivisione territoriale dell'appezzamento - Sottocontea
        /// </summary>
        public string Provincia { get; set; }

        /// <summary>
        /// Suddivisione territoriale dell'appezzamento - Distretto
        /// </summary>
        public string Localita { get; set; }

        /// <summary>
        /// L'anno di esecuzione
        /// </summary>
        public int Anno_movimento { get; set; }

        /// <summary>
        /// Il mese di esecuzione
        /// </summary>
        public int Mese_movimento { get; set; }

        /// <summary>
        /// Data di creazione dell'operazione in agenda
        /// </summary>
        public DateTime Data_creazione { get; set; }

        //public int ghg { get; set; }

    }
}
