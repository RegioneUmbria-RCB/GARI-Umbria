using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Contatti : APP_Agronica_Entity
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public string Cod_Contatto { get; set; }
        public int Cod_RisUm { get; set; }
        public int Cod_Rapporto { get; set; }
        public string Rapporto_Des { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public int id_cf { get; set; }
        public string rag_soc { get; set; }
        public string NrBadge { get; set; }
        public string DatiPatentino { get; set; }
        public DateTime? Data_Nascita { get; set; }
        public string Sesso { get; set; }

        public override string ToString()
        {
            return Cognome + " " + Nome;
        }

    }
}
