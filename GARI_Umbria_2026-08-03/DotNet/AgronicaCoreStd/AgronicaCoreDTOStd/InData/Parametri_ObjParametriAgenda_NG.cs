using System;
using System.Collections.Generic;
using System.Text;

namespace InData
{

    /// <summary>
    /// Tenere aggiornato anche ParametriVariSiti.Parametri_ObjParametriAgenda_NG
    /// I Tipi Enumerativi devono essere indicati come int, perchè i tipi enumerativi non vengono serializzati correttamente lato client
    /// </summary>
    public class Parametri_ObjParametriAgenda_NG
    {
        public int? TipoOperazioneDB;
        public string Piva ;
        public long? Sa_Cod ;
        public long? Fabbricato;
        public long? Campo_Cod ;
        public long? Appezza ;
        public long? Id_Reg ;
        public long? Progetto_Cod ;
        public DateTime Validita_Inizio ;
        public DateTime Validita_Fine;
        public int? Veg_Cod ;
        public string Veg_Des;
        public int? Id_Cod ;
        public string Id_Des;
        public int? Cau_Mov ;
        public int? Mac_Cod ;
        public string Lav_Des ;
        public int? Lav_Cod ;
        public string SaNome ;
        public string RagSoc ;
        public int? Pagina_Provenienza ;
        public int? Pagina_Provenienza_AltroSito;
        public int? Pagina_Richiesta ;
        public DateTime Data ;
        public string Cod_Contatto ;
        public string QueryStringFiltrino ;
        public List<Object> Impianti;
        public int Id_Agenda ;
        public int TipoOperazioneAgenda ;
        public int TipoRicetta ;
        public int Stato ;
        public int Ricetta_Operazione_Cod;
        public int Ricetta_Cod;
        public string GenericObj_string;
        public int TargetOperazione;
        public int Programmazione_Cod;
        public string RedirectUrl;
        public int IdSezione;
        public string Chiave;
        public int Sito_Provenienza;
        public int Regolamento_Cod;
        public int Tipo_Regolamento;
    }
}
