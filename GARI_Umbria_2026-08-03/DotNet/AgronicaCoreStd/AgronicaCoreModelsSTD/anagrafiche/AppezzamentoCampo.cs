using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class AppezzamentoCampo
    {
        public string piva { get; set; }
        public int sa_cod { get; set; }
        public int appezza { get; set; }
        public int campo_cod { get; set; }
        public int sup_app { get; set; }
        public string app_nome { get; set; }
        public IntervalloTemporale validita { get; set; }
        public int id_reg { get; set; }
        public IntervalloTemporale validita_impianto { get; set; }
        public int cul_cod { get; set; }
        public string cul_des { get; set; }
        public string veg_des { get; set; }
        public bool flag_cancellazione { get; set; }

        public AppezzamentoCampo()
        {
            flag_cancellazione = false;
        }

    }
}
