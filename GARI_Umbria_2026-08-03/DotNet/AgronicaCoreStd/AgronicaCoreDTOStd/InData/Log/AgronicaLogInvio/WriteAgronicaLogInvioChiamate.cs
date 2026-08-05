using System;

namespace InData.Log.AgronicaLogInvio
{
    public class WriteAgronicaLogInvioChiamate
    {

        public int ID { get; set; }

        public int? Tipo_Esportazione { get; set; }

        public string Dati_Inviati { get; set; }

        public DateTime? Data_Invio { get; set; }

        public string Esito { get; set; }

        public string Dati_Ricevuti { get; set; }

        public int? Controllata { get; set; }

        public string Tipo_Operazione { get; set; }

        public DateTime? Validita_Inizio { get; set; }

        public DateTime? Validita_Fine { get; set; }

        public string Dettaglio1 { get; set; }

        public string Dettaglio2 { get; set; }

        public string Dettaglio3 { get; set; }

        public WriteAgronicaLogInvioChiamate(int _ID = 0)
        {
            ID = _ID;
            Tipo_Esportazione = null;
            Dati_Inviati = "";
            Data_Invio = null;
            Esito = "";
            Dati_Ricevuti = "";
            Controllata = null;
            Tipo_Operazione = "";
            Validita_Inizio = null;
            Validita_Fine = null;
            Dettaglio1 = "";
            Dettaglio2 = "";
            Dettaglio3 = "";
        }
    }
}
