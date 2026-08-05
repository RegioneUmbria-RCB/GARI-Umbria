using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_MisuraxAvversita: APP_Agronica_Entity
    {
        public int COD { get; set; }
        public int UDM_COD { get; set; }
        public int AV_COD { get; set; }
        public int VEG_COD { get; set; }
        public int? Fondamentale { get; set; }
        public DateTime? DATA_AGG { get; set; }
        public int? ff_Cod { get; set; }
        public int? ordine { get; set; }
        public int? AV_GRU { get; set; }

    }
}
