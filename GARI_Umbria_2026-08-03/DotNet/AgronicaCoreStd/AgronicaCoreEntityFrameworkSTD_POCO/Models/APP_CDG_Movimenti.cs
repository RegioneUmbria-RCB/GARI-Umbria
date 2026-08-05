using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_CDG_Movimenti : APP_Agronica_Entity
    {
        // public string Piva_Superuser { get; set; }
        public string Piva { get; set; }
        public int Id_CDG_Movimenti { get; set; }
        public int Id_Cdg_Generale { get; set; }
        public DateTime Data_Ora_Inizio { get; set; }
        public DateTime Data_Ora_Fine { get; set; }
        public decimal Qta { get; set; }
        public int Mac_Cod { get; set; }
        public int Cod_RisUm { get; set; }
        public string NrBadge { get; set; }
        public int Id_Attivita { get; set; }
        public int Id_Imputazione { get; set; }
        public int sa_cod { get; set; }
        public int appezza { get; set; }
        public int id_reg { get; set; }
        public int progetto_cod { get; set; }
    }
}
