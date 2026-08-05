using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AgronicaCoreUtility
{
    public class LeggiSpecieColtivate
    {
        public Boolean PrimaRiga_Flag { get; set; }
        public string PrimaRiga_Text { get; set; }
        public string PrimaRiga_Value { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public DateTime Data_Da { get; set; }
        public DateTime Data_A { get; set; }
        public Boolean ConsideraTerrenoNudo { get; set; }
        public Boolean leggiAncheBloccati { get; set; }
    }
}
