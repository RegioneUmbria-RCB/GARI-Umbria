using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.GiasAPP
{
    public class ConsultaSincroLog
    {
        public String utente { get; set; } = "";
        public String azienda_cod { get; set; } = "";
        public String azienda_des { get; set; } = "";
        public DateTime data_sincro { get; set; }
        public String Dati { get; set; } = "";
        public int tipo_dato { get; set; }
        public String Riferimento { get; set; } = "";
        public String descrizione { get; set; } = "";
        public String tipo_aggiornamento { get; set; } = "";
        public int stato_applicazione { get; set; }
        public String errori { get; set; } = "";

    }
}
