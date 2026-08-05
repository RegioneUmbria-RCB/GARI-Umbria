using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiRegioni_IN
    {
        public List<string> Stati_List { get; set; } = new List<string>();

        public string Regione { get; set; } = string.Empty;
    }
}
