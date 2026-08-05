using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Reg_Impianti : APP_Agronica_Entity
    {
        public string piva { get; set; }
        public int sa_cod { get; set; }
        public int appezza { get; set; }
        public int id_reg { get; set; }
        public string rag_soc { get; set; }
        public string sa_nome { get; set; }
        public string campo_des { get; set; }
        public string app_nome { get; set; }
        public int veg_cod { get; set; }
        public string veg_des { get; set; }
        public int id_cod { get; set; }
        public string codici_anagrafe_des { get; set; }
        public string cul_des { get; set; }
        public string reg_des { get; set; }
        public decimal sup_imp { get; set; }
        public string copertura { get; set; }
        public string imp_des { get; set; }
        public int progetto_cod { get; set; }
        public string progetto { get; set; }
        public DateTime validita_inizio_distinta { get; set; }
        public DateTime validita_fine_distinta { get; set; }
        public string Codici_Anagrafe_Impianto { get; set; }
        public string codici_anagrafe_appezzamento { get; set; }      

    }
}
