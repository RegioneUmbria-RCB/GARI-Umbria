using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_IndiciMaturitaxSpecieVegetali: APP_Agronica_Entity
    {
        public int IND_MAT_COD { get; set; }
        public int VEG_COD { get; set; }
        public int REG_COD { get; set; }
        public string CLASSE { get; set; }
        public DateTime? DATA_AGG { get; set; }
        public int? Flag_Raccolta { get; set; }
    }
}
