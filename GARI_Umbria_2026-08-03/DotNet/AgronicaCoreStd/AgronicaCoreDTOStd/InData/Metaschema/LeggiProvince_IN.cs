using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiProvince_IN
    {
        public string Stato_Country { get; set; }

        public List<string> Reg_List { get; set; } = new List<string>();
    }
}
