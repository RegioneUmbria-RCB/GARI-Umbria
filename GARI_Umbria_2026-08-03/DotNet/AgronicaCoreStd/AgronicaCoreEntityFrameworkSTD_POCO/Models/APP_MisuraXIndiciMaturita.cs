using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_MisuraXIndiciMaturita: APP_Agronica_Entity
    {
        public int IND_MAT_COD { get; set; }
        public int UDM_COD { get; set; }
        public DateTime? DATA_AGG { get; set; }

    }
}
