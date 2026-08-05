using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe implementata per poter utilizzare AgronicaCoreGestioneRichieste.ParametriAgenda_2010, 
    /// sucessivamente è necessario Serializzarla in stringa e deserializzarla nel tipo AgronicaCoreGestioneRichieste.ParametriAgenda_2010
    /// </summary>
    public class ObjParams_Agenda
    {
        /// <summary>
        /// Assolutamente da lasciare come prima property 
        /// poichè gestisce la sessione nella classe a lvl inferiore
        /// </summary>
        public bool Sessione { get; set; }
        public int PaginaRichiesta { get; set; }
        public int PaginaProvenienza { get; set; }
        public string QueryString_Filtrino { get; set; }
        public int PaginaDestinzazione_Filtrino { get; set; }
        public int SitoDestinazione_Filtrino { get; set; }
        public string Piva { get; set; }

        public int Sa_Cod { get; set; }
        public int Campo_Cod { get; set; }
        public DateTime Data_Selezionata { get; set; }
        public int Veg_Cod { get; set; }
        public int Cul_Cod { get; set; }
        public int Id_Agenda { get; set; }
        public int Operazione { get; set; }
        public int Lavorazione { get; set; }
        public int Appezza { get; set; }
        public int Id_Reg { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
        public string Fabbricato { get; set; }
        public string OperazioneMagazzino { get; set; }
        public int Id_Cod { get; set; }

        public string Chiave { get; set; }
        public string Mode { get; set; }

        public string strVariabili_OpAgenda { get; set; }
        public int Cod_RisUm { get; set; }
        public string Cod_Contatto { get; set; }
        public int Lav_Cod { get; set; }
        public string GenericObj_String { get; set; }

        public int TargetOperazione { get; set; }
        public int Programmazione_Cod { get; set; }
        public int Tipo_Operazione { get; set; }
        public int TipoOperazioneAgenda { get; set; }

        public List<Impianto2010> Impianti { get; set; }
    }
}
