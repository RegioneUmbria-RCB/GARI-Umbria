using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiComuni_IN
    {
        public List<string> PROV_List { get; set; } = new List<string>();

        public string COM_LOCALITA { get; set; } = string.Empty;

        public string COM_PROVINCIA { get; set; } = string.Empty;

        public string SIGLA_PROVINCIA { get; set; } = string.Empty;

        public string CAP { get; set; } = string.Empty;

        public string REG { get; set; } = string.Empty;

        public string Filtro_StrProvincia { get; set; } = string.Empty;

        public string Filtro_StrComune { get; set; } = string.Empty;
    }
}
