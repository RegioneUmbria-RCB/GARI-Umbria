using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_IndiciMaturita: APP_Agronica_Entity
    {
        public int IND_MAT_COD { get; set; }
        public string IND_MAT_DES  { get; set; }
        public DateTime? DATA_AGG { get; set; }
        public int LAV_COD { get; set; }

    }
}
